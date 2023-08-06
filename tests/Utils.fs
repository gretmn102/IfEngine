module Utils
open Fuchu

open IfEngine
open IfEngine.Types

let getPrint exp (cmd: AbstractEngine<'Text, 'Label, 'CustomStatement, 'Arg>) =
    match cmd with
    | AbstractEngine.Print(text, _) ->
        Assert.Equal("", exp, text)
    | x ->
        failwithf "expected Print %A but:\n%A" exp x

let getMenu exp (cmd: AbstractEngine<'Text, 'Label, 'CustomStatement, 'Arg>) =
    match cmd with
    | AbstractEngine.Choices(text, selects, _) ->
        Assert.Equal("", exp, (text, selects))
    | x ->
        failwithf "expected Menu %A\nbut:\n%A" exp x

let getEnd (cmd: AbstractEngine<'Text, 'Label, 'CustomStatement, 'Arg>) =
    match cmd with
    | AbstractEngine.End -> ()
    | x ->
        failwithf "expected End but:\n%A" x

let testCustomStatement (exp: 'CustomStatement) eq (cmd: AbstractEngine<'Text, 'Label, 'CustomStatement, 'Arg>) =
    match cmd with
    | AbstractEngine.AddonAct(customStatement, _) ->
        if not <| eq exp customStatement then
            failwithf "expected: %A\nactual: %A" exp customStatement
    | x ->
        failwithf "expected Menu %A\nbut:\n%A" exp x

let say (txt: string) =
    Say txt

let interSay (getText: _ -> string) =
    InterpolationSay getText
