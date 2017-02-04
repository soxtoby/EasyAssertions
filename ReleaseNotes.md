# v2.0.0
## New Features
 - `ShouldComplete`, for asserting that a Task completes within a particular time span.
 - `ShouldFail` & `ShouldFailWith`, for asserting that a Task fails within a particular time span.
 - `ShouldStartWith`, for asserting that a collection starts with the items of another collection.
 - `ShouldEndWith`, for asserting that a collection ends with the items of another collection.
 - `ShouldBeDistinct`, for asserting that a collection does not contain any duplicates.
 - `ShouldBeASingular`, for asserting that a collection has a single item of the specified type, and returning that item.
 - `ShouldAllBeA`, for asserting that all items in a collection are of the specified type.
 - `ShouldNotMatch`, for asserting that two collections do not have the same items in the same order.
 - Most collection assertions now have an overload that takes a custom equality function.
 - Added a `Buffer.Create()` helper function, which creates an `IBuffer<T>` that can be enumerated multiple times without enumerating the underlying sequence more than once.

## Breaking Changes
### Registering assertions
Major changes have been made to the way assertions are registered, to make it easier to create custom assertions. Unfortunately, this change breaks any existing custom assertions. Fortunately, only minimal changes need to be made to update them.

Assertions can now have both custom assertion logic _and_ use existing assertions, without having to use separate registration functions. Assertion delegates also take in an `IAssertionContext` object, which provides more discoverable access to standard assertion functions, which were previously provided by non-obvious static classes.

Old assertion, using an existing assertion
```c#
public static Actual<int> ShouldBeZero(int actual) {
    return actual.RegisterAssert(a => a.ShouldBe(0));
}
```

New assertion:
```c#
public static Actual<int> ShouldBeZero(int actual) {
    return actual.RegisterAssertion(c => actual.ShouldBe(0));
}
```

Old assertion, using custom assertion logic:
```c#
public static Actual<int> ShouldBeZero(int actual) {
    return actual.RegisterAssert(() => {
        if (!Compare.ObjectsAreEqual(actual, 0))
            throw EasyAssertion.Failure(FailureMessageFormatter.Current.NotEqual(0, actual));
    });
}
```

New assertion:
```c#
public static Actual<int> ShouldBeZero(int actual) {
    return actual.RegisterAssertion(c => {
        if (!c.Test.ObjectsAreEqual(actual, 0))
            throw c.StandardError.NotEqual(0, actual);
    });
}
```

Addtionally, `EasyAssertion.RegisterIndexedAssert` has been changed to the extension method `RegisterIndexedAssertion` with a similar format as `RegisterAssertion`. `EasyAssertion.RegisterInnerAssert` has been changed to the extension method `RegisterUserAssert` which has much clearer semantics.

### Assertion failure messages
The dependency on SmartFormat has been removed, and all `FailureMessage` types have been replaced with a single static helper class called `MessageHelper`, which makes it easy to build useful error messages with plain C# interpolated strings. See [Creating Custom Assertions](doc/CustomAssertions.md]) for more information.

### Number assertions
Number assertion signatures were previously a bit of a mess, in an attempt to stop anyone from asserting on `float`s and `double`s without specifying a tolerance. `float`s and `double`s can now be compared like-for-like with `ShouldBe`, and only mixing types is prohibited. So `(0.1f).ShouldBe(0.1f)` is now OK, but `(0.1f).ShouldBe(0.1)` is not allowed, since comparing a `float` and a `double` requires a tolerance value.

All tolerance values are now `double`s, but this is unlikely to affect any existing tests.

You can no longer compare _any_ `object` with a `float` or `double`. If you have tests that are relying on this, you should either cast the value before asserting on it, or use the `ShouldBeA<>` assertion before comparing to a `float` or `double`.

### Other breaking changes
- Assembly is no longer signed.
- `ShouldMatch` and `ShouldMatchReferences` no longer allow items to be specified as parameters. This was leading to ambiguous situations, particularly with `string`s, and the error messages did not work correctly.
- `ShouldMatch` and `ShouldMatchReferences` are no longer allowed on `IDictionary` and `ISet`, since their order cannot be relied upon. If you have a custom implementation where order _is_ reliable, you can use the `AsEnumerable()` extension to get around this, or create a custom assertion specifically for your implementation.
- `ShouldNotContain`'s generic parameters have been changed to match `ShouldContain`'s. This allows the non-expected items to be sub-types of the actual enumerable's item type, but the assertion will now return an `IEnumerable<>` instead of the same type as the original enumerable.
- `ShouldOnlyContain`, `ShouldContainItems`, and `ItemsShouldBeIn` no longer ignore duplicates. Duplicates in one collection must be in the other collection as well.
- Removed `ShouldNotOnlyContain`. It had limited application, and with the new handling of duplicates it became even less useful.

## Other Changes and Fixes
- Collections are no longer enumerated more than once per assertion. Multiple assertions will still enumerate the collection multiple times.
- Nested `IEnumerable`s' contents are displayed in error messages, instead of just e.g. ```IEnumerable`string```.
- Fixed a bunch of bugs around picking up source expressions that include brackets.
- Expected expressions that include the 'actual' parameter (e.g. `actual.Assert(x => x.ShouldBe(Foo(x)))`) are now filled in with the actual expression in error messages.
- Long strings that differ near the end are now lined up properly in error messages.
- Actual expressions with lots of indenting are shifted to the left to make them easier to read.

# v1.0.0
## New Assertions
- `ItemsShouldBeIn`, for asserting that a collection is a subset of another collection.
- `ShouldNotOnlyContain`, for asserting that a collection is not a subset of another collection.
- `ShouldMatch` for tree structures.
- `ShouldBe` for strings with optional case insensitivity.
- `Should.Throw` with no generic parameter that expect any Exception type.
- `AndShouldNotBeA`, for asserting that the actual value of `Should.Throw` is not of a particular type.
- `ShouldBeEmpty`, for asserting that a `string` is empty.
- `ShouldMatch` & `ShouldNotMatch`, for asserting on `string`s with regular expressions.
      
## Other Changes and Fixes:
- Added null checks to all assertions.
- Fixed an exception that often occurred while running several tests in parallel.
- Fixed difference index when actual string is longer than expected string.
- `string` assertion failure messages no longer insert backslashes before braces.