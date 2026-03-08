namespace Functional.CSharp.Extreme
{
    public delegate TOut Result<T, TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure);

    public static class Result
    {
        public static Result<T, TOut> Success<T, TOut>(T value)
            => (onSuccess, _) => onSuccess(value);

        public static Result<T, TOut> Failure<T, TOut>(Error error)
            => (_, onFailure) => onFailure(error);

        public static TOut Match<T, TOut>(this Result<T, TOut> result, 
            Func<T, TOut> onSuccess,
            Func<Error, TOut> onFailure)
            => result(onSuccess, onFailure);

        public static Result<TOut, TResult> Map<T, TOut, TResult>(
            this Result<T, TResult> result,
            Func<T, TOut> mapper)
            => (onSuccess, onFailure)
                => result(
                    onSuccess: value => onSuccess(mapper(value)),
                    onFailure: onFailure);

        public static Result<TOut, TResult> Bind<T, TOut, TResult>(
            this Result<T, TResult> result,
            Func<T, Result<TOut, TResult>> bind)
            => (onSuccess, onFailure)
                => result(
                    onSuccess: value => bind(value)(onSuccess, onFailure),
                    onFailure: onFailure);
    }
}
