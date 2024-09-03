module AtomicTests
open FsUnit
open Xunit
open FsUnitTyped
open DirectoryAttributeManager.Core.Utils
let inline IsTrue x = should equal true x
let equals (a: 'a) (b: 'a) = b |> shouldEqual a
let inline IsFalse x = should equal false x
//Op: CorrectWhitespace
//Op: IndentOn -
//Op: IndentOn (
//Op: IndentOn |
//Op: IndentOn =
let [<Fact>]``getCorrectCasedPath - lower in dir``          () = @"c:\windows\notepad.exe"           |> FI |> getExactPathName |> equals @"C:\Windows\notepad.exe"
let [<Fact>]``getCorrectCasedPath - lower in dir and file`` () = @"c:\windows\windowsshell.manifest" |> FI |> getExactPathName |> equals @"C:\Windows\WindowsShell.Manifest"
//Op: End