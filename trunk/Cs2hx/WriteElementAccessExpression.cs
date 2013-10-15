﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Compilers.CSharp;

namespace Cs2hx
{
	static class WriteElementAccessExpression
	{
		public static void Go(HaxeWriter writer, ElementAccessExpressionSyntax expression)
		{
			Core.Write(writer, expression.Expression);

			var typeHaxe = TypeProcessor.ConvertType(Program.GetModel(expression).GetTypeInfo(expression.Expression).ConvertedType);
			if (typeHaxe.StartsWith("Array<")) //arrays are the only thing haxe allows using the [] syntax with
			{
				if (expression.ArgumentList.Arguments.Count != 1)
					throw new Exception("Expect array index to have a single argument " + Utility.Descriptor(expression));

				writer.Write("[");
				Core.Write(writer, expression.ArgumentList.Arguments.Single().Expression);
				writer.Write("]");
			}
			else if (typeHaxe == "String")
			{
				//indexing into string to get its character results in a call to charCodeAt
				writer.Write(".charCodeAt(");
				Core.Write(writer, expression.ArgumentList.Arguments.Single().Expression);
				writer.Write(")");
			}
			else
			{
				writer.Write(".GetValue(");
				bool first = true;
				foreach (var arg in expression.ArgumentList.Arguments)
				{
					if (first)
						first = false;
					else
						writer.Write(", ");

					Core.Write(writer, arg.Expression);
				}
				writer.Write(")");
			}
		}
	}
}
