module Hw5.Parser

open System
open System.Globalization
open Hw5.Calculator

let isArgLengthSupported (args:string[]): Result<'a,'b> =
    match args.Length = 3 with
    | true -> Ok args
    | false -> Error Message.WrongArgLength
    
[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let inline isOperationSupported (arg1, operation, arg2): Result<('a * CalculatorOperation * 'b), Message> =
    match operation with
    | "+"  -> Ok (arg1, CalculatorOperation.Plus, arg2)
    |"Plus" -> Ok (arg1, CalculatorOperation.Plus, arg2)
    | "-" |"Minus" -> Ok (arg1, CalculatorOperation.Minus, arg2)
    | "*" |"Multiply" -> Ok (arg1, CalculatorOperation.Multiply, arg2)
    | "/" |"Divide" -> Ok (arg1, CalculatorOperation.Divide, arg2)
    | _ -> Error Message.WrongArgFormatOperation

let parseArgs (args: string[]): Result<('a * CalculatorOperation * 'b), Message> =
    let val1Bool, val1 = Double.TryParse(args[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)
    let val2Bool, val2 = Double.TryParse(args[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)
    match val1Bool with
    | true ->
        match val2Bool with
        | true -> isOperationSupported(val1, args[1], val2)
        | false -> Error Message.WrongArg2Format
    | false -> Error Message.WrongArg1Format

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let inline isDividingByZero (arg1, operation, arg2): Result<('a * CalculatorOperation * 'b), Message> =
    match operation = CalculatorOperation.Divide && arg2 = 0.0 with
    | true -> Error Message.DivideByZero
    | _ -> Ok (arg1, operation, arg2)
    
let parseCalcArguments (args: string[]): Result<'a, 'b> =
    let may = MaybeBuilder.maybe
    may
        {
            let! supported = isArgLengthSupported args
            let! parsed = parseArgs supported
            let! result = isDividingByZero parsed
            return result
        }   