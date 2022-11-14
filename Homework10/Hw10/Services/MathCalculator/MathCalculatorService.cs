using System.Linq.Expressions;
using Hw10.Dto;
using Hw10.Services.Parser;
using Hw10.Services.Expressions;
using Microsoft.AspNetCore.Routing.Template;

namespace Hw10.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
      public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression) 
      { 
          var members = new List<string>();
          var parser = new ParserForExpressions(expression!);
          var parseResult = parser.ParseExpression(members); 
          if (!parseResult.IsSuccess) 
              return parseResult;
          var expressionTree = ConverterToExpressionTree.Convert(members);
          var result = VisitorExpressionTree.Visit(expressionTree);
          if (result.IsSuccess)
              await Task.Delay(1000);
          return result; 
      }
}