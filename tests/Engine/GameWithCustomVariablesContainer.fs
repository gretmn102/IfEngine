module Tests.Engine.GameWithCustomVariablesContainer
open Fuchu
open FsharpMyExtension.ResultExt

open IfEngine
open IfEngine.SyntaxTree
open IfEngine.SyntaxTree.Helpers
open IfEngine.Engine

open Tests.SyntaxTree.Helpers
open Tests.Engine.Utils

type Person =
    {
        Name: string
        Weapon: string
    }

type CustomVar = Person

type Var = Var<Person>

type VarsContainer = VarsContainer<Person>

type PersonVar(varName: string) =
    interface IVar<CustomVar, Person> with
        member __.GetVarName () =
            varName

        member __.Get (varsContainer: VarsContainer) =
            match Map.tryFind varName varsContainer with
            | Some(Var.Custom x) -> x
            | x ->
                failwithf "expected %s = Some(Var.Custom x) but %A" varName x

        member __.Set newValue (varsContainer: VarsContainer) =
            Map.add varName (Var.Custom newValue) varsContainer

        member this.Update mapping varsContainer =
            Var.set
                this
                (mapping (Var.get this varsContainer))
                varsContainer

module VarsContainer =
    let createPerson str = new PersonVar(str)

let jack = VarsContainer.createPerson "jack"

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
            jack := { Name = "Jack"; Weapon = "Spear" }
            interSay (fun vars ->
                let jack = Var.get jack vars
                sprintf "His name is %s and he has %s." jack.Name jack.Weapon
            )
            update jack (fun jack -> { jack with Weapon = "Axe" })
            interSay (fun vars ->
                let jack = Var.get jack vars
                sprintf "%s has %s." jack.Name jack.Weapon
            )
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

            let outputMsg =
                OutputMsg.Print "His name is Jack and he has Spear."
            Assert.Equal("", outputMsg, Engine.getCurrentOutputMsg engine)

            let engine = Engine.update InputMsg.Next engine |> Result.get
            let outputMsg =
                OutputMsg.Print "Jack has Axe."
            Assert.Equal("", outputMsg, Engine.getCurrentOutputMsg engine)

            let engine = Engine.update InputMsg.Next engine |> Result.get
            Assert.Equal("", OutputMsg.End, Engine.getCurrentOutputMsg engine)
    ]
