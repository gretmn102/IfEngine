namespace IfEngine.SyntaxTree.QuestSystem
open IfEngine.SyntaxTree

type Quest<'Content,'Label,'VarsContainer,'CustomStatement> =
    {
        Condition: 'VarsContainer -> bool
        Label: 'Label
        Body: list<Stmt<'Content,'Label,'VarsContainer,'CustomStatement>>
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Quest =
    val create:
        condition: ('VarsContainer -> bool) ->
        label: 'Label ->
        body: list<Stmt<'Content,'Label,'VarsContainer,'CustomStatement>>
        -> Quest<'Content,'Label,'VarsContainer,'CustomStatement>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Scenario =
    val injectQuests:
        quests: list<Quest<'Content,'Label,'VarsContainer,'CustomStatement>> ->
        scenario: Scenario<'Content,'Label,'VarsContainer,'CustomStatement>
        -> Scenario<'Content,'Label,'VarsContainer,'CustomStatement>
