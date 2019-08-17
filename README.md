# Immutable Class

August 17, 2019 Update

New support for immutable value types (string, bool, int, etc.) has been added. 


Using string as an example, there are several ways to create an immutable string variable:

var x = "Foo".ToImmutable();

var x = new ImmutableString("Foo");

It's the value that is immutable: 

x.Value = "A new value"; //This would not compile because there is no setter.


If you need to edit a value and then make it immutable, you first have to make it mutable, make your changes, and then invoke the ToImmutable extension:

var x = "foo".ToImmutable().Value.ToUpper().ToImmutable(); //x == "FOO"

-----------------

This class consumes and extends the system.collections.immutable package from Microsoft. While system.collections.immutable contains the building blocks for a comprehensive immutable class, it does not address a method for putting it all together. This project addresses that gap.

For details on how to implement, checkout my article in CODE Magazine: https://www.codemag.com/Article/1905041/Immutability-in-C#.
