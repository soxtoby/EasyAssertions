# v3.0.2
## Bug Fixes
 - Removed the `ShouldBe` on nullable structs that was marked as `Obsolete` to push people to `ShouldBeValue` - the small improvement in intellisense isn't worth breaking existing code.    

# v3.0.1
## New Features
 - `ShouldBeValue` for nullable value types.
 - `ShouldNotBeValue` for value types.

## Bug Fixes
 - Removed `class` constraint from `ShouldBe`.

## Breaking Changes
 - Moved `ShouldBeValue` from `EnumAssertions` to `ObjectAssertions`.

# v3.0.0
**NOTE:** This release caused too many problems with the `class` constraint on `ShouldBe` - use v3.0.2 instead.

## New Features
 - Added nullable annotations everywhere.
 - `ValueTask` assertions - `ShouldComplete` and `ShouldFail`.
   - Unfortunately there's no way to specify the exception type directly, but you can chain a type assertion, e.g. `task.ShouldFail().And.ShouldBeA<ArgumentException>()`
 - `IAsyncEnumerable` assertions - `ShouldComplete` and `ShouldFail`.
   - `ShouldComplete` returns an `IReadOnlyList` so you can assert on the items afterwards.
 - A `ShouldMatch` collection assertion that takes in an assertion callback instead of an equality function. This makes it easier to re-use assertion code, and will often provide more useful error messages.
 - ~~Separate `ShouldBe` and `ShouldNotBe` assertions for classes and value types, for better value type IntelliSense.~~ Rolled back in subsequent release because it caused too many problems.
 - Improvements for custom assertions
   - Added `RegisterNotNullAssertion` for custom assertions that assert that the actual value isn't null. Tells the compiler that the actual value isn't null after the assertion has been called.
   - Replaced `RegisterUserAssertion` with simpler `Call` method on `IAssertionContext`, which just takes in a single expression that calls the assertion.
   - `Register(NotNull)Assertion` and `Call` methods optionally take in suffixes to add to the actual and expected expressions, for better expressions when asserting on contents of objects.

## Bug Fixes
 - `ItemsSatisfy` and `AllItemsSatisfy` no longer allow passing in a predicate accidentally.
 - `ShouldBeNull` can no longer be called on non-nullable types.
 - Assertions are tracked within the async context, rather than the thread context, which should fix a lot of problems with incorrect source code in errors.

## Breaking Changes
 - `RegisterUserAssertion` has been removed. Use `Call` instead.
```c#
    // Before
    public static Actual<T> MyCustomAssertion(this T actual, Action<T> assertion)
    {
        return actual.RegisterAssertion(c => {
            actual.RegisterUserAssertion(assertion, () => assertion(actual));
        });
    }
    
    // After
    public static Actual<T> MyCustomAssertion(this T actual, Action<T> assertion)
    {
        return actual.RegisterAssertion(c => {
            c.Call(() => assertion(actual));
        });
    }
```
 - `RegisterIndexedAssertion` has been removed. Pass a suffix into `RegisterAssertion` or `Call` instead.
```c#
    // Before
    public static Actual<T> MyIndexedAssertion(this List<T> actual, T expected)
    {
        return actual.RegisterAssertion(c => {
            actual.RegisterIndexedAssertion(1, () => actual[1].ShouldBe(expected));
        });
    }
    
    // After
    public static Actual<T> MyIndexedAssertion(this List<T> actual, T expected)
    {
        return actual.RegisterAssertion(c => {
            actual.RegisterAssertion(c2 => actual[1].ShouldBe(expected), "[1]");
        });
        
        // or
        
        return actual.RegisterAssertion(c => {
            actual[1].ShouldBe(expected);
        }, "[1]");
    }
```
 - ~~ShouldBeValue has been marked as obsolete. Use `ShouldBe` instead.~~ Rolled back in subsequent release because it caused too many problems.
 - .NET framework requirement has been bumped up to .NET 4.6.1
 - Added dependencies on `IndexRange`, `System.Linq.Async`, and `System.Memory`.
 - `TestExpressionProvider` interface has been renamed to `ITestExpressionProvider`.

# v2.2.0
## New Features
 - .NET Standard 2.0 support

# v2.1.0
## New Features
 - Enum assertions ([981787b](https://github.com/soxtoby/EasyAssertions/commit/981787b5b9f337ea471c4c42753221a9c4e38d67), [#5](https://github.com/soxtoby/EasyAssertions/issues/5)).
   - `ShouldBeValue`, for asserting that value types are equal (mostly useful for enum IntelliSense)
   - `ShouldHaveFlag`, for asserting that a flags enum has a particular flag.

## Bug Fixes
 - NuGet package includes XML documentation ([86d3b28](https://github.com/soxtoby/EasyAssertions/commit/86d3b28625be4c1b6f7b88021e290e806f2e7813), [#6](https://github.com/soxtoby/EasyAssertions/issues/6)).

# v2.0.1
## Bug Fixes
 - Fixed null-ref in `ShouldFail` and `ShouldFailWith` when task is cancelled ([9e907db](https://github.com/soxtoby/EasyAssertions/commit/9e907db5ffed6d298b7c791dd3b6f9dbb0b12447), [#4](https://github.com/soxtoby/EasyAssertions/issues/4)).
 - `IEnumerable<string>`s no longer represented as `IEnumerable<IEnumerable<char>>` in error messages ([f0e1fa2](https://github.com/soxtoby/EasyAssertions/commit/f0e1fa20bfb2a203cc57baa995a26375cb48e6e1), [#2](https://github.com/soxtoby/EasyAssertions/issues/2)).

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

Additionally, `EasyAssertion.RegisterIndexedAssert` has been changed to the extension method `RegisterIndexedAssertion` with a similar format as `RegisterAssertion`. `EasyAssertion.RegisterInnerAssert` has been changed to the extension method `RegisterUserAssert` which has much clearer semantics.

### Assertion failure messages
The dependency on SmartFormat has been removed, and all `FailureMessage` types have been replaced with a single static helper class called `MessageHelper`, which makes it easy to build useful error messages with plain C# interpolated strings. See [Creating Custom Assertions](doc/CustomAssertions.md) for more information.

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
