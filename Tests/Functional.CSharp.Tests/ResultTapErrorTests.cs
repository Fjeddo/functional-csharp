namespace Functional.CSharp.Tests;

[TestClass]
public sealed class ResultTapErrorTests
{
    [TestMethod]
    public void Should_invoke_tap_error_action_on_failure()
    {
        var expectedError = new TestError("this is an expected error");
        var sut = Result.Failure<int>(expectedError);
        var wasCalled = false;
        Error? capturedError = null;

        var tapped = sut.TapError(error =>
        {
            wasCalled = true;
            capturedError = error;
        });

        Assert.AreSame(sut, tapped);
        Assert.IsTrue(wasCalled);
        Assert.AreSame(expectedError, capturedError);
    }

    [TestMethod]
    public void Should_not_invoke_tap_error_action_on_success()
    {
        var sut = Result.Success(5);
        var wasCalled = false;

        var tapped = sut.TapError(_ => wasCalled = true);

        Assert.AreSame(sut, tapped);
        Assert.IsFalse(wasCalled);

        tapped.Match(
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
    }

    [TestMethod]
    public void Should_chain_multiple_tap_error_on_failure()
    {
        var sut = Result.Failure<int>(new TestError("this is an expected error"));
        var callCount = 0;

        var tapped = sut
            .TapError(_ => callCount++)
            .TapError(_ => callCount++);

        Assert.AreSame(sut, tapped);
        Assert.AreEqual(2, callCount);
    }

    public record TestError(string Message) : Error(1, Message);
}
