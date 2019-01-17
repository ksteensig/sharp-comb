using System;
using System.Collections.Generic;
using System.Linq;
using MonadicParserCombinator;

namespace MonadicParserCombinator.Samples.Lisp
{
    public class LispExpression
    {

    }

    public class LispAtomic : LispExpression
    {

    }

    public class EmptyListList : LispExpression
    {

    }

    public class LispList : LispExpression
    {

        IEnumerable<LispExpression> expressions;
        
        public LispList(IEnumerable<LispExpression> expr)
        {
            this.expressions = expr;
        }
    }

    public class EmptyLispList : LispExpression
    {

    }

    public class LispTerm : LispAtomic
    {
        int x;

        public LispTerm(int term)
        {
            this.x = term;
        }
    }

    public class LispSymbol : LispAtomic
    {
        string symbol;

        public LispSymbol(string symbol)
        {
            this.symbol = symbol;
        }
    }

    public class LispDefine : LispExpression
    {
        LispSymbol variable;
        LispExpression expression;

        public LispDefine(LispSymbol var, LispExpression expr)
        {
            this.variable = var;
            this.expression = expr;
        }
    }

    public class LispLambda : LispExpression
    {
        LispSymbol parameter;
        LispExpression expression;

        public LispLambda(LispSymbol par, LispExpression expr)
        {
            this.parameter = par;
            this.expression = expr;
        }
    }

    public static class LispParser
    {

        static Parser<char> Space =
            from s in Parser.Char(' ')
            select s;

        static Parser<string> Spaces =
            from s in Space.ManyOne().Text()
            select s;

        static Parser<char> LeftParen =
            from lp in Parser.Char('(')
            select lp;

        static Parser<char> RightParen =
            from rp in Parser.Char(')')
            select rp;

        static Parser<char> Newline =
            from nl in Parser.Char(')')
            select nl;

        static Parser<LispExpression> TermParser =
            from x in Parser.Integer.Text()
            select (LispExpression)new LispTerm(int.Parse(x));

        static Parser<LispExpression> SymbolParser =
            from x in Parser.WhileTrue(c => c != '(' && c != ')' && c != '\0' && c != ' ').Text()
            select (LispExpression)new LispSymbol(x);

        static Parser<LispExpression> DefineParser =
            from define in Parser.CharSequence("define")
            from s1     in Space
            from n      in SymbolParser
            from s2     in Space
            from expr   in ListParserInner
            select (LispExpression)new LispDefine((LispSymbol)n, expr);
        
        static Parser<LispExpression> LambdaParser =
            from define in Parser.CharSequence("lambda")
            from s1     in Space
            from n      in SymbolParser
            from s2     in Space
            from expr   in ListParserInner
            select (LispExpression)new LispLambda((LispSymbol)n, expr);

        static Parser<LispExpression> EmptyListParser =
            from lp in LeftParen
            from ss in Space.Many()
            from rp in RightParen
            select (LispExpression)new EmptyLispList();

        static Parser<LispExpression> ListParserInner =
            from lp in LeftParen
            from s1 in Space.Many()
            from expr in Parser.SeperatedBy<LispExpression, IEnumerable<char>>(
                            Parser.EitherOf(new List<Parser<LispExpression>>(
                                new Parser<LispExpression>[]{
                                    EmptyListParser, TermParser,
                                    SymbolParser, ListParser})), Space.ManyOne())
                                    
            from s2 in Space.Many()
            from rp in RightParen
            select (LispExpression)new LispList(expr);

        static Parser<LispExpression> ListParser =
            from lp   in LeftParen
            from s1   in Space.Many()
            from expr in Parser.EitherOf(new List<Parser<LispExpression>>(
                            new Parser<LispExpression>[]{
                                DefineParser, LambdaParser,
                                EmptyListParser, TermParser,
                                SymbolParser, ListParser}))
            from s2   in Space.Many()
            from rp   in RightParen
            select expr;

        public static Parser<LispExpression> LispProgramParser = ListParser.EndOfInput<LispExpression>();
    }
}
