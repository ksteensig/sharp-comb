using System;

namespace MonadicParserCombinator.Samples.Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var calc = Parser.TryParse(CalculatorParser.Addition, "1  +  1");

            calc.Value.Evaluate();

            Console.WriteLine("Hello World!");
        }
    }
}
