using System.Linq.Expressions;
using Hw9.Dto;
using Hw9.ErrorMessages;
namespace Hw9.Services.MathCalculator;

public class VisitorExprTree : ExpressionVisitor
{
    public CalculationMathExpressionResultDto Visit(Expression node) =>
        node.NodeType switch
        {
            var x when x is ExpressionType.Add or
                    ExpressionType.Subtract or
                    ExpressionType.Multiply or
                    ExpressionType.Divide
                => Calculate(node,
                    Visit((node as BinaryExpression).Left).Result,
                    Visit((node as BinaryExpression).Right).Result),
                ExpressionType.Constant => Calculate(node)
        };

    public CalculationMathExpressionResultDto Calculate(Expression expr, params double[] operands) =>
        expr.NodeType switch
        {
            ExpressionType.Add => new CalculationMathExpressionResultDto(operands[0] + operands[1]),
            ExpressionType.Subtract => new CalculationMathExpressionResultDto(operands[0] - operands[1]),
            ExpressionType.Multiply => new CalculationMathExpressionResultDto(operands[0] * operands[1]),
            ExpressionType.Divide =>
                operands[1] != 0.0
                    ? new CalculationMathExpressionResultDto(operands[0] / operands[1])
                    : new CalculationMathExpressionResultDto(MathErrorMessager.DivisionByZero),
            _ => new CalculationMathExpressionResultDto((double)((((ConstantExpression)expr).Value)))
        };
}