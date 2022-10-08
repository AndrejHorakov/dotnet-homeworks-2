open System
open Hw4

let main (args : string[]) =
    let parsedArgs = Parser.parseCalcArguments args
    let ans = Calculator.calculate parsedArgs.arg1 parsedArgs.operation parsedArgs.arg2
    printf $"{ans}"
    