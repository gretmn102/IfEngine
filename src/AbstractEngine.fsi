namespace IfEngine
open IfEngine.SyntaxTree

[<RequireQualifiedAccess>]
type AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'Arg> =
    | Print of 'Content * (unit -> AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'Arg>)
    | Choices of 'Content * string list * (int -> AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'Arg>)
    | End
    | AddonAct of 'CustomStatement * ('Arg -> AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'Arg>)
    | NextState of State<'Content, 'Label, 'VarsContainer> * (unit -> AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'Arg>)
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module AbstractEngine =
    type CustomStatementHandle<'Content,'Label,'VarsContainer,'CustomStatement,'CustomStatementArg> =
        (State<'Content, 'Label, 'VarsContainer> ->
        BlockStack<'Content,'Label,'VarsContainer,'CustomStatement> ->
        'CustomStatementArg ->
        'CustomStatement ->
        (State<'Content, 'Label, 'VarsContainer> -> AbstractEngine<'Content,'Label,'VarsContainer,'CustomStatement,'CustomStatementArg>) ->
            AbstractEngine<'Content,'Label,'VarsContainer,'CustomStatement,'CustomStatementArg>)

    type CustomStatementRestore<'Content,'Label,'VarsContainer,'CustomStatement> =
        int -> 'CustomStatement -> Result<Block<'Content,'Label,'VarsContainer,'CustomStatement>, string>

    val next:
        stack: BlockStack<'C,'L,'V,'CS> ->
        state: State<'C,'L,'V> ->
        continues: (State<'C,'L,'V> -> AbstractEngine<'C,'L,'V,'CS,'CSA>) ->
            AbstractEngine<'C,'L,'V,'CS,'CSA>

    val down:
        subIndex: int ->
        block: Block<'C,'L,'V,'CS> ->
        stack: BlockStack<'C,'L,'V,'CS> ->
        state: State<'C,'L,'V> ->
        continues: (State<'C,'L,'V> -> AbstractEngine<'C, 'L,'V,'CS, 'CSA>) ->
            AbstractEngine<'C, 'L, 'V, 'CS, 'CSA>

    val create:
        addon: CustomStatementHandle<'C,'L,'V,'CS,'CSA> *
            handleCustomStatement: CustomStatementRestore<'C,'L,'V,'CS> ->
        scenario: Scenario<'C,'L,'V,'CS> ->
        state: State<'C,'L,'V> ->
            Result<AbstractEngine<'C,'L,'V,'CS,'CSA>,string>
            when 'L: comparison
