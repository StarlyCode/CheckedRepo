open FSharp.SystemCommandLine
open System.IO
open DirectoryAttributeManager.Core.Utils
open DirectoryAttributeManager.Core.Commands
open System.CommandLine.Invocation
open System.CommandLine.Help

[<EntryPoint>] 
let main argv =
    let dir = Input.ArgumentMaybe<DirectoryInfo>("Directory", "The directory, or current directory if left blank")

    let attribute = 
        Input.Argument<string>("Attribute", 
            fun arg -> 
                arg.Description <- "An arbitrary attribute name"
                arg.AddValidator 
                    (fun result -> 
                        let nameValue = result.GetValueOrDefault() |> str
                        nameValue
                        |> function
                        | Regex @"^(_|[_a-z0-9]+)$" [x] -> ()
                        | _ -> result.ErrorMessage <- $"Name must contain only letters, numbers, or underscores: {nameValue}"
                    )
        )

    let out (x: string) = System.Console.WriteLine x
    
    //let showHelp (ctx: InvocationContext) =
    //    let hc = HelpContext(ctx.HelpBuilder, ctx.Parser.Configuration.RootCommand, System.Console.Out)
    //    ctx.HelpBuilder.Write(hc)
        
    rootCommand argv {
        description "Directory Attribute Manager"
        setHandler id
        addCommand (
            command "attributeFile" {
                description "Output file path for an attribute"
                inputs attribute
                setHandler (attributeFile >> out)
            })
        addCommand (
            command "attributes" {
                description "List existing attributes"
                setHandler (attributes >> Seq.iter out)
            })
        addCommand (
            command "list" {
                description "List directories with an attribute"
                inputs attribute
                setHandler (list >> Seq.iter out)
            })
        addCommand (
            command "toggle" {
                description "Toggle an attribute for a directory"
                inputs (attribute, dir)
                setHandler (toggle)
            })
        addCommand (
            command "isSet" {
                description "Check if the directory has an attribute"
                inputs (attribute, dir)
                setHandler (isSet >> str >> out)
            })
        addCommand (
            command "set" {
                description "Set an attribute for a directory"
                inputs (attribute, dir)
                setHandler (set)
            })
        addCommand (
            command "unset" {
                description "Unset an attribute for a directory"
                inputs (attribute, dir)
                setHandler (unset)
            })
        //setHandler showHelp
    }
