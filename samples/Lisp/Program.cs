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
            //var prog = LispParser.Definition1.EndOfInput().TryParse("(define (f x y z) (+ x y z))");
            //var prog = LispParser.Definition2.EndOfInput().TryParse("(define x (1 2 3))");
            var prog = LispParser.Grammar.TryParse("(define (f x y z) (+ x y \"hello, world!\")) (define (f x y z) (+ x y z))");

            //var prog = LispParser.Integer.EndOfInput().TryParse("1");

            var pprinter = new LispPrettyPrinter();

            prog.Value.Accept(pprinter);

            Console.WriteLine(prog.IsSuccess);

            Console.WriteLine(prog.Message);
            //Console.WriteLine(prog.Message);
            if (!prog.IsSuccess)
            {
                Console.WriteLine(prog.Remainder.Current);
            }
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
