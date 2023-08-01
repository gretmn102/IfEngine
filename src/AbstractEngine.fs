namespace IfEngine
open IfEngine.Types
open FsharpMyExtension.ResultExt

[<RequireQualifiedAccess>]
type AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg> =
    | Print of 'Text * (unit -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>)
    | Choices of 'Text * string list * (int -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>)
    | End
    | AddonAct of 'Addon * ('Arg -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>)
    | NextState of State<'Text, 'LabelName, 'Addon> * (unit -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module AbstractEngine =
    let next (stack: BlockStack<'Text, 'LabelName, 'Addon>) state continues =
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
            AbstractEngine.NextState(state, fun () -> continues state)
        | None -> AbstractEngine.End

    let down subIndex (block: Block<'Text, 'LabelName, 'Addon>) stack state continues =
        if List.isEmpty block then
            next stack state continues
        else
            let state =
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
            AbstractEngine.NextState(state, fun () -> continues state)

    let rec interp (addon, handleCustomStatement) (scenario: Scenario<'Text, 'LabelName, 'Addon>) (state: State<'Text, 'LabelName, 'Addon>) =
        let loop state =
            interp (addon, handleCustomStatement) scenario state
            |> Result.get

        if List.isEmpty state.LabelState.Stack then
            Ok AbstractEngine.End
        else
            match NamedStack.restoreBlock handleCustomStatement scenario state.LabelState with
            | Ok stack ->
                let stepInto subIndex (block: Block<'Text, 'LabelName, 'Addon>) =
                    down subIndex block stack state (fun state ->
                        loop state
                    )

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
                        let state =
                            { state with
                                LabelState =
                                    let stack =
                                        if List.isEmpty <| snd scenario.[labelName] then
                                            Stack.empty
                                        else
                                            Stack.createSimpleStatement 0
                                    NamedStack.create labelName stack
                            }

                        AbstractEngine.NextState(state, fun () ->
                            loop state
                        )

                    | Menu(caption, xs) ->
                        let labels = xs |> List.map fst
                        AbstractEngine.Choices(caption, labels, fun i ->
                            let _, body = xs.[i]
                            stepInto i body
                        )

                    | If(pred, thenBody, elseBody) ->
                        if pred state.Vars then
                            stepInto 0 thenBody
                        else
                            stepInto 1 elseBody

                    | Say x ->
                        AbstractEngine.Print(x, fun () ->
                            next stack state (fun state ->
                                loop state
                            )
                        )

                    | Addon customStatement ->
                        AbstractEngine.AddonAct(customStatement, fun customStatementArg ->
                            addon state stack customStatementArg customStatement (fun state ->
                                loop state
                            )
                        )

                    | ChangeVars f ->
                        let state =
                            { state with Vars = f state.Vars }

                        next stack state (fun state ->
                            loop state
                        )
                    |> Ok
                | Error err -> Error err
            | Error err -> Error err
