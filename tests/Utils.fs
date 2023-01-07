module Utils
open Fuchu

open IfEngine
open IfEngine.Types
open IfEngine.Interpreter

let getPrint exp (cmd: Interpreter.Command<'Text, 'LabelName, 'Addon, 'Arg>) =
    match cmd with
    | Print(text, _) ->
        Assert.Equal("", exp, text)
    | x ->
        failwithf "expected Print %A but:\n%A" exp x

let getMenu exp (cmd: Interpreter.Command<'Text, 'LabelName, 'Addon, 'Arg>) =
    match cmd with
    | Choices(text, selects, _) ->
        Assert.Equal("", exp, (text, selects))
    | x ->
        failwithf "expected Menu %A\nbut:\n%A" exp x

let getEnd (cmd: Interpreter.Command<'Text, 'LabelName, 'Addon, 'Arg>) =
    match cmd with
    | End -> ()
    | x ->
        failwithf "expected End but:\n%A" x

let testCustomStatement (exp: 'Addon) eq (cmd: Interpreter.Command<'Text, 'LabelName, 'Addon, 'Arg>) =
    match cmd with
    | AddonAct(customStatement, _) ->
        if not <| eq exp customStatement then
            failwithf "expected: %A\nactual: %A" exp customStatement
    | x ->
        failwithf "expected Menu %A\nbut:\n%A" exp x

let say (txt: string) =
    Say txt
