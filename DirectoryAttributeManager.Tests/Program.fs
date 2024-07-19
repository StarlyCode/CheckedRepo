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
let [<Fact>]``getCorrectCasedPath - x`` () = @"c:\windows\notepad.exe" |> FI |> getExactPathName |> equals @"C:\Windows\notepad.exe"
//let [<Fact>]``getCorrectCasedPath - xx`` () = @"c:\windows\" |> getCorrectCasedPath |> equals "x"
//Op: End