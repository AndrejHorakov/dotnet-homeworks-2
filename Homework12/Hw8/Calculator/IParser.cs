using System.Diagnostics.CodeAnalysis;

namespace Hw8.Calculator;

public interface IParser
{
    [ExcludeFromCodeCoverage]
    static void ParseCalcArguments(string[] args,
        out double val1,
        out Operation operation,
        out double val2)
    {
        throw new NotImplementedException();
    }
}