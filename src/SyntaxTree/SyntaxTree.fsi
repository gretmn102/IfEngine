namespace IfEngine.SyntaxTree

type Block<'Content, 'Label, 'VarsContainer, 'CustomStatement> =
    Stmt<'Content, 'Label, 'VarsContainer, 'CustomStatement> list

and Choice<'Content, 'Label, 'VarsContainer, 'CustomStatement> =
    {
        Predicate: ('VarsContainer -> bool) option
        Caption: string
        Body: Block<'Content, 'Label, 'VarsContainer, 'CustomStatement>
    }

and Choices<'Content, 'Label, 'VarsContainer, 'CustomStatement> =
    Choice<'Content, 'Label, 'VarsContainer, 'CustomStatement> list

and Stmt<'Content, 'Label, 'VarsContainer, 'CustomStatement> =
    | Say of 'Content
    | InterpolationSay of ('VarsContainer -> 'Content)
    | Jump of 'Label
    | Menu of 'Content * Choices<'Content, 'Label, 'VarsContainer, 'CustomStatement>
    | InterpolatedMenu  of ('VarsContainer -> 'Content) * Choices<'Content,'Label,'VarsContainer,'CustomStatement>
    | If of
        ('VarsContainer -> bool) *
        Block<'Content, 'Label, 'VarsContainer, 'CustomStatement> *
        Block<'Content, 'Label, 'VarsContainer, 'CustomStatement>
    | ChangeVars of ('VarsContainer -> 'VarsContainer)
    | Addon of 'CustomStatement

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Choice =
    val mapLabel:
        blockMapLabel: ('a -> Block<'C, 'OldLabel, 'V, 'CS> -> Block<'C, 'NewLabel, 'V, 'CS>) ->
        labelMapping: 'a ->
        Choice<'C, 'OldLabel, 'V, 'CS> ->
            Choice<'C, 'NewLabel, 'V, 'CS>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Choices =
    val mapLabel:
        blockMapLabel: ('a -> Block<'C, 'OldLabel, 'V, 'CS> -> Block<'C, 'NewLabel, 'V, 'CS>) ->
        labelMapping: 'a ->
        choices: Choices<'C, 'OldLabel, 'V, 'CS> ->
            Choices<'C, 'NewLabel, 'V, 'CS>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Stmt =
    val equals:
        customEquals: ('CS -> 'CS -> bool) ->
        leftStatement: Stmt<'C, 'L, 'V, 'CS> ->
        rightStatement: Stmt<'C, 'L, 'V, 'CS> ->
            bool
            when 'C: equality and 'L: equality

    val mapLabel:
        blockMapLabel: (('OldLabel -> 'NewLabel) -> Block<'C, 'OldLabel, 'V, 'CS> -> Block<'C, 'NewLabel, 'V, 'CS>) ->
        labelMapping: ('OldLabel -> 'NewLabel) ->
        statement: Stmt<'C, 'OldLabel, 'V, 'CS> ->
            Stmt<'C, 'NewLabel, 'V, 'CS>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Block =
    val mapLabel:
        labelMapping: ('OldLabel -> 'NewLabel) -> block: Block<'C, 'OldLabel, 'V, 'CS> -> Block<'C, 'NewLabel, 'V, 'CS>

type NamedBlock<'Content, 'Label, 'VarsContainer, 'CustomStatement> =
    'Label * Block<'Content, 'Label, 'VarsContainer, 'CustomStatement>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module NamedBlock =
    val mapLabel:
        labelMapping: ('OldLabel -> 'NewLabel) ->
        'OldLabel * Block<'C, 'OldLabel, 'V, 'CS> ->
            NamedBlock<'C, 'NewLabel, 'V, 'CS>

type Scenario<'Content, 'Label, 'VarsContainer, 'CustomStatement when 'Label: comparison> =
    Map<'Label, NamedBlock<'Content, 'Label, 'VarsContainer, 'CustomStatement>>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Scenario =
    val empty: Scenario<'C, 'L, 'V, 'CS>

    val mapLabel:
        labelMapping: ('OldLabel -> 'NewLabel) ->
        scenario: Scenario<'C, 'OldLabel, 'V, 'CS> ->
            Scenario<'C, 'NewLabel, 'V, 'CS>
            when 'OldLabel: comparison and 'NewLabel: comparison

    val toNamedBlockSeq: scenario: Scenario<'C, 'L, 'V, 'CS> -> seq<NamedBlock<'C, 'L, 'V, 'CS>> when 'L: comparison

    val ofNamedBlockList:
        namedBlocks: list<NamedBlock<'C, 'L, 'V, 'CS>> ->
            Scenario<'C, 'L, 'V, 'CS>
