using System.Diagnostics;

namespace PdfGhostScriptWrapper;

public class GhostScriptRunner
{
    private readonly string _executablePath;

    public GhostScriptRunner(string? executablePath = null)
    {
        _executablePath = GhostScriptLocator.Locate(executablePath);
    }

    public async Task<GhostScriptExecutionResult> ExecuteAsync(
        GhostScriptOptions options,
        CancellationToken cancellationToken = default)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        EnsureInputFileExists(options.InputFile);

        var startInfo = new ProcessStartInfo
        {
            FileName = _executablePath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var argument in options.BuildArguments())
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

        process.Start();

        await using var registration = cancellationToken.Register(() => TryTerminateProcess(process));

        var stdOutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stdErrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var result = new GhostScriptExecutionResult(
            process.ExitCode,
            await stdOutTask,
            await stdErrTask);

        if (result.ExitCode != 0)
        {
            throw new GhostScriptExecutionException(
                $"GhostScript exited with code {result.ExitCode}.",
                result);
        }

        return result;
    }

    private static void EnsureInputFileExists(string inputFile)
    {
        if (!File.Exists(inputFile))
        {
            throw new FileNotFoundException($"Input file '{inputFile}' was not found.", inputFile);
        }
    }

    private static void TryTerminateProcess(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch (InvalidOperationException)
        {
        }
    }
}
