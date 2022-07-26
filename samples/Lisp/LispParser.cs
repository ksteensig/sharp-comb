using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MonadicParserCombinator;


// purpose is to implement: https://www.scheme.com/tspl2d/grammar.html
// still a bit of work, but it has come a long way
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

        static Parser<IEnumerable<char>> TabOrNewline = from tn in Parser.EitherOf(new List<Parser<char>>
                                                        {
                                                            Newline,
                                                            Tab
                                                        }).Many()
                                                        select tn;

        static Parser<IEnumerable<char>> WhitespaceE = from tn in TabOrNewline
                                                       from s in Space
                                                       from ws in Whitespaces
                                                       select tn.Concat(s.ToString()
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
                True, False
            })
                                          select (LispNode)new BooleanNode { value = bool.Parse(b) };

        static Parser<char> AsciiQuote = Parser.Char('"');

        static Parser<char> AsciiEscape = Parser.Char('\\');

        static Parser<char> StringChar = Parser.EitherOf(
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
            }
        );

        public static Parser<LispNode> String = from aq1 in AsciiQuote
                                                from sc in StringChar.Many()
                                                from aq2 in AsciiQuote
                                                select (LispNode)new StringNode
                                                {
                                                    value = string.Concat(sc)
                                                };

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

        public static Parser<IEnumerable<char>> Integer2 =
            from i in Digit.ManyOne() // Int.Parse
            select i;

        public static Parser<LispNode> Integer =
            from i in Digit.ManyOne() // Int.Parse
            select (LispNode)new IntegerNode { value = int.Parse(string.Concat(i)) };

        public static Parser<LispNode> Decimal =
            from d1 in Digit.ManyOne() // Double.Parse
            from d in Dot
            from d2 in Digit.ManyOne()
            select (LispNode)new DecimalNode
            {
                value = double.Parse(
                    string.Concat(d1.Concat(d.ReturnIEnumerable()).Concat(d2))
                )
            };

        public static Parser<LispNode> Number = new List<Parser<LispNode>>
            {
                Decimal, // important to parse decimal first as first part is also valid int
                Integer
            }.EitherOf();

        public static Parser<char> Initial = new List<Parser<char>> {
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
            }.EitherOf();



        public static Parser<char> Subsequent = new List<Parser<char>> {
                Initial,
                Plus,
                Digit,
                Dot,
                Plus,
                Minus
            }.EitherOf();


        public static Parser<IEnumerable<char>> InitSub = from i in Initial
                                                          from ss in Subsequent.Many()
                                                          select i.ReturnIEnumerable()
                                                                  .Concat(ss);

        public static Parser<LispNode> Identifier = from id in new List<Parser<IEnumerable<char>>> {
                Dot.Then<char, IEnumerable<char>>(d => d.ReturnIEnumerable().ReturnParser()),
                Plus.Then<char, IEnumerable<char>>(p => p.ReturnIEnumerable().ReturnParser()),
                Minus.Then<char, IEnumerable<char>>(m => m.ReturnIEnumerable().ReturnParser()),
                InitSub
            }.EitherOf()
                                                    select (LispNode)new IdentifierNode { id = string.Concat(id) };

        public static Parser<LispNode> Datum = from d in new List<Parser<LispNode>>
            {
                Number,
                String,
                Boolean,
                    from lp in LParen
                    from ws1 in Whitespaces
                    from d in Datum
                    from _ in (from wse in WhitespaceE
                               from d2 in Datum
                               select d2).Many()
                    from ws2 in Whitespaces
                    from rp in RParen
                    select new LispNode(),
                Identifier
            }.EitherOf()
                                               select d;

        public static Parser<LispNode> LList = from lp in LParen
                                               from ws1 in Whitespaces
                                               from d in Datum
                                               from ds in (from wse in Spaces // problem is to allow tab/nl but ensure a space
                                                           from d2 in Datum
                                                           select d2).Many()
                                               from ws2 in Whitespaces
                                               from rp in RParen
                                               select (LispNode)new LListNode
                                               {
                                                   children = d.ReturnIEnumerable().Concat(ds)
                                               };

        public static Parser<LispNode> ExpressionA = from lp in LParen
                                                     from ws1 in Whitespaces
                                                     from id in Identifier
                                                     from wse1 in Spaces
                                                     from d in Datum
                                                     from ds in (from wse in Spaces
                                                                 from d2 in Datum
                                                                 select d2).Many()
                                                     from ws2 in Whitespaces
                                                     from rp in RParen
                                                     select (LispNode)new ExpressionANode
                                                     {
                                                         id = (IdentifierNode)id,
                                                         ds = d.ReturnIEnumerable().Concat(ds)
                                                     };

        public static Parser<LispNode> ExpressionQ = from sq in SQuote
                                                     from ll in LList
                                                     select (LispNode)new ExpressionQNode
                                                     {
                                                         llist = (LListNode)ll
                                                     };

        public static Parser<LispNode> Expression = Parser.EitherOf(
            new List<Parser<LispNode>> {
                ExpressionA,
                ExpressionQ,
                Datum
            }
        );


        public static Parser<LispNode> Body = from lp in LParen
                                              from ws1 in Whitespaces
                                              from e in Expression
                                              from es in (from ws in Spaces
                                                          from e2 in Expression
                                                          select e2).Many()
                                              from rp in RParen
                                              select (LispNode)new BodyNode
                                              {
                                                  expressions = e.ReturnIEnumerable().Concat(es)
                                              };

        public static Parser<LispNode> ParameterName = Identifier;

        public static Parser<LispNode> Definition1 = from lp1 in LParen
                                                     from ws1 in Whitespaces
                                                     from def in Define
                                                     from s1 in Spaces
                                                     from lp2 in LParen
                                                     from ws2 in Whitespaces
                                                     from id in Identifier
                                                     from pms in (from s in Spaces
                                                                  from pm in ParameterName
                                                                  select pm).ManyOne()
                                                     from ws3 in Whitespaces
                                                     from rp1 in RParen
                                                     from ws4 in Whitespaces
                                                     from b in Body
                                                     from ws5 in Whitespaces
                                                     from rp2 in RParen
                                                     select (LispNode)new Definition1Node
                                                     {
                                                         identifier = (IdentifierNode)id,
                                                         parameters = pms,
                                                         body = (BodyNode)b
                                                     };

        public static Parser<LispNode> Definition2 = from lp in LParen
                                                     from ws1 in Whitespaces
                                                     from def in Define
                                                     from s1 in Spaces
                                                     from id in Identifier
                                                     from s2 in Spaces
                                                     from d in Datum
                                                     from ws2 in Whitespaces
                                                     from rp in RParen
                                                     select (LispNode)new Definition2Node
                                                     {
                                                         identifier = (IdentifierNode)id,
                                                         datum = d
                                                     };

        public static Parser<LispNode> Definition = new List<Parser<LispNode>>
            {
                Definition1,
                Definition2
            }.EitherOf();

        public static Parser<LispNode> Form = new List<Parser<LispNode>> {
                Definition,
                Expression
            }.EitherOf();

        public static Parser<LispNode> Program = from f in Form
                                                 from fs in (from s in Spaces
                                                             from f2 in Form
                                                             select f2).Many()
                                                 from ws in Whitespaces
                                                 select (LispNode)new ProgramNode
                                                 {
                                                     forms = f.ReturnIEnumerable().Concat(fs)
                                                 };

        public static Parser<LispNode> Grammar = Program.EndOfInput();
    }
}