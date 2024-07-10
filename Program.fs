open FSharp.SystemCommandLine
open System
open System.IO

module Utils = 
    type DI = DirectoryInfo
    type FI = FileInfo

    let str (x: obj) = x |> function null -> "" | x -> x.ToString()

    let out (x: string) = Console.WriteLine(x)

    let lower = function null -> "" | (x: string) -> x.ToLower()

    let trimName = function null -> "" | (x: string) -> x.TrimEnd('\\').Trim()

    let cleanName = trimName >> lower

    let lines (x: string) =  x.Split([|'\r'; '\n'|], StringSplitOptions.RemoveEmptyEntries)

open Utils

let checkedRepoFile =
    System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".checkedRepos")

let getCheckedRepoFileContent () =
    try
        File.ReadAllLines(checkedRepoFile)
        |> Seq.toList
    with
    | _ -> []
    |> Seq.map cleanName
    |> Seq.toList

let isCheckedRepo (dir: DI) =
    let dirPath = Path.GetFullPath(dir.FullName) |> trimName |> cleanName

    let checkedDirs = getCheckedRepoFileContent() 
    List.contains dirPath checkedDirs

let writeList (content: string list) =
    content
    |> List.filter (fun x -> x.Trim() <> "")
    |> String.concat "\r\n" 
    |> fun text -> File.WriteAllText(checkedRepoFile, text)

let setCheckedRepo (dir: DI) =
    if not (isCheckedRepo dir) then
        let checkedDirs = getCheckedRepoFileContent() 
        let newDirLine = dir.FullName |> Path.GetFullPath |> cleanName
        checkedDirs @ [newDirLine] 
        |> writeList

let unsetCheckedRepo (dir: DI) =
    let dirPath = dir.FullName |> cleanName
    let checkedDirs = 
        getCheckedRepoFileContent() 
        |> List.filter (fun line -> line <> dirPath)
    
    checkedDirs
    |> writeList

let toggleCheckedRepo (dir: DI) =
    if isCheckedRepo dir then
        unsetCheckedRepo dir
    else
        setCheckedRepo dir

[<EntryPoint>] 
let main argv =
    let dir = Input.OptionMaybe<DirectoryInfo>(["--dir"; "-d"], "The directory")
    let orCurDir = 
        function
        | Some x -> x
        | _ -> System.IO.Directory.GetCurrentDirectory() |> DI
    rootCommand argv {
        description "Checked Repo Manager"
        setHandler id
        addCommand (
            command "settingsFile" {
                description "Output Settings File Path"
                setHandler (fun () -> checkedRepoFile |> out)
            })
        addCommand (
            command "list" {
                description "List Checked Repos"
                setHandler (getCheckedRepoFileContent >> Seq.iter out)
            })
        addCommand (
            command "toggle" {
                description "Toggle Checked Repo for current directory"
                inputs dir
                setHandler (orCurDir >> toggleCheckedRepo)
            })
        addCommand (
            command "isChecked" {
                description "Check if the dir is checked"
                inputs dir
                setHandler (orCurDir >> isCheckedRepo >> str >> out)
            })
        addCommand (
            command "set" {
                description "Set Checked Repo for dir"
                inputs dir
                setHandler (orCurDir >> setCheckedRepo)
            })
        addCommand (
            command "unset" {
                description "Unset Checked Repo for dir"
                inputs dir
                setHandler (orCurDir >> unsetCheckedRepo)
            })
    }
