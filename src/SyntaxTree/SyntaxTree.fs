namespace IfEngine.SyntaxTree

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
