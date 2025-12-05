using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace AdventOfCode.Console.Browser;

/// <summary>
/// Identifies supported browsers from which Advent of Code cookies can be read.
/// </summary>
enum Browser
{
    Chrome,
    Edge
}

/// <summary>
/// Provides helpers for reading Advent of Code session cookies directly from browser profiles.
/// </summary>
static class BrowserCookieReader
{
    
    /// <summary>
    /// Asynchronously retrieves the Advent of Code session cookie value for the specified browser and profile.
    /// </summary>
    /// <param name="browser">The browser from which to read the cookie.</param>
    /// <param name="profileName">The name of the browser profile. Defaults to "Default".</param>
    /// <returns>
    /// The session cookie value if found and accessible; otherwise, null.
    /// </returns>
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public static async Task<string?> GetAdventOfCodeSessionAsync(
        Browser browser,
        string profileName = "Default")
    {
        var cookieDbPath = GetCookieDbPath(browser, profileName);
        var tempPath = Path.GetTempFileName();
        try
        {
            if (!File.Exists(cookieDbPath))
            {
                await System.Console.Error.WriteLineAsync($"Cookie database not found: {cookieDbPath}");
                return null;
            }

            // Copy DB to temp file to avoid locking issues
            File.Copy(cookieDbPath, tempPath, overwrite: true);
        }
        catch (Exception e)
        {
            await System.Console.Error.WriteLineAsync($"Error accessing cookie database: {e.Message}");
            try
            {
                File.Delete(tempPath);
            }
            catch
            {
                /* ignore */
            }
            return null;
        }
      

        try
        {
            await using var connection = new SqliteConnection($"Data Source={tempPath};Mode=ReadOnly");
            await connection.OpenAsync();

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT value, encrypted_value
                FROM cookies
                WHERE host_key LIKE '%adventofcode.com'
                  AND name = 'session'
                ORDER BY expires_utc DESC
                LIMIT 1;
            ";

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                await System.Console.Error.WriteLineAsync("No Advent of Code session cookie found.");
                return null;
            }

            // Some versions store plaintext in 'value', some in 'encrypted_value'
            var value = reader["value"] as string ?? string.Empty;
            var encrypted = (byte[])reader["encrypted_value"];

            if (!string.IsNullOrEmpty(value) && encrypted.Length == 0)
            {
                // Plaintext cookie
                return value;
            }

            if (encrypted.Length > 0)
            {
                // Windows DPAPI decryption
                var decryptedBytes = ProtectedData.Unprotect(
                    encrypted,
                    optionalEntropy: null,
                    scope: DataProtectionScope.CurrentUser);

                return Encoding.UTF8.GetString(decryptedBytes);
            }

            return null;
        }
        finally
        {
            try
            {
                File.Delete(tempPath);
            }
            catch
            {
                /* ignore */
            }
        }
    }

    /// <summary>
    /// Constructs the file path to the cookie database for the specified browser and profile.
    /// </summary>
    /// <param name="browser">The browser for which to construct the path.</param>
    /// <param name="profileName">The name of the browser profile.</param>
    /// <returns>
    /// The file path to the cookie database.
    /// </returns>
    private static string GetCookieDbPath(Browser browser, string profileName)
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        return browser switch
        {
            Browser.Chrome => Path.Combine(
                localAppData,
                "Google", "Chrome", "User Data", profileName, "Network", "Cookies"),

            Browser.Edge => Path.Combine(
                localAppData,
                "Microsoft", "Edge", "User Data", profileName, "Network", "Cookies"),

            _ => throw new ArgumentOutOfRangeException(nameof(browser), browser, null)
        };
    }
}