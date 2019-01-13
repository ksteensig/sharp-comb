using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonadicParserCombinator
{
    public delegate Result<T> Parser<T>(Input input);

    public static class Parser
    {
        public static Parser<U> Select<T, U>(this Parser<T> p, Func<T, U> map) => i =>
            p(i).IfSuccess<T, U>(head =>
                Result<U>.Success(map(head.Value), head.Remainder));
        
        public static Parser<V> SelectMany<T, U, V>(this Parser<T> p, Func<T, Parser<U>> bind, Func<T, U, V> map) => i =>
            p(i).IfSuccess(r1 =>
                bind(r1.Value)(r1.Remainder).IfSuccess(r2 =>
                    Result<V>.Success(map(r1.Value, r2.Value), r2.Remainder)));

        public static Parser<T> Where<T>(this Parser<T> p, Func<T, bool> predicate) => i =>
            p(i).IfSuccess(result =>
                predicate(result.Value)
                    ? Result<T>.Success(result.Value, result.Remainder)
                    : Result<T>.Failure(i, "Didn't match the predicate"));

        public static Parser<U> Then<T, U>(this Parser<T> first, Func<T, Parser<U>> second) => i =>
            first(i).IfSuccess(result => second(result.Value)(result.Remainder));

        public static Parser<char> Any = i =>
            !i.AtEnd
            ? Result<char>.Success(i.Current, i.Next())
            : Result<char>.Failure(i, "End of input");
        
        public static Func<char, Parser<char>> Char = c => Any.Where(result => result == c);

        public static Func<char, Parser<char>> NotChar = c => Any.Where(result => result != c);

        public static Parser<char> Digit = Any.Where(result => char.IsDigit(result));

        public static Parser<string> Text(this Parser<IEnumerable<char>> p) =>
            from chars in p
            select string.Concat(chars.ToArray());
            
        public static Parser<IEnumerable<char>> Integer = Digit.ManyOne();
        public static Parser<IEnumerable<char>> Decimal = 
            from first in Digit.ManyOne()
            from dot in Char('.')
            from second in Digit.ManyOne()
            select first.Append(dot).Concat(second);

        public static Parser<IEnumerable<char>> CharSequence(string s) => i =>
        {
            var results = new List<char>();
            var result = Parser.Char(s[0])(i);

            for (int j = 1; j < s.Length; j++)
            {
                if (result.IsSuccess)
                {
                    results.Add(result.Value);
                    result = Parser.Char(s[j])(i);
                }
                else
                {
                    return Result<IEnumerable<char>>.Failure(i, "Didn't match the char sequence");
                }
            }

            return Result<IEnumerable<char>>.Success(results.AsEnumerable(), result.Remainder);
        };

        public static Parser<IEnumerable<char>> UntilFalse(Func<char, bool> predicate) => i =>
        {
            var results = new List<char>();
            var result = Any(i);

            while (result.IsSuccess && predicate(result.Value))
            {
                results.Add(result.Value);
                result = Any(i);
            }

            return Result<IEnumerable<char>>.Success(results.AsEnumerable(), result.Remainder);
        };

        public static Parser<IEnumerable<T>> Many<T>(this Parser<T> p) => i =>
        {
               var results = new List<T>();
               var result = p(i);
               while (result.IsSuccess)
               {
                   results.Add(result.Value);
                   result = p(result.Remainder);
               }
               return Result<IEnumerable<T>>.Success(results.AsEnumerable(), result.Remainder);
        };

        public static Parser<IEnumerable<T>> ManyOne<T>(this Parser<T> p) => i =>
                p(i).IfSuccess(head => {
                    var tail = p.Many<T>()(head.Remainder);
                    var list = tail.Value.Prepend(head.Value);
                    return Result<IEnumerable<T>>.Success(list, tail.Remainder);});

        public static Parser<T> EitherOf<T>(this IEnumerable<Parser<T>> parsers) => i =>
            parsers.Select(p => p(i))
                   .SkipWhile(r => !r.IsSuccess)
                   .DefaultIfEmpty(Result<T>.Failure(i, "Couldn't match any parser"))
                   .First();
        
        public static IEnumerable<T> ReturnIEnumerable<T>(this T value) => new[] { value }.AsEnumerable();

        public static Parser<T> ReturnParser<T>(this T value) => i => Result<T>.Success(value, i);
        
        public static Parser<IEnumerable<T>> SeperatedBy<T, U>(this Parser<T> p, Parser<U> sep) =>
            from first in (from r in p
                           from s in sep
                           select r).Many()
            from last in p
            select first.Concat(last.ReturnIEnumerable());
        
        public static Parser<T> EndOfInput<T>(this Parser<T> p) => i =>
            p(i).IfSuccess(result =>
                    result.Remainder.AtEnd
                    ? result
                    : Result<T>.Failure(i, "Not end of input"));

        public static Result<T> TryParse<T>(this Parser<T> p, string input) => p(new Input(input));
    }
}