namespace Functional.CSharp.Tests;

[TestClass]
public class ResultBindTests
{
    [TestMethod]
    public void Should_bind_success_to_success()
    {
        var sut = Result.Success(5);

        var bound = sut.Bind(x => Result.Success(x * 2));

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
    public void Should_bind_success_to_failure()
    {
        var sut = Result.Success(5);

        var bound = sut.Bind(x => Result.Failure<int>(new TestError("bind failed")));

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
    public void Should_not_bind_failure()
    {
        var sut = Result.Failure<int>(new TestError("this is an expected error"));

        var bound = sut.Bind(x => Result.Success(x * 2));

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

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Should_preserve_error_on_bind_failure()
    {
        var expectedError = new TestError("original error");
        var sut = Result.Failure<int>(expectedError);

        var bound = sut.Bind(x => Result.Success(x.ToString()));

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
    public void Should_bind_to_different_type()
    {
        var sut = Result.Success(5);

        var bound = sut.Bind(x => Result.Success(x.ToString()));

        var result = bound.Match(
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
    public void Should_chain_multiple_binds()
    {
        var sut = Result.Success(5);

        var result = sut
            .Bind(x => Result.Success(x * 2))
            .Bind(x => Result.Success(x + 3))
            .Bind(x => Result.Success(x.ToString()));

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
    public void Should_short_circuit_on_first_failure_in_chain()
    {
        var sut = Result.Success(5);

        var result = sut
            .Bind(x => Result.Success(x * 2))
            .Bind(x => Result.Failure<int>(new TestError("second bind failed")))
            .Bind(x => Result.Success(x + 100));

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
    }

    [TestMethod]
    public void Should_combine_bind_and_map()
    {
        var sut = Result.Success(5);

        var result = sut
            .Bind(x => Result.Success(x * 2))
            .Map(x => x + 3)
            .Bind(x => Result.Success(x.ToString()));

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

    public record TestError(string Message) : Error(1, Message);
}