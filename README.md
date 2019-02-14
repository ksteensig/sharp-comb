# Sharp Comb - Monadic Parser Combinator
Sharp Comb  is a monadic parser combinator library implemented in C#.

It's bundled as a library in the src folder. While in the samples folder an extremely simple example can be found.

## Example

The parser library can be used through the LINQ comprehension or fluent syntax.

In this example this simple class represents an addition.

```
public class Addition
{
    int x;
    int y;
    public Addition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public void Evaluate()
    {
        Console.WriteLine(x + y);
    }
}
```

Then the following two parsers are implemented.

The first one `Number` is used to parse a number. So it returns a `Parser<int>`.

```
public static Parser<int> Number =
            from x in Parser.Integer.Text()
            select int.Parse(x);
```

The next one `Addition` is used to parse an `Addition`. It uses the `Number` parser to parse the numbers for the `Addition`.

```
public static Parser<Addition> Addition =
            from x  in Number
            from s1 in Parser.Char(' ').Many()
            from op in Parser.Char('+')
            from s2 in Parser.Char(' ').Many()
            from y  in Number.EndOfInput()
            select new Addition(x, y);
```

The parsing is lazily evaluated, so it's only evaluated when required.

To use the parser you need to use the `public static Result<T> TryParse<T>(this Parser<T> p, string input)` method which takes a parser and a string as input.
