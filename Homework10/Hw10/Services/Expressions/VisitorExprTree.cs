using System.Linq.Expressions;
using Hw10.Dto;
using Hw10.ErrorMessages;
namespace Hw10.Services.Expressions;

public class VisitorExpressionTree : ExpressionVisitor
{
    private Dictionary<Expression, Lazy<Task<CalculationMathExpressionResultDto>>> _dictionary = new ();

    protected override Expression VisitBinary(BinaryExpression binaryExpression)
    {
        _dictionary.Add(binaryExpression,
        new Lazy<Task<CalculationMathExpressionResultDto>>(async () =>
        {
            await Task.Delay(1000);
            await Task.WhenAll(_dictionary[binaryExpression.Left].Value, _dictionary[binaryExpression.Right].Value);
            return Calculate(binaryExpression, await _dictionary[binaryExpression.Left].Value,
                                                            await _dictionary[binaryExpression.Right].Value);
        }));
        return base.VisitBinary(binaryExpression);
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        _dictionary.Add(node,
            new Lazy<Task<CalculationMathExpressionResultDto>>(async () => 
            Calculate(node,new CalculationMathExpressionResultDto((double)node.Value!))));
            return base.VisitConstant(node);
    }

    private static CalculationMathExpressionResultDto Calculate(Expression expr,
        params CalculationMathExpressionResultDto[] operands)
    {
        return expr.NodeType switch
        {
            ExpressionType.Add => operands[0].IsSuccess && operands[1].IsSuccess
                ? new CalculationMathExpressionResultDto(operands[0].Result + operands[1].Result)
                : ReturnErrorOperand(operands[0], operands[1]),
            ExpressionType.Subtract => operands[0].IsSuccess && operands[1].IsSuccess
                ? new CalculationMathExpressionResultDto(operands[0].Result - operands[1].Result)
                : ReturnErrorOperand(operands[0], operands[1]),
            ExpressionType.Multiply => operands[0].IsSuccess && operands[1].IsSuccess
                ? new CalculationMathExpressionResultDto(operands[0].Result * operands[1].Result)
                : ReturnErrorOperand(operands[0], operands[1]),
            ExpressionType.Divide =>
                operands[1].Result != 0.0
                    ? new CalculationMathExpressionResultDto(operands[0].Result / operands[1].Result)
                    : new CalculationMathExpressionResultDto(MathErrorMessager.DivisionByZero),
            _ => new CalculationMathExpressionResultDto((double)((ConstantExpression)expr).Value!)
        };
    }

    private static CalculationMathExpressionResultDto ReturnErrorOperand(CalculationMathExpressionResultDto operand1,
        CalculationMathExpressionResultDto operand2) =>
        operand1.IsSuccess ? operand2 : operand1;

    public Dictionary<Expression, Lazy<Task<CalculationMathExpressionResultDto>>> MyVisit(Expression expression)
    {
        base.Visit(expression);
        return _dictionary;
    }
}