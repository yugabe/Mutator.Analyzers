using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mutator.Analyzers.Shorten
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MutatorAnalyzersShortenAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MutatorShortenAnalyzer";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Code quality";

        private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static readonly DiagnosticDescriptor ErrorRule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(WarningRule, ErrorRule);

        public override void Initialize(AnalysisContext context)
        {
            const int warningLimit = 250;
            const int errorLimit = 500;
            context.RegisterSyntaxTreeAction(c =>
            {
                var linesOfCode = c.Tree.GetText().Lines.Count;
                if (linesOfCode > warningLimit)
                    c.ReportDiagnostic(Diagnostic.Create(linesOfCode > errorLimit ? ErrorRule : WarningRule, c.Tree.GetRoot().GetLocation(), linesOfCode, warningLimit, errorLimit));
            });
        }
    }
}
