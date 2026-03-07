namespace Functional.CSharp;

public static class ResultExtensionsMapOverloads
{
    extension<T>(Result<T> result)
    {
        public Result<TOut> Map<TOut>(Func<T, TOut> mapper)
            => result.Match(
                onSuccess: value => Result<TOut>.Success(mapper(value)),
                onFailure: Result<TOut>.Failure);
    }
}