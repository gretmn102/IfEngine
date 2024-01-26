module IfEngine.SyntaxTree.QuestSystem.Tests
open Fuchu

open IfEngine.SyntaxTree
open IfEngine.SyntaxTree.Helpers

type DungeonQuest =
    | PressurePlate = 0
    | SwingingBlade = 1
    | DartSpewingStatues = 2

let dungeonQuest = VarsContainer.createEnum "DungeonQuest"

type SaveShipQuest =
    | One = 0
    | Two = 1

let saveShipQuest = VarsContainer.createEnum "SaveShipQuest"

type Label =
    | Field = 0
    | Dungeon = 1

[<Tests>]
let ``Scenario.injectQuests`` () =
    let quests =
        [
            Quest.create
                (dungeonQuest == DungeonQuest.PressurePlate)
                Label.Dungeon
                [
                    Say "DungeonQuest.PressurePlate"
                ]

            Quest.create
                (saveShipQuest == SaveShipQuest.One)
                Label.Field
                [
                    Say "SaveShipQuest.One"
                ]

            Quest.create
                (dungeonQuest == DungeonQuest.SwingingBlade)
                Label.Dungeon
                [
                    Say "DungeonQuest.SwingingBlade"
                ]


            Quest.create
                (saveShipQuest == SaveShipQuest.Two)
                Label.Field
                [
                    Say "SaveShipQuest.Two"
                ]

            Quest.create
                (dungeonQuest == DungeonQuest.DartSpewingStatues)
                Label.Dungeon
                [
                    Say "DungeonQuest.DartSpewingStatues"
                ]
        ]

    let scenario =
        [
            label Label.Field [
                Say "All ships has been saved!"
            ]
            label Label.Dungeon [
                Say "Dungeon is completed!"
            ]
        ]
        |> Scenario.ofNamedBlockList

    testList "Scenario.injectQuests" [
        testCase "base" <| fun () ->
            let act =
                Scenario.injectQuests quests scenario

            let _, body = act.[Label.Field]
            match body with
            | [If(_, thenBody, elseBody)] ->
                match thenBody with
                | [Say "SaveShipQuest.One"] -> ()
                | xs ->
                    failwithf """expected [Say "SaveShipQuest.One"] but %A""" xs

                match elseBody with
                | [If(_, thenBody, elseBody)] ->
                    match thenBody with
                    | [Say "SaveShipQuest.Two"] -> ()
                    | xs ->
                        failwithf """expected [Say "SaveShipQuest.Two"] but %A""" xs

                    match elseBody with
                    | [Say "All ships has been saved!"] -> ()
                    | xs ->
                        failwithf """expected [Say "All ships has been saved!"] but %A""" xs
                | xs ->
                    failwithf """expected [If(_, thenBody, elseBody)] but %A""" xs
            | xs ->
                failwithf """expected [If(_, thenBody, elseBody)] but %A""" xs

            let _, body = act.[Label.Dungeon]
            match body with
            | [If(_, thenBody, elseBody)] ->
                match thenBody with
                | [Say "DungeonQuest.PressurePlate"] -> ()
                | xs ->
                    failwithf """expected [Say "DungeonQuest.PressurePlate"] but %A""" xs

                match elseBody with
                | [If(_, thenBody, elseBody)] ->
                    match thenBody with
                    | [Say "DungeonQuest.SwingingBlade"] -> ()
                    | xs ->
                        failwithf """expected [Say "DungeonQuest.SwingingBlade"] but %A""" xs

                    match elseBody with
                    | [If(_, thenBody, elseBody)] ->
                        match thenBody with
                        | [Say "DungeonQuest.DartSpewingStatues"] -> ()
                        | xs ->
                            failwithf """expected [Say "DungeonQuest.DartSpewingStatues"] but %A""" xs

                        match elseBody with
                        | [Say "Dungeon is completed!"] -> ()
                        | xs ->
                            failwithf """expected [Say "Dungeon is completed!"] but %A""" xs
                    | xs ->
                        failwithf """expected [If(_, thenBody, elseBody)] but %A""" xs
                | xs ->
                    failwithf """expected [If(_, thenBody, elseBody)] but %A""" xs
            | xs ->
                failwithf """expected [If(_, thenBody, elseBody)] but %A""" xs
    ]
