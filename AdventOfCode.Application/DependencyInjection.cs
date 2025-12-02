using System.Reflection;
using System.Runtime.CompilerServices;
using AdventOfCode.Abstractions;
using AdventOfCode.Application;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides registration helpers to discover and wire Advent of Code solution types.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Scans assemblies for <see cref="BaseSolution"/> implementations and registers them with DI using keyed services.
    /// </summary>
    /// <param name="services">The service collection being configured.</param>
    /// <param name="rootAssembly">Optional root assembly whose references are scanned; defaults to the calling assembly.</param>
    /// <returns>The same service collection for chaining.</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static IServiceCollection AddSolutions(this IServiceCollection services, Assembly? rootAssembly = null)
    {
        var root = rootAssembly ?? Assembly.GetCallingAssembly();
        var assemblies = LoadReferencedAssemblies(root)
            .Concat(LoadSolutionAssemblies(AppContext.BaseDirectory))
            .DistinctBy(static asm => asm.FullName ?? asm.GetName().FullName)
            .ToArray();
        var descriptors = assemblies
            .SelectMany(GetLoadableTypes)
            .Where(static t => t is { IsClass: true, IsAbstract: false }
                               && typeof(BaseSolution).IsAssignableFrom(t)
                               && typeof(ISolution).IsAssignableFrom(t))
            .Select(CreateDescriptor)
            .Where(static descriptor => descriptor is not null)
            .Cast<SolutionDescriptor>()
            .ToList();

        ValidateNoDuplicateKeys(descriptors);

        foreach (var descriptor in descriptors)
        {
            var key = descriptor.Key;
            var implementationType = descriptor.ImplementationType;
            services.AddTransient(implementationType);
            services.AddKeyedTransient<BaseSolution>(key, (sp, _) => (BaseSolution)sp.GetRequiredService(implementationType));
            services.AddKeyedTransient<ISolution>(key, (sp, _) => sp.GetRequiredKeyedService<BaseSolution>(key));
        }

        services.AddSingleton(new SolutionRegistry(descriptors));
        services.AddTransient<ISolutionFactory, SolutionFactory>();

        return services;
    }

    private static SolutionDescriptor? CreateDescriptor(Type type)
    {
        var year = GetConstantIntFromGetter(type, nameof(ISolution.Year));
        var day = GetConstantIntFromGetter(type, nameof(ISolution.Day));
        return year.HasValue && day.HasValue
            ? new SolutionDescriptor(type, year.Value, day.Value)
            : null;
    }

    private static IEnumerable<Assembly> LoadReferencedAssemblies(Assembly root)
    {
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var queue = new Queue<Assembly>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var assembly = queue.Dequeue();
            var identity = assembly.FullName ?? assembly.GetName().FullName;
            if (!visited.Add(identity))
            {
                continue;
            }

            yield return assembly;

            foreach (var reference in assembly.GetReferencedAssemblies())
            {
                if (visited.Contains(reference.FullName))
                {
                    continue;
                }

                try
                {
                    var loaded = AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(a => string.Equals(a.FullName, reference.FullName, StringComparison.OrdinalIgnoreCase))
                                 ?? Assembly.Load(reference);
                    queue.Enqueue(loaded);
                }
                catch
                {
                    // ignore load failures (optional assemblies or trimming)
                }
            }
        }
    }

    private static IEnumerable<Assembly> LoadSolutionAssemblies(string? baseDirectory)
    {
        if (string.IsNullOrWhiteSpace(baseDirectory) || !Directory.Exists(baseDirectory))
        {
            yield break;
        }

        foreach (var path in Directory.EnumerateFiles(baseDirectory, "AdventOfCode.*.Solutions.dll", SearchOption.TopDirectoryOnly))
        {
            AssemblyName? assemblyName;
            try
            {
                assemblyName = AssemblyName.GetAssemblyName(path);
            }
            catch
            {
                continue;
            }

            var existing = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => AssemblyName.ReferenceMatchesDefinition(a.GetName(), assemblyName));
            if (existing is not null)
            {
                yield return existing;
                continue;
            }

            Assembly? loaded = null;
            try
            {
                loaded = Assembly.Load(assemblyName);
            }
            catch
            {
                try
                {
                    loaded = Assembly.LoadFrom(path);
                }
                catch
                {
                    // ignore load failures; file might target incompatible runtime
                }
            }

            if (loaded is not null)
            {
                yield return loaded;
            }
        }
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.DefinedTypes.Select(static ti => (Type)ti);
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types
                .Where(static t => t is not null)
                .Select(static t => t!);
        }
    }

    private static int? GetConstantIntFromGetter(Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var il = property?.GetMethod?.GetMethodBody()?.GetILAsByteArray();
        if (il is null || il.Length == 0)
        {
            return null;
        }

        for (var i = 0; i < il.Length; i++)
        {
            var op = il[i];
            switch (op)
            {
                case 0x15:
                    return -1;
                case >= 0x16 and <= 0x1E:
                    return op - 0x16;
                case 0x1F when i + 1 < il.Length:
                    return (sbyte)il[i + 1];
                case 0x20 when i + 4 < il.Length:
                    return BitConverter.ToInt32(il, i + 1);
            }
        }

        return null;
    }

    private static void ValidateNoDuplicateKeys(IEnumerable<SolutionDescriptor> descriptors)
    {
        var duplicate = descriptors
            .GroupBy(static d => d.Key)
            .FirstOrDefault(static g => g.Count() > 1);

        if (duplicate is not null)
        {
            throw new InvalidOperationException(
                $"Multiple solutions registered for key '{duplicate.Key}'. Ensure each (Year, Day) pair is unique.");
        }
    }
}

/// <summary>
/// Describes a discovered solution implementation.
/// </summary>
/// <param name="ImplementationType">The concrete solution type.</param>
/// <param name="Year">The Advent of Code year implemented.</param>
/// <param name="Day">The Advent of Code day implemented.</param>
public sealed record SolutionDescriptor(Type ImplementationType, int Year, int Day)
{
    /// <summary>
    /// Gets the composite key used for keyed service registration (YYYYDD).
    /// </summary>
    public string Key => $"{Year}{Day:D2}";
}

/// <summary>
/// Stores discovered solution descriptors for later lookup.
/// </summary>
/// <param name="descriptors">The descriptors to track.</param>
public sealed class SolutionRegistry(IEnumerable<SolutionDescriptor> descriptors)
{
    private readonly Dictionary<(int Year, int Day), SolutionDescriptor> _map = descriptors.ToDictionary(static d => (d.Year, d.Day));

    /// <summary>
    /// Attempts to get the descriptor for the requested year/day.
    /// </summary>
    public bool TryGet(int year, int day, out SolutionDescriptor? descriptor) => _map.TryGetValue((year, day), out descriptor);

    /// <summary>
    /// Returns every registered descriptor.
    /// </summary>
    public IEnumerable<SolutionDescriptor> All() => _map.Values;
}

/// <summary>
/// Creates solutions based on their year/day metadata.
/// </summary>
public interface ISolutionFactory
{
    /// <summary>
    /// Builds the requested solution if registered, or returns <see langword="null"/>.
    /// </summary>
    /// <param name="year">Target Advent of Code year.</param>
    /// <param name="day">Target day within the year.</param>
    /// <param name="serviceProvider">The service provider used to instantiate dependencies.</param>
    ISolution? Create(int year, int day, IServiceProvider serviceProvider);
}

/// <summary>
/// Default implementation of <see cref="ISolutionFactory"/> that uses <see cref="ActivatorUtilities"/>.
/// </summary>
/// <param name="registry">Registry used to look up solution descriptors.</param>
public sealed class SolutionFactory(SolutionRegistry registry) : ISolutionFactory
{
    /// <inheritdoc />
    public ISolution? Create(int year, int day, IServiceProvider serviceProvider)
    {
        if (!registry.TryGet(year, day, out var descriptor))
        {
            return null;
        }

        return (ISolution)ActivatorUtilities.CreateInstance(serviceProvider, descriptor!.ImplementationType);
    }
}