module IfEngine.Fable.Utils
open Feliz

open IfEngine.Types

type Text = Fable.React.ReactElement list

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
