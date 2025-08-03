open FSharp.SystemCommandLine
open FSharp.SystemCommandLine.Input
open System.IO
open DirectoryAttributeManager.Core.Utils
open DirectoryAttributeManager.Core.Commands

[<EntryPoint>]
let main argv =
    let out (x: string) = System.Console.WriteLine x

    let dirOrCur =
        Input.option "Directory"
        |> defaultValueFactory (fun _ -> System.IO.Directory.GetCurrentDirectory() |> DI)
        |> desc "The directory, or current directory if left blank"
        |> addValidator
            (fun result ->
                let dir = result.GetValue "Directory"
                if not (Directory.Exists dir) then
                    result.AddError $"Directory does not exist: %s{dir}"
            )

    let attribute =
        Input.argument "Attribute"
        |> desc "An arbitrary attribute name"
        |> addValidator
            (fun result ->
                let nameValue = result.GetValue "Attribute"
                nameValue
                |> function
                | Regex @"^(_|[_a-z0-9]+)$" [x] -> ()
                | _ ->
                    result.AddError $"Name must contain only letters, numbers, or underscores: %s{nameValue}"
            )

    rootCommand argv {
        description $"Directory Attribute Manager"
        setAction id
        addCommand (
            command "attributeFile" {
                description "Output file path for an attribute"
                inputs attribute
                setAction (attributeFile >> out)
            })
        addCommand (
            command "attributes" {
                description "List existing attributes"
                setAction (attributes >> Seq.iter out)
            })
        addCommand (
            command "list" {
                description "List directories with an attribute"
                inputs attribute
                setAction (list >> Seq.iter out)
            })
        addCommand (
            command "toggle" {
                description "Toggle an attribute for a directory"
                inputs (attribute, dirOrCur)
                setAction (toggle)
            })
        addCommand (
            command "isSet" {
                description "Check if the directory has an attribute"
                inputs (attribute, dirOrCur)
                setAction (isSet >> string >> out)
            })
        addCommand (
            command "set" {
                description "Set an attribute for a directory"
                inputs (attribute, dirOrCur)
                setAction (set)
            })
        addCommand (
            command "unset" {
                description "Unset an attribute for a directory"
                inputs (attribute, dirOrCur)
                setAction (unset)
            })
    }
