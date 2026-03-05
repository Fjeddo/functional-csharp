namespace Functional.CSharp.Tests;

[TestClass]
public class ResultMapTests
{
    [TestMethod]
    public void Should_map_success_value()
    {
        var sut = Result.Success(5);

        var mapped = sut.Map(x => x * 2);

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
    public void Should_map_success_value_to_different_type()
    {
        var sut = Result.Success(5);

        var mapped = sut.Map(x => x.ToString());

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
    public void Should_not_map_failure()
    {
        var sut = Result.Failure<int>(new TestError("this is an expected error"));

        var mapped = sut.Map(x => x * 2);

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
    public void Should_preserve_error_on_map_failure()
    {
        var expectedError = new TestError("original error");
        var sut = Result.Failure<int>(expectedError);

        var mapped = sut.Map(x => x.ToString());

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
    public void Should_chain_multiple_maps()
    {
        var sut = Result.Success(5);

        var result = sut
            .Map(x => x * 2)
            .Map(x => x + 3)
            .Map(x => x.ToString());

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