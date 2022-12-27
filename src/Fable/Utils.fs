module IfEngine.Fable.Utils
open Feliz

open IfEngine.Types

let label (labelName: 'LabelName) (stmts: Stmt<_, _> list) =
    labelName, stmts
    : Label<_, _>

let divCenter (xs: seq<Fable.React.ReactElement>) =
    Html.div [
        prop.style [
            style.justifyContent.center
            style.display.flex
        ]

        prop.children xs
    ]

let say (txt: string) =
    Html.p [
        prop.style [
            // style.justifyContent.center
            // style.display.flex
        ]
        prop.text txt
    ]
    |> List.singleton
    |> Say

let says (xs: string list) =
    xs
    |> List.map (fun str ->
        Html.p [
            prop.style [
                // style.justifyContent.center
                // style.display.flex
            ]
            prop.children [
                Html.text str
            ]
        ]
    )
    |> Say

let jump (labelName: 'LabelName) =
    Jump labelName

let choice (caption: string) (body: Stmt<'LabelName, 'Addon> list) = caption, body

let menu caption xs = Menu(caption, xs)

let if' pred thenBody elseBody =
    If(pred, thenBody, elseBody)
