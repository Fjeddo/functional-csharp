namespace Functional.CSharp.Tests;

[TestClass]
public class ResultTests
{
    [TestMethod]
    public void Should_return_Success()
    {
        var sut = Result.Success(5);

        var result = sut.Match(
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
    public void Should_return_Failure()
    {
        var sut = Result.Failure<int>(new TestError("this is an expected error"));

        var result = sut.Match(
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
    public void Should_return_Result()
    {
        var sut = 5.ToResult();

        Assert.IsInstanceOfType<Result<int>>(sut);
        var result = sut.Match(
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

    public record TestError(string Message) : Error(1, Message);
}