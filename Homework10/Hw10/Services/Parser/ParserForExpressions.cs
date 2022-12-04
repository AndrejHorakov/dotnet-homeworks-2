using System.Globalization;
using Hw10.Dto;
using Hw10.ErrorMessages;

namespace Hw10.Services.Parser;

public class ParserForExpressions
{
    private readonly string _expression;

    public ParserForExpressions(string expression)
    {
        _expression = expression;
    }
    
    public CalculationMathExpressionResultDto ParseExpression(List<string> list)
    {
        if (_expression is null or "")
            return new CalculationMathExpressionResultDto(MathErrorMessager.EmptyString);
        
        var checkEasyErrors = ExpressionContainsCorrectMembers();
        return checkEasyErrors.IsSuccess ? ParseExprIntoCorrectStrings(list) : checkEasyErrors;
    }
    
    private CalculationMathExpressionResultDto ExpressionContainsCorrectMembers()
    {
        var bracketCounter = 0;
        
        var satisfyingSet = new HashSet<char>() { '1', '2', '3', '4', '5',
            '6', '7', '8', '9', '0', '.', ',', '+', '-', '/', '*', '(', ')', ' ' };
        if (IsOperation(_expression[0]))
            return new CalculationMathExpressionResultDto(MathErrorMessager.StartingWithOperation);
        var lenghtExpression = _expression.Length;
        for (var i = 0; i < lenghtExpression; i++)
        {
            if (bracketCounter < 0)
                return new CalculationMathExpressionResultDto(MathErrorMessager.IncorrectBracketsNumber);
            
            if (!satisfyingSet.Contains(_expression[i]))
                return new CalculationMathExpressionResultDto(MathErrorMessager
                    .UnknownCharacterMessage(_expression[i]));
            
            if (_expression[i] == '(')
            {
                if (i + 1 < lenghtExpression && _expression[i+1] != '-' && IsOperation(_expression[i+1]))
                    return new CalculationMathExpressionResultDto(MathErrorMessager.
                        InvalidOperatorAfterParenthesisMessage(_expression[i+1].ToString()));
                bracketCounter++;
            }

            if (_expression[i] == ')')
            {
                if (i - 1 > -1 && IsOperation(_expression[i-1]))
                    return new CalculationMathExpressionResultDto(MathErrorMessager.
                        OperationBeforeParenthesisMessage(_expression[i-1].ToString()));
                bracketCounter--;
            }

            if (i + 2 < lenghtExpression && IsOperation(_expression[i]) && IsOperation(_expression[i + 2]))
                return new CalculationMathExpressionResultDto(MathErrorMessager.
                    TwoOperationInRowMessage(_expression[i].ToString(), _expression[i+2].ToString()));

        }

        if (IsOperation(_expression[^1]))
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

    private CalculationMathExpressionResultDto ParseExprIntoCorrectStrings(ICollection<string> list)
    {
        var splExpr = _expression.Split(' ');
        foreach (var str in splExpr)
        {
            if (str[0] == '(')
            {
                if (str[^1] == ')')
                {
                    if (!ParseBracket(str, list, true)
                            .IsSuccess)
                        return ParseBracket(str, list, true);
                    continue;
                }

                if (!ParseBracket(str, list, true).IsSuccess)
                    return ParseBracket(str, list, true);
                continue;
            }

            if (str[^1] == ')')
            {
                if (!ParseBracket(str, list, false).IsSuccess)
                    return ParseBracket(str, list, false); 
                continue;
            }

            if (!char.IsDigit(str[0]) && str[0] != '(')
            {
                list.Add(str);
                continue;
            }
            
            if (!double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                return new CalculationMathExpressionResultDto(MathErrorMessager.NotNumberMessage(str));
            list.Add(str);
        }

        return new CalculationMathExpressionResultDto(1);
    }

    private static CalculationMathExpressionResultDto ParseBracket(string str, ICollection<string> list, bool open)
    {
        var index = open ? 1 : str.Length - 2;
        
        while (!char.IsDigit(str[index]) && str[index] != '-')
            index = open ? index + 1 : index - 1;
        
        var startIndex = open ? index : 0;
        var length = open ? str.Length - index : index + 1;
        
        if (open)
            while (!char.IsDigit(str[startIndex + length - 1]))
                length--;
        
        var maybeNumber = str.Substring(startIndex, length);
        if (!double.TryParse(maybeNumber, NumberStyles.Any, CultureInfo.InvariantCulture, out _ ))
            return new CalculationMathExpressionResultDto(MathErrorMessager.NotNumberMessage(maybeNumber));
        
        if (open)
        {
            for (var i = 0; i < index; i++)
                list.Add("(");
            list.Add(maybeNumber);
            for (var i = 0; i < str.Length - index - length; i++)
                list.Add(")");
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
