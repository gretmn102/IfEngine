namespace IfEngine
open IfEngine.SyntaxTree
open FsharpMyExtension.ResultExt

[<RequireQualifiedAccess>]
type AbstractEngine<'Content, 'Label, 'CustomStatement, 'Arg> =
    | Print of 'Content * (unit -> AbstractEngine<'Content, 'Label, 'CustomStatement, 'Arg>)
    | Choices of 'Content * string list * (int -> AbstractEngine<'Content, 'Label, 'CustomStatement, 'Arg>)
    | End
    | AddonAct of 'CustomStatement * ('Arg -> AbstractEngine<'Content, 'Label, 'CustomStatement, 'Arg>)
    | NextState of State<'Content, 'Label> * (unit -> AbstractEngine<'Content, 'Label, 'CustomStatement, 'Arg>)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module AbstractEngine =
    type CustomStatementHandle<'Content,'Label,'CustomStatement, 'CustomStatementArg> =
        (State<'Content, 'Label> ->
        BlockStack<'Content,'Label,'CustomStatement> ->
        'CustomStatementArg ->
        'CustomStatement ->
        (State<'Content, 'Label> -> AbstractEngine<'Content,'Label,'CustomStatement,'CustomStatementArg>)
        -> AbstractEngine<'Content,'Label,'CustomStatement,'CustomStatementArg>)

    type CustomStatementRestore<'Content,'Label,'CustomStatement> =
        int -> 'CustomStatement -> Result<Block<'Content,'Label,'CustomStatement>, string>

    let next
        (stack: BlockStack<'Content,'Label,'CustomStatement>)
        (state: State<'Content, 'Label>)
        continues
        : AbstractEngine<'Content,'Label,'CustomStatement,'CustomStatementArg> =

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

    let down subIndex (block: Block<'Content, 'Label, 'CustomStatement>) (stack: BlockStack<'Content,'Label,'CustomStatement>) state continues =
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
        (addon: CustomStatementHandle<'Content,'Label,'CustomStatement, 'CustomStatementArg>, handleCustomStatement: CustomStatementRestore<'Content,'Label,'CustomStatement>)
        (scenario: Scenario<'Content, 'Label, 'CustomStatement>)
        (state: State<'Content, 'Label>) =

        let loop state =
            create (addon, handleCustomStatement) scenario state
            |> Result.get

        if List.isEmpty state.LabelState.Stack then
            Ok AbstractEngine.End
        else
            match NamedStack.restoreBlock handleCustomStatement scenario state.LabelState with
            | Ok stack ->
                let stepInto subIndex (block: Block<'Content, 'Label, 'CustomStatement>) =
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
