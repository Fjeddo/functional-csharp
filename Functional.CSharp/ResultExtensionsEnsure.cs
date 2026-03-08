namespace Functional.CSharp;

public static class ResultExtensionsEnsure
{
    extension<T>(Result<T> result)
    {
        public Result<T> Ensure(Func<T, Result<bool>> predicate, Error error)
            => result.Match(
                onSuccess: value =>
                    predicate(value).Match(
                        onSuccess: isValid => isValid ? Result<T>.Success(value) : Result<T>.Failure(error),
                        onFailure: Result<T>.Failure),
                onFailure: Result<T>.Failure);
    }
}