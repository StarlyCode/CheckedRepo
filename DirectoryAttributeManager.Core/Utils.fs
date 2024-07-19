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

    let str (x: obj) = x |> function null -> "" | x -> x.ToString()

    let lower = function null -> "" | (x: string) -> x.ToLower()

    let trimName = function null -> "" | (x: string) -> x.TrimEnd('\\').Trim()
    
    //let rec getCorrectCasedPath path =
    //    let root = Path.GetPathRoot(path)
    //    let restOfPath = path.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar)
    
    //    // Base case: if there's no more path to process, return the root
    //    if String.IsNullOrEmpty(restOfPath) then
    //        root
    //    else
    //        let directoryName, fileName = 
    //            match Path.GetFileName(restOfPath) with
    //            | null -> restOfPath, "" // No file name, assume the entire remaining path is a directory
    //            | fileName ->
    //                let directoryName = Path.GetDirectoryName(restOfPath)
    //                directoryName, fileName
        
    //        let fullPath = Path.Combine(root, directoryName)
        
    //        // Check if the directory exists
    //        let di = DirectoryInfo(fullPath)
    //        if di.Exists then
    //            let correctCasedDirectoryName = di.Name
    //            let nextRoot = Path.Combine(root, correctCasedDirectoryName)
            
    //            // Recursively call this function for the remaining path
    //            let correctedRestOfPath = getCorrectCasedPath(Path.Combine(nextRoot, fileName))
    //            correctedRestOfPath
    //        else
    //            failwith $"Directory '{fullPath}' does not exist."

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

    //let rec getCorrectCasedPath path =
    //    let root = Path.GetPathRoot(path)
    //    let restOfPath = path.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar)
    
    //    // Base case: if there's no more path to process, return the root
    //    if String.IsNullOrEmpty(restOfPath) then
    //        root
    //    else
    //        let directoryName, fileName = 
    //            match Path.GetFileName(restOfPath) with
    //            | null -> restOfPath, "" // No file name, assume the entire remaining path is a directory
    //            | fileName ->
    //                let directoryName = Path.GetDirectoryName(restOfPath)
    //                directoryName, fileName
        
    //        let fullPath = Path.Combine(root, directoryName)
        
    //        // Check if the directory exists
    //        let di = DirectoryInfo(fullPath)
    //        if di.Exists then
    //            let correctCasedDirectoryName = di.Name
    //            let nextRoot = Path.Combine(root, correctCasedDirectoryName)
            
    //            // Correctly handle the remaining path for the recursive call
    //            let correctedRestOfPath =
    //                if String.IsNullOrEmpty(fileName) then
    //                    "" // If there's no file name, we've reached the deepest directory
    //                else
    //                    Path.Combine(fileName, restOfPath.Substring(directoryName.Length + 1))
            
    //            // Recursively call this function for the remaining path
    //            let finalPath = getCorrectCasedPath(Path.Combine(nextRoot, correctedRestOfPath))
    //            finalPath
    //        else
    //            failwith $"Directory '{fullPath}' does not exist."


    let cleanName = trimName >> lower

    let lines (x: string) =  x.Split([|'\r'; '\n'|], StringSplitOptions.RemoveEmptyEntries)
    
    let inline (^) f a = f a
