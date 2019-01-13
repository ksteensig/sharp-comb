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

        LispExpression head;
        LispExpression tail;
        
        public LispList(LispExpression head, LispExpression tail)
        {
            this.head = head;
            this.tail = tail;
        }
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
        
        public static Parser<LispExpression> TermParser =
            from x in Parser.Integer.Text()
            select (LispExpression)new LispTerm(int.Parse(x));

        public static Parser<LispExpression> DefineParser =
            from define in Parser.CharSequence("define")
            from s1     in Parser.Char(' ').ManyOne()
            from pl     in Parser.Char('(')
            from n      in Parser.UntilFalse(c => c != ')').Text()
            from pr     in Parser.Char(')')
            from s2     in Parser.Char(' ').ManyOne()
            from ple    in Parser.Char('(')
            from expr   in ListParserInner
            from pre    in Parser.Char(')')
            select (LispExpression)new LispDefine(new LispSymbol(n), expr);
        
        public static Parser<LispExpression> LambdaParser =
            from lambda in Parser.CharSequence("lambda")
            from s1     in Parser.Char(' ').ManyOne()
            from pl     in Parser.Char('(')
            from n      in Parser.UntilFalse(c => c != ')').Text()
            from pr     in Parser.Char(')')
            from s2     in Parser.Char(' ').ManyOne()
            from ple    in Parser.Char('(')
            from expr   in ListParserInner
            from pre    in Parser.Char(')')
            select (LispExpression)new LispLambda(new LispSymbol(n), expr);

        public static Parser<LispExpression> EmptyListParser =
            from pl in Parser.Char('(')
            from pr in Parser.Char('(')
            select (LispExpression)new LispExpression();

        public static Parser<LispExpression> ListParserInner =
            from pl in Parser.Char('(')
            from s1 in Parser.Char(' ').ManyOne()
            from t1 in Parser.EitherOf(new List<Parser<LispExpression>>(
                    new Parser<LispExpression>[]{
                        DefineParser, LambdaParser, TermParser, ListParserInner}))
            from s2 in Parser.Char(' ').ManyOne()
            from t2 in Parser.EitherOf(new List<Parser<LispExpression>>(
                    new Parser<LispExpression>[]{
                        EmptyListParser, ListParserInner}))
            from s3 in Parser.Char(' ').Many()
            from n  in Parser.Char('\n').Many()
            from pr in Parser.Char(')')
            select (LispExpression)new LispList(t1, t2);

        public static Parser<LispExpression> ListParser =
            from pl in Parser.Char('(')
            from s1 in Parser.Char(' ').ManyOne()
            from t1 in Parser.EitherOf(new List<Parser<LispExpression>>(
                    new Parser<LispExpression>[]{
                        DefineParser, LambdaParser, TermParser, ListParserInner}))
            from s2 in Parser.Char(' ').ManyOne()
            from t2 in Parser.EitherOf(new List<Parser<LispExpression>>(
                    new Parser<LispExpression>[]{
                        EmptyListParser, ListParserInner}))
            from s3 in Parser.Char(' ').Many()
            from n  in Parser.Char('\n').Many()
            from pr in Parser.Char(')').EndOfInput()
            select (LispExpression)new LispList(t1, t2);
    }
}
