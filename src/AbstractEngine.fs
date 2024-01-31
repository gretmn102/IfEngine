namespace IfEngine
open IfEngine.SyntaxTree
open FsharpMyExtension.ResultExt

[<RequireQualifiedAccess>]
type AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'Arg> =
    | Print of 'Content * (unit -> AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'Arg>)
    | Choices of 'Content * string list * (int -> AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'Arg>)
    | End
    | AddonAct of 'CustomStatement * ('Arg -> AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'Arg>)
    | NextState of State<'Content, 'Label, 'VarsContainer> * (unit -> AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'Arg>)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module AbstractEngine =
    type CustomStatementHandle<'Content,'Label,'VarsContainer,'CustomStatement,'CustomStatementArg> =
        (State<'Content, 'Label, 'VarsContainer> ->
        BlockStack<'Content,'Label,'VarsContainer,'CustomStatement> ->
        'CustomStatementArg ->
        'CustomStatement ->
        (State<'Content, 'Label, 'VarsContainer> -> AbstractEngine<'Content,'Label,'VarsContainer,'CustomStatement,'CustomStatementArg>) ->
            AbstractEngine<'Content,'Label,'VarsContainer,'CustomStatement,'CustomStatementArg>)

    type CustomStatementRestore<'Content,'Label,'VarsContainer,'CustomStatement> =
        int -> 'CustomStatement -> Result<Block<'Content,'Label,'VarsContainer,'CustomStatement>, string>

    let next
        (stack: BlockStack<'C,'L,'V,'CS>)
        (state: State<'C,'L,'V>)
        continues
        : AbstractEngine<'C,'L,'V,'CS,'CSA> =

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

    let down subIndex (block: Block<'C,'L,'V,'CS>) (stack: BlockStack<'C,'L,'V,'CS>) state continues =
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

    let rec create
        (addon: CustomStatementHandle<'C,'L,'V,'CS,'CSA>,
         handleCustomStatement: CustomStatementRestore<'C,'L,'V,'CS>)
        (scenario: Scenario<'C,'L,'V,'CS>)
        (state: State<'C,'L,'V>)
            : Result<AbstractEngine<'C,'L,'V,'CS,'CSA>,string> =

        let loop state =
            create (addon, handleCustomStatement) scenario state
            |> Result.get

        if List.isEmpty state.LabelState.Stack then
            Ok AbstractEngine.End
        else
            match NamedStack.restoreBlock handleCustomStatement scenario state.LabelState with
            | Ok stack ->
                let stepInto subIndex (block: Block<'C,'L,'V,'CS>) =
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

                    | InterpolatedMenu(getContent, xs) ->
                        let labels = xs |> List.map fst
                        AbstractEngine.Choices(getContent state.Vars, labels, fun i ->
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

                    | InterpolationSay getText ->
                        AbstractEngine.Print(getText state.Vars, fun () ->
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
