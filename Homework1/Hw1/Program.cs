using Hw1;

double arg1;
CalculatorOperation operation;
double arg2;

Parser.ParseCalcArguments(args, out arg1, out operation, out arg2);
Console.WriteLine(Calculator.Calculate(arg1, operation, arg2));