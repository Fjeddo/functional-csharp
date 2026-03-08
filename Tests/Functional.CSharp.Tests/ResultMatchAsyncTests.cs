namespace Functional.CSharp.Tests;

[TestClass]
public class ResultMatchAsyncTests
{
    [TestMethod]
    public async Task Should_match_task_result_success_async()
    {
        Task<Result<int>> sut = Task.FromResult(Result.Success(5));

        var matched = await sut.MatchAsync(
            onSuccess: value => Task.FromResult(value * 2),
            onFailure: _ =>
            {
                Assert.Fail("Expected success, but got failure.");
                return Task.FromResult(0);
            });

        Assert.AreEqual(10, matched);
    }

    [TestMethod]
    public async Task Should_match_task_result_failure_async()
    {
        var expectedError = new TestError("task failed");
        Task<Result<int>> sut = Task.FromResult(Result.Failure<int>(expectedError));

        var matched = await sut.MatchAsync(
            onSuccess: value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return Task.FromResult(value);
            },
            onFailure: error =>
            {
                Assert.AreSame(expectedError, error);
                Assert.AreEqual("task failed", error.Message);
                return Task.FromResult(0);
            });

        Assert.AreEqual(0, matched);
    }

    [TestMethod]
    public async Task Should_unfold_async_pipeline_with_match_async()
    {
        var matched = await Task.FromResult(Result.Success(5))
            .MapAsync(x => Task.FromResult(x * 2))
            .BindAsync(x => Task.FromResult(Result.Success(x + 3)))
            .MatchAsync(
                onSuccess: value => Task.FromResult(value.ToString()),
                onFailure: _ =>
                {
                    Assert.Fail("Expected success, but got failure.");
                    return Task.FromResult(string.Empty);
                });

        Assert.AreEqual("13", matched);
    }

    [TestMethod]
    public async Task Should_unfold_failed_async_pipeline_with_match_async()
    {
        var successHandlerWasCalled = false;

        var matched = await Task.FromResult(Result.Success(5))
            .MapAsync(x => Task.FromResult(x * 2))
            .BindAsync(_ => Task.FromResult(Result.Failure<int>(new TestError("bind failed"))))
            .MatchAsync(
                onSuccess: value =>
                {
                    successHandlerWasCalled = true;
                    return Task.FromResult(value.ToString());
                },
                onFailure: error =>
                {
                    Assert.IsInstanceOfType<TestError>(error);  
                    Assert.AreEqual("bind failed", error.Message);
                    return Task.FromResult("error");
                });

        Assert.IsFalse(successHandlerWasCalled);
        Assert.AreEqual("error", matched);
    }

    public record TestError(string Message) : Error(1, Message);
}
