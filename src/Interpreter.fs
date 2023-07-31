module IfEngine.Interpreter
open IfEngine.Types

type StatementIndexInBlock =
    | BlockStatement of int * int
    /// Must be last element in stack
    | SimpleStatement of int

type Stack = StatementIndexInBlock list

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Stack =
    let empty : Stack = []

    let createSimpleStatement index : Stack =
        [SimpleStatement index]

    let tryHead (stack: Stack) =
        match stack with
        | head::_ ->
            match head with
            | SimpleStatement index -> Ok index
            | _ ->
                sprintf "First element in stack must be SimpleStatement but %A" head
                |> Error
            |> Some
        | [] ->
            None

    let push indexStatement (stack: Stack) : Stack =
        indexStatement :: stack

type StackStatements<'Text, 'LabelName, 'Addon> =
    (StatementIndexInBlock * Block<'Text, 'LabelName, 'Addon>) list

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module StackStatements =
    let ofStack handleCustomStatement (startedBlock: Block<'Text, 'LabelName, 'Addon>) (stack: Stack) : Result<StackStatements<'Text, 'LabelName, 'Addon>, _> =
        let get (index: StatementIndexInBlock) (block: Block<'Text, 'LabelName, 'Addon>) =
            match index with
            | BlockStatement(index, subIndex) ->
                if index < block.Length then
                    match block.[index] with
                    | Menu(_, blocks) ->
                        if subIndex < blocks.Length then
                            Ok (snd blocks.[subIndex])
                        else
                            Error "subIndex < blocks.Length"
                    | If(_, firstBlock, secondBlock) ->
                        match subIndex with
                        | 0 ->
                            Ok firstBlock
                        | 1 ->
                            Ok secondBlock
                        | _ ->
                            Error "subIndex in If statement"
                    | Addon x ->
                        handleCustomStatement subIndex x
                    | x -> Error (sprintf "Expected block statement but %A" x)
                else
                    Error (sprintf "%d < block.Length:\n%A" index block)
            | SimpleStatement index ->
                Ok block

        match stack with
        | [] ->
            Ok []
        | stack ->
            let rec f (block: Block<'Text, 'LabelName, 'Addon>) acc = function
                | [index] ->
                    match index with
                    | SimpleStatement _ ->
                        Ok ((index, block)::acc)
                    | _ ->
                        sprintf "First element in stack must be SimpleStatement but %A" index
                        |> Error
                | index::indexes ->
                    match get index block with
                    | Ok newBlock ->
                        f newBlock ((index, block)::acc) indexes
                    | Error err -> Error err
                | [] -> failwith "Expected index::indexes but []"

            f startedBlock [] (List.rev stack)

    let toStack (stackStatements: StackStatements<'Text, 'LabelName, 'Addon>) : Stack =
        List.map fst stackStatements

    let next (stackStatements: StackStatements<'Text, 'LabelName, 'Addon>) =
        let rec next (stack: StackStatements<'Text, 'LabelName, 'Addon>) =
            match stack with
            | (index, block)::restStack ->
                let index =
                    match index with
                    | SimpleStatement index
                    | BlockStatement(index, _) ->
                        index

                let index = index + 1
                if index < List.length block then
                    (SimpleStatement index, block)::restStack
                    |> Some
                else
                    next restStack
            | [] -> None
        next stackStatements

type LabelState<'LabelName> =
    {
        Label: 'LabelName
        Stack: Stack
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module LabelState =
    let create label stack : LabelState<'LabelName> =
        {
            Label = label
            Stack = stack
        }

    let restoreBlock handleCustomStatement (scenario: Scenario<'Text, 'LabelName, 'Addon>) (labelState: LabelState<'LabelName>) =
        match Map.tryFind labelState.Label scenario with
        | Some (_, block) ->
            StackStatements.ofStack handleCustomStatement block labelState.Stack
        | None ->
            Error (sprintf "Not found %A label" labelState.Label)

type State<'Text, 'LabelName, 'Addon> =
    {
        LabelState: LabelState<'LabelName>
        Vars: Vars
    }

type AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg> =
    | Print of 'Text * (unit -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>)
    | Choices of 'Text * string list * (int -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>)
    | End
    | AddonAct of 'Addon * ('Arg -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>)
    | NextState of State<'Text, 'LabelName, 'Addon>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module AbstractEngine =
    let next changeState (stack: StackStatements<'Text, 'LabelName, 'Addon>) state =
        match StackStatements.next stack with
        | Some stackStatements ->
            let state =
                { state with
                    LabelState =
                        { state.LabelState with
                            Stack =
                                stackStatements
                                |> StackStatements.toStack
                        }
                }
            NextState (changeState state)
        | None -> End

    let down subIndex (block: Block<'Text, 'LabelName, 'Addon>) stack state =
        if List.isEmpty block then
            next id stack state
        else
            { state with
                LabelState =
                    { state.LabelState with
                        Stack =
                            match state.LabelState.Stack with
                            | SimpleStatement index::restStack ->
                                restStack
                                |> Stack.push (BlockStatement(index, subIndex))
                                |> Stack.push (SimpleStatement 0)
                            | x ->
                                failwithf "Expected SimpleStatement index in state.LabelState.Stack but %A" x
                    }
            }
            |> NextState

    let interp (addon, handleCustomStatement) (scenario: Scenario<'Text, 'LabelName, 'Addon>) (state: State<'Text, 'LabelName, 'Addon>) =
        if List.isEmpty state.LabelState.Stack then
            Ok End
        else
            match LabelState.restoreBlock handleCustomStatement scenario state.LabelState with
            | Ok stack ->
                let next changeState (stack: StackStatements<'Text, 'LabelName, 'Addon>) =
                    next changeState stack state

                let down subIndex (block: Block<'Text, 'LabelName, 'Addon>) =
                    down subIndex block stack state

                let headStack = List.head stack
                let currentStatement =
                    match headStack with
                    | SimpleStatement index, block ->
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
                        |> NextState

                    | Menu(caption, xs) ->
                        let labels = xs |> List.map fst
                        Choices(caption, labels, fun i ->
                            let _, body = xs.[i]
                            down i body
                        )

                    | If(pred, thenBody, elseBody) ->
                        if pred state.Vars then
                            down 0 thenBody
                        else
                            down 1 elseBody

                    | Say x ->
                        Print(x, fun () ->
                            next id stack
                        )

                    | Addon addonArg ->
                        AddonAct(addonArg, fun res ->
                            addon state stack res addonArg
                        )

                    | ChangeVars f ->
                        stack
                        |> next (fun state -> { state with Vars = f state.Vars })
                    |> Ok
                | Error err -> Error err
            | Error err -> Error err
