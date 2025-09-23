open FSharp.SystemCommandLine
open FSharp.SystemCommandLine.Input
open DirectoryAttributeManager.Core.Utils
open DirectoryAttributeManager.Core.Commands
open System

[<EntryPoint>]
let main argv =
    let out (x: string) = System.Console.WriteLine x

    let specificDirectoryOrCurrent =
        Input.argumentMaybe<string> "Directory"
        |> desc "The directory, or current directory if left blank"
        |> defaultValueFactory (fun _ -> System.IO.Directory.GetCurrentDirectory() |> Some)
        |> addValidator
            (fun result ->
                let specificDirectoryOrCurrent = (result.GetValue<string option> "Directory").Value
                if specificDirectoryOrCurrent |> System.IO.Directory.Exists |> not then
                    result.AddError $"Directory does not exist: %s{specificDirectoryOrCurrent}"
            )

    let attribute =
        Input.argument "Attribute"
        |> desc "An arbitrary attribute name"
        |> addValidator
            (fun result ->
                let attributeName = result.GetValue "Attribute"
                attributeName
                |> function
                | Regex @"^(_|[_a-z0-9]+)$" [x] -> ()
                | _ ->
                    result.AddError $"Name must contain only letters, numbers, or underscores: %s{attributeName}"
            )

    let adapt (fn: ('t * DI -> unit)) = 
        fun (a: 't, b: string option) -> 
            b
            |> function 
            | Some x -> fn(a, (x |> DI))
            | _ -> failwith "Assumption failed"
            

    let adapt0 (fn: (DI -> unit)) = 
        fun (b: string option) -> 
            b
            |> function 
            | Some x -> fn(x |> DI)
            | _ -> failwith "Assumption failed"
            
    rootCommand argv {
        description $"Directory Attribute Manager"
        setAction id
        addCommand (
            command "attributes" {
                description "List existing attributes"
                setAction (attributes >> Seq.iter out)
            })
        addCommand (
            command "attributeFile" {
                description "Output file path for an attribute"
                inputs attribute
                setAction (attributeFile >> out)
            })
        addCommand (
            command "here" {
                description "List attributes for a directory"
                inputs specificDirectoryOrCurrent
                setAction (adapt0 (here >> Seq.iter out))
            })
        addCommand (
            command "isSet" {
                description "Check if the directory has an attribute"
                inputs (attribute, specificDirectoryOrCurrent)
                setAction (adapt (isSet >> string >> out))
            })
        addCommand (
            command "list" {
                description "List directories with an attribute"
                inputs attribute
                setAction (list >> Seq.iter out)
            })
        addCommand (
            command "set" {
                description "Set an attribute for a directory"
                inputs (attribute, specificDirectoryOrCurrent)
                setAction (adapt set)
            })
        addCommand (
            command "toggle" {
                description "Toggle an attribute for a directory"
                inputs (attribute, specificDirectoryOrCurrent)
                setAction (adapt toggle)
            })
        addCommand (
            command "unset" {
                description "Unset an attribute for a directory"
                inputs (attribute, specificDirectoryOrCurrent)
                setAction (adapt unset)
            })
    }
