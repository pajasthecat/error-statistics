// Learn more about F# at http://fsharp.org
open CommandLine
open Domain.FileProcessor
open Domain.Outputer

type Options = {
    [<Option('p', "path", Required = true, HelpText = "Path to directory with files.")>] path: string;
    [<Option('o', "output", Required = false, HelpText = "Get an output of the result in .csv.")>] output: bool;
}

[<EntryPoint>]
let main argv =

    let start (res: ParserResult<Options>) = 
        match res with
        | :? Parsed<Options> as parsed when parsed.Value.output ->  processFiles (printToDisc, parsed.Value.path)
        | :? Parsed<Options> as parsed ->  processFiles (printToConsole, parsed.Value.path)
        | :? NotParsed<Options> as notParsed -> printToConsole notParsed.Errors
        | _ -> printToConsole ["Unable to process."]

    CommandLine.Parser.Default.ParseArguments<Options>(argv) |> start

    0