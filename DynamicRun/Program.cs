using DynamicRun.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using DynamicRun.Sources.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace DynamicRun
{
   static class Program
   {
      static void Main()
      {
         var sourcesPath = Path.Combine(Environment.CurrentDirectory, "Sources");

         Console.WriteLine($"Running from: {Environment.CurrentDirectory}");
         Console.WriteLine($"Sources from: {sourcesPath}");
         Console.WriteLine("Modify the sources to compile and run it!");

         var compiler = new Compiler();
         var runner = new Runner();

         using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = @".\Sources"; }))
         {
            var changes = watcher.Changed.Throttle(TimeSpan.FromSeconds(.5))
               .Where(c => c.FullPath.EndsWith(@"DynamicProgram.cs")).Select(c => c.FullPath);

            changes.Subscribe(filepath => runner.Execute(compiler.Compile(filepath), new[] { "France" }));

            watcher.Start();

            Console.WriteLine("Press any key to exit!");
            Console.ReadLine();
         }

         // ------------------------------------------------------------------------------------------------------------
         // --------------- Rude coding just for experiment dynamic compilation with interfaces ------------------------
         // ------------------------------------------------------------------------------------------------------------

         var sourceCode = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Sources\\MyMath.cs");

         var codeString = SourceText.From(sourceCode);
         var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);
         var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

         var references = new List<MetadataReference>
         {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
         };

         references.Add(MetadataReference.CreateFromFile(AppDomain.CurrentDomain.BaseDirectory +
                                                         Assembly.GetEntryAssembly().GetName().Name + ".dll"));

         Assembly.GetEntryAssembly()?.GetReferencedAssemblies().ToList().ForEach(assemblyName =>
         {
            references.Add(MetadataReference.CreateFromFile(Assembly.Load(assemblyName).Location));
         });

         var compiledCode = CSharpCompilation.Create(
            assemblyName: "MyMath.dll",
            syntaxTrees: new[] { parsedSyntaxTree },
            references: references,
            options: new CSharpCompilationOptions(
               OutputKind.DynamicallyLinkedLibrary,
               optimizationLevel: OptimizationLevel.Debug,
               assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default)
         );

         byte[] compilationResult;

         using (var memoryStream = new MemoryStream())
         {
            var result = compiledCode.Emit(memoryStream);

            if (!result.Success)
            {
               Console.WriteLine("Compilation done with error.");

               var failures = result.Diagnostics.Where(diagnostic =>
                  diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

               foreach (var diagnostic in failures)
               {
                  Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
               }
            }

            Console.WriteLine("Compilation done without any error.");
            memoryStream.Seek(0, SeekOrigin.Begin);
            compilationResult = memoryStream.ToArray();
         }

         using (var memoryStream = new MemoryStream(compilationResult))
         {
            var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();
            var assembly = assemblyLoadContext.LoadFromStream(memoryStream);

            var instance = (IMath)assembly.CreateInstance("DynamicRun.Sources.MyMath");
            
            // instance is now usable in main code by his interface :D

            if(System.Diagnostics.Debugger.IsAttached)
               System.Diagnostics.Debugger.Break();
            
            assemblyLoadContext.Unload();

            // var w = new WeakReference(assemblyLoadContext);
         }
      }
   }
}