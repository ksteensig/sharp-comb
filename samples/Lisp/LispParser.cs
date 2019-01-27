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
    public class LispVariable : LispDatum
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
            if (boolean.ToLower() == "#f")
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
        public IEnumerable<LispDatum> list;

        public LispList(IEnumerable<LispDatum> list)
        {
            this.list = list;
        }
    }

    public class LispVector : LispDatum
    {
        public IEnumerable<LispDatum> vector;

        public LispVector(IEnumerable<LispDatum> vector)
        {
            this.vector = vector;
        }
    }

    public class LispAbbreviation : LispDatum
    {
        public LispDatum data;

        public LispAbbreviation(LispDatum data)
        {
            this.data = data;
        }
    }

    public class LispNil : LispDatum { }

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
            from l in Parser.MatchRegex(new Regex("(?i)a-z"))
            select l.First();

        static Parser<LispDatum> NumberParser =
            from n in Parser.EitherOf(new List<Parser<LispDatum>>
            {
                IntParser,
                DecimalParser
            })
            select n;

        static Parser<LispDatum> IntParser =
            from i in Parser.MatchRegex(new Regex("0-90-9*"))
            select (LispDatum)new LispInt(int.Parse(i));

        static Parser<LispDatum> DecimalParser =
            from d in Parser.MatchRegex(new Regex("0-9*.0-90-9*"))
            select (LispDatum)new LispDecimal(Double.Parse(d));

        static Parser<LispDatum> BooleanParser =
            from b in Parser.EitherOf(new List<Parser<string>>
            {
                Parser.MatchString("(?i)#f"),
                Parser.MatchString("(?i)#t")
            })
            select (LispDatum)new LispBoolean(b);

        static Parser<LispDatum> StringParser =
            from q1 in Parser.Char('"')
            from s in StringCharacterParser.Many().Text()
            from q2 in Parser.Char('"')
            select (LispDatum)new LispString(s);

        static Parser<LispDatum> CharParser =
            from s in Parser.EitherOf(new List<Parser<string>>
            {
                /* ignore case with (?i) */
                Parser.MatchString(@"(?i)#\newline"),
                Parser.MatchString(@"(?i)#\space"),
                Parser.MatchRegex(new Regex(@"(?i)#\a-z")),
            })
            select (LispDatum)new LispChar(s);


        static Parser<char> StringCharacterParser =
            from s in Parser.EitherOf(new List<Parser<string>>
            {
                Parser.MatchString("\""),
                Parser.MatchString("\\"),
                Parser.MatchRegex(new Regex("[^\"\\]"))
            })
            select s.First();

        static Parser<LispDatum> DotListParser =
            from lp in LeftParen
            from front in Parser.SeperatedBy(DatumParser, Spaces)
            from s1 in Spaces
            from d in Parser.Char('.')
            from s2 in Spaces
            from back in DatumParser
            from rp in RightParen
            select (LispDatum)new LispList(front.Append(back));

        static Parser<LispDatum> ListParser =
            from lp in LeftParen
            from dl in Parser.SeperatedBy(DatumParser, Spaces)
            from rp in RightParen
            select (LispDatum)new LispList(dl);

        static Parser<LispDatum> AbbreviationParser =
            from q in Parser.Char('\'')
            from d in DatumParser
            select (LispDatum)new LispAbbreviation(d);

        static Parser<LispDatum> SymbolParser =
            from v in VariableParser
            select (LispDatum)v;

        static Parser<LispDatum> VectorParser =
            from h in Parser.Char('#')
            from lp in LeftParen
            from v in Parser.SeperatedBy(DatumParser, Spaces)
            from rp in RightParen
            select (LispDatum)new LispVector(v);

        static Parser<LispDatum> DatumParser =
            from d in Parser.EitherOf<LispDatum>(new List<Parser<LispDatum>>
            {
                BooleanParser, NumberParser, CharParser,
                StringParser, SymbolParser,
                DotListParser, ListParser, AbbreviationParser
            })
            select d;

    }
}