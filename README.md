# DynamicRun

Dynamically run code using .NET (Core 3.0, Core 3.1, 5 and 6) Roslyn and AssemblyLoadContext

You can read about it on my blog "[Dynamically compile and run code using .NET Core 3.0](https://laurentkempe.com/2019/02/18/dynamically-compile-and-run-code-using-dotNET-Core-3.0/)"

![.NET Core 3.0 preview 1, Roslyn and AssemblyLoadContext](https://raw.githubusercontent.com/laurentkempe/DynamicRun/master/doc/screenshot.png)

---

### Lab

New file class _MyMath.cs_ has been added. An interface has been introduces _IMath.cs_ and implemented.
Thanks to _CSharpCompilation_ the source code can be compiled and a new instance can be created via _SimpleUnloadableAssemblyLoadContext_.

### References

- [Dynamically compile and run code using .NET Core 3.0 - 2019 - net core 6 ](https://laurentkempe.com/2019/02/18/dynamically-compile-and-run-code-using-dotNET-Core-3.0/)
- [Surfer - Rick Srahl's web blog](https://weblog.west-wind.com/posts/2022/Jun/07/Runtime-CSharp-Code-Compilation-Revisited-for-Roslyn)
- [Stackoverflow #1](https://stackoverflow.com/questions/71474900/dynamic-compilation-in-net-core-6)
- [Stackoverflow #2](https://stackoverflow.com/questions/6041332/best-way-to-get-application-folder-path)
- [Stackoverflow #3](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly.createinstance?view=net-6.0)
