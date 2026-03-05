namespace Functional.CSharp;

public abstract class Result<T>
{
    public abstract TOut Match<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure);

    private class ResultSuccess<TValue>(TValue value) : Result<TValue>
    {
        public override TOut Match<TOut>(Func<TValue, TOut> onSuccess, Func<Error, TOut> onFailure) 
            => onSuccess(value);
    }

    private class ResultFailure<TValue>(Error error) : Result<TValue>
    {
        public override TOut Match<TOut>(Func<TValue, TOut> onSuccess, Func<Error, TOut> onFailure) 
            => onFailure(error);
    }

    public static Result<T> Success(T value) => new ResultSuccess<T>(value);
    public static Result<T> Failure(Error error) => new ResultFailure<T>(error);
}

public static class Result
{
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);
}