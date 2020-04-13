// Learn more about F# at http://fsharp.org
open System.IO
open CommandLine

type Options = {
    [<Option('p', "path", Required = true, HelpText = "Path to directory with files.")>] path: string;
    [<Option('o', "output", Required = false, HelpText = "Get an output of the result in .csv.")>] output: bool;
}

[<EntryPoint>]
let main argv =

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

    let getOutput printToDisc = 
        match printToDisc with
        | false -> (fun x -> printfn "%A," x)
        | true -> (fun x -> File.WriteAllLines("output.csv", x))

    let printToDisc = fun (x: string [])-> File.WriteAllLines("output.csv", x)
    let printToConsole = (fun x -> printfn "%A," x)

    let start (res: ParserResult<Options>) = 
        match res with
        | :? Parsed<Options> as parsed when parsed.Value.output ->  processFiles (printToDisc, parsed.Value.path)
        | :? Parsed<Options> as parsed ->  processFiles (printToConsole, parsed.Value.path)
        | :? NotParsed<Options> as notParsed -> printToConsole notParsed.Errors
        | _ -> printToConsole ["Unable to process."]

    CommandLine.Parser.Default.ParseArguments<Options>(argv) |> start

    0