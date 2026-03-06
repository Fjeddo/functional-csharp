using static System.Threading.Tasks.Task;

namespace Functional.CSharp.Tests;

[TestClass]
public class ResultAsyncMatchTests
{
    [TestMethod]
    public async Task Should_match_async_success()
    {
        var sut = FromResult(Result.Success(5));

        var result = await sut.MatchAsync(
            onSuccess: value =>
            {
                Assert.AreEqual(5, value);
                return value;
            },
            onFailure: _ =>
            {
                Assert.Fail("Expected success, but got failure.");
                return 0;
            });

        Assert.AreEqual(5, result);
    }

    [TestMethod]
    public async Task Should_match_async_failure()
    {
        var sut = FromResult(Result.Failure<int>(new TestError("this is an expected error")));

        var result = await sut.MatchAsync(
            onSuccess: value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return value;
            },
            onFailure: error =>
            {
                Assert.IsInstanceOfType<TestError>(error);
                Assert.AreEqual("this is an expected error", error.Message);
                return 0;
            });

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public async Task Should_match_async_success_to_different_type()
    {
        var sut = FromResult(Result.Success(5));

        var result = await sut.MatchAsync(
            onSuccess: value => value.ToString(),
            onFailure: error => string.Empty);

        Assert.AreEqual("5", result);
    }

    [TestMethod]
    public async Task Should_match_async_failure_to_different_type()
    {
        var sut = FromResult(Result.Failure<int>(new TestError("error")));

        var result = await sut.MatchAsync(
            onSuccess: value => value.ToString(),
            onFailure: error => $"Error: {error.Message}");

        Assert.AreEqual("Error: error", result);
    }

    [TestMethod]
    public async Task Should_preserve_error_on_async_match_failure()
    {
        var expectedError = new TestError("original error");
        var sut = FromResult(Result.Failure<int>(expectedError));

        await sut.MatchAsync(
            onSuccess: value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return string.Empty;
            },
            onFailure: error =>
            {
                Assert.AreSame(expectedError, error);
                Assert.AreEqual("original error", error.Message);
                return string.Empty;
            });
    }

    [TestMethod]
    public async Task Should_await_long_running_task()
    {
        var sut = Run(async () => await FromResult(Result.Success(42)));

        var result = await sut.MatchAsync(
            onSuccess: value =>
            {
                Assert.AreEqual(42, value);
                return value;
            },
            onFailure: _ =>
            {
                Assert.Fail("Expected success, but got failure.");
                return 0;
            });

        Assert.AreEqual(42, result);
    }

    public record TestError(string Message) : Error(1, Message);
}
