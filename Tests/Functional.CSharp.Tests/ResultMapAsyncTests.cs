namespace Functional.CSharp.Tests;

[TestClass]
public class ResultMapAsyncTests
{
    [TestMethod]
    public async Task Should_map_success_value_async()
    {
        var sut = Result.Success(5);

        var mapped = await sut.MapAsync(x => Task.FromResult(x * 2));

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
    public async Task Should_map_success_value_to_different_type_async()
    {
        var sut = Result.Success(5);

        var mapped = await sut.MapAsync(x => Task.FromResult(x.ToString()));

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
    public async Task Should_not_map_failure_async()
    {
        var sut = Result.Failure<int>(new TestError("this is an expected error"));
        var mapperWasCalled = false;

        var mapped = await sut.MapAsync(x =>
        {
            mapperWasCalled = true;
            return Task.FromResult(x * 2);
        });

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

        Assert.IsFalse(mapperWasCalled);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public async Task Should_preserve_error_on_map_failure_async()
    {
        var expectedError = new TestError("original error");
        var sut = Result.Failure<int>(expectedError);

        var mapped = await sut.MapAsync(x => Task.FromResult(x.ToString()));

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
    public async Task Should_chain_multiple_maps_async()
    {
        var sut = Result.Success(5);

        var result = await sut.MapAsync(x => Task.FromResult(x * 2));
        result = await result.MapAsync(x => Task.FromResult(x + 3));
        var finalResult = await result.MapAsync(x => Task.FromResult(x.ToString()));

        finalResult.Match(
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
    public async Task Should_chain_multiple_maps_async2()
    {
        var sut = Result.Success(5);

        var result = await sut
            .MapAsync(x => Task.FromResult(x * 2))
            .MapAsync(x => Task.FromResult(x + 3))
            .MapAsync(x => Task.FromResult(x.ToString()));

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
    public async Task Should_chain_multiple_maps_async3()
    {
        var sut = Result.Success(5);

        var result = await sut
            .MapAsync(x => Task.FromResult(x * 2))
            .MapAsync(x => x + 3)
            .MapAsync(x => x + 4)
            .MapAsync(x => Task.FromResult(x.ToString()));

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

    public record TestError(string Message) : Error(1, Message);
}