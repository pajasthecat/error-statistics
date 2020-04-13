module Domain 

open System.IO

module FileProcessor = 

    let getFileAndName (fileName: string) = 
        fileName 
        |> fun x -> x.Split '/' 
        |> fun x -> x.[x.Length - 1], fileName |> File.ReadAllLines

    let removeFirstLine (filename: string, lines: string []) = filename, lines.[1..lines.Length - 1]

    let splitLine (file: string) = file.Split ','

    let ifRaven (word: string) =
        if word = "Team Raven" then 1 else 0

    let getPointByLine (line: string []) =
        line
        |> Array.map ifRaven
        |> Array.toList
        |> List.sum

    let getTeamRavenErrors (filename: string, lines: string []) =
        let sum =
            lines
            |> Array.map (splitLine >> getPointByLine)
            |> Array.toList
            |> List.sum
        (filename, sum)

    let formatOutput (filename: string, count: int) = filename + ", " + count.ToString()

    let processFiles (output:(string [] -> unit), path: string) =
        path
        |> Directory.GetFiles
        |> Array.map
            (getFileAndName
             >> removeFirstLine
             >> getTeamRavenErrors
             >> formatOutput)
        |> output

module Outputer =
    let printToDisc = fun (x: string [])-> File.WriteAllLines("output.csv", x)
    let printToConsole = (fun x -> printfn "%A," x)