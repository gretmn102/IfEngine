module IfEngine.Fable.Index
open Fable.React
open Fable.FontAwesome
open Fulma
open Feliz
open Zanaptak.TypedCssClasses

open IfEngine
open IfEngine.Fable.Utils

type Bulma = CssClasses<"https://cdnjs.cloudflare.com/ajax/libs/bulma/0.9.1/css/bulma.min.css", Naming.PascalCase>

let nav dispatch =
    Html.div [
        prop.className [
            Bulma.Tabs
            Bulma.IsCentered
        ]
        prop.children [
            Html.ul [
                prop.children [
                    Html.li [
                        Html.a [
                            prop.onClick (fun _ -> dispatch Game.NewGame)
                            prop.children [
                                Html.div [
                                    Fa.i [ Fa.Solid.File ] []
                                    span [] [ str " " ]
                                    Html.text "New Game"
                                ]
                            ]
                        ]
                    ]
                    Html.li [
                        Html.a [
                            prop.onClick (fun _ -> dispatch Game.Save)
                            prop.children [
                                Html.div [
                                    Fa.i [ Fa.Solid.Save ] []
                                    span [] [ str " " ]
                                    Html.text "Save"
                                ]
                            ]
                        ]
                    ]
                    Html.li [
                        Html.a [
                            prop.onClick (fun _ -> dispatch Game.Load)
                            prop.children [
                                Html.div [
                                    Fa.i [ Fa.Solid.Upload ] []
                                    span [] [ str " " ]
                                    Html.text "Load"
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

let gameView addon (state: Game.State<Text, 'LabelName, 'Addon, 'Arg>) dispatch =
    let print (xs: ReactElement list) =
        Html.div [
            prop.className Bulma.Content
            prop.children xs
        ]

    match state.Game with
    | Interpreter.Print(xs, _) ->
        Html.div [
            prop.children [
                print xs

                Html.div [
                    prop.style [
                        style.justifyContent.center
                        style.display.flex
                    ]
                    prop.children [

                        Html.button [
                            prop.className [
                                Bulma.Button
                            ]
                            prop.onClick (fun _ -> dispatch Game.Next)

                            prop.text "..."
                        ]
                    ]
                ]
            ]
        ]
    | Interpreter.End ->
        Html.div [
            prop.style [
                style.justifyContent.center
                style.display.flex
            ]
            prop.text "Конец"
        ]
    | Interpreter.Choices(caption, choices, _) ->
        let xs =
            choices
            |> List.mapi (fun i label ->
                Html.div [
                    prop.style [
                        style.justifyContent.center
                        style.display.flex
                    ]
                    prop.children [
                        Html.button [
                            prop.className [
                                Bulma.Button
                            ]
                            prop.onClick (fun _ -> dispatch (Game.Choice i))
                            prop.text label
                        ]
                    ]
                ]
            )
        Html.div [
            prop.children (print caption :: xs)
        ]
    | Interpreter.AddonAct(arg, _) ->
        addon arg state dispatch
    | Interpreter.NextState _ ->
        failwith "failwith NextState"

let view addon (state: Game.State<Text, 'LabelName, 'Addon, 'Arg>) (dispatch: Game.Msg<'Addon, 'Arg> -> unit) =
    Html.section [
        prop.style [
            style.padding 20
        ]
        prop.children [
            nav dispatch

            Column.column [
                Column.Width (Screen.All, Column.Is6)
                Column.Offset (Screen.All, Column.Is3)
            ] [
                Box.box' [] [gameView addon state dispatch]
            ]
        ]
    ]
