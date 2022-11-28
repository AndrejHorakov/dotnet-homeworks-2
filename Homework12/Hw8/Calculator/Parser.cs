using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace Hw8.Calculator;

public class Parser : IParser
{
    public static Event ParseCalcArguments(string[] args, 
        out double val1, 
        out Operation operation, 
        out double val2)
    {
        val1 = double.NaN;
        operation = Operation.Invalid;
        val2 = double.NaN;
        if (!Double.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out val1))
            return Event.BadArgument;
        if (!Double.TryParse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture, out val2))
            return Event.BadArgument;
        if (!ParseOperation(args[1], out operation))
            return Event.BadOperation;
        if (val2 == 0)
            return Event.DividingByZero;
        return Event.Success;
    }

    private static bool ParseOperation(string arg, out Operation operation)
    {
        switch (arg)
        {
            case "+" :
            case "Plus":
                operation = Operation.Plus;
                return true;
            case "-" :
            case "Minus":
                operation = Operation.Minus;
                return true;
            case "*" :
            case "Multiply":
                operation = Operation.Multiply;
                return true;
            case "/" :
            case "Divide":
                operation = Operation.Divide;
                return true;
            default:
                operation = Operation.Invalid;
                return false;
        }
    }
}
