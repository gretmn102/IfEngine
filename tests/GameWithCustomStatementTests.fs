module GameWithCustomStatementTests
open Fuchu

open IfEngine.Types
open IfEngine.Interpreter
open IfEngine.Utils
open IfEngine

open Utils

type Text = string

type LabelName =
    | Crossroad
    | LeftRoad
    | RightRoad

type CustomStatement =
    | CustomStatement of Block<Text, LabelName, CustomStatement> []

type CustomStatementArg = int

let sampleCustomStatement: CustomStatement =
    CustomStatement ([|
        [
            say "1.0.0"
            say "1.0.1"
        ]
        [
            say "1.1.0"
            say "1.1.1"
        ]
    |])

let scenario, vars =
    let vars = Map.empty
    let getApplesCount, updateApplesCount, vars = createNumVar "apples" 0 vars
    let getRoadApplesCount, updateRoadApplesCount, vars = createNumVar "applesOnRoad" 1 vars

    [
        label Crossroad [
            say "0"
            Addon sampleCustomStatement
            say "2"
        ]
    ]
    |> List.map (fun (labelName, body) -> labelName, (labelName, body))
    |> Map.ofList
    |> fun scenario ->
        (scenario: Scenario<_, _, CustomStatement>), vars

[<Tests>]
let InterpreterTests =
    testList "InterpreterTests" [
        testCase "base" <| fun () ->
            let beginLoc = Crossroad

            let init =
                {
                    LabelState =
                        LabelState.create
                            beginLoc
                            (Stack.createSimpleStatement 0)

                    Vars = vars
                }

            let interp gameState =
                gameState
                |> interp
                    ((fun state stack (customStatementArg: CustomStatementArg) customStatement ->
                        match customStatement with
                        | CustomStatement blocs ->
                            down customStatementArg blocs.[customStatementArg] stack state
                    ),
                    (fun subIndex customStatement ->
                        match customStatement with
                        | CustomStatement blocs ->
                            Ok blocs.[subIndex]
                    ))
                    scenario
                |> function
                    | Ok x -> x
                    | Error err -> failwithf "%A" err

            let initGameState =
                {
                    Game.Game = interp init
                    Game.GameState = init
                    Game.SavedGameState = init
                }

            let update msg gameState =
                Game.update interp init msg gameState

            let state = initGameState

            getPrint "0" state.Game

            let state = update Game.Next state
            state.Game
            |> testCustomStatement sampleCustomStatement (fun x y ->
                let rec f = function
                    | CustomStatement lBlocks, CustomStatement rBlocks ->
                        Array.forall2
                            (List.forall2
                                (Stmt.equals (fun x y -> f (x, y)))
                            )
                            lBlocks
                            rBlocks
                f (x, y)
            )

            let handleCustomState = function
                | CustomStatement blocks ->
                    0
            let state = update (Game.HandleCustomState handleCustomState) state
            getPrint "1.0.0" state.Game

            let state = update Game.Next state
            getPrint "1.0.1" state.Game

            let state = update Game.Next state
            getPrint "2" state.Game

            let state = update Game.Next state
            getEnd state.Game
    ]
