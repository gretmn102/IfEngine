namespace IfEngine
open IfEngine.Types

[<RequireQualifiedAccess>]
type AbstractEngine<'Text, 'Label, 'Addon, 'Arg> =
    | Print of 'Text * (unit -> AbstractEngine<'Text, 'Label, 'Addon, 'Arg>)
    | Choices of 'Text * string list * (int -> AbstractEngine<'Text, 'Label, 'Addon, 'Arg>)
    | End
    | AddonAct of 'Addon * ('Arg -> AbstractEngine<'Text, 'Label, 'Addon, 'Arg>)
    | NextState of State<'Text, 'Label, 'Addon> * (unit -> AbstractEngine<'Text, 'Label, 'Addon, 'Arg>)
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module AbstractEngine =
    type CustomStatementHandle<'Text,'Label,'CustomStatement, 'CustomStatementArg> =
        State<'Text,'Label,'CustomStatement> -> BlockStack<'Text,'Label,'CustomStatement> -> 'CustomStatementArg -> 'CustomStatement -> (State<'Text,'Label,'CustomStatement> -> AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>) -> AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>

    type CustomStatementRestore<'Text,'Label,'CustomStatement> =
        int -> 'CustomStatement -> Result<Block<'Text,'Label,'CustomStatement>, string>

    val next:
        stack: BlockStack<'Text,'Label,'Addon> ->
        state: State<'Text,'Label,'Addon> ->
        continues: (State<'Text,'Label,'Addon> -> AbstractEngine<'Text,'Label,'Addon,'CustomStatementArg>) ->
        AbstractEngine<'Text,'Label,'Addon,'CustomStatementArg>

    val down:
        subIndex: int ->
        block: Block<'Text,'Label,'Addon> ->
        stack: BlockStack<'Text,'Label,'Addon> ->
        state: State<'Text,'Label,'Addon> ->
        continues: (State<'Text,'Label,'Addon> -> AbstractEngine<'Text, 'Label, 'Addon, 'Arg>) ->
        AbstractEngine<'Text, 'Label, 'Addon, 'Arg>

    val interp:
      addon: CustomStatementHandle<'Text,'Label,'CustomStatement, 'CustomStatementArg> *
      handleCustomStatement: CustomStatementRestore<'Text,'Label,'CustomStatement> ->
        scenario: Scenario<'Text,'Label,'CustomStatement> ->
        state: State<'Text,'Label,'CustomStatement> ->
        Result<AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>,string>
        when 'Label: comparison
