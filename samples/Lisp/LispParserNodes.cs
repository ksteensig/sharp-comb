using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MonadicParserCombinator;

namespace MonadicParserCombinator.Samples.Lisp
{
    public interface IVisitor
    {
        void Visit(BooleanNode node);
        void Visit(StringNode node);
        void Visit(IntegerNode node);
        void Visit(DecimalNode node);
        void Visit(IdentifierNode node);
        void Visit(LListNode node);
        void Visit(ExpressionANode node);
        void Visit(ExpressionQNode node);
        void Visit(BodyNode node);
        void Visit(Definition1Node node);
        void Visit(Definition2Node node);
        void Visit(ProgramNode node);
    }

    public class LispNode
    {
        public virtual void Accept(IVisitor visitor) { }
    }

    public class BooleanNode : LispNode
    {
        public bool value { get; set; }

        public override void Accept(IVisitor visitor) { visitor.Visit(this); }
    }

    public class StringNode : LispNode
    {
        public string value { get; set; }

        public override void Accept(IVisitor visitor) { visitor.Visit(this); }
    }

    public class IntegerNode : LispNode
    {
        public int value { get; set; }

        public override void Accept(IVisitor visitor) { visitor.Visit(this); }
    }

    public class DecimalNode : LispNode
    {
        public double value { get; set; }

        public override void Accept(IVisitor visitor) { visitor.Visit(this); }
    }

    public class IdentifierNode : LispNode
    {
        public string id { get; set; }

        public override void Accept(IVisitor visitor) { visitor.Visit(this); }
    }

    public class LListNode : LispNode
    {
        public IEnumerable<LispNode> children { get; set; }

        public override void Accept(IVisitor visitor) { visitor.Visit(this); }
    }

    public class ExpressionANode : LispNode
    {
        public IdentifierNode id { get; set; }
        public IEnumerable<LispNode> ds;

        public override void Accept(IVisitor visitor) { visitor.Visit(this); }
    }

    public class ExpressionQNode : LispNode
    {
        public LListNode llist { get; set; }

        public override void Accept(IVisitor visitor) { visitor.Visit(this); }
    }

    public class BodyNode : LispNode
    {
        public IEnumerable<LispNode> expressions { get; set; }

        public override void Accept(IVisitor visitor) { visitor.Visit(this); }
    }

    public class Definition1Node : LispNode
    {
        public IdentifierNode identifier { get; set; }

        // LispNode instead of IdentifierNode to prevent need for casting to IEnumerable<IdentifierNode>
        public IEnumerable<LispNode> parameters { get; set; }
        public BodyNode body { get; set; }

        public override void Accept(IVisitor visitor) { visitor.Visit(this); }
    }

    public class Definition2Node : LispNode
    {
        public IdentifierNode identifier { get; set; }
        public LispNode datum { get; set; }

        public override void Accept(IVisitor visitor) { visitor.Visit(this); }
    }

    public class ProgramNode : LispNode
    {
        public IEnumerable<LispNode> forms { get; set; }

        public override void Accept(IVisitor visitor) { visitor.Visit(this); }
    }
}