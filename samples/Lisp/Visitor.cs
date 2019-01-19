using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            node.expression.Accept(this);
        }

        public void Visit(LispLambda node)
        {
            Console.Write("lambda ");
            Console.Write(" ");
            Console.Write(node.parameter.symbol);
            Console.Write(" ");
            node.expression.Accept(this);
        }

    }

    public class LispSymbolMangler : IVisitor
    {
        // (symbol, times spotted)
        List<(string Symbol, int Count)> symbols;

        public LispSymbolMangler()
        {
            symbols = new List<(string, int)>();
        }

        
        public void Visit(LispList node)
        {
            foreach (var e in node.expressions)
            {
                e.Accept(this);
            }
        }

        public void Visit(EmptyLispList node)
        {
        }

        public void Visit(LispTerm node)
        {
        }

        public void Visit(LispSymbol node)
        {
            var old_name = node.symbol;
            var index = symbols.FindIndex(s => s.Symbol == old_name);

            if (index != -1)
            {
                var postfix = symbols[index].Count;
                node.symbol = old_name + postfix.ToString();
            }
        }

        public void Visit(LispDefine node)
        {
            
            var old_name = node.parameter.symbol;

            var index = symbols.FindIndex(s => s.Symbol == old_name);

            if (index != -1)
            {
                symbols[index] = (old_name, symbols[index].Count + 1);
                var postfix = symbols[index].Count;
                node.parameter = new LispSymbol(old_name + postfix.ToString());
                node.expression.Accept(this);
                symbols[index] = (old_name, symbols[index].Count - 1);
            }
            else
            {
                symbols.Add((old_name, 0));
                node.expression.Accept(this);
                symbols.Remove((old_name, 0));
            }
        }

        public void Visit(LispLambda node)
        {
            var old_name = node.parameter.symbol;

            var index = symbols.FindIndex(s => s.Symbol == old_name);

            if (index != -1)
            {
                symbols[index] = (old_name, symbols[index].Count + 1);
                var postfix = symbols[index].Count;
                node.parameter = new LispSymbol(old_name + postfix.ToString());
                node.expression.Accept(this);
                symbols[index] = (old_name, symbols[index].Count - 1);
            }
            else
            {
                symbols.Add((old_name, 0));
                node.expression.Accept(this);
                symbols.Remove((old_name, 0));
            }
        }
    }
}
