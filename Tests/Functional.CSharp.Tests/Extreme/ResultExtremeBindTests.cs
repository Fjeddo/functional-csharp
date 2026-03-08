namespace Functional.CSharp.Tests.Extreme;

using Functional.CSharp.Extreme;

[TestClass]
public class ResultExtremeBindTests
{
    [TestMethod]
    public void Should_bind_success_to_success()
    {
        var sut = Result.Success<int, int>(5);

        var bound = sut.Bind(x => Result.Success<int, int>(x * 2));

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
}