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
        abstractEngine: IfEngine.AbstractEngine<'Content,'Label,'CustomStatement,'CustomStatementArg> ->
        OutputMsg<'Content,'CustomStatementOutput>

type CustomStatementHandler<'Content,'Label,'CustomStatement,'CustomStatementArg,'CustomStatementOutput> =
    {
        Handle: AbstractEngine.CustomStatementHandle<'Content,'Label,'CustomStatement, 'CustomStatementArg>
        RestoreBlockFromStack: AbstractEngine.CustomStatementRestore<'Content,'Label,'CustomStatement>
        Transformer: ('CustomStatement -> 'CustomStatementOutput)
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module CustomStatementHandler =
    val empty:
      CustomStatementHandler<'Content,'Label,'CustomStatement,'CustomStatementArg, 'CustomStatementOutput>

type Engine<'Content,'Label,'CustomStatement,'CustomStatementArg,'CustomStatementOutput> =
    {
        AbstractEngine:
            IfEngine.AbstractEngine<'Content,'Label,'CustomStatement,'CustomStatementArg>
        GameState: IfEngine.State<'Content,'Label>
        CustomStatementTransformer:
            CustomStatementTransformer<'CustomStatement,'CustomStatementOutput>
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Engine =
    val nextState:
       abstractEngine: AbstractEngine<'Content,'Label,'CustomStatement,'CustomStatementArg> ->
       engine        : Engine<'Content,'Label,'CustomStatement,'CustomStatementArg,'CustomStatementOutput>
                    -> Engine<'Content,'Label,'CustomStatement,'CustomStatementArg,'CustomStatementOutput>

    val create:
        customStatementHandler: CustomStatementHandler<'Content,'Label,
                                                     'CustomStatement,
                                                     'CustomStatementArg,
                                                     'CustomStatementOutput> ->
        scenario: Scenario<'Content,'Label,'CustomStatement> ->
        gameState: IfEngine.State<'Content,'Label> ->
        Result<Engine<'Content,'Label,'CustomStatement,'CustomStatementArg,
                      'CustomStatementOutput>,string> when 'Label: comparison

    val getCurrentOutputMsg:
        engine: Engine<'Content,'Label,'CustomStatement,'CustomStatementArg,'CustomStatementOutput> ->
        OutputMsg<'Content,'CustomStatementOutput>

    val update:
        msg: InputMsg<'CustomStatementArg> ->
        engine: Engine<'Content,'Label,'CustomStatement,'CustomStatementArg,
                       'CustomStatementOutput> ->
        Result<Engine<'Content,'Label,'CustomStatement,'CustomStatementArg,
                      'CustomStatementOutput>,string>
