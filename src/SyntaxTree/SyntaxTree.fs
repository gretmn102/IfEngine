namespace IfEngine.SyntaxTree

type Block<'Content,'Label,'VarsContainer,'CustomStatement> = Stmt<'Content,'Label,'VarsContainer,'CustomStatement> list
and Choice<'Content,'Label,'VarsContainer,'CustomStatement> = string * Block<'Content,'Label,'VarsContainer,'CustomStatement>
and Choices<'Content,'Label,'VarsContainer,'CustomStatement> = Choice<'Content,'Label,'VarsContainer,'CustomStatement> list
and Stmt<'Content,'Label,'VarsContainer,'CustomStatement> =
    | Say of 'Content
    | InterpolationSay of ('VarsContainer -> 'Content)
    | Jump of 'Label
    | Menu of 'Content * Choices<'Content,'Label,'VarsContainer,'CustomStatement>
    | If of ('VarsContainer -> bool) * Block<'Content,'Label,'VarsContainer,'CustomStatement> * Block<'Content,'Label,'VarsContainer,'CustomStatement>
    | ChangeVars of ('VarsContainer -> 'VarsContainer)
    | Addon of 'CustomStatement

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Choice =
    let mapLabel blockMapLabel labelMapping ((description, body): Choice<'C,'OldLabel,'V,'CS>) : Choice<'C,'NewLabel,'V,'CS> =
        description, blockMapLabel labelMapping body

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Choices =
    let mapLabel blockMapLabel labelMapping (choices: Choices<'C,'OldLabel,'V,'CS>) : Choices<'C,'NewLabel,'V,'CS> =
        choices
        |> List.map (Choice.mapLabel blockMapLabel labelMapping)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Stmt =
    let equals customEquals (leftStatement: Stmt<'Content,'Label,'VarsContainer,'CustomStatement>) (rightStatement: Stmt<'Content,'Label,'VarsContainer,'CustomStatement>) =
        let rec f x =
            let blockEquals (l: Block<'Content,'Label,'VarsContainer,'CustomStatement>) (r: Block<'Content,'Label,'VarsContainer,'CustomStatement>) =
                l.Length = r.Length
                && List.forall2 (fun l r -> f (l, r)) l r

            match x with
            | Say l, Say r ->
                l = r
            | Jump l, Jump r ->
                l = r
            | Menu(lDesc, lChoices), Menu(rDesc, rChoices) ->
                let choicesEquals (l: Choices<'Content,'Label,'VarsContainer,'CustomStatement>) (r: Choices<'Content,'Label,'VarsContainer,'CustomStatement>) =
                    l.Length = r.Length
                    && List.forall2
                        (fun ((lDesc, lBlock): Choice<'Content,'Label,'VarsContainer,'CustomStatement>) ((rDesc, rBlock): Choice<'Content,'Label,'VarsContainer,'CustomStatement>) ->
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

    let mapLabel blockMapLabel labelMapping (statement: Stmt<'C,'OldLabel,'V,'CS>) : Stmt<'C,'NewLabel,'V,'CS> =
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
    let rec mapLabel labelMapping (block: Block<_,'OldLabel,_,_>) : Block<_,'NewLabel,_,_> =
        block
        |> List.map (Stmt.mapLabel mapLabel labelMapping)

type NamedBlock<'Content,'Label,'VarsContainer,'CustomStatement> = 'Label * Block<'Content,'Label,'VarsContainer,'CustomStatement>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module NamedBlock =
    let mapLabel labelMapping ((label, block): NamedBlock<_,'OldLabel,_,_>) : NamedBlock<_,'NewLabel,_,_> =
        let label = labelMapping label
        label, Block.mapLabel labelMapping block

type Scenario<'Content,'Label,'VarsContainer,'CustomStatement> when 'Label : comparison =
    Map<'Label, NamedBlock<'Content,'Label,'VarsContainer,'CustomStatement>>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Scenario =
    let empty : Scenario<'Content,'Label,'VarsContainer,'CustomStatement> = Map.empty

    let mapLabel labelMapping (scenario: Scenario<_,'OldLabel,_,_>) : Scenario<_,'NewLabel,_,_> =
        scenario
        |> Seq.fold
            (fun st (KeyValue(k, v)) ->
                let key = labelMapping k
                let v = NamedBlock.mapLabel labelMapping v
                Map.add key v st
            )
            Map.empty

    let toNamedBlockSeq (scenario: Scenario<_,_,_,_>) : seq<NamedBlock<_,_,_,_>> =
        scenario
        |> Seq.map (fun x -> x.Value)

    let ofNamedBlockList (namedBlocks: NamedBlock<_,_,_,_> list) : Scenario<'Content,'Label,'VarsContainer,'CustomStatement> =
        namedBlocks
        |> List.map (fun (labelName, body) -> labelName, (labelName, body))
        |> Map.ofList
