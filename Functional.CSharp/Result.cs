namespace Functional.CSharp;

public abstract class Result<T>
{
    private readonly Error _error;
    private readonly T _value;

    protected Result(T value) 
        : this(value, null!) {}

    protected Result(Error error) 
        : this(default!, error) {}

    private Result(T value, Error error)
    {
        _value = value;
        _error = error;
    }
    
    public abstract TOut Match<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure);

    private class ResultSuccess<TValue>(TValue value) : Result<TValue>(value)
    {
        public override TOut Match<TOut>(Func<TValue, TOut> onSuccess, Func<Error, TOut> onFailure) 
            => onSuccess(_value);
    }

    private class ResultFailure<TValue>(Error error) : Result<TValue>(error)
    {
        public override TOut Match<TOut>(Func<TValue, TOut> onSuccess, Func<Error, TOut> onFailure) 
            => onFailure(_error);
    }

    public static Result<T> Success(T value) 
    {
        ArgumentNullException.ThrowIfNull(value);
        return new ResultSuccess<T>(value);
    }

    public static Result<T> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new ResultFailure<T>(error);
    }
}

public static class Result
{
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);
}