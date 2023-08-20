namespace IfEngine
open IfEngine.SyntaxTree

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
        State<'Content,'Label> -> BlockStack<'Content,'Label,'CustomStatement> -> 'CustomStatementArg -> 'CustomStatement -> (State<'Content, 'Label> -> AbstractEngine<'Content,'Label,'CustomStatement,'CustomStatementArg>) -> AbstractEngine<'Content,'Label,'CustomStatement,'CustomStatementArg>

    type CustomStatementRestore<'Content,'Label,'CustomStatement> =
        int -> 'CustomStatement -> Result<Block<'Content,'Label,'CustomStatement>, string>

    val next:
        stack: BlockStack<'Content,'Label,'CustomStatement> ->
        state: State<'Content,'Label> ->
        continues: (State<'Content,'Label> -> AbstractEngine<'Content,'Label,'CustomStatement,'CustomStatementArg>) ->
        AbstractEngine<'Content,'Label,'CustomStatement,'CustomStatementArg>

    val down:
        subIndex: int ->
        block: Block<'Content,'Label,'CustomStatement> ->
        stack: BlockStack<'Content,'Label,'CustomStatement> ->
        state: State<'Content,'Label> ->
        continues: (State<'Content,'Label> -> AbstractEngine<'Content, 'Label, 'CustomStatement, 'Arg>) ->
        AbstractEngine<'Content, 'Label, 'CustomStatement, 'Arg>

    val create:
      addon: CustomStatementHandle<'Content,'Label,'CustomStatement, 'CustomStatementArg> *
      handleCustomStatement: CustomStatementRestore<'Content,'Label,'CustomStatement> ->
        scenario: Scenario<'Content,'Label,'CustomStatement> ->
        state: State<'Content,'Label> ->
        Result<AbstractEngine<'Content,'Label,'CustomStatement,'CustomStatementArg>,string>
        when 'Label: comparison
