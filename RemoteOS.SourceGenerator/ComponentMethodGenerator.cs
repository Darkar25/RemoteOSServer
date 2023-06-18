using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class ComponentMethodGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context)
	{
		context.RegisterForSyntaxNotifications(() => new ComponentSyntaxReceiver());
	}

	public void Execute(GeneratorExecutionContext context)
	{
		if (!(context.SyntaxReceiver is ComponentSyntaxReceiver receiver))
		{
			return;
		}

		var compilation = context.Compilation;

		var componentSymbol = compilation.GetTypeByMetadataName("RemoteOS.OpenComputers.Component");
		var componentIgnoreSymbol = compilation.GetTypeByMetadataName("RemoteOS.Helpers.ComponentIgnoreAttribute");

		if (componentSymbol == null || componentIgnoreSymbol == null)
		{
			return;
		}

		foreach (var classDeclaration in receiver.CandidateClasses)
		{
			var methodBuilder = new StringBuilder();

			var classSymbol = compilation.GetSemanticModel(classDeclaration.SyntaxTree).GetDeclaredSymbol(classDeclaration);

			if (classSymbol == null || !IsDerivedFrom(classSymbol, componentSymbol) || !IsPartial(classSymbol))
			{
				continue;
			}

			foreach (var methodDeclaration in classDeclaration.Members.OfType<MethodDeclarationSyntax>())
			{
				var methodSymbol = compilation.GetSemanticModel(methodDeclaration.SyntaxTree).GetDeclaredSymbol(methodDeclaration);

				if (methodSymbol == null ||
					methodSymbol.ReturnsVoid ||
					methodSymbol.IsImplicitlyDeclared ||
					methodSymbol.IsGenericMethod ||
					!methodSymbol.IsPartialDefinition)
				{
					continue;
				}

				if (HasAttribute(methodSymbol, componentIgnoreSymbol))
				{
					continue;
				}

				if (!(methodSymbol.ReturnType is INamedTypeSymbol returnTypeSymbol) || (!returnTypeSymbol.Name.Equals("Task") && !returnTypeSymbol.Name.StartsWith("Task<")))
				{
					continue;
				}

				var isAsync = returnTypeSymbol.TypeArguments.Any();
				var methodName = ConvertName(methodSymbol.Name);
				var invokeMethod = isAsync ? "InvokeFirst" : "Invoke";

				var parameters = "";
				var arguments = "";

				if (methodSymbol.Parameters.Any())
				{
					parameters = methodSymbol.Parameters.Select(p => (p.IsParams ? "params " : "") + p.Type + " " + p.Name).Aggregate((p1, p2) => $"{p1}, {p2}");
					arguments = ", " + methodSymbol.Parameters.Select(p => p.Name).Aggregate((p1, p2) => $"{p1}, {p2}");
				}

				methodBuilder.AppendLine($@"
		{methodDeclaration.Modifiers}{(isAsync ? " async" : "")} {returnTypeSymbol} {methodSymbol.Name}({parameters}) => {(isAsync ? "await " : "")}{invokeMethod}(""{methodName}""{arguments});");
			}

			var sourceCode = $@"namespace {classSymbol.ContainingNamespace.ToDisplayString()}
{{
	public partial class {classSymbol.Name}
	{{
{methodBuilder}
	}}
}}";

			context.AddSource($"{classSymbol.Name}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
		}
	}

	private bool IsDerivedFrom(INamedTypeSymbol? derivedType, INamedTypeSymbol baseType)
	{
		while (derivedType != null)
		{
			if (derivedType.Equals(baseType, SymbolEqualityComparer.Default))
			{
				return true;
			}
			
			derivedType = derivedType.BaseType;
		}

		return false;
	}

	private bool HasAttribute(ISymbol symbol, INamedTypeSymbol attributeType)
	{
		return symbol.GetAttributes().Any(a => a.AttributeClass?.Equals(attributeType, SymbolEqualityComparer.Default) == true);
	}

	private bool IsPartial(INamedTypeSymbol type)
	{
		return type.DeclaringSyntaxReferences.Select(x => x.GetSyntax()).OfType<TypeDeclarationSyntax>().Any(x => x.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword)));
	}

	private class ComponentSyntaxReceiver : ISyntaxReceiver
	{
		public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (syntaxNode is ClassDeclarationSyntax classDeclaration)
				CandidateClasses.Add(classDeclaration);
		}
	}

	private static string ConvertName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return name;
		}

		// Convert consecutive uppercase characters to lowercase
		var fixedName = new string(name.Select((x, y) => y > 0 && char.IsUpper(name[y - 1]) ? char.ToLowerInvariant(x) : x).ToArray());
		// Make first letter lowercase
		return char.ToLowerInvariant(fixedName[0]) + fixedName.Substring(1);
	}
}