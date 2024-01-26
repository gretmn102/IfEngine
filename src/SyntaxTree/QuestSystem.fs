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
    let create condition (label: 'Label) (body: list<Stmt<'Content,'Label,'CustomStatement>>) =
        {
            Condition = condition
            Label = label
            Body = body
        }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Scenario =
    let injectQuests
        (quests: list<Quest<'Content,'Label,'CustomStatement>>)
        (scenario: Scenario<'Content,'Label,'CustomStatement>)
        : Scenario<'Content,'Label,'CustomStatement> =

        List.foldBack
            (fun (quest: Quest<_,_,_>) (scenario: Scenario<_,_,_>) ->
                let label = quest.Label
                match Map.tryFind label scenario with
                | Some (label, elseBody) ->
                    let namedBlock : NamedBlock<_,_,_> =
                        label, [
                            If(quest.Condition, quest.Body, elseBody)
                        ]
                    Map.add label namedBlock scenario
                | None ->
                    failwithf "not found %A label" label
            )
            quests
            scenario
