namespace DiscordRPG.Common;

public class Result
{
    public Result(bool wasSuccessful, string errorMessage)
    {
        WasSuccessful = wasSuccessful;
        ErrorMessage = errorMessage;
    }

    public bool WasSuccessful { get; private set; }
    public string ErrorMessage { get; private set; }

    public static Result Success() => new Result(true, null);
    public static Result Failure(string reason) => new Result(false, reason);
}

public class Result<T> : Result
{
    private Result(bool wasSuccessful, string errorMessage, T value) : base(wasSuccessful, errorMessage)
    {
        Value = value;
    }

    public T Value { get; private set; }

    public static Result<T> Success(T value) => new Result<T>(true, null, value);
    public new static Result<T> Failure(string reason) => new Result<T>(false, reason, default(T));
}