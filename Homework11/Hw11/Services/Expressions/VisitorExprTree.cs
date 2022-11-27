using System.Linq.Expressions;
using Hw11.Dto;
using Hw11.ErrorMessages;
namespace Hw11.Services.Expressions;

public class VisitorExpressionTree : ExpressionVisitor
{
    private Dictionary<Expression, Lazy<Task<CalculationMathExpressionResultDto>>> _dictionary = new ();

    private void Visit(BinaryExpression binaryExpression)
    {
        _dictionary.Add(binaryExpression,
        new Lazy<Task<CalculationMathExpressionResultDto>>(async () =>
        {
            await Task.Delay(1000);
            await Task.WhenAll(_dictionary[binaryExpression.Left].Value, _dictionary[binaryExpression.Right].Value);
            return Calculate(binaryExpression, await _dictionary[binaryExpression.Left].Value,
                                                            await _dictionary[binaryExpression.Right].Value);
        }));
        Visit((dynamic)binaryExpression.Left);
        Visit((dynamic)binaryExpression.Right);
    }

    private void Visit(ConstantExpression node)
    {
        _dictionary.Add(node,
            new Lazy<Task<CalculationMathExpressionResultDto>>(async () => 
            Calculate(node,new CalculationMathExpressionResultDto((double)node.Value!))));
    }

    private static CalculationMathExpressionResultDto Calculate(Expression expr,
        params CalculationMathExpressionResultDto[] operands)
    {
        return expr.NodeType switch
        {
            ExpressionType.Add => new CalculationMathExpressionResultDto(operands[0].Result + operands[1].Result),
            ExpressionType.Subtract => new CalculationMathExpressionResultDto(operands[0].Result - operands[1].Result),
            ExpressionType.Multiply => new CalculationMathExpressionResultDto(operands[0].Result * operands[1].Result),
            ExpressionType.Divide =>
                operands[1].Result != 0.0
                    ? new CalculationMathExpressionResultDto(operands[0].Result / operands[1].Result)
                    : throw new DivideByZeroException(MathErrorMessager.DivisionByZero),
            _ => new CalculationMathExpressionResultDto((double)((ConstantExpression)expr).Value!)
        };
    }

    public Dictionary<Expression, Lazy<Task<CalculationMathExpressionResultDto>>> MyVisit(Expression expression)
    {
        this.Visit((dynamic)expression);
        return _dictionary;
    }
}