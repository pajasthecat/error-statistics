// Learn more about F# at http://fsharp.org
open System.IO

[<EntryPoint>]
let main argv =

    let getFileAndName (fileName: string) = fileName, fileName |> File.ReadAllLines

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

    let files (path: string) =
        path
        |> Directory.GetFiles
        |> Array.map
            (getFileAndName
             >> removeFirstLine
             >> getTeamRavenErrors)
        |> printfn "%A,"

    argv.[0] |> files

    0
    