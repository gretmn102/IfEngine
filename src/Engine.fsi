module IfEngine.Engine
open IfEngine.SyntaxTree

[<RequireQualifiedAccess>]
type InputMsg<'CustomStatementArg> =
    | Next
    | Choice of int
    | HandleCustomStatement of 'CustomStatementArg

type CustomStatementTransformer<'CustomStatementInput,'CustomStatementOutput> =
    'CustomStatementInput -> 'CustomStatementOutput

[<RequireQualifiedAccess>]
type OutputMsg<'Content,'CustomStatement> =
    | Print of 'Content
    | Choices of 'Content * string list
    | End
    | CustomStatement of 'CustomStatement

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module OutputMsg =
    val ofAbstractEngine:
        customStatementTransformer: CustomStatementTransformer<'CustomStatement, 'CustomStatementOutput> ->
        abstractEngine: IfEngine.AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg> ->
            OutputMsg<'Content,'CustomStatementOutput>

type CustomStatementHandler<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput> =
    {
        Handle: AbstractEngine.CustomStatementHandle<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg>
        RestoreBlockFromStack: AbstractEngine.CustomStatementRestore<'Content,'Label,'VarsContainer,'CustomStatement>
        Transformer: ('CustomStatement -> 'CustomStatementOutput)
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module CustomStatementHandler =
    val empty:
      CustomStatementHandler<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>

type Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput> =
    {
        AbstractEngine:
            IfEngine.AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg>
        GameState: IfEngine.State<'Content, 'Label, 'VarsContainer>
        CustomStatementTransformer:
            CustomStatementTransformer<'CustomStatement,'CustomStatementOutput>
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Engine =
    val nextState:
       abstractEngine: AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg> ->
       engine        : Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>
                    -> Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>

    val create:
        customStatementHandler: CustomStatementHandler<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput> ->
        scenario: Scenario<'Content,'Label,'VarsContainer,'CustomStatement> ->
        gameState: IfEngine.State<'Content, 'Label, 'VarsContainer> ->
        Result<Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>,string> when 'Label: comparison

    val getCurrentOutputMsg:
        engine: Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput> ->
        OutputMsg<'Content,'CustomStatementOutput>

    val update:
        msg: InputMsg<'CustomStatementArg> ->
        engine: Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput> ->
        Result<Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>,string>
