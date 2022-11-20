open System
open System.Globalization
open Hw5

let matcher (val1, operation, val2) = Calculator.calculate val1 operation val2
let main (args: string[]) =
    let parsedArgs = Parser.parseCalcArguments args
    match parsedArgs with
    | Ok res -> printf $"{matcher res}"
    | Error errorValue -> printf $"{errorValue}"
   
