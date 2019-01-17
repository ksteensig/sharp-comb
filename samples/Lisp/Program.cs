using System;
using System.Collections.Generic;
using MonadicParserCombinator;

namespace MonadicParserCombinator.Samples.Lisp
{
    class Program
    {
        static void Main(string[] args)
        {
            var prog = Parser.TryParse(LispParser.LispProgramParser, "(define xx (+ 1 1))");

            Console.WriteLine(prog.IsSuccess);
            Console.WriteLine(prog.Value.GetType());
            //Console.WriteLine(prog.Remainder.Current);
            //Console.WriteLine(prog.Remainder.Position);
        }
    }
}
