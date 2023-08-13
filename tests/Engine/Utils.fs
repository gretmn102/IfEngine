module Tests.Engine.Utils
open Fuchu

open IfEngine
open IfEngine.SyntaxTree
open IfEngine.Engine

let equalPrint exp (cmd: OutputMsg<'Content, 'CustomStatement>) =
    match cmd with
    | OutputMsg.Print(text) ->
        Assert.Equal("", exp, text)
    | x ->
        failwithf "expected Print %A but:\n%A" exp x

let equalMenu exp (cmd: OutputMsg<'Content, 'CustomStatement>) =
    match cmd with
    | OutputMsg.Choices(text, selects) ->
        Assert.Equal("", exp, (text, selects))
    | x ->
        failwithf "expected Menu %A\nbut:\n%A" exp x

let equalEnd (cmd: OutputMsg<'Content, 'CustomStatement>) =
    match cmd with
    | OutputMsg.End -> ()
    | x ->
        failwithf "expected End but:\n%A" x

let equalCustomStatement (exp: 'CustomStatement) eq (cmd: OutputMsg<'Content, 'CustomStatement>) =
    match cmd with
    | OutputMsg.CustomStatement(customStatement) ->
        if not <| eq exp customStatement then
            failwithf "expected: %A\nactual: %A" exp customStatement
    | x ->
        failwithf "expected Menu %A\nbut:\n%A" exp x

let say (txt: string) =
    Say txt

let interSay (getText: _ -> string) =
    InterpolationSay getText
