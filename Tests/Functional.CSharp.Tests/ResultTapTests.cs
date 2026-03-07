namespace Functional.CSharp.Tests;

[TestClass]
public sealed class ResultTapTests
{
    [TestMethod]
    public void Should_invoke_tap_action_on_success()
    {
        var sut = Result.Success(5);
        var wasCalled = false;
        var capturedValue = 0;

        var tapped = sut.Tap(value =>
        {
            wasCalled = true;
            capturedValue = value;
        });

        Assert.AreSame(sut, tapped);
        Assert.IsTrue(wasCalled);
        Assert.AreEqual(5, capturedValue);
    }

    [TestMethod]
    public void Should_not_invoke_tap_action_on_failure()
    {
        var expectedError = new TestError("this is an expected error");
        var sut = Result.Failure<int>(expectedError);
        var wasCalled = false;

        var tapped = sut.Tap(_ => wasCalled = true);

        Assert.AreSame(sut, tapped);
        Assert.IsFalse(wasCalled);

        tapped.Match(
            onSuccess: _ =>
            {
                Assert.Fail("Expected failure, but got success.");
                return 0;
            },
            onFailure: error =>
            {
                Assert.AreSame(expectedError, error);
                return 0;
            });
    }

    [TestMethod]
    public void Should_chain_multiple_taps_on_success()
    {
        var sut = Result.Success(5);
        var sum = 0;

        var tapped = sut
            .Tap(x => sum += x)
            .Tap(x => sum += x * 2);

        Assert.AreSame(sut, tapped);
        Assert.AreEqual(15, sum);
    }

    public record TestError(string Message) : Error(1, Message);
}
