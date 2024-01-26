namespace IfEngine.SyntaxTree.QuestSystem
open IfEngine.SyntaxTree

type Quest<'Content,'Label,'CustomStatement> =
    {
        Condition: VarsContainer -> bool
        Label: 'Label
        Body: list<Stmt<'Content,'Label,'CustomStatement>>
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Quest =
    val create:
        condition: (VarsContainer -> bool) ->
        label: 'Label ->
        body: list<Stmt<'Content,'Label,'CustomStatement>>
        -> Quest<'Content,'Label,'CustomStatement>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Scenario =
    val injectQuests:
        quests: list<Quest<'Content,'Label,'CustomStatement>> ->
        scenario: Scenario<'Content,'Label,'CustomStatement>
        -> Scenario<'Content,'Label,'CustomStatement>
