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
    val empty: Stack

    val createSimpleStatement: index: int -> Stack

    val tryHead: stack: Stack -> Result<int,string> option

    val push: indexStatement: StatementIndexInBlock -> stack: Stack -> Stack

type BlockStack<'Content,'Label,'CustomStatement> =
    (StatementIndexInBlock * Block<'Content,'Label,'CustomStatement>) list

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module BlockStack =
    val ofStack:
        handleCustomStatement: (int ->
                                 'CustomStatement ->
                                 Result<Block<'Content,'Label,'CustomStatement>, string>) ->
        startedBlock: Block<'Content,'Label,'CustomStatement> ->
        stack: Stack -> Result<BlockStack<'Content,'Label,'CustomStatement>,string>

    val toStack:
        stackStatements: BlockStack<'Content,'Label,'CustomStatement> -> Stack

    val next:
        stackStatements: BlockStack<'Content,'Label,'CustomStatement> ->
        (StatementIndexInBlock * Block<'Content,'Label,'CustomStatement>) list option

type NamedStack<'Label> =
    {
        Label: 'Label
        Stack: Stack
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module NamedStack =
    val create: label: 'Label -> stack: Stack -> NamedStack<'Label>

    val restoreBlock:
        handleCustomStatement: (int ->
                                 'CustomStatement ->
                                 Result<Block<'Content,'Label,'CustomStatement>, string>) ->
        scenario: Scenario<'Content,'Label,'CustomStatement> ->
        labelState: NamedStack<'Label> ->
        Result<BlockStack<'Content,'Label,'CustomStatement>,string>
        when 'Label: comparison

type State<'Content, 'Label> =
    {
        LabelState: NamedStack<'Label>
        Vars: VarsContainer
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module State =
    val init: beginLocation: 'Label -> initVars: VarsContainer -> State<'Content, 'Label>
