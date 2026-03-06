using static System.Threading.Tasks.Task;

namespace Functional.CSharp.Tests;

[TestClass]
public class ResultAsyncMapTests
{
    [TestMethod]
    public async Task Should_map_async_success_value()
    {
        var sut = FromResult(Result.Success(5));

        var mapped = await sut.MapAsync(x => x * 2);

        var result = mapped.Match(
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
    public async Task Should_map_async_success_value_to_different_type()
    {
        var sut = FromResult(Result.Success(5));

        var mapped = await sut.MapAsync(x => x.ToString());

        var result = mapped.Match(
            onSuccess: value =>
            {
                Assert.AreEqual("5", value);
                return value;
            },
            onFailure: _ =>
            {
                Assert.Fail("Expected success, but got failure.");
                return string.Empty;
            });

        Assert.AreEqual("5", result);
    }

    [TestMethod]
    public async Task Should_not_map_async_failure()
    {
        var sut = FromResult(Result.Failure<int>(new TestError("this is an expected error")));

        var mapped = await sut.MapAsync(x => x * 2);

        var result = mapped.Match(
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
    public async Task Should_preserve_error_on_map_async_failure()
    {
        var expectedError = new TestError("original error");
        var sut = FromResult(Result.Failure<int>(expectedError));

        var mapped = await sut.MapAsync(x => x.ToString());

        mapped.Match(
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
    public async Task Should_map_long_running_async_task()
    {
        var sut = Run(async () => await FromResult(Result.Success(5)));

        var mapped = await sut.MapAsync(x => x * 2);

        var result = mapped.Match(
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
    public async Task Should_map_async_success_value_with_async_mapper()
    {
        var sut = FromResult(Result.Success(5));

        var mapped = await sut.MapAsync(async x => await FromResult(x * 2));

        var result = mapped.Match(
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
    public async Task Should_map_async_success_value_to_different_type_with_async_mapper()
    {
        var sut = FromResult(Result.Success(5));

        var mapped = await sut.MapAsync(async x => await FromResult(x.ToString()));

        var result = mapped.Match(
            onSuccess: value =>
            {
                Assert.AreEqual("5", value);
                return value;
            },
            onFailure: _ =>
            {
                Assert.Fail("Expected success, but got failure.");
                return string.Empty;
            });

        Assert.AreEqual("5", result);
    }

    [TestMethod]
    public async Task Should_not_map_async_failure_with_async_mapper()
    {
        var sut = FromResult(Result.Failure<int>(new TestError("this is an expected error")));

        var mapped = await sut.MapAsync(async x => await FromResult(x * 2));

        var result = mapped.Match(
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
    public async Task Should_preserve_error_on_map_async_failure_with_async_mapper()
    {
        var expectedError = new TestError("original error");
        var sut = FromResult(Result.Failure<int>(expectedError));

        var mapped = await sut.MapAsync(async x => await Task.FromResult(x.ToString()));

        mapped.Match(
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
    public async Task Should_chain_multiple_async_maps()
    {
        var sut = FromResult(Result.Success(5));

        var result = await sut
            .MapAsync(x => x * 2)
            .MapAsync(x => x + 3)
            .MapAsync(x => x.ToString());

        var final = result.Match(
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

        Assert.AreEqual("13", final);
    }

    [TestMethod]
    public async Task Should_chain_async_map_with_async_mapper()
    {
        var sut = FromResult(Result.Success(5));

        var mapped1 = await sut.MapAsync(async x => await FromResult(x * 2));

        var mapped2 = await FromResult(mapped1).MapAsync(async x => await FromResult(x + 3));

        var result = mapped2.Match(
            onSuccess: value =>
            {
                Assert.AreEqual(13, value);
                return value;
            },
            onFailure: _ =>
            {
                Assert.Fail("Expected success, but got failure.");
                return 0;
            });

        Assert.AreEqual(13, result);
    }

    public record TestError(string Message) : Error(1, Message);
}
