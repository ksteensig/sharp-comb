using System;

namespace MonadicParserCombinator.Samples.Lisp
{
    class Program
    {
        static void Main(string[] args)
        {
            var prog = Parser.TryParse(LispParser.ListParser, "(define x (+ 1 1))");

            Console.WriteLine(prog.IsSuccess);

            Console.WriteLine("Hello World!");
        }
    }
}
