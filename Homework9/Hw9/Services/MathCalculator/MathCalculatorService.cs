using System.Linq.Expressions;
using Hw9.Dto;
using Hw9.ErrorMessages;
using Hw9.Parser;
namespace Hw9.Services.MathCalculator;

public class MathCalculatorService : IMathCalculatorService
{
    public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
    {
        var members = new List<string>();
        var parseResult = ParserForExpressions.ParseExpression(expression, members);
        if (!parseResult.IsSuccess)
            return parseResult;
        var preRes = await MatchWithTasks(members);
        var visitor = new VisitorExprTree();
        Task<CalculationMathExpressionResultDto> result;
        if (preRes is BinaryExpression)
        {
            result = new Task<CalculationMathExpressionResultDto>(() =>
                visitor.Visit(preRes));
        }
        else
        {
            result = new Task<CalculationMathExpressionResultDto>(() => 
                new CalculationMathExpressionResultDto(visitor.Calculate(preRes as ConstantExpression).Result));
        }
        result.Start();
        //await Task.Delay(1000);
        return await result;
    }

    private async Task<Expression> MatchWithTasks(List<string> members)
    {
        var stackExprs = new Stack<Expression>();
        var stackOperations = new Stack<string>();
        foreach (var member in members)
        {
            switch (member)
            {
                case "(":
                    stackOperations.Push(member);
                    break;
                
                case ")":
                    while (stackOperations.Peek() != "(")
                        PopWhile(stackExprs, stackOperations);
                    stackOperations.Pop();
                    await Task.Delay(400);
                    break;
                
                case "+":
                case "-":
                    string lastOperation;
                    while (stackOperations.Count > 0 && (stackOperations.Peek() == "*" || stackOperations.Peek() == "/"))
                        PopWhile(stackExprs, stackOperations);
                    stackOperations.Push(member);
                    await Task.Delay(1000);
                    break;
                
                case "*":
                case "/":
                    stackOperations.Push(member);
                    await Task.Delay(200);
                    break;
                
                default:
                    var constant = Expression.Constant(double.Parse(member));
                    stackExprs.Push(constant);
                    break;
            }
        }
        
        string operation;
        while (stackOperations.TryPop(out operation))
        {
            Expression lastExpr1;
            Expression lastExpr2;
            stackExprs.TryPop(out lastExpr1);
            stackExprs.TryPop(out lastExpr2);
            stackExprs.Push(ReturnExpression(lastExpr2, lastExpr1, operation));
        }

        var result = new Task<Expression>(() => stackExprs.Pop());
        result.Start();
        return await result;
    }

    private void PopWhile(Stack<Expression> stackExpr, Stack<string> stackOperations)
    {
        string lastOperation;
        stackOperations.TryPop(out lastOperation);
        Expression lastExpr1;
        Expression lastExpr2;
        stackExpr.TryPop(out lastExpr1);
        stackExpr.TryPop(out lastExpr2);
        stackExpr.Push(ReturnExpression(lastExpr2, lastExpr1, lastOperation));
    }
    
    private Expression ReturnExpression(Expression expr1, Expression expr2, string operation) =>
        (operation) switch
        {
            "+" => Expression.Add(expr1, expr2),
            "-" => Expression.Subtract(expr1, expr2),
            "*" => Expression.Multiply(expr1, expr2),
            _ => Expression.Divide(expr1, expr2),
        };
}