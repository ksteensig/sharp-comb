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

    public class LispExpression : LispForm { }
    public class LispVariable
    {
        public string identifier;

        public LispVariable(string id)
        {
            this.identifier = id;
        }
    }

    public class LispDatum { }

    public class LispBoolean : LispDatum
    {
        public bool boolean;

        public LispBoolean(bool boolean)
        {
            this.boolean = boolean;
        }

        public LispBoolean(string boolean)
        {
            if (boolean == "#f")
            {
                this.boolean = false;
            }
            else
            {
                this.boolean = true;
            }
        }
    }

    public class LispNumber : LispDatum { }

    public class LispInt : LispNumber
    {
        public int value;

        public LispInt(int value)
        {
            this.value = value;
        }
    }

    public class LispDecimal : LispNumber
    {
        public double value;

        public LispDecimal(double value)
        {
            this.value = value;
        }
    }

    public class LispChar : LispDatum
    {
        public string character;

        public LispChar(string character)
        {
            this.character = character;
        }
    }

    public class LispString : LispDatum
    {
        public string str;

        public LispString(string str)
        {
            this.str = str;
        }
    }

    public class LispList : LispDatum
    {
        public LispDatum head;
        public LispDatum tail;

        public LispList(LispDatum head, LispDatum tail)
        {
            this.head = head;
            this.tail = tail;
        }
    }

    public class LispVector : LispDatum
    {
        public List<LispDatum> vector;

        public LispVector(List<LispDatum> vector)
        {
            this.vector = vector;
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

        static Parser<LispVariable> VariableParser =
            from i in IdentifierParser
            select new LispVariable(i);

        static Parser<string> IdentifierParser =
            from i in Parser.EitherOf(new List<Parser<string>>
            {
                from i in InitialParser
                from s in SubsequentParser.Many()
                select string.Concat(i.ToString(), s),
                Parser.MatchRegex(new Regex("+")),
                Parser.MatchRegex(new Regex("-")),
                Parser.MatchRegex(new Regex("..."))
            })
            select i;

        static Parser<char> InitialParser =
            from c in Parser.EitherOf(new List<Parser<char>>
            {
                LetterParser,
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
            select c;

        static Parser<char> SubsequentParser =
            from c in Parser.EitherOf(new List<Parser<char>>
            {
                InitialParser,
                Parser.Digit,
                Parser.Char('.'),
                Parser.Char('+'),
                Parser.Char('-')
            })
            select c;

        static Parser<char> LetterParser =
            from l in Parser.MatchRegex(new Regex("a-z"))
            select l.First();

        static Parser<LispInt> IntParser =
            from i in Parser.MatchRegex(new Regex("0-90-9*"))
            select new LispInt(int.Parse(i));

        static Parser<LispDecimal> DecimalParser =
            from d in Parser.MatchRegex(new Regex("0-9*.0-90-9*"))
            select new LispDecimal(Double.Parse(d));

        static Parser<LispBoolean> BooleanParser =
            from b in Parser.EitherOf(new List<Parser<string>>
            {
                Parser.MatchString("#f"),
                Parser.MatchString("#t")
            })
            select new LispBoolean(b);
        /*
                static Parser<LispChar> CharParser =
                    from s in Parser.EitherOf(new List<Parser>
                    {
                        Parser.MatchString(@"#\newline"),
                        Parser.MatchString(@"#\space"),
                        Parser.MatchRegex(new Regex(@"#\a-z")),
                        Parser.MatchRegex(new Regex(@"#\A-Z"))
                    })
                    */
    }
}