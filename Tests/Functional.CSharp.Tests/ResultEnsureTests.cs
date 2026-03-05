namespace Functional.CSharp.Tests;

[TestClass]
public sealed class ResultEnsureTests
{
    [TestMethod]
    public void Should_ensure_success_when_predicate_returns_true()
    {
        var sut = Result.Success(10);

        var ensured = sut.Ensure(
            x => Result.Success(x > 5),
            new TestError("Value must be greater than 5"));

        var result = ensured.Match(
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
    public void Should_ensure_failure_when_predicate_returns_false()
    {
        var sut = Result.Success(3);

        var ensured = sut.Ensure(
            x => Result.Success(x > 5),
            new TestError("Value must be greater than 5"));

        var result = ensured.Match(
            onSuccess: value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return value;
            },
            onFailure: error =>
            {
                Assert.IsInstanceOfType<TestError>(error);
                Assert.AreEqual("Value must be greater than 5", error.Message);
                return 0;
            });

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Should_not_ensure_when_result_is_failure()
    {
        var originalError = new TestError("original error");
        var sut = Result.Failure<int>(originalError);

        var ensured = sut.Ensure(
            x => Result.Success(x > 5),
            new TestError("Value must be greater than 5"));

        var result = ensured.Match(
            onSuccess: value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return value;
            },
            onFailure: error =>
            {
                Assert.AreSame(originalError, error);
                Assert.AreEqual("original error", error.Message);
                return 0;
            });

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Should_preserve_original_error_when_result_is_failure()
    {
        var expectedError = new TestError("original error");
        var sut = Result.Failure<int>(expectedError);

        var ensured = sut.Ensure(
            x => Result.Success(true),
            new TestError("this should not be used"));

        ensured.Match(
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
    public void Should_propagate_predicate_failure()
    {
        var predicateError = new TestError("predicate failed");
        var sut = Result.Success(10);

        var ensured = sut.Ensure(
            x => Result.Failure<bool>(predicateError),
            new TestError("this should not be used"));

        var result = ensured.Match(
            onSuccess: value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return value;
            },
            onFailure: error =>
            {
                Assert.AreSame(predicateError, error);
                Assert.AreEqual("predicate failed", error.Message);
                return 0;
            });

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Should_chain_multiple_ensures()
    {
        var sut = Result.Success(10);

        var result = sut
            .Ensure(
                x => Result.Success(x > 5),
                new TestError("Value must be greater than 5"))
            .Ensure(
                x => Result.Success(x < 20),
                new TestError("Value must be less than 20"))
            .Ensure(
                x => Result.Success(x % 2 == 0),
                new TestError("Value must be even"));

        result.Match(
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
    }

    [TestMethod]
    public void Should_fail_on_first_ensure_violation_in_chain()
    {
        var sut = Result.Success(25);

        var result = sut
            .Ensure(
                x => Result.Success(x > 5),
                new TestError("Value must be greater than 5"))
            .Ensure(
                x => Result.Success(x < 20),
                new TestError("Value must be less than 20"))
            .Ensure(
                x => Result.Success(x % 2 == 0),
                new TestError("Value must be even"));

        result.Match(
            onSuccess: value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return value;
            },
            onFailure: error =>
            {
                Assert.IsInstanceOfType<TestError>(error);
                Assert.AreEqual("Value must be less than 20", error.Message);
                return 0;
            });
    }

    [TestMethod]
    public void Should_combine_ensure_with_map_and_bind()
    {
        var sut = Result.Success(5);

        var result = sut
            .Map(x => x * 2)
            .Ensure(
                x => Result.Success(x > 5),
                new TestError("Value must be greater than 5"))
            .Bind(x => Result.Success(x.ToString()));

        result.Match(
            onSuccess: value =>
            {
                Assert.AreEqual("10", value);
                return value;
            },
            onFailure: _ =>
            {
                Assert.Fail("Expected success, but got failure.");
                return string.Empty;
            });
    }

    [TestMethod]
    public void Should_ensure_with_complex_predicate()
    {
        var sut = Result.Success("hello");

        var ensured = sut.Ensure(
            x => Result.Success(x.Length >= 3 && x.Length <= 10),
            new TestError("String length must be between 3 and 10"));

        ensured.Match(
            onSuccess: value =>
            {
                Assert.AreEqual("hello", value);
                return value;
            },
            onFailure: _ =>
            {
                Assert.Fail("Expected success, but got failure.");
                return string.Empty;
            });
    }

    [TestMethod]
    public void Should_use_provided_error_when_predicate_returns_false()
    {
        var expectedError = new TestError("Custom validation error");
        var sut = Result.Success(100);

        var ensured = sut.Ensure(
            x => Result.Success(x < 50),
            expectedError);

        ensured.Match(
            onSuccess: _ =>
            {
                Assert.Fail("Expected failure, but got success.");
                return 0;
            },
            onFailure: error =>
            {
                Assert.AreSame(expectedError, error);
                Assert.AreEqual("Custom validation error", error.Message);
                return 0;
            });
    }

    public record TestError(string Message) : Error(1, Message);
}