using System.Linq.Expressions;

namespace Hw10.Services.Expressions;

public static class ConverterToExpressionTree
{
    
     public static Expression Convert(List<string> members)
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
                     break;
                    
                 case "*":
                 case "/":
                 case "+": 
                 case "-": 
                     while (stackOperations.Count > 0 && (stackOperations.Peek() == "*" || stackOperations.Peek() == "/")) 
                         PopWhile(stackExprs, stackOperations);
                     stackOperations.Push(member); 
                     break;
                    
                 default: 
                     var constant = Expression.Constant(double.Parse(member)); 
                     stackExprs.Push(constant); 
                     break;
             }
         }

         while (stackOperations.Count > 0)
             PopWhile(stackExprs, stackOperations);

         return stackExprs.Pop();
    }

    private static void PopWhile(Stack<Expression> stackExpr, Stack<string> stackOperations)
    {
        stackOperations.TryPop(out var lastOperation);
        stackExpr.TryPop(out var lastExpr1);
        stackExpr.TryPop(out var lastExpr2);
        stackExpr.Push(ReturnExpression(lastExpr2!, lastExpr1!, lastOperation!));
    }
    
    private static Expression ReturnExpression(Expression left, Expression right, string operation) =>
        (operation) switch
        {
            "+" => Expression.Add(left, right),
            "-" => Expression.Subtract(left, right),
            "*" => Expression.Multiply(left, right),
            _ => Expression.Divide(left, right),
        };
}