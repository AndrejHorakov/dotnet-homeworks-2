using System.Globalization;
using Hw9.Dto;
using Hw9.ErrorMessages;
namespace Hw9.Parser;

public static class ParserForExpressions
{
    public static CalculationMathExpressionResultDto ParseExpression(string? expression, List<string> list)
    {
        if (expression is null or "")
            return new CalculationMathExpressionResultDto(MathErrorMessager.EmptyString);
        
        var checkNotEasyErrors = ExpressionContainsCorrectMembers(expression);
        if (checkNotEasyErrors.IsSuccess)
        {
            var result = ParseExprIntoCorrectStrings(expression, list);
            return result;
        }
        else
            return checkNotEasyErrors;
    }
    private static CalculationMathExpressionResultDto ExpressionContainsCorrectMembers(string expression)
    {
        var bracketCounter = 0;
        var satisfyingSet = new HashSet<char>() { '1', '2', '3', '4', '5',
            '6', '7', '8', '9', '0', '.', ',', '+', '-', '/', '*', '(', ')', ' ' };
        if (IsOperation(expression[0]))
            return new CalculationMathExpressionResultDto(MathErrorMessager.StartingWithOperation);
        var lenghtExpression = expression.Length;
        for (int i = 0; i < lenghtExpression; i++)
        {
            if (bracketCounter < 0)
                return new CalculationMathExpressionResultDto(MathErrorMessager.IncorrectBracketsNumber);
            
            if (!satisfyingSet.Contains(expression[i]))
                return new CalculationMathExpressionResultDto(MathErrorMessager
                    .UnknownCharacterMessage(expression[i]));
            
            if (expression[i] == '(')
            {
                if (i + 1 < lenghtExpression && expression[i+1] != '-' && IsOperation(expression[i+1]))
                    return new CalculationMathExpressionResultDto(MathErrorMessager.
                        InvalidOperatorAfterParenthesisMessage(expression[i+1].ToString()));
                bracketCounter++;
            }

            if (expression[i] == ')')
            {
                if (i - 1 > -1 && IsOperation(expression[i-1]))
                    return new CalculationMathExpressionResultDto(MathErrorMessager.
                        OperationBeforeParenthesisMessage(expression[i-1].ToString()));
                bracketCounter--;
            }

            if (i + 2 < lenghtExpression && IsOperation(expression[i]) && IsOperation(expression[i + 2]))
                return new CalculationMathExpressionResultDto(MathErrorMessager.
                    TwoOperationInRowMessage(expression[i].ToString(), expression[i+2].ToString()));

        }

        if (IsOperation(expression[lenghtExpression-1]))
            return new CalculationMathExpressionResultDto(MathErrorMessager.EndingWithOperation);
        
        return bracketCounter != 0 ? 
            new CalculationMathExpressionResultDto(MathErrorMessager.IncorrectBracketsNumber) : 
            new CalculationMathExpressionResultDto(1);
    }

    private static bool IsOperation(char sign) =>
        sign switch
        {
            '-' or '+' or '*' or '/' => true,
            _ => false
        };

    private static CalculationMathExpressionResultDto ParseExprIntoCorrectStrings(string expression, List<String> list)
    {
        var splExpr = expression.Split(' ');
        foreach (var str in splExpr)
        {
            if (str[0] == '(')
            {
                if (str[^1] == ')')
                {
                    if (!ParseBracket(str[..^1], list, true)
                            .IsSuccess)
                        return ParseBracket(str[..^1], list, true);
                    list.Add(")");
                    continue;
                }
                else if (!ParseBracket(str, list, true).IsSuccess)
                    return ParseBracket(str, list, true);
                else continue;
            }

            if (str[^1] == ')')
                if (!ParseBracket(str, list, false).IsSuccess)
                {
                    return ParseBracket(str, list, false);
                }
                else continue;

            if (!char.IsDigit(str[0]) && str[0] != '(')
            {
                list.Add(str);
                continue;
            }
            
            if (!double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var p))
                return new CalculationMathExpressionResultDto(MathErrorMessager.NotNumberMessage(str));
            list.Add(str);
        }

        return new CalculationMathExpressionResultDto(1);
    }

    private static CalculationMathExpressionResultDto ParseBracket(string str, List<string> list, bool open)
    {
        var index = open ? 1 : str.Length - 2;
        
        while (!char.IsDigit(str[index]) && str[index] != '-')
            index = open ? index++ : index--;
        
        var startIndex = open ? index : 0;
        var length = open ? str.Length - index : str.Length - index - 1;
        var maybeNumber = str.Substring(startIndex, length);
        if (!double.TryParse(maybeNumber, NumberStyles.Any, CultureInfo.InvariantCulture, out var p ))
            return new CalculationMathExpressionResultDto(MathErrorMessager.NotNumberMessage(maybeNumber));
        
        if (open)
        {
            for (var i = 0; i < index; i++)
                list.Add("(");
            list.Add(maybeNumber);
        }
        else
        {
            list.Add(maybeNumber);
            for (var i = 0; i < str.Length - index - 1; i++)
                list.Add(")");
        }

        return new CalculationMathExpressionResultDto(1);
    }
}