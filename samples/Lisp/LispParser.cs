using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MonadicParserCombinator;

namespace MonadicParserCombinator.Samples.Lisp
{
    public interface IVisitor { }
    /*
    public abstract class LispExpression
    {
        public abstract void Accept(IVisitor visitor);
    }
    */

    public class LispProgram { }

    public class LispForm : LispProgram { }

    public class LispDefinition : LispForm { }

    public class LispVariableDefinition : LispDefinition { }

    public class LispBeginDefinition : LispDefinition { }

    public class LispVariableDefinition1 : LispVariableDefinition { }
    public class LispVariableDefinition2 : LispVariableDefinition { }
    public class LispVariableDefinition3 : LispVariableDefinition { }

    public class LispVariable
    {
        string identifier;

        public LispVariable(string id)
        {
            this.identifier = id;
        }
    }



    public static class LispParser
    {
        //static Parser<IEnumerable<char>> SpaceNewline =
        //    from s in Parser.EitherOf(Space, Char('\n')).Many()
        //    select s;

        static Parser<char> Space =
            from s in Parser.Char(' ')
            select s;

        static Parser<IEnumerable<char>> Spaces =
            from s in Space.ManyOne()
            select s;

        static Parser<char> LeftParen =
            from lp in Parser.Char('(')
            select lp;

        static Parser<char> RightParen =
            from rp in Parser.Char(')')
            select rp;

        static Parser<char> Newline =
            from nl in Parser.Char('\n')
            select nl;
        /*
                static Parser<string> IdentifierParser = Parser.EitherOf(new List<string>
                {
                    from i in InitialParser
                    from s in SubsequentParser.Many()
                    select i,

                    Parser.Char('+').Text(),
                    Parser.Char('-').Text(),
                    Parser.MatchString("...")
                });
        */
        static Parser<string> InitialParser = Parser.EitherOf(new List<Parser<string>>
        {
            Parser.MatchRegex(new Regex("a-z")),
            InitialCharParser.Text()
        });

        static Parser<IEnumerable<char>> InitialCharParser =
            from c in Parser.EitherOf(new List<Parser<char>>{
                Parser.Char('!'),
                Parser.Char('$'),
                Parser.Char('%'),
                Parser.Char('&'),
                Parser.Char('*'),
                Parser.Char('/'),
                Parser.Char(':'),
                Parser.Char('<'),
                Parser.Char('='),
                Parser.Char('>'),
                Parser.Char('?'),
                Parser.Char('~'),
                Parser.Char('_'),
                Parser.Char('^')
            })
            select Parser.ReturnIEnumerable(c);


        static Parser<string> SubsequentParser =
            from s in Parser.EitherOf(new List<Parser<string>> {
                InitialParser, SubsequentCharParser.Text()})
            select s;

        static Parser<IEnumerable<char>> SubsequentCharParser =
            from c in Parser.EitherOf(new List<Parser<char>>
            {
                Parser.Digit,
                Parser.Char('.'),
                Parser.Char('+'),
                Parser.Char('-')
            })
            select Parser.ReturnIEnumerable(c);
    }
}