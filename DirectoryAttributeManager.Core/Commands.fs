namespace DirectoryAttributeManager.Core
open System
open System.IO
open DirectoryAttributeManager.Core.Utils

module Commands =
    let private defaultToCurrentDirectory =
        function
        | Some x -> x
        | _ -> System.IO.Directory.GetCurrentDirectory() |> DI

    let attributeFile attribute = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +/+ $"%s{attribute}.DirAttr"

    let list attribute =
        try
            File.ReadAllLines(attributeFile attribute)
            |> Seq.toList
        with
        | _ -> []
        |> Seq.map (cleanName >> DI >> getExactPathName)
        |> Seq.toList

    let isSet(attribute, dir) =
        let dir = defaultToCurrentDirectory dir
        let dirPath = Path.GetFullPath(dir.FullName) |> trimName |> cleanName

        let checkedDirs = list attribute |> List.map lower
        List.contains dirPath checkedDirs

    let private writeList(attribute, (content: string list)) =
        content
        |> List.filter (fun x -> x.Trim() <> "")
        |> function
        | [] -> File.Delete(attributeFile attribute)
        | lines -> 
            lines
            |> String.concat "\r\n" 
            |> fun text -> File.WriteAllText(attributeFile attribute, text)

    let set(attribute, dir) =
        let dir = defaultToCurrentDirectory dir
        if not (isSet(attribute, Some dir)) then
            let checkedDirs = list attribute 
            let newDirLine = dir.FullName |> Path.GetFullPath |> cleanName
            writeList (attribute, (checkedDirs @ [newDirLine]))

    let unset(attribute, dir) =
        let dir = defaultToCurrentDirectory dir
        let dirPath = dir.FullName |> cleanName
        let checkedDirs = 
            list attribute
            |> List.filter (fun line -> lower line <> lower dirPath)
    
        writeList(attribute, checkedDirs)

    let toggle(attribute, dir) =
        if isSet(attribute, dir) then
            unset(attribute, dir)
        else
            set(attribute, dir)

    let attributes () =
        Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "*.DirAttr")
        |+ fun x -> Path.GetFileNameWithoutExtension(x)
        |> Seq.toList
