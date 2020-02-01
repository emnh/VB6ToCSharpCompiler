using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using io.proleap.vb6;
using io.proleap.vb6.asg.metamodel;
using io.proleap.vb6.asg.metamodel.call.impl;
using io.proleap.vb6.asg.metamodel.impl;
using io.proleap.vb6.asg.metamodel.statement.ifstmt.impl;
using io.proleap.vb6.asg.metamodel.statement.@let.impl;
using io.proleap.vb6.asg.metamodel.valuestmt.impl;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public class TranslatorForExpression
    {
        private Translator translator;

        public TranslatorForExpression(Translator translator)
        {
            this.translator = translator;
        }

        public List<ExpressionSyntax> GetGoodChildren(ParseTree tree, List<StatementSyntax> statements)
        {
            var returnValue = new List<ExpressionSyntax>();
            for (int i = 0; i < tree.getChildCount(); i++)
            {
                var child = tree.getChild(i);
                var expr = GetExpression(child, statements);
                if (expr != null)
                {
                    returnValue.Add(expr);
                }
            }
            return returnValue;
        }

        public ExpressionSyntax GetFirstGoodChild(string forwardType, ParseTree tree, List<StatementSyntax> statements)
        {
            Console.Error.WriteLine("GetFirstGoodChild: " + forwardType);
            var list = GetGoodChildren(tree, statements);
            if (list.Count > 1)
            {
                throw new InvalidOperationException("More than one good child.");
            }
            else if (list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public ExpressionSyntax GetExpression(ApiProcedureCallImpl asg, List<StatementSyntax> statements)
        {
            throw new InvalidOperationException("Prefer patterns over this method.");

            var tree = asg.getCtx();

            var callName = asg.getApiProcedure().getName();

            var argList = new List<ArgumentSyntax>();

            GetArguments(statements, tree, argList);

            var callSyntax =
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName(callName),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(argList)));

            return callSyntax;
        }

        public ExpressionSyntax GetExpression(FunctionCallImpl asg, List<StatementSyntax> statements)
        {
            throw new InvalidOperationException("Prefer patterns over this method.");

            var tree = asg.getCtx();

            var callName = asg.getFunction().getName();

            var argList = new List<ArgumentSyntax>();

            GetArguments(statements, tree, argList);

            var callSyntax =
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName(callName),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(argList)));

            return callSyntax;
        }

        private void GetArguments(List<StatementSyntax> statements, ParserRuleContext tree, List<ArgumentSyntax> argList)
        {
            for (int i = 0; i < tree.getChildCount(); i++)
            {
                var child = tree.getChild(i);

                if (child is VisualBasic6Parser.ArgsCallContext)
                {
                    for (int j = 0; j < child.getChildCount(); j++)
                    {
                        var argChild = child.getChild(j);
                        if (argChild != null)
                        {
                            var expr = GetExpression(argChild, statements);
                            if (expr != null)
                            {
                                argList.Add(SyntaxFactory.Argument(expr));
                            }
                        }
                    }
                }
            }
        }

        

        public ExpressionSyntax GetExpression(CallValueStmtImpl asg, List<StatementSyntax> statements)
        {
            throw new InvalidOperationException("Prefer patterns over this method.");
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }
            return GetFirstGoodChild(nameof(CallValueStmtImpl), asg.getCtx(), statements);
        }

        public ExpressionSyntax GetExpression(LiteralValueStmtImpl asg, List<StatementSyntax> statements)
        {
            throw new InvalidOperationException("Prefer patterns over this method.");
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            return GetFirstGoodChild(nameof(LiteralValueStmtImpl), asg.getCtx(), statements);
        }

        public ExpressionSyntax GetExpression(LiteralImpl asg, List<StatementSyntax> statements)
        {
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }


            if (asg.getCtx().STRINGLITERAL() != null)
            {
                //return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(asg.getCtx().STRINGLITERAL().getText());
                return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(asg.getValue()));
            }
            else if (asg.getCtx().INTEGERLITERAL() != null)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(Int32.Parse(asg.getValue())));
            }
            else if (asg.getCtx().DOUBLELITERAL() != null)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(Double.Parse(asg.getValue())));
            }
            else if (asg.getCtx().FILENUMBER() != null)
            {

                var num = asg.getValue().Trim('#');
                if (num.All(c => char.IsDigit(c)))
                {
                    return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(int.Parse(num)));
                }
                else
                {
                    return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(asg.getValue()));
                }
                
            }
            else if (asg.getCtx().COLORLITERAL() != null)
            {
                throw new InvalidOperationException("color literal");
                //return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(asg.getCtx().COLORLITERAL().getText()));
            }
            else
            {
                // TODO: A bit risky. Assumes that literals are same in VB6 and C#.
                return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(asg.getValue()));
            }

            //return GetExpression(asg.getCtx().getChild(0), statements);
        }

        public ExpressionSyntax GetExpression(CallDelegateImpl asg, List<StatementSyntax> statements)
        {
            throw new InvalidOperationException("Prefer patterns over this method.");
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            return GetFirstGoodChild(nameof(CallDelegateImpl),asg.getCtx(), statements);
        }

        /*
        public ExpressionSyntax GetExpression(LetImpl asg, List<StatementSyntax> statements)
        {
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            return GetFirstGoodChild(nameof(LetImpl), asg.getCtx(), statements);
        }
        */

        public ExpressionSyntax GetExpression(ArgCallImpl asg, List<StatementSyntax> statements)
        {
            throw new InvalidOperationException("Prefer patterns over this method.");
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            return SyntaxFactory.IdentifierName(asg.getArg().getName());
        }

        public ExpressionSyntax GetExpression(StringValueStmtImpl asg, List<StatementSyntax> statements)
        {
            throw new InvalidOperationException("Prefer patterns over this method.");
            var ctx = asg.getCtx();
            var children = GetGoodChildren(asg.getCtx(), statements);
            if (children.Count < 2)
            {
                throw new NotImplementedException("String add < 2: " + ctx.GetType().Name + ": " + ctx.getText());
            }
            return SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, children[0], children[1]);
        }

        // TODO: make sure all calls are resolving
        public ExpressionSyntax GetExpression(UndefinedCallImpl asg, List<StatementSyntax> statements)
        {
            var tree = asg.getCtx();

            var callName = asg.getName();

            var argList = new List<ArgumentSyntax>();

            GetArguments(statements, tree, argList);

            /*
            for (int i = 0; i < tree.getChildCount(); i++)
            {
                var child = tree.getChild(i);

                if (child is VisualBasic6Parser.ArgsCallContext)
                {
                    for (int j = 0; j < child.getChildCount(); j++)
                    {
                        var argChild = child.getChild(j);
                        if (argChild != null)
                        {
                            var expr = GetExpression(argChild, statements);
                            if (expr != null)
                            {
                                argList.Add(SyntaxFactory.Argument(expr));
                            }
                        }
                    }
                }
            }
            */

            var callSyntax =
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName("UndefinedCall_" + callName),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(argList)));

            return callSyntax;
        }

        public ExpressionSyntax GetExpression(ArgValueAssignmentImpl asg, List<StatementSyntax> statements)
        {
            throw new InvalidOperationException("Prefer patterns over this method.");
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            return GetFirstGoodChild(nameof(ArgValueAssignmentImpl), asg.getCtx(), statements);
        }

        public ExpressionSyntax GetExpression(IfConditionImpl asg, List<StatementSyntax> statements)
        {
            throw new InvalidOperationException("Prefer patterns over this method.");
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            return GetFirstGoodChild(nameof(IfConditionImpl), asg.getCtx(), statements);
        }

        public ExpressionSyntax GetExpression(TypeElementCallImpl asg, List<StatementSyntax> statements)
        {
            throw new InvalidOperationException("Prefer patterns over this method.");

            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            return GetFirstGoodChild(nameof(IfConditionImpl), asg.getCtx(), statements);
        }

        public ExpressionSyntax GetExpression(ConstantCallImpl asg, List<StatementSyntax> statements)
        {
            throw new InvalidOperationException("Prefer patterns over this method.");
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            return GetFirstGoodChild(nameof(ConstantCallImpl), asg.getCtx(), statements);
        }

        /*
        public ExpressionSyntax GetExpression(MembersCallImpl asg, List<StatementSyntax> statements)
        {
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            //var left = GetExpression(asg.getCtx().getChild(0), statements);
            var left = GetFirstGoodChild(nameof(MembersCallImpl), asg.getCtx(), statements);
            var right = asg.getName(); // GetExpression(asg.getCtx().getChild(1)), statements);
            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, left, SyntaxFactory.Token(SyntaxKind.DotToken), SyntaxFactory.IdentifierName(right));
            //return GetFirstGoodChild(asg.getCtx(), statements);
        }*/

        public ExpressionSyntax GetExpression(VariableCallImpl asg, List<StatementSyntax> statements)
        {
            throw new InvalidOperationException("Prefer patterns over this method.");

            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            //var left = GetExpression(asg.getCtx().getChild(0), statements);
            //var left = GetFirstGoodChild(asg.getCtx(), statements);
            //var right = asg.getName(); // GetExpression(asg.getCtx().getChild(1)), statements);
            //return SyntaxFactory.MemberAccessExpression(SyntaxKind.MemberBindingExpression, left, SyntaxFactory.Token(SyntaxKind.DotToken), SyntaxFactory.IdentifierName(right));
            //return GetFirstGoodChild(asg.getCtx(), statements);
            var name = asg.getVariable().getName();
            return SyntaxFactory.IdentifierName(name);
        }




        public ExpressionSyntax GetExpression(ParseTree tree, List<StatementSyntax> statements)
        {
            if (statements == null)
            {
                throw new ArgumentNullException(nameof(statements));
            }
            if (tree == null)
            {
                throw new ArgumentNullException(nameof(tree));
            }

            var asg = this.translator.program.getASGElementRegistry().getASGElement(tree);

            if (TranslatorForPattern.CanTranslate(translator, tree))
            {
                return TranslatorForPattern.TranslateExpression(this.translator, tree);
            }

            var goodChildren = GetGoodChildren(tree, statements);
            if (goodChildren.Count == 1)
            {
                Console.Error.WriteLine("FORWARDED: " + Pattern.LookupNodeType(tree) + ": " + tree.getText());
                return goodChildren[0];
            }

            // TODO: optimize if too slow
            var methods = this.GetType().GetMethods();
            foreach (var method in methods)
            {
                if (method.Name.Equals("GetExpression", StringComparison.InvariantCulture))
                {
                    var methodParameters = method.GetParameters();

                    if (methodParameters.Length > 0 && asg != null && methodParameters[0].ParameterType == asg.GetType())
                    {
                        Console.Error.WriteLine("Invoking specific GetExpression on: " + tree.getText());
                        //statements.Add(SyntaxFactory.EmptyStatement().WithLeadingTrivia(SyntaxFactory.Comment("// Invoking GetExpression: " + asg.GetType().Name + ":" + asg.getCtx().depth())));
                        return (ExpressionSyntax)method.Invoke(this, new object[] { asg, statements });
                    }
                }
            }

            /*else if (tree is VisualBasic6Parser.TypeHintContext)
            {
                return SyntaxFactory.CastExpression(
                    SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                    SyntaxFactory.ParseTypeName("string"),
                    SyntaxFactory.Token(SyntaxKind.CloseParenToken),
                );*/

            if (tree is TerminalNodeImpl)
            {
                return null;
            } else if (tree is VisualBasic6Parser.CertainIdentifierContext || 
                       tree is VisualBasic6Parser.AmbiguousIdentifierContext ||
                       tree is VisualBasic6Parser.AmbiguousKeywordContext)
            {
                var name = tree.getText().Trim();
                if (!name.All(c => char.IsLetterOrDigit(c) || "_$".Contains(c)))
                {
                    throw new InvalidOperationException("Identifier was not alphanumeric: " + name);
                }
                return SyntaxFactory.IdentifierName(name);
            } else if (tree is VisualBasic6Parser.ArgsCallContext)
            {
                return GetFirstGoodChild(nameof(ConstantCallImpl), tree, statements);
            }

            var explanation = "// " + Pattern.LookupNodeType(tree) + " not in [" + TranslatorForPattern.DocPatterns() + "]" + Translator.NewLine;
            Console.Error.WriteLine(nameof(GetExpression) + ": " + Pattern.LookupNodeType(tree) + ": " + tree.getText());
            Console.Error.WriteLine(explanation);
            if (asg != null)
            {
                Console.Error.WriteLine(nameof(GetExpression) + ": " + asg.GetType().Name);
            }
            // TODO: Reenable.
            throw new InvalidOperationException("Expression returned null");

            return null;
        }
    }
}
