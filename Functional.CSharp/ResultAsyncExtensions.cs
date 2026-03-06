namespace Functional.CSharp;

public static class ResultAsyncExtensions
{
    extension<T>(Task<Result<T>> resultTask)
    {
        public async Task<TOut> MatchAsync<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure)
        {
            var result = await resultTask.ConfigureAwait(false);
            return result.Match(onSuccess, onFailure);
        }

        public async Task<Result<TOut>> MapAsync<TOut>(Func<T, TOut> mapper)
        {
            var result = await resultTask.ConfigureAwait(false);
            return result.Map(mapper);
        }

        public async Task<Result<TOut>> MapAsync<TOut>(Func<T, Task<TOut>> mapper)
        {
            var result = await resultTask.ConfigureAwait(false);
            return await result.Match(
                onSuccess: async value => Result<TOut>.Success(await mapper(value).ConfigureAwait(false)),
                onFailure: error => Task.FromResult(Result<TOut>.Failure(error)));
        }
    }
}