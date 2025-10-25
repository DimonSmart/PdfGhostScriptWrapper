using System.Runtime.InteropServices;

namespace PdfGhostScriptWrapper;

public static class GhostScriptLocator
{
    private static readonly string[] WindowsCandidates =
    {
        "gswin64c.exe",
        "gswin32c.exe"
    };

    private static readonly string[] UnixCandidates =
    {
        "gs"
    };

    public static string Locate(string? overridePath = null)
    {
        if (!string.IsNullOrWhiteSpace(overridePath))
        {
            var explicitPath = overridePath!;
            if (File.Exists(explicitPath))
            {
                return explicitPath;
            }

            throw new FileNotFoundException($"GhostScript executable not found at '{explicitPath}'.", explicitPath);
        }

        var envPath = Environment.GetEnvironmentVariable("GHOSTSCRIPT_PATH");
        if (!string.IsNullOrWhiteSpace(envPath) && File.Exists(envPath))
        {
            return envPath;
        }

        foreach (var candidate in GetCandidates())
        {
            var resolved = ResolveFromPath(candidate);
            if (resolved is not null)
            {
                return resolved;
            }
        }

        throw new FileNotFoundException("Unable to locate GhostScript executable. Set the GHOSTSCRIPT_PATH environment variable or specify the path explicitly.");
    }

    private static IEnumerable<string> GetCandidates()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            foreach (var candidate in WindowsCandidates)
            {
                yield return candidate;
            }
        }
        else
        {
            foreach (var candidate in UnixCandidates)
            {
                yield return candidate;
            }
        }
    }

    private static string? ResolveFromPath(string executableName)
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(pathEnv))
        {
            return null;
        }

        var separator = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':';
        foreach (var directory in pathEnv.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        {
            var candidate = Path.Combine(directory, executableName);
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }
}
