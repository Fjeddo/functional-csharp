namespace Functional.CSharp;

public static class ResultExtensions
{
    extension<T>(T o)
    {
        public Result<T> ToResult() => Result<T>.Success(o);
    }
}