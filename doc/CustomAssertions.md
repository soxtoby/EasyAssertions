# Creating Custom Assertions
Creating custom assertions has been made as easy as possible, but you _do_ need to follow a few guidelines in order for EasyAssertions to be able to show the correct source code when an assertion fails.

# Anatomy of an Assertion Method
Let's say you wanted to assert that the numbers in a `List<int>` add up to a particular number.
```c#
numberOfEvilRobots = 10;
List<int> evilRobotsDestroyed = KillAllTheRobots();
evilRobotsDestroyed.ShouldTotal(numberOfEvilRobots);
```

Such an assertion method might look something like this:
```c#
public static Actual<List<int>> ShouldTotal(this List<int> actual, int expectedTotal)
{
    return actual.RegisterAssertion(assertionContext =>
        {
            if (actual.Sum() != expectedTotal)
                throw assertionContext.Error.Custom($"should total {expectedTotal}, but totalled {actual.Sum()}");
        });
}
```
There are a number of important aspects to this method:

1. It is an extension method. All assertions _must_ be extension methods.
2. The `Actual<>` return type. This isn't required, but it allows assertions to be chained (e.g. `myList.ShouldTotal(10).And.ShouldContain(5);`).
3. `actual.RegisterAssertion()`. This tells EasyAssertions that this is an assertion method. You must call this directly from your assertion method, otherwise the wrong method will be identified as the assertion method.
`RegisterAssertion` returns the actual value wrapped in an `Actual<>` object, so you can just return the result directly, as in this example.
Feel free to ignore the return value if you want to return something different from your assertion method.
4. `assertionContext`. The delegate passed in to `RegisterAssertion` takes in a context object that provides helpers for creating custom assertions that behave similarly to the built-in assertions.
4. No `Exception` is created directly in this method. Instead, `assertionContext.Error.Custom()` creates the exception to be thrown.
This allows the exception type to be overridden via `EasyAssertion.UseFrameworkExceptions()` so the appropriate exception type is thrown for the unit testing framework you're using.

This method is functional, but when it fails, the message just says something like

    should total 10, but totalled 8

which doesn't really give us much information. _What_ totalled 8? And why?

# Building a Better Error Message
EasyAssertions contains a `FailureMessage` class to help build useful messages. `FailureMessage` is a static class, so you can use its methods directly by statically importing it with:
```c#
using static EasyAssertions.FailureMessage;
```
For our `ShouldTotal` assertion, building an error message might look like this:
```c#
throw assertionContext.Error.Custom($@"{ActualExpression}
should total {Expected(expectedTotal, @"
             ")}
but totalled {Value(actual.Sum())}");
```
In this example, we're using three members from `FailureMessage`:

1. `ActualExpression` - outputs the **actual expression** (the text of the source code that was asserted on with `ShouldTotal`).
2. `Expected` - outputs the **expected expression** and the expected value. The string full of whitespace, passed in as the second argument, is to line things up on separate lines.
3. `Value` - outputs a value, handily wrapped in < >, or " " if the actual value was a string.

Putting `ActualExpression` at the top of an error message is so common that `assertionContext.Error` provides a `WithActualExpression` method that does this for you.

Also note that we're making use of verbatim strings so we can put new-lines directly into the message. This may look a little odd, but makes it much easier to line up different parts of the message.

Replacing our simple message with our new `FailureMessage`-based message, our assertion now fails with something like
```
evilRobotsDestroyed
should total numberOfEvilRobots
             <10>
but totalled <8>
```
which gives us a much better idea of what's being asserted on. It'd be nice to know what was in that list though.

Fortunately, `FailureMessage` provides some handy methods for outputting the contents of collections.
We can replace our message with
```c#
throw assertionContext.Error.Custom($@"{ActualExpression}
should total {Expected(expectedTotal, @"
             ")}
but was {Sample(actual)}
totalling {Value(actual.Sum())}");
```
which produces a message like
```
evilRobotsDestroyed
should total numberOfEvilRobots
             <10>
but was [
    <1>,
    <3>,
    <4>
]
totalling <8> 
```
Excellent!

# Wrapping it Up
Since the assertion is an extension method, it needs to be in a static class.
Also, you'll often want to provide some extra context for an assertion via an optional message parameter, so we'll add that too (using the `OnNewLine()` extension to put it on the next line down), and combine everything into the final assertion.
```c#
public static class CustomAssertions
{
    public static Actual<List<int>> ShouldTotal(this List<int> actual, int expectedTotal, string message = null)
    {
        return actual.RegisterAssertion(assertionContext =>
            {
                if (actual.Sum() != expectedTotal)
                    throw assertionContext.Error.WithActualExpression($@"
should total {Expected(expectedTotal, @"
             ")}
but was {Sample(actual)}
totalling {Value(actual.Sum())}" + message.OnNewLine());
            });
    }
```

Finally, we can assert like we've always wanted to:
```c#
numberOfEvilRobots = 10;
List<int> evilRobotsDestroyed = KillAllTheRobots();
evilRobotsDestroyed.ShouldTotal(numberOfEvilRobots, "You are killed by the remaining robots.");
```
and get a seriously useful exception message when the assertion fails:
```
evilRobotsDestroyed
should total numberOfEvilRobots
             <10>
but was [
    <1>,
    <3>,
    <4>
]
totalling <8>
You are killed by the remaining robots.
```

# What's next?
`assertionContext.StandardError` provides access to all the errors thrown by the built-in assertions. Often, custom assertions will use their own testing logic, but can re-use these standard errors.

If you want to create more complicated error messages, then the [implementation of StandardError](../EasyAssertions/StandardErrors.cs) provides plenty of examples.