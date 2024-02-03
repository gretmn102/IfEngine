namespace IfEngine
open IfEngine.SyntaxTree

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

type BlockStack<'Content,'Label,'VarsContainer,'CustomStatement> =
    (StatementIndexInBlock * Block<'Content,'Label,'VarsContainer,'CustomStatement>) list

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module BlockStack =
    let ofStack handleCustomStatement (startedBlock: Block<'Content,'Label,'VarsContainer,'CustomStatement>) (stack: Stack) : Result<BlockStack<'Content,'Label,'VarsContainer,'CustomStatement>, _> =
        let get (index: StatementIndexInBlock) (block: Block<'Content,'Label,'VarsContainer,'CustomStatement>) =
            match index with
            | StatementIndexInBlock.BlockStatement(index, subIndex) ->
                if index < block.Length then
                    match block.[index] with
                    | Menu(_, blocks)
                    | InterpolatedMenu(_, blocks) ->
                        if subIndex < blocks.Length then
                            Ok (blocks.[subIndex].Body)
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
            let rec f (block: Block<'Content,'Label,'VarsContainer,'CustomStatement>) acc = function
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

    let toStack (stackStatements: BlockStack<'Content,'Label,'VarsContainer,'CustomStatement>) : Stack =
        List.map fst stackStatements

    let next (stackStatements: BlockStack<'Content,'Label,'VarsContainer,'CustomStatement>) =
        let rec next (stack: BlockStack<'Content,'Label,'VarsContainer,'CustomStatement>) =
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

type NamedStack<'Label> =
    {
        Label: 'Label
        Stack: Stack
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module NamedStack =
    let create label stack : NamedStack<'Label> =
        {
            Label = label
            Stack = stack
        }

    let restoreBlock handleCustomStatement (scenario: Scenario<'Content,'Label,'VarsContainer,'CustomStatement>) (labelState: NamedStack<'Label>) =
        match Map.tryFind labelState.Label scenario with
        | Some (_, block) ->
            BlockStack.ofStack handleCustomStatement block labelState.Stack
        | None ->
            Error (sprintf "Not found %A label" labelState.Label)

type State<'Content, 'Label, 'VarsContainer> =
    {
        LabelState: NamedStack<'Label>
        Vars: 'VarsContainer
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module State =
    let init beginLocation initVars =
        {
            LabelState =
                NamedStack.create
                    beginLocation
                    (Stack.createSimpleStatement 0)

            Vars = initVars
        }
