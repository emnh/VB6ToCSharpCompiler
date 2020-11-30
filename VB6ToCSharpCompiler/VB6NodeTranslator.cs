﻿using org.antlr.v4.runtime.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler
{
    public abstract class VB6NodeTranslator {
        public Dictionary<ContextNodeType, VB6NodeTranslator> translatorDict { get;  }
        
        public VB6NodeTree nodeTree { get; }

        public abstract ContextNodeType GetNodeContextType();

        public VB6NodeTranslator(VB6NodeTree nodeTree, Dictionary<ContextNodeType, VB6NodeTranslator> translatorDict)
        {
            this.nodeTree = nodeTree;
            this.translatorDict = translatorDict;
        }

        public static string GetNodeTypeName(ParseTree node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            var typeName = node.GetType().Name;
            typeName =
                    typeName[0]
                        .ToString(System.Globalization.CultureInfo.InvariantCulture)
                        .ToUpper(System.Globalization.CultureInfo.InvariantCulture) + typeName.Substring(1);
            return typeName;
        }

        public static IEnumerable<string> GetTokens(ParseTree node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            for (int i = 0; i < node.getChildCount(); i++)
            {
                var child = node.getChild(i);
                var text = child.getText();
                if (text.Length > 0)
                {
                    if (child is TerminalNodeImpl)
                    {
                        yield return ((TerminalNodeImpl)child).getSymbol().getText();
                    }
                }
            }
        }

        public IEnumerable<string> UniversalTranslate(ParseTree node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            if (!Enum.TryParse(GetNodeTypeName(node), out ContextNodeType contextNodeType))
            {
                throw new ArgumentException("contextNodeType");
            }
            var translator = translatorDict[contextNodeType];
            foreach (var child in translator.Translate(nodeTree.GetChildren(node)))
            {
                yield return child;
            }
            foreach (var token in GetTokens(node))
            {
                yield return token;
            }
            //foreach (var child in nodeTree.GetChildren(node))
            //{
                
            //}
        }

        public IEnumerable<string> UniversalTranslate(List<ParseTree> parseTrees)
        {
            if (parseTrees == null)
            {
                throw new ArgumentNullException(nameof(parseTrees));
            }

            foreach (var node in parseTrees)
            {
                foreach (var child in UniversalTranslate(node))
                {
                    yield return child;
                }
            }
        }

        public virtual IEnumerable<string> PreTranslate(List<ParseTree> parseTrees)
        {
            return new List<string>();
        }

        public virtual IEnumerable<string> PostTranslate(List<ParseTree> parseTrees)
        {
            return new List<string>();
        }

        public virtual IEnumerable<string> Translate(List<ParseTree> parseTrees)
        {
            foreach (var child in PreTranslate(parseTrees))
            {
                yield return child;
            }
            foreach (var child in UniversalTranslate(parseTrees))
            {
                yield return child;
            }
            foreach (var child in PostTranslate(parseTrees))
            {
                yield return child;
            }
        }
    }
}
