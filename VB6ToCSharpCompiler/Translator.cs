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

namespace VB6ToCSharpCompiler
{
    public static class Translator
    {

        

        public static CompilationUnitSyntax Translate(ModuleImpl module)
        {

            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            var ctx = module.getCtx();
            var body = ctx.moduleBody();
            

            /*
            var class = SyntaxFactory.ClassDeclaration(
                SyntaxFactory.SyntaxList<AttributeListSyntax>(),
                SyntaxTokenList.Create(),
                new SyntaxToken(),
                new SyntaxToken(),
                null,
                null,
                new SyntaxList<TypeParameterConstraintClauseSyntax>(),
                new SyntaxToken(),
                new SyntaxList<MemberDeclarationSyntax>(),
                new SyntaxToken(), 
                new SyntaxToken()
            );
            */

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
                new SyntaxList<TypeParameterConstraintClauseSyntax>(),
                SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                new SyntaxList<MemberDeclarationSyntax>(),
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
            var csharpNamespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(nsName), nsExterns, nsUsings,
                nsMembers);

            /*
            foreach (var element in body.moduleBodyElement().JavaListToCSharpList<VisualBasic6Parser.ModuleBodyElementContext>())
            {
                
            }
            */

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
