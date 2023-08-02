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
    val empty: Stack

    val createSimpleStatement: index: int -> Stack

    val tryHead: stack: Stack -> Result<int,string> option

    val push: indexStatement: StatementIndexInBlock -> stack: Stack -> Stack

type BlockStack<'Text,'Label,'Addon> =
    (StatementIndexInBlock * Block<'Text,'Label,'Addon>) list

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module BlockStack =
    val ofStack:
        handleCustomStatement: (int ->
                                 'Addon ->
                                 Result<Block<'Text,'Label,'Addon>, string>) ->
        startedBlock: Block<'Text,'Label,'Addon> ->
        stack: Stack -> Result<BlockStack<'Text,'Label,'Addon>,string>

    val toStack:
        stackStatements: BlockStack<'Text,'Label,'Addon> -> Stack

    val next:
        stackStatements: BlockStack<'Text,'Label,'Addon> ->
        (StatementIndexInBlock * Block<'Text,'Label,'Addon>) list option

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
                                 'Addon ->
                                 Result<Block<'Text,'Label,'Addon>, string>) ->
        scenario: Scenario<'Text,'Label,'Addon> ->
        labelState: NamedStack<'Label> ->
        Result<BlockStack<'Text,'Label,'Addon>,string>
        when 'Label: comparison

type State<'Text,'Label,'Addon> =
    {
        LabelState: NamedStack<'Label>
        Vars: Vars
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module State =
    val init: beginLocation: 'a -> initVars: Vars -> State<'b,'a,'c>
