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

type BlockStack<'Text,'LabelName,'Addon> =
    (StatementIndexInBlock * Block<'Text,'LabelName,'Addon>) list

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module BlockStack =
    val ofStack:
        handleCustomStatement: (int ->
                                 'Addon ->
                                 Result<Block<'Text,'LabelName,'Addon>, string>) ->
        startedBlock: Block<'Text,'LabelName,'Addon> ->
        stack: Stack -> Result<BlockStack<'Text,'LabelName,'Addon>,string>

    val toStack:
        stackStatements: BlockStack<'Text,'LabelName,'Addon> -> Stack

    val next:
        stackStatements: BlockStack<'Text,'LabelName,'Addon> ->
        (StatementIndexInBlock * Block<'Text,'LabelName,'Addon>) list option

type NamedStack<'LabelName> =
    {
        Label: 'LabelName
        Stack: Stack
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module NamedStack =
    val create: label: 'LabelName -> stack: Stack -> NamedStack<'LabelName>

    val restoreBlock:
        handleCustomStatement: (int ->
                                 'Addon ->
                                 Result<Block<'Text,'LabelName,'Addon>, string>) ->
        scenario: Scenario<'Text,'LabelName,'Addon> ->
        labelState: NamedStack<'LabelName> ->
        Result<BlockStack<'Text,'LabelName,'Addon>,string>
        when 'LabelName: comparison

type State<'Text,'LabelName,'Addon> =
    {
        LabelState: NamedStack<'LabelName>
        Vars: Vars
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module State =
    val init: beginLocation: 'a -> initVars: Vars -> State<'b,'a,'c>
