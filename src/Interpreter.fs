namespace IfEngine
open IfEngine.Types

[<RequireQualifiedAccess>]
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
        [StatementIndexInBlock.SimpleStatement index]

    let tryHead (stack: Stack) =
        match stack with
        | head::_ ->
            match head with
            | StatementIndexInBlock.SimpleStatement index -> Ok index
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
            | StatementIndexInBlock.BlockStatement(index, subIndex) ->
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
            | StatementIndexInBlock.SimpleStatement index ->
                Ok block

        match stack with
        | [] ->
            Ok []
        | stack ->
            let rec f (block: Block<'Text, 'LabelName, 'Addon>) acc = function
                | [index] ->
                    match index with
                    | StatementIndexInBlock.SimpleStatement _ ->
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
                    | StatementIndexInBlock.SimpleStatement index
                    | StatementIndexInBlock.BlockStatement(index, _) ->
                        index

                let index = index + 1
                if index < List.length block then
                    (StatementIndexInBlock.SimpleStatement index, block)::restStack
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
