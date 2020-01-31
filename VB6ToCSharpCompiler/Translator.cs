using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using com.ibm.icu.util;
using com.sun.org.apache.bcel.@internal.classfile;
using com.sun.org.apache.xpath.@internal.functions;
using com.sun.tools.classfile;
using io.proleap.vb6;
using io.proleap.vb6.asg.metamodel;
using io.proleap.vb6.asg.metamodel.call.impl;
using io.proleap.vb6.asg.metamodel.impl;
using io.proleap.vb6.asg.metamodel.statement.function.impl;
using io.proleap.vb6.asg.metamodel.statement.@let.impl;
using io.proleap.vb6.asg.metamodel.statement.sub.impl;
using io.proleap.vb6.asg.metamodel.valuestmt;
using io.proleap.vb6.asg.metamodel.valuestmt.impl;
using javax.management.openmbean;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using org.antlr.v4.runtime.tree;
using sun.security.util;
using Type = System.Type;

namespace VB6ToCSharpCompiler
{
    public class Translator
    {

        const string FunctionReturnValueName = "functionReturnValue";
        const string NewLine = "\r\n";

        public io.proleap.vb6.asg.metamodel.Program program;

        public Translator(io.proleap.vb6.asg.metamodel.Program program)
        {
            this.program = program;
        }


        public static string TranslateType(string vbType)
        {
            switch (vbType)
            {
                case "Boolean":
                    return "bool";
                case "String":
                    return "string";
                case "Variant":
                    return "UknownType_Variant";
                case "Date":
                    return "TDato";
                case "Byte":
                    return "byte";
                case "Integer":
                    return "int";
                case "Long":
                    return "long";
                case "Single":
                    return "single";
                case "Double":
                    return "double";
                case "Currency":
                    return "TCurrency";
                case "Form":
                    return "Form";
            }

            //return "UknownType_" + vbType;
            return vbType;
        }

        [Obsolete("We use ASG, not parser context.")]
        public static string LookupType(VisualBasic6Parser.TypeContext typeContext)
        {
            if (typeContext == null)
            {
                throw new ArgumentNullException(nameof(typeContext));
            }

            if (typeContext.baseType() != null)
            {
                if (typeContext.baseType().BOOLEAN() != null)
                {
                    return typeof(bool).Name;
                }
            }

            return "unknownType";
        }

        public static string LookupType(io.proleap.vb6.asg.metamodel.type.Type typeContext)
        {
            if (typeContext == null)
            {
                throw new ArgumentNullException(nameof(typeContext));
            }

            return TranslateType(typeContext.getName());
        }


        public List<LocalDeclarationStatementSyntax> GetVariableDeclaration(
            VisualBasic6Parser.VariableStmtContext statement)
        {
            if (statement == null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            var a = statement.variableListStmt();
            var declarations = new List<LocalDeclarationStatementSyntax>();

            var listStmt = statement.variableListStmt();

            /*
            
            var asg = (VariableImpl)program.getASGElementRegistry().getASGElement(listStmt);

            var declName = asg.getName();

            var declTypeName = LookupType(asg.getType());
            */

            foreach (var varDecl in listStmt.variableSubStmt()
                .JavaListToCSharpList<VisualBasic6Parser.VariableSubStmtContext>())
            {
                var asgSub = (VariableImpl) program.getASGElementRegistry().getASGElement(varDecl);

                var name = asgSub.getName();

                var typeName = LookupType(asgSub.getType());

                //Console.Error.WriteLine("VAR: " + declName + " " + declTypeName + " " + name + " " + typeName);

                var variables =
                    SyntaxFactory.SeparatedList<VariableDeclaratorSyntax>(
                        new List<VariableDeclaratorSyntax> {SyntaxFactory.VariableDeclarator(name)}
                    );

                //variables.Add(SyntaxFactory.VariableDeclarator("blah"));

                var declaration = SyntaxFactory
                    .LocalDeclarationStatement(
                        SyntaxFactory
                            .VariableDeclaration(SyntaxFactory.IdentifierName(
                                SyntaxFactory.Identifier(typeName)), variables));

                declarations.Add(declaration);
            }

            return declarations;
        }

        public void AddDebugInfo(ParseTree statement, List<StatementSyntax> statements)
        {
            if (statement == null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            if (statements == null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            var asg = program.getASGElementRegistry().getASGElement(statement);

            var asgParent = program.getASGElementRegistry().getASGElement(statement.getParent());
            string parentTypeName = asgParent != null ? asgParent.GetType().Name + "/" : "";

            if (asg != null)
            {
                var type = asg.GetType();
                var typeName = type.Name;
                statements.Add(SyntaxFactory.EmptyStatement()
                    .WithLeadingTrivia(
                        SyntaxFactory.Comment("// TypeName: " + parentTypeName + typeName + ": " +
                                              statement.getText())));
            }
        }

        public StatementSyntax GetReturnValueDeclaration(TypeSyntax returnType)
        {
            var name = FunctionReturnValueName;

            var typeName = returnType;

            //Console.Error.WriteLine("VAR: " + declName + " " + declTypeName + " " + name + " " + typeName);

            var variables =
                SyntaxFactory.SeparatedList<VariableDeclaratorSyntax>(
                    new List<VariableDeclaratorSyntax> {SyntaxFactory.VariableDeclarator(name)}
                );

            //variables.Add(SyntaxFactory.VariableDeclarator("blah"));

            var declaration = SyntaxFactory
                .LocalDeclarationStatement(
                    SyntaxFactory
                        .VariableDeclaration(returnType, variables));

            return declaration;
        }

        public T GetAsg<T>(ParseTree tree)
        {
            return (T) program.getASGElementRegistry().getASGElement(tree);
        }

        
        ExpressionSyntax GetExpression(ParseTree tree, List<StatementSyntax> statements)
        {
            var tfe = new TranslatorForExpression(this);
            return tfe.GetExpression(tree, statements);
        }

        public ExpressionSyntax GetRightHandSide(LetImpl asg, List<StatementSyntax> statementList)
        {
            var right = GetExpression(asg.getRightHandValueStmt().getCtx(), statementList);
            return right;
        }

        public ExpressionStatementSyntax GetAssignment(ExpressionSyntax left, ExpressionSyntax right)
        {
            if (right == null)
            {
                right = SyntaxFactory.IdentifierName("RHS_NULL");
            }
            var assignment =
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        left, right));
            return assignment;
        }

        public BlockSyntax GetBody(TypeSyntax returnType, VisualBasic6Parser.BlockContext block)
        {
            var comment = SyntaxFactory.Comment("// METHOD BODY");

            var statementList = new List<StatementSyntax>
            {
                // TODO: Don't do it if only returning at end of function
                GetReturnValueDeclaration(returnType),

                SyntaxFactory.EmptyStatement().WithLeadingTrivia(comment)
            };

            if (block != null)
            {
                foreach (var stmt in block.blockStmt().JavaListToCSharpList<VisualBasic6Parser.BlockStmtContext>())
                {
                    statementList.Add(SyntaxFactory.EmptyStatement().WithLeadingTrivia(SyntaxFactory.Comment("// " + stmt.getText())));
                    if (stmt.variableStmt() != null)
                    {
                        var variableDeclarations = GetVariableDeclaration(stmt.variableStmt());
                        statementList.AddRange(variableDeclarations);
                    }
                    else if (stmt.letStmt() != null)
                    {
                        var letStmt = stmt.letStmt();
                        var asg = (LetImpl) program.getASGElementRegistry().getASGElement(letStmt);
                        if (asg.isSettingReturnVariable())
                        {
                            var left = SyntaxFactory.IdentifierName(FunctionReturnValueName);
                            var right = GetRightHandSide(asg, statementList);
                            statementList.Add(GetAssignment(left, right));
                        }
                        else
                        {
                            string varName =
                                letStmt
                                    ?.implicitCallStmt_InStmt()
                                    ?.iCS_S_VariableOrProcedureCall()
                                    ?.ambiguousIdentifier()
                                    ?.IDENTIFIER(0)
                                    ?.getSymbol()
                                    ?.getText();

                            if (varName == null)
                            {
                                varName = "UNKNOWN_ASSIGNMENT_STATEMENT_TARGET";
                            }
                            var left = SyntaxFactory.IdentifierName(varName);
                            var right = GetRightHandSide(asg, statementList);
                            statementList.Add(GetAssignment(left, right));
                        }
                    }
                    else
                    {
                        // TODO: optimize if necessary
                        try
                        {
                            // Get the type handle of a specified class.
                            Type myType = stmt.GetType();

                            // Get the methods of the specified class.
                            var methods = myType.GetMethods();

                            for (int i = 0; i < methods.Length; i++)
                            {
                                var method = methods[i];
                                var methodParameters = method.GetParameters();
                                if (methodParameters.Length == 0)
                                {
                                    var methodName = methods[i].Name;
                                    if (methodName.EndsWith("Stmt", false, CultureInfo.CurrentCulture))
                                    {
                                        try
                                        {
                                            var ret = method.Invoke(stmt, new object[0]);
                                            if (ret != null)
                                            {
                                                statementList.Add(SyntaxFactory.EmptyStatement()
                                                    .WithLeadingTrivia(SyntaxFactory.Comment(
                                                        "// Statement Type: " + methodName + ":" +
                                                        (ret != null).ToString())));
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine("Exception : {0} ", e.Message);
                                        }
                                    }
                                }
                                
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }

            var returnStatement = SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName(FunctionReturnValueName));
            statementList.Add(returnStatement);

            return SyntaxFactory.Block(SyntaxFactory.List(
                statementList
            ));
        }

        public List<MethodDeclarationSyntax> GetMethods(VisualBasic6Parser.ModuleBodyContext vb6Body)
        {
            var methods = new List<MethodDeclarationSyntax>();

            var attributeLists = SyntaxFactory.List<AttributeListSyntax>();
            var modifiers = SyntaxFactory.TokenList(new List<SyntaxToken>()
            {
                SyntaxFactory.Token(SyntaxKind.PublicKeyword)
            });

            if (vb6Body == null)
            {
                throw new ArgumentNullException(nameof(vb6Body));
            }

            foreach (var element in vb6Body.moduleBodyElement().JavaListToCSharpList<VisualBasic6Parser.ModuleBodyElementContext>())
            {

                Console.Error.WriteLine("Element Type: " + element.GetType().Name);
                Console.Error.WriteLine("Element Type: " + (element.subStmt() != null));

                if (element.subStmt() != null)
                {
                    var method = GetMethod(program, element, attributeLists, modifiers);
                    methods.Add(method);
                }
                if (element.functionStmt() != null)
                {
                    var method = GetMethod(program, element, attributeLists, modifiers);
                    methods.Add(method);
                }
            }

            return methods;
        }

        private MethodDeclarationSyntax GetMethod(io.proleap.vb6.asg.metamodel.Program program, VisualBasic6Parser.ModuleBodyElementContext element,
            SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers)
        {
            //Console.Error.WriteLine("Element Type: " + element.subStmt().GetType().Name);

            var subStmt = element.subStmt();
            var funStmt = element.functionStmt();

            SyntaxToken identifier = SyntaxFactory.Identifier("uknownMethodName");
            TypeSyntax returnType = null;
            if (subStmt != null)
            {
                var asg = (SubImpl) program.getASGElementRegistry().getASGElement(subStmt);
                if (asg == null)
                {
                    throw new InvalidOperationException("asg is null");
                }
                returnType = SyntaxFactory.ParseTypeName("void");
                identifier = SyntaxFactory.Identifier(asg.getName());
            }
            else
            {
                var asg = (FunctionImpl)program.getASGElementRegistry().getASGElement(funStmt);
                if (asg == null)
                {
                    throw new InvalidOperationException(nameof(asg) + " is null");
                }
                returnType = SyntaxFactory.ParseTypeName(LookupType(asg.getType()));
                identifier = SyntaxFactory.Identifier(asg.getName());
            }

            if (returnType == null)
            {
                throw new InvalidOperationException(nameof(returnType) + " is null");
            }
            if (identifier == null)
            {
                throw new InvalidOperationException(nameof(identifier) + " is null");
            }

            ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier = null; // SyntaxFactory.ExplicitInterfaceSpecifier(SyntaxFactory.ParseName("testInterface"));

            TypeParameterListSyntax typeParameterList = null; // SyntaxFactory.TypeParameterList(new SeparatedSyntaxList<TypeParameterSyntax>());

            // TODO: fix it
            var parameterList = new List<ParameterSyntax>();
            var argList = subStmt != null ? subStmt.argList() : funStmt.argList();
            foreach (var arg in argList.arg().JavaListToCSharpList<VisualBasic6Parser.ArgContext>())
            {
                // TODO: check if BYREF or BYVAL
                // TODO: check if arg.toString() gives the correct name
                var argImpl = (ArgImpl) program.getASGElementRegistry().getASGElement(arg);
                Console.Error.WriteLine("Parameter Type: " + LookupType(argImpl.getType()));

                var argName = argImpl.getName();
                var typeName = LookupType(argImpl.getType());
                
                parameterList.Add(
                    SyntaxFactory.Parameter(
                        SyntaxFactory.List<AttributeListSyntax>(),
                        SyntaxFactory.TokenList(),
                        SyntaxFactory.ParseTypeName(typeName),
                        SyntaxFactory.Identifier(argName),
                        null));
            }

            var parameterListSyntax = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameterList));

            var constraintClauses = SyntaxFactory.List<TypeParameterConstraintClauseSyntax>();
            
            var block = subStmt != null ? subStmt.block() : funStmt.block();

            if (block == null)
            {
                Console.Error.WriteLine("Block is null for: " + element.toStringTree());
            }


            var csharpBody = GetBody(returnType, block);

            var semicolonToken = SyntaxFactory.Token(SyntaxKind.SemicolonToken);

            var method = SyntaxFactory.MethodDeclaration(attributeLists, modifiers, returnType,
                explicitInterfaceSpecifier, identifier, typeParameterList, parameterListSyntax, constraintClauses, csharpBody,
                semicolonToken);
            return method;
        }

        public static string GetPath(ParseTree node)
        {
            var parent = node;
            string s = "";
            //var seen = new Dictionary<ParseTree, bool>();
            while (parent != null)
            {
                s = parent.GetType().Name + "/" + s;
                parent = parent.getParent();
            }
            return s;
        }
        public static string GetPath(ASGElement node)
        {
            var parent = node;
            string s = "";
            while (parent != null)
            {
                s = parent.GetType().Name + "/" + s;
                parent = parent.getParent();
            }
            return s;
        }

        public static string GetElementProperties(object obj)
        {
            string s = "";

            var type = obj.GetType();

            var methods = type.GetMethods();

            for (int i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                var methodParameters = method.GetParameters();
                if (methodParameters.Length == 0)
                {
                    var methodName = methods[i].Name;
                    
                    try
                    {
                        var ret = method.Invoke(obj, new object[0]);
                        if (ret != null)
                        {
                            s += "Prop: " + methodName + ": " + ret.GetType().Name + ": " + ret.ToString() + NewLine;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception : {0} ", e.Message);
                    }
                }

            }

            return s;
        }


        public string Dump(ParseTree node)
        {
            string s = "";

            var asg = GetAsg<ASGElement>(node);

            s += "Node Type Path: " + GetPath(node) + NewLine;
            s += "ASG  Type Path: " + GetPath(asg) + NewLine;
            s += "     Node Text: " + node.getText() + NewLine;
            s += "Node Properties: " + GetElementProperties(node) + NewLine;
            if (asg != null)
            {
                s += "ASG Properties: " + GetElementProperties(asg) + NewLine;
            }

            return s;
        }

        public CompilationUnitSyntax Translate(io.proleap.vb6.asg.metamodel.Program program, ModuleImpl module)
        {

            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            var ctx = module.getCtx();
            var body = ctx.moduleBody();

            var methods = GetMethods(body);

            var classMembers = SyntaxFactory.List<MemberDeclarationSyntax>(methods);

            var classes = new List<ClassDeclarationSyntax>();
            var classAttributeLists = SyntaxFactory.List<AttributeListSyntax>(
                new List<AttributeListSyntax>()
                {
                    //SyntaxFactory.AttributeList()
                }
            );
            var classModifiers = SyntaxFactory.TokenList(new List<SyntaxToken>()
            {
                SyntaxFactory.Token(SyntaxKind.PublicKeyword)
            });
            var className = module.getName();
            var cls = SyntaxFactory.ClassDeclaration(
                classAttributeLists,
                classModifiers,
                SyntaxFactory.Token(SyntaxKind.ClassKeyword),
                SyntaxFactory.Identifier(className),
                null,
                null,
                SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
                SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                classMembers,
                SyntaxFactory.Token(SyntaxKind.CloseBraceToken), 
                SyntaxFactory.Token(SyntaxKind.SemicolonToken)
            );
            classes.Add(cls);

            var nsExterns = SyntaxFactory.List<ExternAliasDirectiveSyntax>(
                new List<ExternAliasDirectiveSyntax>()
                {
                    //SyntaxFactory.ExternAliasDirective(SyntaxFactory.Token(SyntaxKind.ExternKeyword))
                });

            var nsUsings = SyntaxFactory.List<UsingDirectiveSyntax>(new List<UsingDirectiveSyntax>()
                {
                    //SyntaxFactory.UsingDirective(SyntaxFactory.NameEquals("usingAlias"), SyntaxFactory.ParseName("blah"))
                }
            );

            var nsAttributeLists = SyntaxFactory.List<AttributeListSyntax>(
                new List<AttributeListSyntax>()
                {
                    //SyntaxFactory.AttributeList()
                }
            );

            var nsMembers = SyntaxFactory.List<MemberDeclarationSyntax>(new List<MemberDeclarationSyntax>(
                classes
            ));

            var nsName = "VB6ConvertedApp";
            var csharpNamespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(nsName), nsExterns,
                nsUsings,
                nsMembers);

            var externs = SyntaxFactory.List<ExternAliasDirectiveSyntax>(
                new List<ExternAliasDirectiveSyntax>()
                {
                    //SyntaxFactory.ExternAliasDirective(SyntaxFactory.Token(SyntaxKind.ExternKeyword))
                });

            var usings = SyntaxFactory.List<UsingDirectiveSyntax>(new List<UsingDirectiveSyntax>()
                {
                    SyntaxFactory.UsingDirective(SyntaxFactory.NameEquals("usingAlias"), SyntaxFactory.ParseName("blah"))
                }
            );

            var attributeLists = SyntaxFactory.List<AttributeListSyntax>(
                new List<AttributeListSyntax>()
                {
                    //SyntaxFactory.AttributeList()
                }
            );

            var members = SyntaxFactory.List<MemberDeclarationSyntax>(new List<MemberDeclarationSyntax>(
                new List<MemberDeclarationSyntax>() { csharpNamespace }
                ));

            var compilationUnit = SyntaxFactory.CompilationUnit(
                externs,
                usings,
                attributeLists,
                members,
                SyntaxFactory.Token(SyntaxKind.EndOfFileToken));

            return compilationUnit.NormalizeWhitespace();
        }
    }
}
