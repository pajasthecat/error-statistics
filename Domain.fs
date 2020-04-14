module Domain 

open System.IO
open FSharp.Data

module FileProcessor = 
    
    let getFileAndName (fileName: string) = 
        CsvFile.Load(fileName, hasHeaders = true) 
        |> fun x -> x.Rows 
        |> Seq.map  (fun x -> (x.["escalation_policy_name"], x.["created_on"]))
        |> Seq.toArray

    let ifRaven (word: string) =
        if word = "Team Raven" then 1 else 0

    let getPointByLine (team: string, date:string) =
        team |> ifRaven, 
        date |> System.DateTime.Parse |> System.Globalization.ISOWeek.GetWeekOfYear

    let getTeamRavenErrors (values: (string * string) []) =    
        values
            |> Array.map getPointByLine
            |> Array.groupBy (fun (_, week) -> week)
            |> Array.map (fun (week: int, values) -> week, values |> Array.sumBy (fun (count, _) -> count) )

    let formatOutput (values: (int * int) []) = 
        values 
        |> Array.map (fun (week, count) -> week.ToString() + ", " + count.ToString())
    
    let processFiles(output:(string [] -> unit), path: string) =
        path
        |> Directory.GetFiles
        |> Array.collect
            (getFileAndName
             >> getTeamRavenErrors 
             >> formatOutput)
        |> output

module Outputer =
    let printToDisc = fun (x: string [])-> File.WriteAllLines("output.csv", x)
    let printToConsole = (fun x -> printfn "%A," x)