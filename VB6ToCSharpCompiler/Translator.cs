using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using io.proleap.vb6;
using io.proleap.vb6.asg.metamodel.impl;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using org.antlr.v4.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public static class Translator
    {


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

        public static BlockSyntax GetBody(VisualBasic6Parser.BlockContext block)
        {
            var comment = SyntaxFactory.Comment("// METHOD BODY");

            var commentList = new List<StatementSyntax>();

            commentList.Add(SyntaxFactory.EmptyStatement().WithLeadingTrivia(comment));

            if (block != null)
            {
                foreach (var stmt in block.blockStmt().JavaListToCSharpList<VisualBasic6Parser.BlockStmtContext>())
                {
                    commentList.Add(SyntaxFactory.EmptyStatement().WithLeadingTrivia(SyntaxFactory.Comment("// " + stmt.getText())));
                }
            }

            return SyntaxFactory.Block(SyntaxFactory.List<StatementSyntax>(
                commentList
            ));
        }

        public static List<MethodDeclarationSyntax> GetMethods(io.proleap.vb6.asg.metamodel.Program program, VisualBasic6Parser.ModuleBodyContext vb6Body)
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

            if (program == null)
            {
                throw new ArgumentNullException(nameof(program));
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

        private static MethodDeclarationSyntax GetMethod(io.proleap.vb6.asg.metamodel.Program program, VisualBasic6Parser.ModuleBodyElementContext element,
            SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers)
        {
            //Console.Error.WriteLine("Element Type: " + element.subStmt().GetType().Name);

            var subStmt = element.subStmt();
            var funStmt = element.functionStmt();

            var asg =
                subStmt != null ?
                    program.getASGElementRegistry().getASGElement(subStmt) :
                    program.getASGElementRegistry().getASGElement(funStmt);

            if (asg == null)
            {
                throw new InvalidOperationException("asg is null");
            }

            // TODO: correct it
            
            var returnType = SyntaxFactory.ParseTypeName(asg.GetType().Name);

            var explicitInterfaceSpecifier = SyntaxFactory.ExplicitInterfaceSpecifier(SyntaxFactory.ParseName("testInterface"));

            var identifier = SyntaxFactory.Identifier("methodName");

            var typeParameterList =
                SyntaxFactory.TypeParameterList(new SeparatedSyntaxList<TypeParameterSyntax>(
                ));

            // TODO: fix it
            var parameterList = new List<ParameterSyntax>();
            var argList = subStmt != null ? subStmt.argList() : funStmt.argList();
            foreach (var arg in argList.arg().JavaListToCSharpList<VisualBasic6Parser.ArgContext>())
            {
                // TODO: check if BYREF or BYVAL
                // TODO: check if arg.toString() gives the correct name
                var argName = arg.toString();
                var typeName = LookupType(arg.asTypeClause().type());
                // var csharpArg = arg.typeHint()
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
            

            var csharpBody = GetBody(block);

            var semicolonToken = SyntaxFactory.Token(SyntaxKind.SemicolonToken);

            var method = SyntaxFactory.MethodDeclaration(attributeLists, modifiers, returnType,
                explicitInterfaceSpecifier, identifier, typeParameterList, parameterListSyntax, constraintClauses, csharpBody,
                semicolonToken);
            return method;
        }

        public static CompilationUnitSyntax Translate(io.proleap.vb6.asg.metamodel.Program program, ModuleImpl module)
        {

            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            var ctx = module.getCtx();
            var body = ctx.moduleBody();

            var methods = GetMethods(program, body);

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
