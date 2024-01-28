module Tests.Engine.SimpleTestGame
open Fuchu
open FsharpMyExtension.ResultExt
open IfEngine
open IfEngine.SyntaxTree
open IfEngine.SyntaxTree.Helpers
open IfEngine.Engine

open Tests.SyntaxTree.Helpers
open Tests.Engine.Utils

let applesCount = VarsContainer.createNum "apples"
let isApplesOnRightRoad = VarsContainer.createBool "isApplesOnRightRoad"

type CustomStatement = unit

type LabelName =
    | VariablesDefinition
    | Crossroad
    | LeftRoad
    | RightRoad

let beginLoc = VariablesDefinition

let scenario : Scenario<string,LabelName,VarsContainer,unit> =
    [
        label VariablesDefinition [
            applesCount := 0
            isApplesOnRightRoad := true
            jump Crossroad
        ]

        label Crossroad [
            menu "Ты стоишь на развилке в лесу." [
                choice "пойти влево" [ jump LeftRoad ]
                choice "пойти вправо" [ jump RightRoad ]
            ]
        ]

        label LeftRoad [
            if' (fun vars ->
                Var.get applesCount vars > 0
            ) [
                menu "По левой дороге ты встречаешь ежика. Ежик голоден и хочет поесть." [
                    choice "Покормить" [
                        update applesCount (fun count -> count - 1)
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
            if' (Var.get isApplesOnRightRoad) [
                menu "По правой дороге ты находишь яблоко." [
                    choice "Поднять" [
                        isApplesOnRightRoad := false
                        update applesCount ((+) 1)

                        interSay (fun vars ->
                            Var.get applesCount vars
                            |> sprintf "Теперь у тебя %d яблок."
                        )

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
    |> Scenario.ofNamedBlockList

[<Tests>]
let test =
    testList "Engine.SimpleTestGame" [
        testCase "base" <| fun () ->
            let initGameState =
                State.init beginLoc VarsContainer.empty

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
            let outputMsg =
                OutputMsg.Print "Теперь у тебя 1 яблок."
            Assert.Equal("", outputMsg, Engine.getCurrentOutputMsg engine)

            let engine = Engine.update InputMsg.Next engine |> Result.get
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
