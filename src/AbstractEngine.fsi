namespace IfEngine
open IfEngine.Types

[<RequireQualifiedAccess>]
type AbstractEngine<'Text, 'Label, 'CustomStatement, 'Arg> =
    | Print of 'Text * (unit -> AbstractEngine<'Text, 'Label, 'CustomStatement, 'Arg>)
    | Choices of 'Text * string list * (int -> AbstractEngine<'Text, 'Label, 'CustomStatement, 'Arg>)
    | End
    | AddonAct of 'CustomStatement * ('Arg -> AbstractEngine<'Text, 'Label, 'CustomStatement, 'Arg>)
    | NextState of State<'Text, 'Label, 'CustomStatement> * (unit -> AbstractEngine<'Text, 'Label, 'CustomStatement, 'Arg>)
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module AbstractEngine =
    type CustomStatementHandle<'Text,'Label,'CustomStatement, 'CustomStatementArg> =
        State<'Text,'Label,'CustomStatement> -> BlockStack<'Text,'Label,'CustomStatement> -> 'CustomStatementArg -> 'CustomStatement -> (State<'Text,'Label,'CustomStatement> -> AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>) -> AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>

    type CustomStatementRestore<'Text,'Label,'CustomStatement> =
        int -> 'CustomStatement -> Result<Block<'Text,'Label,'CustomStatement>, string>

    val next:
        stack: BlockStack<'Text,'Label,'CustomStatement> ->
        state: State<'Text,'Label,'CustomStatement> ->
        continues: (State<'Text,'Label,'CustomStatement> -> AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>) ->
        AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>

    val down:
        subIndex: int ->
        block: Block<'Text,'Label,'CustomStatement> ->
        stack: BlockStack<'Text,'Label,'CustomStatement> ->
        state: State<'Text,'Label,'CustomStatement> ->
        continues: (State<'Text,'Label,'CustomStatement> -> AbstractEngine<'Text, 'Label, 'CustomStatement, 'Arg>) ->
        AbstractEngine<'Text, 'Label, 'CustomStatement, 'Arg>

    val interp:
      addon: CustomStatementHandle<'Text,'Label,'CustomStatement, 'CustomStatementArg> *
      handleCustomStatement: CustomStatementRestore<'Text,'Label,'CustomStatement> ->
        scenario: Scenario<'Text,'Label,'CustomStatement> ->
        state: State<'Text,'Label,'CustomStatement> ->
        Result<AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>,string>
        when 'Label: comparison
