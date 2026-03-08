namespace Functional.CSharp;

public static class ResultExtensionsMatch
{
    extension<T>(Task<Result<T>> resultTask)
    {
        public async Task<TOut> MatchAsync<TOut>(
            Func<T, Task<TOut>> onSuccess, 
            Func<Error, Task<TOut>> onFailure)
        {
            var result = await resultTask.ConfigureAwait(false);
            return await result.Match(onSuccess, onFailure).ConfigureAwait(false);
        }
    }
}