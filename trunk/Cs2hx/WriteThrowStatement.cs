﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cs2hx
{
    static class WriteThrowStatement
    {
        public static void Go(HaxeWriter writer, ThrowStatementSyntax statement)
        {
            writer.WriteIndent();

            if (!ReturnsVoid(statement))
                writer.Write("return "); //"return" the throw statement. This works around the "return missing" haxe limitation

            writer.Write("throw ");

            if (statement.Expression == null)
            {
                //On just "throw" with no exception name, navigate up the stack to find the nearest catch block and insert the exception's name
                CatchClauseSyntax catchBlock;
                SyntaxNode node = statement;
                do
                    catchBlock = (node = node.Parent) as CatchClauseSyntax;
                while (catchBlock == null);

                if (catchBlock == null)
                    throw new Exception("throw statement with no exception name, and could not locate a catch block " + Utility.Descriptor(statement));

                var exName = catchBlock.Declaration.Identifier.ValueText;

                if (string.IsNullOrWhiteSpace(exName))
                    exName = "__ex";

                writer.Write(exName);
            }
            else
                Core.Write(writer, statement.Expression);
            writer.Write(";\r\n");
        }

        static bool ReturnsVoid(SyntaxNode node)
        {
            while (node != null)
            {
                var method = node as MethodDeclarationSyntax;
                if (method != null)
                    return method.ReturnType.ToString() == "void";

                var prop = node as PropertyDeclarationSyntax;
                if (prop != null)
                    return prop.Type.ToString() == "void";

                if (node is ConstructorDeclarationSyntax)
                    return true;

                var lambda1 = node as ParenthesizedLambdaExpressionSyntax;
                var lambda2 = node as SimpleLambdaExpressionSyntax;
                if (lambda1 != null || lambda2 != null)
                {
                    var lambda = lambda1 == null ? (ExpressionSyntax)lambda2 : (ExpressionSyntax)lambda1;
                    var methodSymbol = Program.GetModel(lambda).GetTypeInfo(lambda).ConvertedType.As<INamedTypeSymbol>().DelegateInvokeMethod.As<IMethodSymbol>();

                    return methodSymbol.ReturnsVoid;
                }

                node = node.Parent;
            }

            throw new Exception("Node not in a body");
        }

        public static void GoExpression(HaxeWriter writer, ThrowExpressionSyntax expr)
        {
            writer.Write("throw ");
            Core.Write(writer, expr.Expression);
        }
    }
}
