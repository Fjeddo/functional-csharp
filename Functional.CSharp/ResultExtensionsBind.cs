namespace Functional.CSharp;

public static class ResultExtensionsBind
{
    extension<T>(Result<T> result)
    {
        public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> binder)
            => result.Match(
                onSuccess: binder,
                onFailure: Result<TOut>.Failure);

        public Task<Result<TOut>> BindAsync<TOut>(Func<T, Task<Result<TOut>>> binder)
            => result.Match(
                onSuccess: binder,
                onFailure: error => Task.FromResult(Result.Failure<TOut>(error))
            );
    }

    extension<T>(Task<Result<T>> resultTask)
    {
        public async Task<Result<TOut>> BindAsync<TOut>(Func<T, Task<Result<TOut>>> binder)
        {
            var result = await resultTask.ConfigureAwait(false);
            return await result.BindAsync(binder).ConfigureAwait(false);
        }

        public async Task<Result<TOut>> BindAsync<TOut>(Func<T, Result<TOut>> binder)
        {
            var result = await resultTask.ConfigureAwait(false);
            return result.Bind(binder);
        }
    }
}