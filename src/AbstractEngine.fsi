namespace IfEngine
open IfEngine.Types

[<RequireQualifiedAccess>]
type AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg> =
    | Print of 'Text * (unit -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>)
    | Choices of 'Text * string list * (int -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>)
    | End
    | AddonAct of 'Addon * ('Arg -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>)
    | NextState of State<'Text, 'LabelName, 'Addon> * (unit -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>)
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module AbstractEngine =
    type CustomStatementHandle<'Text,'Label,'CustomStatement, 'CustomStatementArg> =
        State<'Text,'Label,'CustomStatement> -> BlockStack<'Text,'Label,'CustomStatement> -> 'CustomStatementArg -> 'CustomStatement -> (State<'Text,'Label,'CustomStatement> -> AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>) -> AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>

    type CustomStatementRestore<'Text,'Label,'CustomStatement> =
        int -> 'CustomStatement -> Result<Block<'Text,'Label,'CustomStatement>, string>

    val next:
        stack: BlockStack<'Text,'LabelName,'Addon> ->
        state: State<'Text,'LabelName,'Addon> ->
        continues: (State<'Text,'LabelName,'Addon> -> AbstractEngine<'Text,'LabelName,'Addon,'CustomStatementArg>) ->
        AbstractEngine<'Text,'LabelName,'Addon,'CustomStatementArg>

    val down:
        subIndex: int ->
        block: Block<'Text,'LabelName,'Addon> ->
        stack: BlockStack<'Text,'LabelName,'Addon> ->
        state: State<'Text,'LabelName,'Addon> ->
        continues: (State<'Text,'LabelName,'Addon> -> AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>) ->
        AbstractEngine<'Text, 'LabelName, 'Addon, 'Arg>

    val interp:
      addon: CustomStatementHandle<'Text,'Label,'CustomStatement, 'CustomStatementArg> *
      handleCustomStatement: CustomStatementRestore<'Text,'Label,'CustomStatement> ->
        scenario: Scenario<'Text,'Label,'CustomStatement> ->
        state: State<'Text,'Label,'CustomStatement> ->
        Result<AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>,string>
        when 'Label: comparison
