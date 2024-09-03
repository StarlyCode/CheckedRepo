namespace DirectoryAttributeManager.Core
open System
open System.IO
open System.Diagnostics
open System.Text.RegularExpressions

module Utils = 
    let os = Option.defaultValue ""
    
    let ns = function null -> "" | x -> x
    
    let (|NS|) (x: string) : string = x |> ns

    [<DebuggerHidden>]
    let private regexActivePattern (NS pattern) (NS input) options =
        let regexMatch = Regex.Match(input, pattern, options)
        if regexMatch.Success then
            List.tail [ for g in regexMatch.Groups -> g.Value ]
            |> Some
        else
            None

    [<DebuggerHidden>]
    let (|Regex|_|) pattern input = regexActivePattern pattern input RegexOptions.IgnoreCase

    let trim = ns >> fun x -> x.Trim()
    
    let (+/+) x y = System.IO.Path.Combine(x, y |> trim)
    
    let inline swap a b c = a c b
    
    let inline (|+) a b = Seq.map b a

    type DI = DirectoryInfo

    type FI = FileInfo

    let lower = function null -> "" | (x: string) -> x.ToLower()

    let trimName = function null -> "" | (x: string) -> x.TrimEnd('\\').Trim()
    
    let rec getExactPathName (f: FileSystemInfo) : string =
        let pathName = f.FullName
        if not (File.Exists(pathName) || Directory.Exists(pathName)) then
            pathName
        else
            let di = DirectoryInfo(pathName)
            match di.Parent with
            | null -> di.Name.ToUpper()
            | parent ->
                Path.Combine(
                    getExactPathName(parent),
                    (parent.GetFileSystemInfos(di.Name).[0]).Name)

    let cleanName = trimName >> lower

    let lines (x: string) =  x.Split([|'\r'; '\n'|], StringSplitOptions.RemoveEmptyEntries)
    
    let inline (^) f a = f a
