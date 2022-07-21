using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MonadicParserCombinator;

namespace MonadicParserCombinator.Samples.Lisp
{
    public class LispPrettyPrinter : IVisitor
    {
        public void Visit(BooleanNode node)
        {
            Console.Write(node.value);
        }

        public void Visit(StringNode node)
        {
            Console.Write("\"" + node.value + "\"");
        }

        public void Visit(IntegerNode node)
        {
            Console.Write(node.value);
        }

        public void Visit(DecimalNode node)
        {
            Console.Write(node.value);
        }

        public void Visit(IdentifierNode node)
        {
            Console.Write(node.id);
        }

        public void Visit(LListNode node)
        {
            Console.Write("(");
            bool first = true;
            foreach (LispNode n in node.children)
            {
                if (!first)
                {
                    Console.Write(" ");
                }
                n.Accept(this);
                first = false;

            }
            Console.Write(")");
        }

        public void Visit(ExpressionANode node)
        {
            Console.Write("(");
            Console.Write(node.id.id);
            Console.Write(" ");
            bool first = true;
            foreach (LispNode n in node.ds)
            {
                if (!first)
                {
                    Console.Write(" ");
                }
                n.Accept(this);
                first = false;
            }
            Console.Write(")");
        }

        public void Visit(ExpressionQNode node)
        {
            Console.Write("'");
            node.llist.Accept(this);
        }

        public void Visit(BodyNode node)
        {
            Console.Write("(");
            bool first = true;
            foreach (LispNode n in node.expressions)
            {
                if (!first)
                {
                    Console.Write(" ");
                }
                n.Accept(this);
                first = false;
            }
            Console.Write(")");
        }

        public void Visit(Definition1Node node)
        {
            Console.Write("(");
            Console.Write("define");
            Console.Write(" ");
            Console.Write("(");

            node.identifier.Accept(this);
            Console.Write(" ");

            bool first = true;

            foreach (LispNode n in node.parameters)
            {
                if (!first)
                {
                    Console.Write(" ");
                }
                n.Accept(this);
                first = false;
            }
            Console.Write(")");
            Console.Write(" ");

            node.body.Accept(this);

            Console.Write(")");
        }

        public void Visit(Definition2Node node)
        {
            Console.Write("(");
            Console.Write("define");
            Console.Write(" ");

            node.identifier.Accept(this);

            Console.Write(" ");

            node.datum.Accept(this);

            Console.Write(")");
        }

        public void Visit(ProgramNode node)
        {
            bool first = true;
            foreach (var n in node.forms)
            {
                if (!first)
                {
                    Console.Write(" ");
                }
                n.Accept(this);
                first = false;
            }

            Console.WriteLine(" ");
        }
    }
}