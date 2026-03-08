namespace Functional.CSharp.Tests;

[TestClass]
public class ResultBindAsyncTests
{
    [TestMethod]
    public async Task Should_bind_success_to_success_async()
    {
        var sut = Result.Success(5);

        var bound = await sut.BindAsync(x => Task.FromResult(Result.Success(x * 2)));

        var result = bound.Match(
            onSuccess: value =>
            {
                Assert.AreEqual(10, value);
                return value;
            },
            onFailure: _ =>
            {
                Assert.Fail("Expected success, but got failure.");
                return 0;
            });

        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public async Task Should_bind_success_to_failure_async()
    {
        var sut = Result.Success(5);

        var bound = await sut.BindAsync(x => Task.FromResult(Result.Failure<int>(new TestError("bind failed"))));

        var result = bound.Match(
            onSuccess: value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return value;
            },
            onFailure: error =>
            {
                Assert.IsInstanceOfType<TestError>(error);
                Assert.AreEqual("bind failed", error.Message);
                return 0;
            });

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public async Task Should_not_bind_failure_async()
    {
        var sut = Result.Failure<int>(new TestError("this is an expected error"));
        var binderWasCalled = false;

        var bound = await sut.BindAsync(x =>
        {
            binderWasCalled = true;
            return Task.FromResult(Result.Success(x * 2));
        });

        var result = bound.Match(
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

        Assert.IsFalse(binderWasCalled);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public async Task Should_preserve_error_on_bind_failure_async()
    {
        var expectedError = new TestError("original error");
        var sut = Result.Failure<int>(expectedError);

        var bound = await sut.BindAsync(x => Task.FromResult(Result.Success(x.ToString())));

        bound.Match(
            onSuccess: _ =>
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
    public async Task Should_chain_multiple_binds_async()
    {
        var sut = Result.Success(5);

        var result = await sut
            .BindAsync(x => Task.FromResult(Result.Success(x * 2)))
            .BindAsync(x => Task.FromResult(Result.Success(x + 3)))
            .BindAsync(x => Task.FromResult(Result.Success(x.ToString())));

        result.Match(
            onSuccess: value =>
            {
                Assert.AreEqual("13", value);
                return value;
            },
            onFailure: _ =>
            {
                Assert.Fail("Expected success, but got failure.");
                return string.Empty;
            });
    }

    [TestMethod]
    public async Task Should_chain_multiple_binds_async2()
    {
        var sut = Result.Success(5);

        var result = await sut
            .BindAsync(x => Task.FromResult(Result.Success(x * 2)))
            .BindAsync(x => Result.Success( x + 3))
            .BindAsync(x => Result.Success( x + 4))
            .BindAsync(x => Task.FromResult(Result.Success(x.ToString())));

        result.Match(
            onSuccess: value =>
            {
                Assert.AreEqual("17", value);
                return value;
            },
            onFailure: _ =>
            {
                Assert.Fail("Expected success, but got failure.");
                return string.Empty;
            });
    }

    [TestMethod]
    public async Task Should_short_circuit_on_first_failure_in_async_chain()
    {
        var sut = Result.Success(5);
        var thirdBinderWasCalled = false;

        var result = await sut
            .BindAsync(x => Task.FromResult(Result.Success(x * 2)))
            .BindAsync(x => Task.FromResult(Result.Failure<int>(new TestError("second bind failed"))))
            .BindAsync(x =>
            {
                thirdBinderWasCalled = true;
                return Task.FromResult(Result.Success(x + 100));
            });

        result.Match(
            onSuccess: value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return value;
            },
            onFailure: error =>
            {
                Assert.IsInstanceOfType<TestError>(error);
                Assert.AreEqual("second bind failed", error.Message);
                return 0;
            });

        Assert.IsFalse(thirdBinderWasCalled);
    }

    [TestMethod]
    public async Task Should_bind_task_result_success_async()
    {
        Task<Result<int>> sut = Task.FromResult(Result.Success(5));

        var bound = await sut.BindAsync(x => Task.FromResult(Result.Success(x * 2)));

        var result = bound.Match(
            onSuccess: value =>
            {
                Assert.AreEqual(10, value);
                return value;
            },
            onFailure: _ =>
            {
                Assert.Fail("Expected success, but got failure.");
                return 0;
            });

        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public async Task Should_not_bind_task_failure_async()
    {
        var expectedError = new TestError("task failed");
        Task<Result<int>> sut = Task.FromResult(Result.Failure<int>(expectedError));
        var binderWasCalled = false;

        var bound = await sut.BindAsync(x =>
        {
            binderWasCalled = true;
            return Task.FromResult(Result.Success(x * 2));
        });

        bound.Match(
            onSuccess: _ =>
            {
                Assert.Fail("Expected failure, but got success.");
                return 0;
            },
            onFailure: error =>
            {
                Assert.AreSame(expectedError, error);
                Assert.AreEqual("task failed", error.Message);
                return 0;
            });

        Assert.IsFalse(binderWasCalled);
    }

    public record TestError(string Message) : Error(1, Message);
}
