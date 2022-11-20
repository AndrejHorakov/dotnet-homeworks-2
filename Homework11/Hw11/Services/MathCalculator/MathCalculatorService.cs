using Hw11.Services.Expressions;
using Hw11.Services.Parser;
namespace Hw11.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
    public async Task<double> CalculateMathExpressionAsync(string? expression) 
    {
        var members = new List<string>();
        var parser = new ParserForExpressions(expression!);
        parser.ParseExpression(members); 
        var expressionTree = ConverterToExpressionTree.Convert(members);
        var result = await new VisitorExpressionTree().MyVisit(expressionTree)[expressionTree].Value;
        return result.Result;
    }
}