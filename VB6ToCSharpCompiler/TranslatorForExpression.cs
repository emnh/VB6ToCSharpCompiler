using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using io.proleap.vb6;
using io.proleap.vb6.asg.metamodel;
using io.proleap.vb6.asg.metamodel.call.impl;
using io.proleap.vb6.asg.metamodel.impl;
using io.proleap.vb6.asg.metamodel.statement.@let.impl;
using io.proleap.vb6.asg.metamodel.valuestmt.impl;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using org.antlr.v4.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public class TranslatorForExpression
    {
        private Translator translator;
        private int depth = 0;

        //private Dictionary<ASGElement, ExpressionSyntax> mapping = new Dictionary<ASGElement, ExpressionSyntax>();

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

        public ExpressionSyntax GetFirstGoodChild(ParseTree tree, List<StatementSyntax> statements)
        {
            var list = GetGoodChildren(tree, statements);
            if (list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public ExpressionSyntax GetExpression(ApiProcedureCallImpl asg, List<StatementSyntax> statements)
        {
            var tree = asg.getCtx();

            var callName = asg.getApiProcedure().getName();

            var argList = new List<ArgumentSyntax>();

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

            var callSyntax =
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName(callName),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(argList)));

            return callSyntax;
        }

        public ExpressionSyntax GetExpression(CallValueStmtImpl asg, List<StatementSyntax> statements)
        {
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            return GetFirstGoodChild(asg.getCtx(), statements);
        }

        public ExpressionSyntax GetExpression(LiteralValueStmtImpl asg, List<StatementSyntax> statements)
        {
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            return GetFirstGoodChild(asg.getCtx(), statements);
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
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(Int32.Parse(asg.getValue())));
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
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            return GetFirstGoodChild(asg.getCtx(), statements);
        }

        public ExpressionSyntax GetExpression(LetImpl asg, List<StatementSyntax> statements)
        {
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            return GetFirstGoodChild(asg.getCtx(), statements);
        }

        public ExpressionSyntax GetExpression(ArgCallImpl asg, List<StatementSyntax> statements)
        {
            
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            return SyntaxFactory.IdentifierName(asg.getArg().getName());
        }

        public ExpressionSyntax GetExpression(StringValueStmtImpl asg, List<StatementSyntax> statements)
        {
            var ctx = asg.getCtx();
            var children = GetGoodChildren(asg.getCtx(), statements);
            if (children.Count < 2)
            {
                throw new NotImplementedException("String add < 2: " + ctx.GetType().Name + ": " + ctx.getText());
            }
            return SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, children[0], children[1]);
        }

        public ExpressionSyntax GetExpression(ArithmeticValueStmtImpl asg, List<StatementSyntax> statements)
        {
            var ctx = asg.getCtx();
            var children = GetGoodChildren(asg.getCtx(), statements);
            if (children.Count < 2)
            {
                throw new NotImplementedException("Arithmetic < 2: " + ctx.GetType().Name + ": " + ctx.getText());
            }
            if (ctx is VisualBasic6Parser.VsMinusContext)
            {
                return SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, children[0], children[1]);
            //} else if (ctx is VisualBasic6Parser.VsPlusContext)
            //{
            //    return SyntaxFactory.BinaryExpression(SyntaxKind.PlusToken, children[0], children[1]);
            } else if (ctx is VisualBasic6Parser.VsAddContext)
            {
                return SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, children[0], children[1]);
            }
            else if (ctx is VisualBasic6Parser.VsMultContext)
            {
                return SyntaxFactory.BinaryExpression(SyntaxKind.MultiplyExpression, children[0], children[1]);
            }
            else if (ctx is VisualBasic6Parser.VsDivContext)
            {
                return SyntaxFactory.BinaryExpression(SyntaxKind.DivideExpression, children[0], children[1]);
            }
            else
            {
                throw new NotImplementedException("Arithmetic: " + ctx.GetType().Name + ": " + ctx.getText());
            }
        }

        // TODO: make sure all calls are resolving
        public ExpressionSyntax GetExpression(UndefinedCallImpl asg, List<StatementSyntax> statements)
        {
            var tree = asg.getCtx();

            var callName = asg.getName();

            var argList = new List<ArgumentSyntax>();

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

            var callSyntax =
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName(callName),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(argList)));

            return callSyntax;
        }

        public ExpressionSyntax GetExpression(ArgValueAssignmentImpl asg, List<StatementSyntax> statements)
        {
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            return GetFirstGoodChild(asg.getCtx(), statements);
        }

        public ExpressionSyntax GetExpression(MembersCallImpl asg, List<StatementSyntax> statements)
        {
            if (asg == null)
            {
                throw new ArgumentNullException(nameof(asg));
            }

            //var left = GetExpression(asg.getCtx().getChild(0), statements);
            var left = GetFirstGoodChild(asg.getCtx(), statements);
            var right = asg.getName(); // GetExpression(asg.getCtx().getChild(1)), statements);
            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, left, SyntaxFactory.Token(SyntaxKind.DotToken), SyntaxFactory.IdentifierName(right));
            //return GetFirstGoodChild(asg.getCtx(), statements);
        }

        public ExpressionSyntax GetExpression(VariableCallImpl asg, List<StatementSyntax> statements)
        {
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

            /*if (asg is CallValueStmtImpl call)
            {
                if (tree.getChildCount() > 1)
                {
                    throw new InvalidOperationException("CallValueStmtImpl has more than one child");
                }
                for (int i = 0; i < tree.getChildCount(); i++)
                {
                    var child = tree.getChild(i);
                    // TODO: what to do if there is more than one child?
                    return GetExpression(child, statements);
                }
            }
            else
            {*/
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

            if (asg == null)
            {
                return GetFirstGoodChild(tree, statements);
            }

            // For debugging
            //translator.AddDebugInfo(tree, statements);
            //for (int i = 0; i < tree.getChildCount(); i++)
            //{
            //    var child = tree.getChild(i);
            //    GetExpression(child, statements);
            //}

            //}
            /*
            AddDebugInfo(tree.getCtx(), statements);

            //var asg = (LetImpl) program.getASGElementRegistry().getASGElement(tree);

            foreach (var sub in tree.getSubValueStmts().JavaListToCSharpList<ValueStmt>())
            {

                //AddDebugInfo();
            }
            */

            //var comment = "// Unhandled GetExpression: " + tree.GetType().Name + ":" + tree.getText();
            //statements.Add(SyntaxFactory.EmptyStatement().WithLeadingTrivia(
            //    SyntaxFactory.Comment(comment)));

            //return SyntaxFactory.IdentifierName("Unhandled: " + tree.GetType().Name + ":" + tree.getText());
            return null;
        }
    }
}
