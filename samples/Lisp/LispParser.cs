using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MonadicParserCombinator;


namespace MonadicParserCombinator.Samples.Lisp
{
    public partial class LispParser
    {
        public static Parser<char> Space =
            from c in Parser.Char(' ')
            select c;

        static Parser<IEnumerable<char>> Spaces =
            from s in Space.ManyOne()
            select s;

        static Parser<IEnumerable<char>> SpacesP =
            from s in Space.ManyOne()
            select s;

        static Parser<IEnumerable<char>> SpacesS =
            from s in Space.Many()
            select s;
        static Parser<char> Newline =
                    from nl in Parser.Char('\n')
                    select nl;

        static Parser<char> Tab =
                    from tab in Parser.Char('\t')
                    select tab;

        static Parser<char> Whitespace = Parser.EitherOf(
            new List<Parser<char>> {
                Tab, Space, Newline
            });

        static Parser<IEnumerable<char>> Whitespaces = from ws in Whitespace.Many()
                                                       select ws;

        static Parser<char> TabOrNewline = from tn in Parser.EitherOf(new List<Parser<char>>
                                                        {
                                                            Newline,
                                                            Tab
                                                        })
                                           select tn;

        static Parser<IEnumerable<char>> WhitespaceE = from tn in TabOrNewline
                                                       from s in Space
                                                       from ws in Whitespaces
                                                       select tn.ToString()
                                                                .AsEnumerable()
                                                                .Concat(s.ToString()
                                                                         .AsEnumerable())
                                                                .Concat(ws);

        static Parser<string> True = from b in Parser.MatchString("#t")
                                     select "true";

        static Parser<string> False = from b in Parser.MatchString("#f")
                                      select "false";

        static Parser<char> Quote = from s in Parser.MatchString("\\\"")
                                    select '"';


        static Parser<char> Escape = from s in Parser.MatchString("\\\\")
                                     select '\\';

        static Parser<char> Dot = from s in Parser.Char('.')
                                  select s;

        static Parser<char> Plus = from s in Parser.Char('+')
                                   select s;

        static Parser<char> Minus = from s in Parser.Char('-')
                                    select s;

        static Parser<char> Exclamation = from s in Parser.Char('!')
                                          select s;
        static Parser<char> Dollar = from s in Parser.Char('$')
                                     select s;
        static Parser<char> Percent = from s in Parser.Char('%')
                                      select s;
        static Parser<char> Ambersand = from s in Parser.Char('&')
                                        select s;
        static Parser<char> Slash = from s in Parser.Char('/')
                                    select s;

        static Parser<char> Colon = from s in Parser.Char(':')
                                    select s;

        static Parser<char> LT = from s in Parser.Char('<')
                                 select s;

        static Parser<char> EQ = from s in Parser.Char('=')
                                 select s;

        static Parser<char> GT = from s in Parser.Char('>')
                                 select s;

        static Parser<char> Question = from s in Parser.Char('?')
                                       select s;

        static Parser<char> Tilde = from s in Parser.Char('~')
                                    select s;

        static Parser<char> Underscore = from s in Parser.Char('_')
                                         select s;

        static Parser<char> Caret = from s in Parser.Char('^')
                                    select s;

        public static Parser<char> LParen = from lp in Parser.Char('(')
                                            select lp;

        public static Parser<char> RParen = from rp in Parser.Char(')')
                                            select rp;

        static Parser<char> SQuote = from s in Parser.Char('\'')
                                     select s;

        static Parser<string> Define = from s in Parser.MatchString("define")
                                       select s;



        static Parser<LispNode> Boolean = from b in Parser.EitherOf(
            new List<Parser<string>>
            {
                True, False // Boolean.Parse
            })
                                          select new LispNode();

        static Parser<char> AsciiQuote = from s in Parser.MatchString("\"")
                                         select '\"';

        static Parser<char> AsciiEscape = from s in Parser.MatchString("\\")
                                          select '\\';

        static Parser<char> StringChar = from s in Parser.EitherOf(
            new List<Parser<char>>
            {
                Quote,
                Escape,
                Parser.Exclude(
                    Parser.Ascii,
                    Parser.EitherOf(
                    new List<Parser<char>>(
                        new List<Parser<char>> {
                            AsciiQuote,
                            AsciiEscape
                        })
                    )
                )
            })
                                         select s;

        public static Parser<LispNode> String = from aq1 in AsciiQuote
                                                from sc in StringChar.Many()
                                                from aq2 in AsciiQuote
                                                select new LispNode(); // return sc

        public static Parser<char> Digit = from ad in Parser.EitherOf(
            new List<Parser<char>> {
                Parser.Char('0'),
                Parser.Char('1'),
                Parser.Char('2'),
                Parser.Char('3'),
                Parser.Char('4'),
                Parser.Char('5'),
                Parser.Char('6'),
                Parser.Char('7'),
                Parser.Char('8'),
                Parser.Char('9')
            })
                                           select ad;

        public static Parser<LispNode> Integer =
            from i in Digit.ManyOne() // Int.Parse
            select new LispNode();

        public static Parser<LispNode> Decimal =
            from d1 in Digit.ManyOne() // Double.Parse
            from d in Dot
            from d2 in Digit.ManyOne()
            select new LispNode();

        public static Parser<LispNode> Number = from n in
            new List<Parser<LispNode>>
            {
                Decimal, // important to parse decimal first as first part is also valid int
                Integer
            }.EitherOf()
                                                select n;

        public static Parser<char> Initial = from i in Parser.EitherOf(
            new List<Parser<char>> {
                Parser.Ascii.Where(c => c >= 'a' && c <= 'z'),
                Exclamation,
                Dollar,
                Percent,
                Ambersand,
                Slash,
                Colon,
                LT,
                EQ,
                GT,
                Question,
                Tilde,
                Underscore,
                Caret
            })
                                             select i;




        public static Parser<char> Subsequent = from s in Parser.EitherOf(
            new List<Parser<char>> {
                Initial,
                Plus,
                Digit,
                Dot,
                Plus,
                Minus
            }
        )
                                                select s;


        public static Parser<IEnumerable<char>> InitSub = from i in Initial
                                                          from ss in Subsequent.Many()
                                                          select i.ReturnIEnumerable()
                                                                  .Concat(ss);

        public static Parser<LispNode> Identifier = from i in Parser.EitherOf(
            new List<Parser<IEnumerable<char>>> {
                Dot.Then<char, IEnumerable<char>>(d => d.ReturnIEnumerable().ReturnParser()),
                Plus.Then<char, IEnumerable<char>>(p => p.ReturnIEnumerable().ReturnParser()),
                Minus.Then<char, IEnumerable<char>>(m => m.ReturnIEnumerable().ReturnParser()),
                InitSub
            }
        )
                                                    select new LispNode();

        public static Parser<LispNode> datum = from d in Parser.EitherOf(
            new List<Parser<LispNode>>
            {
                Number,
                String,
                Boolean,
                    from lp in LParen
                    from ws1 in Whitespaces
                    from d in Datum
                    from _ in (from wse in WhitespaceE
                               from ds in Datum
                               select new LispNode()).Many()
                    from ws2 in Whitespaces
                    from rp in RParen
                    select new LispNode(),
                Identifier
    }
        )
                                               select new LispNode();

        public static Parser<LispNode> LList = from lp in LParen
                                               from ws1 in Whitespaces
                                               from d in Datum
                                               from ds in (from wse in WhitespaceE
                                                           from ds in Datum
                                                           select new LispNode()).Many()
                                               from ws2 in Whitespaces
                                               from rp in RParen
                                               select new LispNode();

        public static Parser<LispNode> ExpressionA = from lp in LParen
                                                     from ws1 in Whitespaces
                                                     from id in Identifier
                                                     from wse1 in WhitespaceE
                                                     from d in Datum
                                                     from _ in (from wse in WhitespaceE
                                                                from ds in Datum
                                                                select new LispNode()).Many()
                                                     from ws2 in Whitespaces
                                                     from rp in RParen
                                                     select new LispNode();

        public static Parser<LispNode> Program = ExpressionA.EndOfInput();

        public static Parser<LispNode> Datum { get => Datum1; set => Datum1 = value; }
        public static Parser<LispNode> Datum1 { get => datum; set => datum = value; }
        public static Parser<LispNode> Datum2 { get => datum; set => datum = value; }
    }
}