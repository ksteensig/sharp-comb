using System;
using System.Collections.Generic;
using System.Linq;

namespace MonadicParserCombinator
{
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

    public static class CalculatorParser
    {
        public static Parser<int> Number =
            from x in Parser.Integer.Text()
            select int.Parse(x);
        
        public static Parser<Addition> Addition =
            from x  in Number
            from s1 in Parser.Char(' ').Many()
            from op in Parser.Char('+')
            from s2 in Parser.Char(' ').Many()
            from y  in Number.EndOfInput()
            select new Addition(x, y);
    }
}
