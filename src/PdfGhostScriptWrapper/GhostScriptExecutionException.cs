namespace PdfGhostScriptWrapper;

public class GhostScriptExecutionException : Exception
{
    public GhostScriptExecutionException(string message, GhostScriptExecutionResult result)
        : base(message)
    {
        Result = result;
    }

    public GhostScriptExecutionResult Result { get; }
}
