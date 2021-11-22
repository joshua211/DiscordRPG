namespace DiscordRPG.Common;

public class ExecutionResult
{
    public ExecutionResult(bool wasSuccessful, string errorMessage)
    {
        WasSuccessful = wasSuccessful;
        ErrorMessage = errorMessage;
    }

    public bool WasSuccessful { get; private set; }
    public string ErrorMessage { get; private set; }
    public static ExecutionResult Success() => new ExecutionResult(true, String.Empty);
    public static ExecutionResult Failure(string error) => new ExecutionResult(false, error);
}