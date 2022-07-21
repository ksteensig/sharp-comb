using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MonadicParserCombinator;

namespace MonadicParserCombinator.Samples.Lisp
{
    class Program
    {
        static void Main(string[] args)
        {

            var parser = new LispParser();
            var prog = LispParser.Grammar.TryParse("(+ \"hellow, world!\" 1)");

            Console.WriteLine(prog.IsSuccess);

            Console.WriteLine(prog.Message);
            //Console.WriteLine(prog.Message);
            //Console.WriteLine(prog.Remainder.Current);
            //Console.WriteLine(prog.Remainder.Current);
            //Console.WriteLine(prog.Remainder.Position);

            /*
            var sprinter = new LispSymbolMangler();
            var pprinter = new LispPrettyPrinter();
            var oprinter = new LispVariableCollisionChecker();

            var list = new LispList(prog.Value);

            list.Accept(sprinter);
            list.Accept(pprinter);
            list.Accept(oprinter);
            */

            //Console.Write("\nContaining name overlap? ");
            //Console.WriteLine(oprinter.ContainsNameOverlap());


        }
    }
}
