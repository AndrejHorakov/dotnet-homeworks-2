module Hw4.Parser

open System
open Hw4.Calculator


type CalcOptions = {
    arg1: float
    arg2: float
    operation: CalculatorOperation
}

let isArgLengthSupported (args : string[]) =
    args.Length = 3

let parseOperation (arg : string) =
    match arg with
    |"+" -> CalculatorOperation.Plus
    |"-" -> CalculatorOperation.Minus
    |"*" -> CalculatorOperation.Multiply
    |"/" -> CalculatorOperation.Divide
    |_ -> ArgumentException() |> raise
    
let parseCalcArguments(args : string[]) =
    let val1Bool, value1 = System.Double.TryParse args[0]
    let val2Bool, value2 = System.Double.TryParse args[2]
    let currOperation = parseOperation args[1]
    if not (val1Bool && val2Bool && isArgLengthSupported args)
    then ArgumentException() |> raise
    {
        arg1 = value1
        arg2 = value2
        operation = currOperation
    }