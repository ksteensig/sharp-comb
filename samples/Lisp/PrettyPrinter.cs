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
            foreach (LispNode n in node.children)
            {

                n.Accept(this);
                Console.Write(" ");

            }
            Console.Write(")");
        }

        public void Visit(ExpressionANode node)
        {
            Console.Write("(");
            Console.Write(node.id.id);
            Console.Write(" ");
            foreach (LispNode n in node.ds)
            {
                n.Accept(this);
                Console.Write(" ");
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
            foreach (LispNode n in node.expressions)
            {
                n.Accept(this);
                Console.Write(" ");
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

            foreach (LispNode n in node.parameters)
            {
                n.Accept(this);
                Console.Write(" ");
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
            Console.WriteLine(" ");
            foreach (var n in node.forms)
            {
                n.Accept(this);
                Console.Write(" ");
            }

            Console.WriteLine(" ");
        }
    }
}