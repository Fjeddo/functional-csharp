namespace Functional.CSharp;

public static class ResultExtensionsTapOverloads 
{
    extension<T>(Result<T> result)
    {
        public Result<T> Tap(Action<T> action) =>
            result.Match(
                onSuccess: value =>
                {
                    action(value);
                    return result;
                },
                onFailure: _ => result);

        public Result<T> TapError(Action<Error> action) =>
            result.Match(
                onSuccess: _ => result,
                onFailure: error =>
                {
                    action(error);
                    return result;
                });
    }
}