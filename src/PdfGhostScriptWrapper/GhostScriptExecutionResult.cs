namespace PdfGhostScriptWrapper;

public sealed record GhostScriptExecutionResult(
    int ExitCode,
    string StandardOutput,
    string StandardError);
