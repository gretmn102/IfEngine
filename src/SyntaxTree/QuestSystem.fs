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
    let create condition (label: 'Label) (body: list<Stmt<'Content,'Label,'VarsContainer,'CustomStatement>>) =
        {
            Condition = condition
            Label = label
            Body = body
        }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Scenario =
    let injectQuests
        (quests: list<Quest<'C,'L,'V,'CS>>)
        (scenario: Scenario<'C,'L,'V,'CS>)
        : Scenario<'C,'L,'V,'CS> =

        List.foldBack
            (fun (quest: Quest<_,_,_,_>) (scenario: Scenario<_,_,_,_>) ->
                let label = quest.Label
                match Map.tryFind label scenario with
                | Some (label, elseBody) ->
                    let namedBlock : NamedBlock<_,_,_,_> =
                        label, [
                            If(quest.Condition, quest.Body, elseBody)
                        ]
                    Map.add label namedBlock scenario
                | None ->
                    failwithf "not found %A label" label
            )
            quests
            scenario
