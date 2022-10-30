using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Hw8.Calculator;
using Microsoft.AspNetCore.Mvc;

namespace Hw8.Controllers;

public class CalculatorController : Controller
{
    public ActionResult<double> Calculate([FromServices] ICalculator calculator,
        string val1,
        string operation,
        string val2)
    {
        double arg1;
        Operation oper;
        double arg2;
        var occasion = Parser.ParseCalcArguments(new []{val1, operation, val2},
            out arg1, out oper, out arg2);
        if (occasion == Event.Success)
            return oper switch
            {
                Operation.Plus => Ok(calculator.Plus(arg1, arg2)),
                Operation.Minus => Ok(calculator.Minus(arg1, arg2)),
                Operation.Multiply => Ok(calculator.Multiply(arg1, arg2)),
                Operation.Divide => Ok(calculator.Divide(arg1, arg2))
            };
        return occasion switch
        {
            Event.BadArgument => BadRequest(Messages.InvalidNumberMessage),
            Event.BadOperation => BadRequest(Messages.InvalidOperationMessage),
            Event.DividingByZero => BadRequest(Messages.DivisionByZeroMessage)
        };
    }
    
    [ExcludeFromCodeCoverage]
    public IActionResult Index()
    {
        return Content(
            "Заполните val1, operation(plus, minus, multiply, divide) и val2 здесь '/calculator/calculate?val1= &operation= &val2= '\n" +
            "и добавьте её в адресную строку.");
    }
}