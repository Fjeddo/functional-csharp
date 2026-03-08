namespace Functional.CSharp;

public static class ResultExtensionsMap
{
    extension<T>(Result<T> result)
    {
        public Result<TOut> Map<TOut>(Func<T, TOut> mapper)
            => result.Match(
                onSuccess: value => Result<TOut>.Success(mapper(value)),
                onFailure: Result<TOut>.Failure);

        public Task<Result<TOut>> MapAsync<TOut>(Func<T, Task<TOut>> mapper)
            => result.Match(
                onSuccess: async value => Result.Success(await mapper(value).ConfigureAwait(false)),
                onFailure: error => Task.FromResult(Result.Failure<TOut>(error))
            );
    }

    extension<T>(Task<Result<T>> resultTask)
    {
        public async Task<Result<TOut>> MapAsync<TOut>(Func<T, Task<TOut>> mapper)
        {
            var result = await resultTask.ConfigureAwait(false);
            return await result.MapAsync(mapper).ConfigureAwait(false);
        }
    }
}