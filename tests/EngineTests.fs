module EngineTests
open Fuchu
open FsharpMyExtension
open FsharpMyExtension.ResultExt

open IfEngine
open IfEngine.SyntaxTree
open IfEngine.SyntaxTree.Helpers
open IfEngine.Engine

open Tests.SyntaxTree.Helpers
open Tests.Engine.Utils

let applesCount = VarsContainer.createNum "apples"
let isApplesOnRightRoad = VarsContainer.createBool "isApplesOnRightRoad"

module SimpleTestGame =
    type CustomStatement = unit

    type LabelName =
        | VariablesDefinition
        | Crossroad
        | LeftRoad
        | RightRoad

    let beginLoc = VariablesDefinition

    let scenario =
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
        |> List.map (fun (labelName, body) -> labelName, (labelName, body))
        |> Map.ofList
        |> fun scenario ->
            (scenario: Scenario<_, _, CustomStatement>)

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

module TestGameWithCustomStatement =
    type FightParams =
        {
            EnemyName: string
            EnemyStrength: int
        }

    type Text = string

    type Location =
        | Crossroad
        | AngryForest
        | Swamp

    [<RequireQualifiedAccess>]
    type CustomStatement =
        | Fight of FightParams * winBody:Block<Text, Location, CustomStatement> * loseBody:Block<Text, Location, CustomStatement>
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module CustomStatement =
        let equals (l: CustomStatement) (r: CustomStatement) =
            match r, l with
            | CustomStatement.Fight(l, _, _), CustomStatement.Fight(r, _, _) ->
                l = r

    [<RequireQualifiedAccess>]
    type CustomStatementOutput =
        | Fight of FightParams
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module CustomStatementOutput =
        let ofCustomStatement (customStatement: CustomStatement) =
            match customStatement with
            | CustomStatement.Fight(fightParams, _, _) ->
                CustomStatementOutput.Fight(fightParams)

    [<RequireQualifiedAccess>]
    type CustomStatementArg =
        | Win
        | Lose

    let fight enemy winBody loseBody =
        Addon(
            CustomStatement.Fight(enemy, winBody, loseBody)
        )

    let scenario =
        [
            label Crossroad [
                menu "Ты стоишь на развилке двух дорог." [
                    choice "пойти в злой лес" [ jump AngryForest ]
                    choice "Пойти на болото" [ jump Swamp ]
                ]
            ]

            label AngryForest [
                menu "В лесу ты встречаешь крокодила." [
                    choice "Атаковать" [
                        fight
                            { EnemyName = "Крокодил"; EnemyStrength = 10 }
                            [
                                menu "Ты победил крокодила!" [
                                    choice "Вернуться на развилку" [ jump Crossroad ]
                                ]
                            ]
                            [
                                say "Крокодил победил тебя..."
                            ]
                    ]
                    choice "Сбежать на развилку" [ jump Crossroad ]
                ]
            ]

            label Swamp [
                menu "В болоте ты встречаешь оленя." [
                    choice "Атаковать" [
                        fight
                            { EnemyName = "Олень"; EnemyStrength = 5 }
                            [
                                menu "Ты победил оленя!" [
                                    choice "Вернуться на развилку" [ jump Crossroad ]
                                ]
                            ]
                            [
                                say "Олень победил тебя..."
                            ]
                    ]
                    choice "Сбежать на развилку" [ jump Crossroad ]
                ]
            ]
        ]
        |> List.map (fun (labelName, body) -> labelName, (labelName, body))
        |> Map.ofList
        |> fun scenario ->
            (scenario: Scenario<_, _, CustomStatement>)

    [<Tests>]
    let tests =
        testList "TestGameWithCustomStatement" [
            testCase "base" <| fun () ->
                let customStatementHandler : CustomStatementHandler<Text, Location, CustomStatement, CustomStatementArg, CustomStatementOutput> =
                    {
                        Handle =
                            fun state blockStack customStatementArg customStatement continues ->
                                match customStatement with
                                | CustomStatement.Fight(fightParams, winBody, loseBody) ->
                                    let index, block =
                                        match customStatementArg with
                                        | CustomStatementArg.Win -> 0, winBody
                                        | CustomStatementArg.Lose -> 1, loseBody

                                    AbstractEngine.down index block blockStack state continues

                        RestoreBlockFromStack =
                            fun index customStatement ->
                                match customStatement with
                                | CustomStatement.Fight(_, winBody, loseBody) ->
                                    match index with
                                    | 0 -> Ok winBody
                                    | 1 -> Ok loseBody
                                    | x ->
                                        Error (sprintf "expected 0 or 1 but %d" x)

                        Transformer =
                            CustomStatementOutput.ofCustomStatement
                    }

                let gameState = State.init Crossroad VarsContainer.empty

                let engine =
                    Engine.create
                        customStatementHandler
                        scenario
                        gameState
                    |> Result.get

                let exp =
                    "Ты стоишь на развилке двух дорог.", [
                        "пойти в злой лес"
                        "Пойти на болото"
                    ]
                equalMenu exp (Engine.getCurrentOutputMsg engine)

                let engine = Engine.update (InputMsg.Choice 0) engine |> Result.get // в злой лес
                let exp =
                    "В лесу ты встречаешь крокодила.", [
                        "Атаковать"
                        "Сбежать на развилку"
                    ]
                equalMenu exp (Engine.getCurrentOutputMsg engine)

                let engine = Engine.update (InputMsg.Choice 0) engine |> Result.get // атаковать
                let exp =
                    CustomStatementOutput.Fight(
                        { EnemyName = "Крокодил"; EnemyStrength = 10 }
                    )
                Engine.getCurrentOutputMsg engine
                |> equalCustomStatement exp (=)

                do // lose
                    let engine =
                        Engine.update
                            (InputMsg.HandleCustomStatement CustomStatementArg.Lose)
                            engine |> Result.get
                    let exp =
                        "Крокодил победил тебя..."
                    Engine.getCurrentOutputMsg engine
                    |> equalPrint exp

                    let engine =
                        Engine.update
                            InputMsg.Next
                            engine |> Result.get
                    Engine.getCurrentOutputMsg engine
                    |> equalEnd

                let engine =
                    Engine.update
                        (InputMsg.HandleCustomStatement CustomStatementArg.Win)
                        engine |> Result.get
                let exp =
                    "Ты победил крокодила!", [
                        "Вернуться на развилку"
                    ]
                Engine.getCurrentOutputMsg engine
                |> equalMenu exp
        ]
