using System;
using System.Collections.Generic;
using System.Linq;
using MonadicParserCombinator;

namespace MonadicParserCombinator.Samples.Lisp
{
    public interface IVisitor
    {
        void Visit(LispList node);
        void Visit(EmptyLispList node);
        void Visit(LispTerm node);
        void Visit(LispSymbol node);
        void Visit(LispDefine node);
        void Visit(LispLambda node);
    }

    public class LispPrettyPrinter : IVisitor {
        
        public void Visit(LispList node)
        {
            Console.Write("(");
            Console.Write(" ");            
            foreach (LispExpression e in node.expressions)
            {
                e.Accept(this);
                Console.Write(" ");
            }
            
            Console.Write(")");

        }

        public void Visit(EmptyLispList node)
        {
            Console.Write("()");
        }

        public void Visit(LispTerm node)
        {
            Console.Write(node.value);
        }

        public void Visit(LispSymbol node)
        {
            Console.Write(node.symbol);
        }

        public void Visit(LispDefine node)
        {
            Console.Write("define");
            Console.Write(" ");
            Console.Write(node.parameter.symbol);
            Console.Write(" ");
            Console.Write("(");
            node.expression.Accept(this);
            Console.Write(")");
        }

        public void Visit(LispLambda node)
        {
            Console.Write("lambda ");
            Console.Write(" ");
            Console.Write(node.parameter.symbol);
            Console.Write(" ");
            Console.Write("(");
            node.expression.Accept(this);
            Console.Write(")");
        }

    }
}
