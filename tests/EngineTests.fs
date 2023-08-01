module EngineTests
open Fuchu
open FsharpMyExtension
open FsharpMyExtension.ResultExt

open IfEngine
open IfEngine.Types
open IfEngine.Utils
open IfEngine.Engine

open Utils

module SimpleTestGame =
    type CustomStatement = unit

    type LabelName =
        | Crossroad
        | LeftRoad
        | RightRoad
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
            (scenario: Scenario<_, _, CustomStatement>), vars

    [<Tests>]
    let test =
        testList "Engine.SimpleTestGame" [
            testCase "base" <| fun () ->
                let beginLoc = Crossroad

                let initGameState =
                    State.init beginLoc vars

                let engine =
                    Engine.create
                        CustomStatementHandler.empty
                        scenario
                        initGameState
                    |> Result.get

                let crossroad =
                    OutputMsg.Choices("Ты стоишь на развилке в лесу.", [
                        "пойти влево"
                        "пойти вправо"
                    ])
                Assert.Equal("", crossroad, Engine.getCurrentOutputMsg engine)

                let engine = Engine.update (InputMsg.Choice 0) engine |> Result.get
                let leftRoadMenu =
                    OutputMsg.Choices("По левой дороге ты встречаешь ежика. Ежик голоден и хочет поесть.", [
                        "Вернуться"
                    ])
                Assert.Equal("", leftRoadMenu, Engine.getCurrentOutputMsg engine)

                let engine = Engine.update (InputMsg.Choice 0) engine |> Result.get
                Assert.Equal("", crossroad, Engine.getCurrentOutputMsg engine)

                let engine = Engine.update (InputMsg.Choice 1) engine |> Result.get
                let menu =
                    OutputMsg.Choices("По правой дороге ты находишь яблоко.", [
                        "Поднять"
                        "Вернуться"
                    ])
                Assert.Equal("", menu, Engine.getCurrentOutputMsg engine)

                let engine = Engine.update (InputMsg.Choice 0) engine |> Result.get
                let menu =
                    OutputMsg.Choices("По правой дороге больше ничего нет.", [
                        "Вернуться"
                    ])
                Assert.Equal("", menu, Engine.getCurrentOutputMsg engine)

                let engine = Engine.update (InputMsg.Choice 0) engine |> Result.get
                Assert.Equal("", crossroad, Engine.getCurrentOutputMsg engine)

                let engine = Engine.update (InputMsg.Choice 0) engine |> Result.get
                let rightRoadMenu =
                    OutputMsg.Choices("По левой дороге ты встречаешь ежика. Ежик голоден и хочет поесть.", [
                        "Покормить"
                        "Вернуться на развилку"
                    ])
                Assert.Equal("", rightRoadMenu, Engine.getCurrentOutputMsg engine)

                let engine = Engine.update (InputMsg.Choice 0) engine |> Result.get
                Assert.Equal("", OutputMsg.Print("Ты покормил ёжика"), Engine.getCurrentOutputMsg engine)

                let engine = Engine.update InputMsg.Next engine |> Result.get
                Assert.Equal("", OutputMsg.End, Engine.getCurrentOutputMsg engine)
        ]
