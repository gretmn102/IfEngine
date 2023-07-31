namespace IfEngine
open IfEngine.Types

[<RequireQualifiedAccess>]
type AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg> =
    | Print of 'Text * (unit -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>)
    | Choices of 'Text * string list * (int -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>)
    | End
    | AddonAct of 'Addon * ('Arg -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>)
    | NextState of State<'Text, 'LabelName, 'Addon>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module AbstractEngine =
    let next changeState (stack: BlockStack<'Text, 'LabelName, 'Addon>) state =
        match BlockStack.next stack with
        | Some stackStatements ->
            let state =
                { state with
                    LabelState =
                        { state.LabelState with
                            Stack =
                                stackStatements
                                |> BlockStack.toStack
                        }
                }
            AbstractEngine.NextState (changeState state)
        | None -> AbstractEngine.End

    let down subIndex (block: Block<'Text, 'LabelName, 'Addon>) stack state =
        if List.isEmpty block then
            next id stack state
        else
            { state with
                LabelState =
                    { state.LabelState with
                        Stack =
                            match state.LabelState.Stack with
                            | StatementIndexInBlock.SimpleStatement index::restStack ->
                                restStack
                                |> Stack.push (StatementIndexInBlock.BlockStatement(index, subIndex))
                                |> Stack.push (StatementIndexInBlock.SimpleStatement 0)
                            | x ->
                                failwithf "Expected SimpleStatement index in state.LabelState.Stack but %A" x
                    }
            }
            |> AbstractEngine.NextState

    let interp (addon, handleCustomStatement) (scenario: Scenario<'Text, 'LabelName, 'Addon>) (state: State<'Text, 'LabelName, 'Addon>) =
        if List.isEmpty state.LabelState.Stack then
            Ok AbstractEngine.End
        else
            match LabelState.restoreBlock handleCustomStatement scenario state.LabelState with
            | Ok stack ->
                let next changeState (stack: BlockStack<'Text, 'LabelName, 'Addon>) =
                    next changeState stack state

                let down subIndex (block: Block<'Text, 'LabelName, 'Addon>) =
                    down subIndex block stack state

                let headStack = List.head stack
                let currentStatement =
                    match headStack with
                    | StatementIndexInBlock.SimpleStatement index, block ->
                        Ok block.[index]
                    | _ ->
                        sprintf "First element in stack must be SimpleStatement"
                        |> Error

                match currentStatement with
                | Ok currentStatement ->
                    match currentStatement with
                    | Jump labelName ->
                        { state with
                            LabelState =
                                let stack =
                                    if List.isEmpty <| snd scenario.[labelName] then
                                        Stack.empty
                                    else
                                        Stack.createSimpleStatement 0
                                LabelState.create labelName stack
                        }
                        |> AbstractEngine.NextState

                    | Menu(caption, xs) ->
                        let labels = xs |> List.map fst
                        AbstractEngine.Choices(caption, labels, fun i ->
                            let _, body = xs.[i]
                            down i body
                        )

                    | If(pred, thenBody, elseBody) ->
                        if pred state.Vars then
                            down 0 thenBody
                        else
                            down 1 elseBody

                    | Say x ->
                        AbstractEngine.Print(x, fun () ->
                            next id stack
                        )

                    | Addon addonArg ->
                        AbstractEngine.AddonAct(addonArg, fun res ->
                            addon state stack res addonArg
                        )

                    | ChangeVars f ->
                        stack
                        |> next (fun state -> { state with Vars = f state.Vars })
                    |> Ok
                | Error err -> Error err
            | Error err -> Error err
