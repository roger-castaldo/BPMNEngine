using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Workers = 25, Scope = ExecutionScope.MethodLevel)]