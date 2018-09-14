# Semantic Types

This is the demo code from my presentation *Typing from back to front* on the [Hamburg .NET Usergroup on the 2. March 2016](http://www.meetup.com/Hamburg-C-Net-Meetup/events/227288536/). We have been 41 people in total and it was a great night! Thanks everyone for coming!


## About

Semantic types are value types which are given a semantic meaning.

Let's take this simply method for example:

```csharp
public Passenger GetPax(int paxId) {..}
```

This seems pretty clear and straightforward, but it has slight issues. Mainly: What prevents us from providing a wrong id? For example somewhere else we have a `int orderId`, which we now pass to the method.

*What? We're allowed to do this? THIS IS MADNESS! In what world is an order Id a passenger id?*

This of course is slightly exaggerated, but it has a valid point: Why are we allowed to do this? Why doesn't the compiler stop us? Because for the compiler there is no difference, both are integers. Nothing more. But of course we know this already.

We can solve this issue by creating a wrapper type, which wraps an integer.


## How to create a semantic type

Let's take the example from before and create a `PaxId` wrapper type, which wraps the `int` value.

```csharp
public struct PaxId
{
    // Mutable?!
    public int Value { get; set; }
}
```

This is the very first primitive approach, but it has a heavy drawback: The `int` was an immutable type, whereas this code is mutable. So let's adjust this drawback:

```csharp
public struct PaxId
{
    // Constructor that accepts the value.
    public PaxId(int value)
    {
        _value = value;
    }

    // Returns the internally stored value.
    public int Value { get { return _value; } }

    // Readonly, so it won't be changed!
    private readonly int _value;
}
```

By providing a constructor and storing the value in a readonly field we made this value immutable. We have the rule that passenger numbers are always equal or larger than 0. We can even inforce this step in the constructor now!

```csharp
    // Constructor that verifies the value is valid.
    public PaxId(int value)
    {
        if (value < 0)
            throw new ArgumentException("The value must be larger or equal than 0!", nameof(value));

        _value = value;
    }
```

Next step is comparison: Usually we want to compare the types, so we need to override a method and introduce a few more methods.

```csharp
public struct PaxId
{
    public PaxId(int value)
    {
        if (value < 0)
            throw new ArgumentException("The value must be larger or equal than 0!", nameof(value));

        _value = value;
    }

    public int Value { get { return _value; } }

    private readonly int _value;

    // Provide a equals method that operates on the type we introduce.
    // No need to check for null as this is a struct that can't be null.
    public bool Equals(PaxId other) => _value == other._value;

    // Define the comparison operators.
    public static bool operator==(PaxId first, PaxId second) => first.Equals(second);
    public static bool operator!=(PaxId first, PaxId second) => !(first == second);

    // Override the equals method of the base class (object).
    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != typeof(PaxId))
            return false;

        var otherPaxId = (PaxId)obj;
        return Equals(otherPaxId);
    }
}
```

Wow, this gets qite big already. A lot of boilerplate code - but let's not focus on this for now.

Very often we have the need to have an "undefined" or "not-set" state of the field. With `int` we'd usually make it *nullable* and just use `int?`. Of course we could do this too with semantic types, but there's a better option: Use a *nullable* backing field, and use a default value for comparison:

```csharp
public struct PaxId
{
    public PaxId(int value)
    {
        if (value < 0)
            throw new ArgumentException("The value must be larger or equal than 0!", nameof(value));

        _value = value;
    }

    // Since the backing field is now nullable,
    // we return an "invalid" value when the backing field is null.
    // The invalid value is -1, since this can't be passed to the constructor.
    public int Value { get { return _value ?? -1; } }

    // Make the backing field nullable.
    private readonly int? _value;

    public bool Equals(PaxId other) => _value == other._value;
    public static bool operator==(PaxId first, PaxId second) => first.Equals(second);
    public static bool operator!=(PaxId first, PaxId second) => !(first == second);

    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != typeof(PaxId))
            return false;

        var otherPaxId = (PaxId)obj;
        return Equals(otherPaxId);
    }

    // Define a default value for comparisons.
    // Always equals default(PaxId), the uninitialized state.
    public static readonly PaxId Default = default(PaxId);
}
```

Now we can have simply compare the value and see if it was defined (the constructor was used with a correct value), or if it was uninitialized:

```csharp
new PaxId(1) == new PaxId(1)    // true
new PaxId(0) == new PaxId(0)    // true
new PaxId() == PaxId.Default    // true
default(PaxId) == PaxId.Default // true
new PaxId(1) == new PaxId(0)    // false
new PaxId(1) == new PaxId()     // false
new PaxId(1) == default(PaxId)  // false
new PaxId(1) == PaxId.Default   // false
```

Lastly, because we want to print the value type as a regular `int` and use it in dictionaries, we also should override `GetHashCode` and `ToString`:


```csharp
public struct PaxId
{
    public PaxId(int value)
    {
        if (value < 0)
            throw new ArgumentException("The value must be larger or equal than 0!", nameof(value));

        _value = value;
    }

    public int Value { get { return _value ?? -1; } }

    private readonly int? _value;

    public bool Equals(PaxId other) => _value == other._value;
    public static bool operator==(PaxId first, PaxId second) => first.Equals(second);
    public static bool operator!=(PaxId first, PaxId second) => !(first == second);

    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != typeof(PaxId))
            return false;

        var otherPaxId = (PaxId)obj;
        return Equals(otherPaxId);
    }

    public static readonly PaxId Default = default(PaxId);

    // If no value is set, use 0 instead.
    public override int GetHashCode() => _value?.GetHashCode() ?? 0;

    // Just print the value. The check for the null value is done in the property.
    public override string ToString() => Value.ToString();
}
```


## Advantages

That is **a lot** of code! What do we gain from this?

### Not mixable types

We have an explicit difference between a `OrderId` and a `PaxId`. Methods accepting a `PaxId` can't accidentally receive a `OrderId` as an argument.

### More expressive code
Our code is a lot more expressive. At work I had a case where we had a `SsrCode`, and a `SsrGroupCode`. Both values are 4 letter upper-case strings, sometimes even the same value. It was very easy to mix those two. In one case we needed to map the `SsrCode` to it's matching `SsrGroupCode`, for which we created a dictionary:

```csharp
var ssrGroupCodeToSsrGroupCodeMapping = new Dictionary<string, string>();
...
// ssrGroupCodeToSsrGroupCodeMapping[ <accepts a string> ]
```

That's... not nice to read. By introducing a `SsrCode` and `SsrGroupCode` semantic type we could improve the readability a lot:

```csharp
var mapping = new Dictionary<SsrCode, SsrGroupCode>();
...
// mapping[ <accepts a SsrCode> ]
```

### Backing field defined in one place

Let's assume we later switch, for whatever reasons, the value of the `PaxId` from `int` to `string`. If we use value types we only have to change the type in one place. Neat-o!


## Boilerplate vs magic

The introduction of semantic types takes a lot of boiler plate code, that is given. The code above is completely undocumented and it already takes plenty of lines. But we can easily make this nicer to read by using the magic that snippets are, functionality of most editors like Visual Studio.

![Snippet demonstration](doc/snippet-demo.gif)


## Questions

Got any question? Feel free to send me a message anywhere. I'm happy to help and answer any question.
