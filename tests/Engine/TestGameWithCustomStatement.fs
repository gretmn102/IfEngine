module Tests.Engine.TestGameWithCustomStatement
open Fuchu
open FsharpMyExtension.ResultExt
open IfEngine
open IfEngine.SyntaxTree
open IfEngine.SyntaxTree.Helpers
open IfEngine.SyntaxTree.CommonContent
open IfEngine.SyntaxTree.CommonContent.Helpers
open Farkdown.Experimental.Helpers
open IfEngine.Engine

open Tests.Engine.Utils

type FightParams =
    {
        EnemyName: string
        EnemyStrength: int
    }

type Location =
    | Crossroad
    | AngryForest
    | Swamp

[<RequireQualifiedAccess>]
type CustomStatement =
    | Fight of FightParams * winBody:Block<Content, Location, VarsContainer, CustomStatement> * loseBody:Block<Content, Location, VarsContainer, CustomStatement>
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

let health = VarsContainer.createNum "health"

let scenario : Scenario<_, _, _> =
    [
        label Crossroad [
            health := 10
            menu [
                p [[ text "Ты стоишь на развилке двух дорог." ]]
            ] [
                choice "пойти в злой лес" [ jump AngryForest ]
                choice "Пойти на болото" [ jump Swamp ]
            ]
        ]

        label AngryForest [
            menu [
                p [[ text "В лесу ты встречаешь крокодила." ]]
            ] [
                choice "Атаковать" [
                    let strength = 10
                    fight
                        { EnemyName = "Крокодил"; EnemyStrength = strength }
                        [
                            update health (fun health -> health - strength)
                            interSay (fun vars ->
                                [
                                    p [
                                        [ text "Ты победил крокодила!" ]
                                        [ text "У тебя осталось "; bold (text (sprintf "%dHP" (Var.get health vars))) ]
                                    ]
                                ]
                            )
                            menu [
                                p [[ text "Что делаем дальше?" ]]
                            ] [
                                choice "Вернуться на развилку" [ jump Crossroad ]
                            ]
                        ]
                        [
                            say [
                                p [[ text "Крокодил победил тебя..." ]]
                            ]
                        ]
                ]
                choice "Сбежать на развилку" [ jump Crossroad ]
            ]
        ]

        label Swamp [
            menu [
                p [[ text "В болоте ты встречаешь оленя." ]]
            ] [
                choice "Атаковать" [
                    let strength = 5
                    fight
                        { EnemyName = "Олень"; EnemyStrength = strength }
                        [
                            interSay (fun vars ->
                                [
                                    p [
                                        [ text "Ты победил оленя!" ]
                                        [ text "У тебя осталось "; bold (text (sprintf "%dHP" (Var.get health vars))) ]
                                    ]
                                ]
                            )

                            menu [
                                p [[ text "Что делаем дальше?" ]]
                            ] [
                                choice "Вернуться на развилку" [ jump Crossroad ]
                            ]
                        ]
                        [
                            say [
                                p [[ text "Олень победил тебя..." ]]
                            ]
                        ]
                ]
                choice "Сбежать на развилку" [ jump Crossroad ]
            ]
        ]
    ]
    |> Scenario.ofNamedBlockList

[<Tests>]
let tests =
    testList "TestGameWithCustomStatement" [
        testCase "base" <| fun () ->
            let customStatementHandler : CustomStatementHandler<Content, Location, _, CustomStatement, CustomStatementArg, CustomStatementOutput> =
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
                [ p [[ text "Ты стоишь на развилке двух дорог." ]] ], [
                    "пойти в злой лес"
                    "Пойти на болото"
                ]
            equalMenu exp (Engine.getCurrentOutputMsg engine)

            let engine = Engine.update (InputMsg.Choice 0) engine |> Result.get // в злой лес
            let exp =
                [ p [[ text "В лесу ты встречаешь крокодила." ]] ], [
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
                    [ p [[ text "Крокодил победил тебя..." ]]]
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
                [
                    p [
                        [text "Ты победил крокодила!"]
                        [ text "У тебя осталось "; bold (text (sprintf "%dHP" 0)) ]
                    ]
                ]
            Engine.getCurrentOutputMsg engine
            |> equalPrint exp

            let engine =
                Engine.update
                    InputMsg.Next
                    engine |> Result.get
            let exp =
                [ p [[text "Что делаем дальше?"]] ], [
                    "Вернуться на развилку"
                ]
            Engine.getCurrentOutputMsg engine
            |> equalMenu exp
    ]
