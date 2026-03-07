namespace Functional.CSharp;

public static class ResultExtensionsBindOverloads
{
    extension<T>(Result<T> result)
    {
        public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> binder)
            => result.Match(
                onSuccess: binder,
                onFailure: Result<TOut>.Failure);
    }
}