module Hw6.Logic

open System.Diagnostics.CodeAnalysis
open Giraffe
open Hw5

[<ExcludeFromCodeCoverage>]
[<CLIMutable>]
type CalcVals =
    {
        value1: string
        operation: string
        value2:string
    }

[<ExcludeFromCodeCoverage>]
let matcher (val1, operation, val2) = Calculator.calculate val1 operation val2

let reWriteResults (mess, args : string[]): Result<'a, string> =
    match mess with
    | Error Message.WrongArgFormatOperation -> Error $"Could not parse value '{args[1]}'"
    | Error Message.WrongArg1Format -> Error $"Could not parse value '{args[0]}'"
    | Error Message.WrongArg2Format -> Error $"Could not parse value '{args[2]}'"
    | Error Message.DivideByZero -> Ok "DivideByZero"
    | Ok ok -> Ok ((matcher ok).ToString())
    
let calculatorHandler: HttpHandler =
    fun next ctx ->
        let strArgs = ctx.TryBindQueryString<CalcVals>()
        match strArgs with
        | Ok ok ->
            let args = [|ok.value1; ok.operation; ok.value2|]
            let parsed = Parser.parseCalcArguments args
            let result = reWriteResults (parsed, args)
            match result with
            | Ok ok -> (setStatusCode 200 >=> text ok) next ctx
            | Error error -> (setStatusCode 400 >=> text error) next ctx
        | Error error -> (setStatusCode 400 >=> text error) next ctx

let webApp: HttpHandler =
    choose [
        GET >=> choose [
             route "/" >=> text "Use //calculate?value1=<VAL1>&operation=<OPERATION>&value2=<VAL2>"
             route "/calculate" >=> calculatorHandler
        ]
        setStatusCode 404 >=> text "Not Found" 
    ]

        
