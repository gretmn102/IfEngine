namespace IfEngine.SyntaxTree

[<RequireQualifiedAccess>]
type Var =
    | String of string
    | Bool of bool
    | Num of int

type VarsContainer = Map<string, Var>

type IVar<'Value> =
    abstract Get: VarsContainer -> 'Value
    abstract Set: 'Value -> VarsContainer -> VarsContainer
    abstract Update: ('Value -> 'Value) -> VarsContainer -> VarsContainer
    abstract GetVarName: unit -> string

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Var =
    let get (var: #IVar<'Value>) varsContainer =
        var.Get varsContainer

    let set (var: #IVar<'Value>) newValue varsContainer =
        var.Set newValue varsContainer

    let update (var: #IVar<'Value>) mapping varsContainer =
        var.Update mapping varsContainer

    let equals (var: #IVar<'Value>) otherValue varsContainer =
        (get var varsContainer) = otherValue

    let getVarName (var: #IVar<'Value>) =
        var.GetVarName()

type NumVar(varName: string) =
    interface IVar<int> with
        member __.GetVarName () =
            varName

        member __.Get varsContainer =
            match Map.tryFind varName varsContainer with
            | Some(Var.Num x) -> x
            | _ ->
                failwithf "expected Some(Num x) but %s" varName

        member __.Set newValue varsContainer =
            Map.add varName (Var.Num newValue) varsContainer

        member this.Update mapping varsContainer =
            Var.set
                this
                (mapping (Var.get this varsContainer))
                varsContainer

type StringVar(varName: string) =
    interface IVar<string> with
        member __.GetVarName () =
            varName

        member __.Get varsContainer =
            match Map.tryFind varName varsContainer with
            | Some(Var.String x) -> x
            | _ ->
                failwithf "expected Some(String x) but %s" varName

        member __.Set newValue varsContainer =
            Map.add varName (Var.String newValue) varsContainer

        member this.Update mapping varsContainer =
            Var.set
                this
                (mapping (Var.get this varsContainer))
                varsContainer

type BoolVar(varName: string) =
    interface IVar<bool> with
        member __.GetVarName () =
            varName

        member __.Get varsContainer =
            match Map.tryFind varName varsContainer with
            | Some(Var.Bool x) -> x
            | _ ->
                failwithf "expected Some(Bool x) but %s" varName

        member __.Set newValue varsContainer =
            Map.add varName (Var.Bool newValue) varsContainer

        member this.Update mapping varsContainer =
            Var.set
                this
                (mapping (Var.get this varsContainer))
                varsContainer

type EnumVar<'T when 'T: enum<int32>>(varName: string)  =
    interface IVar<'T> with
        member __.Get varsContainer =
            let enum (x: int) =
                #if FABLE_COMPILER
                unbox x : 'T
                #else
                unbox (System.Enum.ToObject(typeof<'T>, x)) : 'T
                #endif

            match Map.tryFind varName varsContainer with
            | Some(Var.Num x) -> enum x
            | _ ->
                failwithf "expected Some(Bool x) but %s" varName

        member __.GetVarName() =
            varName

        member __.Set (newValue) varsContainer =
            let newValue = int (unbox newValue)
            Map.add varName (Var.Num newValue) varsContainer

        member this.Update mapping varsContainer =
            Var.set
                this
                (mapping (Var.get this varsContainer))
                varsContainer

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module VarsContainer =
    let empty: VarsContainer = Map.empty

    let createNum varName = new NumVar(varName)

    let createEnum varName = new EnumVar<'T>(varName)

    let createString varName = new StringVar(varName)

    let createBool varName = new BoolVar(varName)

type Block<'Content, 'Label, 'CustomStatement> = Stmt<'Content, 'Label, 'CustomStatement> list
and Choice<'Content, 'Label, 'CustomStatement> = string * Block<'Content, 'Label, 'CustomStatement>
and Choices<'Content, 'Label, 'CustomStatement> = Choice<'Content, 'Label, 'CustomStatement> list
and Stmt<'Content, 'Label, 'CustomStatement> =
    | Say of 'Content
    | InterpolationSay of (VarsContainer -> 'Content)
    | Jump of 'Label
    | Menu of 'Content * Choices<'Content, 'Label, 'CustomStatement>
    | If of (VarsContainer -> bool) * Block<'Content, 'Label, 'CustomStatement> * Block<'Content, 'Label, 'CustomStatement>
    | ChangeVars of (VarsContainer -> VarsContainer)
    | Addon of 'CustomStatement

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Choice =
    let mapLabel blockMapLabel labelMapping ((description, body): Choice<_, 'OldLabel, _>) : Choice<_, 'NewLabel, _> =
        description, blockMapLabel labelMapping body

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Choices =
    let mapLabel blockMapLabel labelMapping (choices: Choices<_, 'OldLabel, _>) : Choices<_, 'NewLabel, _> =
        choices
        |> List.map (Choice.mapLabel blockMapLabel labelMapping)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Stmt =
    let equals customEquals (leftStatement: Stmt<'Content, 'Label, 'CustomStatement>) (rightStatement: Stmt<'Content, 'Label, 'CustomStatement>) =
        let rec f x =
            let blockEquals (l: Block<'Content, 'Label, 'CustomStatement>) (r: Block<'Content, 'Label, 'CustomStatement>) =
                l.Length = r.Length
                && List.forall2 (fun l r -> f (l, r)) l r

            match x with
            | Say l, Say r ->
                l = r
            | Jump l, Jump r ->
                l = r
            | Menu(lDesc, lChoices), Menu(rDesc, rChoices) ->
                let choicesEquals (l: Choices<'Content, 'Label, 'CustomStatement>) (r: Choices<'Content, 'Label, 'CustomStatement>) =
                    l.Length = r.Length
                    && List.forall2
                        (fun ((lDesc, lBlock): Choice<'Content, 'Label, 'CustomStatement>) ((rDesc, rBlock): Choice<'Content, 'Label, 'CustomStatement>) ->
                            lDesc = rDesc
                            && blockEquals lBlock rBlock
                        )
                        l
                        r

                lDesc = rDesc
                && choicesEquals lChoices rChoices

            | If(_, lThenBlock, lElseBlock), If(_, rThenBlock, rElseBlock) ->
                blockEquals lThenBlock rThenBlock
                && blockEquals lElseBlock rElseBlock
            | ChangeVars _, ChangeVars _ ->
                true
            | Addon l, Addon r ->
                customEquals l r
            | _ ->
                false

        f (leftStatement, rightStatement)

    let mapLabel blockMapLabel labelMapping (statement: Stmt<_, 'OldLabel, _>) : Stmt<_, 'NewLabel, _> =
        match statement with
        | Stmt.Say content ->
            Stmt.Say content
        | Stmt.InterpolationSay getContent ->
            Stmt.InterpolationSay getContent
        | Stmt.Jump label ->
            Stmt.Jump (labelMapping label)
        | Stmt.Menu(content, choices) ->
            Stmt.Menu(content, Choices.mapLabel blockMapLabel labelMapping choices)
        | Stmt.If(condition, thenBody, elseBody) ->
            Stmt.If(condition, blockMapLabel labelMapping thenBody, blockMapLabel labelMapping elseBody)
        | Stmt.ChangeVars updateVarContainer ->
            Stmt.ChangeVars updateVarContainer
        | Stmt.Addon customStatement ->
            Stmt.Addon customStatement

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Block =
    let rec mapLabel labelMapping (block: Block<_, 'OldLabel, _>) : Block<_, 'NewLabel, _> =
        block
        |> List.map (Stmt.mapLabel mapLabel labelMapping)

type NamedBlock<'Content, 'Label, 'CustomStatement> = 'Label * Block<'Content, 'Label, 'CustomStatement>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module NamedBlock =
    let mapLabel labelMapping ((label, block): NamedBlock<_, 'OldLabel, _>) : NamedBlock<_, 'NewLabel, _> =
        let label = labelMapping label
        label, Block.mapLabel labelMapping block

type Scenario<'Content, 'Label, 'CustomStatement> when 'Label : comparison =
    Map<'Label, NamedBlock<'Content, 'Label, 'CustomStatement>>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Scenario =
    let empty : Scenario<'Content,'Label,'CustomStatement> = Map.empty

    let mapLabel labelMapping (scenario: Scenario<_, 'OldLabel, _>) : Scenario<_, 'NewLabel,  _> =
        scenario
        |> Seq.fold
            (fun st (KeyValue(k, v)) ->
                let key = labelMapping k
                let v = NamedBlock.mapLabel labelMapping v
                Map.add key v st
            )
            Map.empty

    let toNamedBlockSeq (scenario: Scenario<_, _, _>) : seq<NamedBlock<'a,'b,'c>> =
        scenario
        |> Seq.map (fun x -> x.Value)

    let ofNamedBlockList (namedBlocks: NamedBlock<_,_,_> list) : Scenario<'Content,'Label,'CustomStatement> =
        namedBlocks
        |> List.map (fun (labelName, body) -> labelName, (labelName, body))
        |> Map.ofList
