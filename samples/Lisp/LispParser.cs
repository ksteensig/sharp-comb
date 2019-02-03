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

    public class LispProgram
    {
        public IEnumerable<LispForm> forms;

        public LispProgram(IEnumerable<LispForm> forms)
        {
            this.forms = forms;
        }
    }

    public class LispForm { }

    public class LispDefinition : LispForm { }

    public class LispBeginDefinition : LispDefinition
    {
        public IEnumerable<LispDefinition> definitions;

        public LispBeginDefinition(IEnumerable<LispDefinition> defs)
        {
            this.definitions = defs;
        }
    }

    public class LispVariableDefinition : LispDefinition { }

    public class LispVariableDefinition1 : LispVariableDefinition
    {
        public LispVariable variable;
        public LispExpression expression;

        public LispVariableDefinition1(LispVariable var, LispExpression expr)
        {
            this.variable = var;
            this.expression = expr;
        }
    }
    public class LispVariableDefinition2 : LispVariableDefinition
    {
        public IEnumerable<LispVariable> variables;
        public LispBody body;

        public LispVariableDefinition2(IEnumerable<LispVariable> vars, LispBody body)
        {
            this.variables = vars;
            this.body = body;
        }
    }

    public class LispBody
    {
        public IEnumerable<LispDefinition> definitions;
        public IEnumerable<LispExpression> expressions;

        public LispBody(IEnumerable<LispDefinition> defs, IEnumerable<LispExpression> exprs)
        {
            this.definitions = defs;
            this.expressions = exprs;
        }

    }

    public class LispExpression : LispForm { }
   
    public class LispVariable : LispExpression
    {
        public string identifier;

        public LispVariable(string id)
        {
            this.identifier = id;
        }
    }

    public class LispConstant : LispExpression
    {
        public LispDatum constant;

        public LispConstant(LispDatum constant)
        {
            this.constant = constant;
        }
    }


    public class LispLambda : LispExpression
    {
        public IEnumerable<LispVariable> formals;
        public LispBody body;

        public LispLambda(IEnumerable<LispVariable> formals, LispBody body)
        {
            this.formals = formals;
            this.body = body;
        }
    }

    public class LispIf : LispExpression { }

    public class LispIf2 : LispIf
    {
        public LispExpression expression1;
        public LispExpression expression2;

        public LispIf2(LispExpression expr1, LispExpression expr2)
        {
            this.expression1 = expr1;
            this.expression2 = expr2;
        }
    }

    public class LispIf3 : LispIf
    {
        public LispExpression expression1;
        public LispExpression expression2;
        public LispExpression expression3;

        public LispIf3(LispExpression expr1, LispExpression expr2, LispExpression expr3)
        {
            this.expression1 = expr1;
            this.expression2 = expr2;
            this.expression3 = expr3;
        }
    }

    public class LispSet : LispExpression
    {
        public LispVariable variable;
        public LispExpression expression;

        public LispSet(LispVariable var, LispExpression expr)
        {
            this.variable = var;
            this.expression = expr;
        }
    }

    public class LispApplication : LispExpression
    {
        public LispExpression applier;
        public IEnumerable<LispExpression> applicants;

        public LispApplication(LispExpression applier, IEnumerable<LispExpression> applicants)
        {
            this.applier = applier;
            this.applicants = applicants;
        }
    }

#region Datum
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
    
    public class LispSymbol : LispDatum
    {
        public string symbol;

        public LispSymbol(string id)
        {
            this.symbol = id;
        }
    }

    public class LispNil : LispDatum { }
#endregion
  
    public class LispParser
    {
        //static Parser<IEnumerable<char>> SpaceNewline =
        //    from s in Parser.EitherOf(Space, Char('\n')).Many()
        //    select s;

        public static Parser<char> Space =
            from s in Parser.Char(' ')
            select s;

        static Parser<IEnumerable<char>> Spaces =
            from s in Space.ManyOne()
            select s;

        public static Parser<char> LeftParen =
            from lp in Parser.Char('(')
            select lp;

        public static Parser<char> RightParen =
            from rp in Parser.Char(')')
            select rp;

        static Parser<char> Newline =
            from nl in Parser.Char('\n')
            select nl;

        public static Parser<LispDatum> IntParser =
            from i in Parser.MatchRegex(new Regex("0-90-9*"))
            select (LispDatum)new LispInt(int.Parse(i));

        public static Parser<LispDatum> DecimalParser =
            from d in Parser.MatchRegex(new Regex("0-9*.0-90-9*"))
            select (LispDatum)new LispDecimal(Double.Parse(d));

        public static Parser<LispDatum> NumberParser =
            from n in new List<Parser<LispDatum>>
                        {
                            IntParser,
                            DecimalParser
                        }.EitherOf()
            select n;

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
                //Parser.MatchRegex(new Regex("[^\"\\]"))
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
            from q in Parser.EitherOf(new List<Parser<string>>
            {
                Parser.MatchString("\'"),
                Parser.MatchString("quote ")
            })
            from d in DatumParser
            select (LispDatum)new LispAbbreviation(d);

        static Parser<LispDatum> SymbolParser =
            from i in IdentifierParser
            select (LispDatum) new LispSymbol(i);

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

        static Parser<char> LetterParser =
            from l in Parser.MatchRegex(new Regex("(?i)a-z"))
            select l.First();

        static Parser<string> InitialSubsequentParser =
            from i in InitialParser
            from s in SubsequentParser.Many()
            select string.Concat(i.ToString(), s);

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

        static Parser<string> IdentifierParser =
            from i in Parser.EitherOf(new List<Parser<string>>
            {
                InitialSubsequentParser,
                Parser.MatchString("+"),
                Parser.MatchString("-"),
                Parser.MatchString("...")
            })
            select i;

        public static Parser<LispExpression> VariableParser =
            from i in IdentifierParser
            select (LispExpression)new LispVariable(i);

        static Parser<LispExpression> ApplicationParser = 
            from lp in LeftParen
            from e in ExpressionParser
            from ss in Spaces
            from es in Parser.SeperatedBy(ExpressionParser, Spaces)
            from rp in RightParen
            select (LispExpression)new LispApplication(e, es);

        static Parser<LispExpression> SetParser =
            from lp in LeftParen
            from set in Parser.MatchString("set!")
            from ss in Spaces
            from v in VariableParser
            from ss2 in Spaces
            from e in ExpressionParser
            from rp in RightParen
            select (LispExpression)new LispSet((LispVariable)v, e);

        static Parser<LispExpression> LambdaParser =
            from lp in LeftParen
            from vs in Parser.SeperatedBy(VariableParser, Spaces)
            from ss in Spaces
            from b in BodyParser
            select (LispExpression)new LispLambda((IEnumerable<LispVariable>)vs, b);

        public static Parser<LispExpression> ConstantParser =
            from d in new List<Parser<LispDatum>>
            {
                NumberParser, BooleanParser, CharParser, StringParser
            }.EitherOf()
            select (LispExpression)new LispConstant(d);

        static Parser<LispIf> If2Parser =
            from i in Parser.MatchString("if")
            from ss in Spaces
            from e1 in ExpressionParser
            from ss2 in Spaces
            from e2 in ExpressionParser
            select (LispIf)new LispIf2(e1, e2);

        static Parser<LispIf> If3Parser =
            from i in Parser.MatchString("if")
            from ss in Spaces
            from e1 in ExpressionParser
            from ss2 in Spaces
            from e2 in ExpressionParser
            from ss3 in Spaces
            from e3 in ExpressionParser
            select (LispIf)new LispIf3(e1, e2, e3);

        static Parser<LispExpression> IfParser =
            from lp in LeftParen
            from i in Parser.EitherOf(new List<Parser<LispIf>>
            {
                If2Parser, If3Parser
            })
            from rp in RightParen
            select (LispExpression)i;


        public static Parser<LispExpression> ExpressionParser =
            from e in Parser.EitherOf(new List<Parser<LispExpression>>
            {
                ApplicationParser,
                ConstantParser,
                VariableParser,
                LambdaParser,
                IfParser,
                SetParser
            })
            select e;

        static Parser<LispDefinition> BeginDefinitionParser =
            from lp in LeftParen
            from begin in Parser.MatchString("begin")
            from ss in Spaces
            from ds in Parser.SeperatedBy(DefinitionParser, Spaces)
            from rp in RightParen
            select (LispDefinition)new LispBeginDefinition(ds);

        static Parser<LispBody> BodyParser =
            from ds in Parser.SeperatedBy(DefinitionParser, Spaces)
            from es in Parser.SeperatedBy(ExpressionParser, Spaces)
            select new LispBody(ds, es);

        static Parser<LispVariableDefinition> VariableDefinition2Parser =
            from lp in LeftParen
            from define in Parser.MatchString("define")
            from ss in Spaces
            from lp2 in LeftParen
            from vs in Parser.SeperatedBy(VariableParser, Spaces)
            from rp2 in RightParen
            from ss2 in Spaces
            from b in BodyParser
            from rp in RightParen
            select (LispVariableDefinition)
                new LispVariableDefinition2((IEnumerable<LispVariable>)vs, b);

        static Parser<LispVariableDefinition> VariableDefinition2ConsParser =
            from lp in LeftParen
            from define in Parser.MatchString("define")
            from ss in Spaces
            from lp2 in LeftParen
            from vs in Parser.SeperatedBy(VariableParser, Spaces)
            from ss2 in Spaces
            from dot in Parser.Char('.')
            from ss3 in Spaces
            from v in VariableParser
            from rp2 in RightParen
            from ss4 in Spaces
            from b in BodyParser
            from rp in RightParen
            select (LispVariableDefinition)
                new LispVariableDefinition2((IEnumerable<LispVariable>)vs.Append(v), b);

        static Parser<LispVariableDefinition> VariableDefinition1Parser =
            from lp in LeftParen
            from define in Parser.MatchString("define")
            from ss in Spaces
            from v in VariableParser
            from e in ExpressionParser
            from rp in RightParen
            select (LispVariableDefinition)
                new LispVariableDefinition1((LispVariable)v, e);

        static Parser<LispDefinition> VariableDefinitionParser =
            from vd in Parser.EitherOf(new List<Parser<LispVariableDefinition>>
            {
                VariableDefinition1Parser,
                VariableDefinition2Parser,
                VariableDefinition2ConsParser
            })
            select (LispDefinition)vd;

        static Parser<LispDefinition> DefinitionParser =
            from d in Parser.EitherOf(new List<Parser<LispDefinition>>
            {
                VariableDefinitionParser,
                BeginDefinitionParser
            })
            select d;

        static Parser<LispForm> FormDefinitionParser =
            from d in DefinitionParser
            select (LispForm)d;

        static Parser<LispForm> FormExpressionParser =
            from e in ExpressionParser
            select (LispForm)e;

        public static Parser<LispForm> FormParser =
            from f in Parser.EitherOf(new List<Parser<LispForm>>
            {
                FormDefinitionParser,
                FormExpressionParser
            })
            select f;

        public static Parser<LispProgram> ProgramParser =
            from fs in Parser.SeperatedBy(FormParser, Parser.Char('\n').ManyOne()).EndOfInput()
            select new LispProgram(fs);

    }
}