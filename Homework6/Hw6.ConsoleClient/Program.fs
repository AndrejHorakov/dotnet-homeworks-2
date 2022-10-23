module Hw6.ConsoleClient

open System
open System.Diagnostics.CodeAnalysis
open System.Linq.Expressions
open System.Net.Http
open System.Runtime.InteropServices
open System.Threading.Tasks
open Microsoft.FSharp.Control

let getAsyncQuery (client: HttpClient) (args: String[]) =
    async {
        let uri = $"https://localhost:5001/calculate?value1={args[0]}&operation={args[1]}&value2={args[2]}"
        //Task.Delay(2000)
        let! response = client.GetAsync(uri) |>Async.AwaitTask 
        return response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
    }
    
[<ExcludeFromCodeCoverage>]
[<EntryPoint>]
let main (args : string[])=
    use httpHandler = new HttpClientHandler( ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator)
    use client = new HttpClient(httpHandler)
    printfn "Write like:\n <val1> <operation> <val2> \n enter 'exit' to exit the program \n Write 'start'"
    let enterS = Console.ReadLine()
    while not (enterS = "exit") do
        let enter = Console.ReadLine()
        let args = enter.Split()
        match args.Length with
        | 3 ->
            let answer = (getAsyncQuery client args) |> Async.RunSynchronously
            printfn $"result = {answer}"
        | _ -> printfn "bad" 
    0     
        