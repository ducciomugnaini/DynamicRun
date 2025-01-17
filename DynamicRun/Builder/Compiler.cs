using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace DynamicRun.Builder
{
   internal class Compiler
   {
      public byte[] Compile(string filepath)
      {
         Console.WriteLine($"Starting compilation of: '{filepath}'");

         var sourceCode = File.ReadAllText(filepath);

         using (var memoryStream = new MemoryStream())
         {
            var result = GenerateCode(sourceCode).Emit(memoryStream);

            if (!result.Success)
            {
               Console.WriteLine("Compilation done with error.");

               var failures = result.Diagnostics.Where(diagnostic =>
                  diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

               foreach (var diagnostic in failures)
               {
                  Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
               }

               return null;
            }

            Console.WriteLine("Compilation done without any error.");

            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream.ToArray();
         }
      }

      private static CSharpCompilation GenerateCode(string sourceCode)
      {
         var codeString = SourceText.From(sourceCode);
         var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);
         var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

         var references = new List<MetadataReference>
         {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
         };
         Assembly.GetEntryAssembly()?.GetReferencedAssemblies().ToList().ForEach(assemblyName =>
         {
            references.Add(MetadataReference.CreateFromFile(Assembly.Load(assemblyName).Location));
         });

         var compiledCode = CSharpCompilation.Create(
            assemblyName: "Hello.dll",
            syntaxTrees: new[] { parsedSyntaxTree },
            references: references,
            options: new CSharpCompilationOptions(
               OutputKind.ConsoleApplication,
               optimizationLevel: OptimizationLevel.Release,
               assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default)
         );

         return compiledCode;
      }
   }
}