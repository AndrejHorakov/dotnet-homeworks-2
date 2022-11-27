using System.Globalization;
using Hw11.Dto;
using Hw11.ErrorMessages;
using Hw11.Exceptions;

namespace Hw11.Services.Parser;

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
            throw new ArgumentException(MathErrorMessager.EmptyString);
        ExpressionContainsCorrectMembers();
        return ParseExprIntoCorrectStrings(list);
    }
    
    private CalculationMathExpressionResultDto ExpressionContainsCorrectMembers()
    {
        var bracketCounter = 0;
        
        var satisfyingSet = new HashSet<char>() { '1', '2', '3', '4', '5',
            '6', '7', '8', '9', '0', '.', ',', '+', '-', '/', '*', '(', ')', ' ' };
        if (IsOperation(_expression[0]))
            throw new InvalidSyntaxException(MathErrorMessager.StartingWithOperation);
        var lenghtExpression = _expression.Length;
        for (var i = 0; i < lenghtExpression; i++)
        {
            if (bracketCounter < 0)
                throw new InvalidSyntaxException(MathErrorMessager.IncorrectBracketsNumber);
            
            if (!satisfyingSet.Contains(_expression[i]))
                throw new InvalidSymbolException(MathErrorMessager
                    .UnknownCharacterMessage(_expression[i]));
            
            if (_expression[i] == '(')
            {
                if (i + 1 < lenghtExpression && _expression[i+1] != '-' && IsOperation(_expression[i+1]))
                    throw new InvalidSyntaxException(MathErrorMessager.
                        InvalidOperatorAfterParenthesisMessage(_expression[i+1].ToString()));
                bracketCounter++;
                continue;
            }

            if (_expression[i] == ')')
            {
                if (i - 1 > -1 && IsOperation(_expression[i-1]))
                    throw new InvalidSyntaxException(MathErrorMessager.
                        OperationBeforeParenthesisMessage(_expression[i-1].ToString()));
                bracketCounter--;
                continue;
            }

            if (i + 2 < lenghtExpression && IsOperation(_expression[i]) && IsOperation(_expression[i + 2]))
                throw new InvalidSyntaxException(MathErrorMessager.
                    TwoOperationInRowMessage(_expression[i].ToString(), _expression[i+2].ToString()));
        }

        if (IsOperation(_expression[^1]))
            throw new InvalidSyntaxException(MathErrorMessager.EndingWithOperation);
        
        return bracketCounter != 0 ? 
            throw new InvalidSyntaxException(MathErrorMessager.IncorrectBracketsNumber) : 
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
                ParseBracket(str, list, true);
                continue;
            }

            if (str[^1] == ')')
            {
                ParseBracket(str, list, false); 
                continue;
            }

            if (!char.IsDigit(str[0]) && str[0] != '(')
            {
                list.Add(str);
                continue;
            }
            
            if (!double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                throw new InvalidNumberException(MathErrorMessager.NotNumberMessage(str));
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
            throw new InvalidNumberException(MathErrorMessager.NotNumberMessage(maybeNumber));
        
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
