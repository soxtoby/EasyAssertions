# EasyAssertions
Easy-to-use fluent assertions with actually useful failure messages.

Instead of...

```c#
var a = "foo";
Assert.AreEqual("foo", a);
```

write

```c#
var a = "foo"
a.ShouldBe("foo");
```

...and if something goes wrong, get an actually useful error message:

```c#
var name = "Frod";
name.ShouldBe("Fred");
```
    >> name
    >> should be "Fred"
    >> but was   "Frod"
    >>              ^
    >> Difference at index 2.

One assertion not enough? Just keep going! EasyAssertions will keep track of what you're really asserting on:
```c#
var name = "Fred";
name.ShouldEndWith("ed")
    .And.Length.ShouldBe(5);        
```
    >> name.Length
    >> should be <5>
    >> but was   <4>

# Getting Started
Using EasyAssertions is as easy as installing the [NuGet package](https://www.nuget.org/packages/EasyAssertions/).

# Documentation
See [examples of all the available assertions](doc/Assertions).

Or find out [how to make your own assertions](doc/CustomAssertions).