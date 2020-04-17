module Domain

open System.IO
open FSharp.Data

module FileProcessor =

    let getFile (fileName: string) =
        CsvFile.Load(fileName, hasHeaders = true)
        |> fun x -> x.Rows
        |> Seq.map (fun x -> (x.["escalation_policy_name"], x.["created_on"]))
        |> Seq.toArray

    let ifRaven (word: string) =
        if word = "Team Raven" then 1 else 0

    let getPointByLine (team: string, date: string) =
        date
        |> System.DateTime.Parse
        |> System.Globalization.ISOWeek.GetWeekOfYear, team |> ifRaven

    let groupAndFlatten (array: (int * int) []) =
        array
        |> Array.groupBy (fun (week, _) -> week)
        |> Array.map (fun (week: int, values) -> week, values |> Array.sumBy (fun (_, count) -> count))

    let getTeamRavenErrors (values: (string * string) []) =
        values
        |> Array.map getPointByLine
        |> groupAndFlatten

    let formatOutput (values: (int * int) []) =
        values
        |> Array.map (fun (week, count) -> week.ToString() + ", " + count.ToString())
        |> Array.append [| "week, amount of errors." |]

    let processFiles (output: string [] -> unit, path: string) =
        path
        |> Directory.GetFiles
        |> Array.collect (getFile >> getTeamRavenErrors)
        |> groupAndFlatten
        |> formatOutput
        |> output

module Outputer =
    let printToDisc = fun (x: string []) -> File.WriteAllLines("output.csv", x)
    let printToConsole = (fun x -> printfn "%A," x)
