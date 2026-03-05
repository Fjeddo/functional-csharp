namespace Functional.CSharp;

public static class ResultExtensions
{
    extension<T>(Result<T> result)
    {
        public Result<TOut> Map<TOut>(Func<T, TOut> mapper)
            => result.Match(value => Result<TOut>.Success(mapper(value)), Result<TOut>.Failure);

        public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> binder)
            => result.Match(binder, Result<TOut>.Failure);

        public Result<T> Ensure(Func<T, Result<bool>> predicate, Error error)
            => result.Match(
                onSuccess: value => predicate(value).Match(
                    onSuccess: isValid => isValid 
                        ? Result<T>.Success(value) 
                        : Result<T>.Failure(error),
                    onFailure: Result<T>.Failure),
                onFailure: Result<T>.Failure);
    }

    extension<T>(T o)
    {
        public Result<T> ToResult() => Result<T>.Success(o);
    }
}