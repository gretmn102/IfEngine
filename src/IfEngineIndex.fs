module IfEngine.Index
open Elmish
open FsharpMyExtension.Either

open IfEngine

type State<'LabelName, 'Addon, 'Arg> =
    {
        Game: Core.T<'LabelName, 'Addon, 'Arg>
        GameState: Core.State<'LabelName, 'Addon>

        SavedGameState: Core.State<'LabelName, 'Addon>
    }

type Msg =
    | Next
    | Choice of int
    | NextState
    | Save
    | Load
    | NewGame

let update interp scenarioInit (msg: Msg) (state: State<'LabelName, 'Addon, 'Arg>) =
    let nextState x =
        let rec nextState gameState = function
            | Core.NextState newGameState ->
                nextState newGameState (interp newGameState)
            | game ->
                { state with
                    GameState = gameState
                    Game = game }
        nextState state.GameState x
    match msg with
    | Next ->
        match state.Game with
        | Core.Print(_, f) ->
            nextState (f ()), Cmd.none
        | Core.NextState x ->
            failwith "nextNextState"
        | Core.End
        | Core.Choices _
        | Core.AddonAct _ ->
            state, Cmd.none
    | Choice i ->
        match state.Game with
        | Core.Choices(_, _, f)->
            nextState (f i), Cmd.none
        | Core.Print(_, f) ->
            nextState (f ()), Cmd.none
        | Core.NextState x ->
            failwith "choiceNextState"
        | Core.End
        | Core.AddonAct _ -> state, Cmd.none
    | NextState ->
        nextState state.Game, Cmd.none
    | Save ->
        let state =
            { state with
                SavedGameState = state.GameState }
        state, Cmd.none
    | Load ->
        let state =
            let gameState = state.SavedGameState
            { state with
                Game = interp gameState
                GameState = gameState }
        state, Cmd.none
    | NewGame ->
        let state =
            { state with
                Game = interp scenarioInit }
        state, Cmd.none

open Zanaptak.TypedCssClasses
type Bulma = CssClasses<"https://cdnjs.cloudflare.com/ajax/libs/bulma/0.9.1/css/bulma.min.css", Naming.PascalCase>

open Fable.React
open Fable.React.Props

open Fable.FontAwesome
open Fulma

open Feliz

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
                            prop.onClick (fun _ -> dispatch NewGame)
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
                            prop.onClick (fun _ -> dispatch Save)
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
                            prop.onClick (fun _ -> dispatch Load)
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
let gameView addon (state:State<'LabelName, 'Addon, 'Arg>) dispatch =
    let print (xs:ReactElement list) =
        Html.div [
            prop.className Bulma.Content
            prop.children xs
        ]

    match state.Game with
    | Core.Print(xs, _) ->
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
                            prop.onClick (fun _ -> dispatch Next)

                            prop.text "..."
                        ]
                    ]
                ]
            ]
        ]
    | Core.End ->
        Html.div [
            prop.style [
                style.justifyContent.center
                style.display.flex
            ]
            prop.text "Конец"
        ]
    | Core.Choices(caption, choices, _) ->
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
                            prop.onClick (fun _ -> dispatch (Choice i))
                            prop.text label
                        ]
                    ]
                ]
            )
        Html.div [
            prop.children (print caption :: xs)
        ]
    | Core.AddonAct(arg, _) ->
        addon arg state dispatch
    | Core.NextState x ->
        Html.div [
            prop.text "NextState"
            prop.ref (fun e ->
                dispatch NextState
            )
        ]
let view addon (state:State<'LabelName, 'Addon, 'Arg>) (dispatch: Msg -> unit) =
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