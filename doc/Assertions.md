Most assertions require the expected type to be the actual's type or a derivative, so if you're asserting on the return value of a method, and that return value's type changes, the test will no longer compile.

Almost all assertions take in an optional message as their last parameter.

# Equality
```c#
// objects
"foo".ShouldBe("foo");
"foo".ShouldNotBe("bar");

// floats and doubles
(1.2).ShouldBe(1, 0.2);    // 1, within a tolerance of 0.2
(1.2).ShouldNotBe(1, 0.1); // not 1, with a tolerance of 0.1
double.NaN.ShouldBeNaN();  // also works for float.NaN
(1.2).ShouldNotBeNaN();
```

# Reference Equality
```c#
var obj = new object();
obj.ShouldReferTo(obj);
obj.ShouldNotReferTo(new object());
```

# Nulls
```c#
null.ShouldBeNull();
new object().ShouldNotBeNull();
```

# Strings
```c#
"FOO".ShouldBe("foo", Case.Insensitive);
"foo".ShouldStartWith("fo");
"foo".ShouldEndWith("oo");
"foobar".ShouldContain("oba");
"foo".ShouldNotContain("bar");
"".Empty.ShouldBeEmpty();
"foo".ShouldMatch("f.+");
"foo".ShouldMatch("f.+", RegexOptions.IgnoreCase);
```

# Comparables
```c#
2.ShouldBeGreaterThan(1);
1.ShouldBeLessThan(2);
```

# Collections
```c#
Enumerable.Empty<int>().ShouldBeEmpty();

var singleItemCollection = new[] { 1 };
singleItemCollection.ShouldBeSingular();
singleItemCollection.ShouldBeASingular<int>();
    
var collection = new[] { 1, 2, 3 };
collection.ShouldNotBeEmpty();
collection.ShouldBeLength(3);
collection.ShouldContain(2);
collection.ShouldNotContain(4);
collection.ShouldContainItems(new[] { 2, 1 });
collection.ShouldNotContainItems(new[] { 4, 5 });
collection.ShouldOnlyContain(new[] { 3, 2, 1 });
collection.ShouldMatch(new[] { 1, 2, 3 });
collection.ShouldMatch(1, 2, 3);
collection.ShouldNotMatch(new[] { 2, 1, 3 });
collection.ItemsShouldBeIn(new[] { 4, 2, 1, 3, 5 });
collection.ShouldNotOnlyContain(new[] { 3, 2 });
collection.ShouldStartWith(new[] { 1, 2 });
collection.ShouldEndWith(new[] { 2, 3 });
collection.ShouldBeDistinct();

var keyedCollection = new KeyedOnFirstCharCollection { "foo" }; // KeyedCollection<char, string>
keyedCollection.ShouldContainKey('f');
keyedCollection.ShouldNotContainKey('b');
    
// floats or doubles with a tolerance
new[] { 1.2, 2.1 }.ShouldMatch(new[]{ 1, 2 }, 0.2);
    
// Match on reference equality
var a = new object(), b = new object();
var objects = new[] { a, b };
objects.ShouldMatchReferences(new[] { a, b });
objects.ShouldMatchReferences(a, b);
    
// Assert on each item
collection.AllItemsSatisfy(i => i.ShouldNotBe(3));
collection.ItemsSatisfy(i1 => i1.ShouldBe(1), i2 => i2.ShouldBe(2));
```

# Types
```c#
object obj = new List();
obj.ShouldBeA<List>();
```

# Expected Exceptions
```c#
Should.Throw<NullReferenceException>(() => ((object)null).ToString());
Should.Throw(() => ((object)null).ToString())
    .AndShouldNotBeA<ArgumentNullException>();
```

# Tasks
```c#
var completedTask = Task.FromResult(0);
completedTask.ShouldComplete();    // Default timeout of 1s
completedTask.ShouldComplete(100); // ms
completedTask.ShouldComplete(TimeSpan.FromMilliseconds(100));

var failingTask = Task.Run(() => { throw new NotImplementedException(); });
failingTask.ShoulFail<NotImplementedException>();     // Default timeout of 1s
failingTask.ShouldFail<NotImplementedException>(100); // ms
failingTask.ShouldFail<NotImplementedException>(TimeSpan.FromMilliseconds(100));
failingTask.ShouldFail()
    .AndShouldNotBeA<ArgumentNullException>();
```

# Continuing Assertions
```c#
var collection = new[] { 1, 2 };
collection.ShouldNotBeEmpty()
    .And.ShouldNotMatch(new[] { 2, 1 })
    .And.Length.ShouldBe(2);
    
var obj = new KeyValuePair<string, string>("foo", "bar");
obj.Assert(kv => kv.Key.ShouldBe("foo"))
    .And(kv => kv.Value.ShouldBe("bar"));
```