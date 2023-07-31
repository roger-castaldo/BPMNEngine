using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Workers = 50, Scope = ExecutionScope.MethodLevel)]