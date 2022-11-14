using System.Linq.Expressions;
using Hw10.Dto;
using Hw10.ErrorMessages;
namespace Hw10.Services.Expressions;

public class VisitorExpressionTree : ExpressionVisitor
{
    public new static CalculationMathExpressionResultDto Visit(Expression node) =>
        node.NodeType switch
        {
            ExpressionType.Add or
                ExpressionType.Subtract or
                ExpressionType.Multiply or
                ExpressionType.Divide => Calculate(node,
                    Visit((node as BinaryExpression)!.Left),
                    Visit((node as BinaryExpression)!.Right)),
            _ => Calculate(node)
        };


    private static CalculationMathExpressionResultDto Calculate(Expression expr, params CalculationMathExpressionResultDto[] operands) =>
        expr.NodeType switch
        {
            ExpressionType.Add => operands[0].IsSuccess  && operands[1].IsSuccess ? 
                new CalculationMathExpressionResultDto(operands[0].Result + operands[1].Result) :
                ReturnErrorOperand(operands[0], operands[1]),
            ExpressionType.Subtract => operands[0].IsSuccess  && operands[1].IsSuccess ? 
                new CalculationMathExpressionResultDto(operands[0].Result - operands[1].Result) :
                ReturnErrorOperand(operands[0], operands[1]),
            ExpressionType.Multiply => operands[0].IsSuccess  && operands[1].IsSuccess ? 
                new CalculationMathExpressionResultDto(operands[0].Result * operands[1].Result) :
                ReturnErrorOperand(operands[0], operands[1]),
            ExpressionType.Divide =>
                operands[1].Result != 0.0
                    ? new CalculationMathExpressionResultDto(operands[0].Result / operands[1].Result)
                    : new CalculationMathExpressionResultDto(MathErrorMessager.DivisionByZero),
            _ => new CalculationMathExpressionResultDto((double)(((((ConstantExpression)expr).Value)))!)
        };

    private static CalculationMathExpressionResultDto ReturnErrorOperand(CalculationMathExpressionResultDto operand1,
        CalculationMathExpressionResultDto operand2) =>
        operand1.IsSuccess ? operand2 : operand1;
}