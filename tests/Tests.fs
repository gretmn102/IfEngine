module Tests
open Fuchu
open FsharpMyExtension
open FsharpMyExtension.ListZipper

open IfEngine.Types
open IfEngine.Interpreter
open IfEngine.Utils
open IfEngine

open Utils

type Addon = unit

type LabelName =
    | Crossroad
    | LeftRoad
    | RightRoad

let say (txt: string) =
    Say txt

let scenario, vars =
    let vars = Map.empty
    let getApplesCount, updateApplesCount, vars = createNumVar "apples" 0 vars
    let getRoadApplesCount, updateRoadApplesCount, vars = createNumVar "applesOnRoad" 1 vars

    [
        label Crossroad [
            menu "Ты стоишь на развилке в лесу." [
                choice "пойти влево" [ jump LeftRoad ]
                choice "пойти вправо" [ jump RightRoad ]
            ]
        ]

        label LeftRoad [
            if' (fun vars ->
                getApplesCount vars > 0
            ) [
                menu "По левой дороге ты встречаешь ежика. Ежик голоден и хочет поесть." [
                    choice "Покормить" [
                        updateApplesCount (fun count -> count - 1)
                        say "Ты покормил ёжика"
                    ]
                    choice "Вернуться на развилку" [ jump Crossroad ]
                ]
            ] [
                menu "По левой дороге ты встречаешь ежика. Ежик голоден и хочет поесть." [
                    choice "Вернуться" [ jump Crossroad ]
                ]
            ]
        ]

        label RightRoad [
            if' (fun vars ->
                getRoadApplesCount vars > 0
            ) [
                menu "По правой дороге ты находишь яблоко." [
                    choice "Поднять" [
                        updateRoadApplesCount (fun x -> x - 1)
                        updateApplesCount ((+) 1)

                        jump RightRoad
                    ]
                    choice "Вернуться" [ jump Crossroad ]
                ]
            ] [
                menu "По правой дороге больше ничего нет." [
                    choice "Вернуться" [ jump Crossroad ]
                ]
            ]
        ]
    ]
    |> List.map (fun (labelName, body) -> labelName, (labelName, body))
    |> Map.ofList
    |> fun scenario ->
        (scenario: Scenario<_, _, Addon>), vars

[<Tests>]
let InterpreterTests =
    testList "InterpreterTests" [
        testCase "base" <| fun () ->
            let beginLoc = Crossroad

            let scenario =
                let init =
                    {
                        LabelState =
                            LabelState.create
                                beginLoc
                                (Stack.createSimpleStatement 0)

                        Vars = vars
                    }
                {|
                    Scenario = scenario
                    Init = init
                |}

            let interp gameState =
                gameState
                |> interp
                    ((fun state isWin addon ->
                        failwith "addon not implemented"
                    ),
                    (fun subIndex x ->
                        failwithf "handleCustomStatement %A" x
                    ))
                    scenario.Scenario
                |> function
                    | Ok x -> x
                    | Error err -> failwithf "%A" err

            let initGameState =
                {
                    Game.Game = interp scenario.Init
                    Game.GameState = scenario.Init
                    Game.SavedGameState = scenario.Init
                }

            let update msg gameState =
                Game.update interp scenario.Init msg gameState

            let state = initGameState
            let crossroad =
                "Ты стоишь на развилке в лесу.", [
                    "пойти влево"
                    "пойти вправо"
                ]
            getMenu crossroad state.Game

            let state = update (Game.Choice 0) initGameState
            let leftRoadMenu =
                "По левой дороге ты встречаешь ежика. Ежик голоден и хочет поесть.", [
                    "Вернуться"
                ]
            getMenu leftRoadMenu state.Game

            let state = update (Game.Choice 0) state
            getMenu crossroad state.Game

            let state = update (Game.Choice 1) state
            let menu =
                "По правой дороге ты находишь яблоко.", [
                    "Поднять"
                    "Вернуться"
                ]
            getMenu menu state.Game

            let state = update (Game.Choice 0) state
            let menu =
                "По правой дороге больше ничего нет.", [
                    "Вернуться"
                ]
            getMenu menu state.Game

            let state = update (Game.Choice 0) state
            getMenu crossroad state.Game

            let state = update (Game.Choice 0) state
            let rightRoadMenu =
                "По левой дороге ты встречаешь ежика. Ежик голоден и хочет поесть.", [
                    "Покормить"
                    "Вернуться на развилку"
                ]
            getMenu rightRoadMenu state.Game

            let state = update (Game.Choice 0) state
            getPrint "Ты покормил ёжика" state.Game

            let state = update Game.Next state
            getEnd state.Game
    ]

[<EntryPoint>]
let main arg =
    defaultMainThisAssembly arg
