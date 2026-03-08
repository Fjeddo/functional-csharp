namespace Functional.CSharp;

public static class ResultExtensionsMap
{
    extension<T>(Result<T> result)
    {
        public Result<TOut> Map<TOut>(Func<T, TOut> mapper)
            => result.Match(
                onSuccess: value => Result<TOut>.Success(mapper(value)),
                onFailure: Result<TOut>.Failure);
    }
}