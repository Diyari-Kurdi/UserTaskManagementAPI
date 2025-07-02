namespace UserTaskManagementAPI.Shared;

public class Result
{
    public bool Succeeded { get; set; }
    public bool Failed => !Succeeded;

    public Error? Error { get; set; }

    public Result(bool succeeded, Error? error)
    {
        Succeeded = succeeded;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true, null);
    public static Result<T> Failure<T>(Error error) => new(default!, false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool succeeded, Error? error) : base(succeeded, error)
    {
        _value = value;
    }

    public TValue Value => Succeeded
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

}