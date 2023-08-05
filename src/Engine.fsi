module IfEngine.Engine
open IfEngine.Types

[<RequireQualifiedAccess>]
type InputMsg<'CustomStatementArg> =
    | Next
    | Choice of int
    | HandleCustomStatement of 'CustomStatementArg

type CustomStatementTransformer<'CustomStatementInput,'CustomStatementOutput> =
    'CustomStatementInput -> 'CustomStatementOutput

[<RequireQualifiedAccess>]
type OutputMsg<'Text,'CustomStatement> =
    | Print of 'Text
    | Choices of 'Text * string list
    | End
    | CustomStatement of 'CustomStatement

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module OutputMsg =
    val ofAbstractEngine:
        customStatementTransformer: CustomStatementTransformer<'CustomStatement, 'CustomStatementOutput> ->
        abstractEngine: IfEngine.AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg> ->
        OutputMsg<'Text,'CustomStatementOutput>

type CustomStatementHandler<'Text,'Label,'CustomStatement,'CustomStatementArg,'CustomStatementOutput> =
    {
        Handle: AbstractEngine.CustomStatementHandle<'Text,'Label,'CustomStatement, 'CustomStatementArg>
        RestoreBlockFromStack: AbstractEngine.CustomStatementRestore<'Text,'Label,'CustomStatement>
        Transformer: ('CustomStatement -> 'CustomStatementOutput)
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module CustomStatementHandler =
    val empty:
      CustomStatementHandler<'Text,'Label,'CustomStatement,'CustomStatementArg, 'CustomStatementOutput>

type Engine<'Text,'Label,'CustomStatement,'CustomStatementArg,'CustomStatementOutput> =
    {
        AbstractEngine:
            IfEngine.AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>
        GameState: IfEngine.State<'Text,'Label,'CustomStatement>
        CustomStatementTransformer:
            CustomStatementTransformer<'CustomStatement,'CustomStatementOutput>
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Engine =
    val nextState:
       abstractEngine: AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg> ->
       engine        : Engine<'Text,'Label,'CustomStatement,'CustomStatementArg,'CustomStatementOutput>
                    -> Engine<'Text,'Label,'CustomStatement,'CustomStatementArg,'CustomStatementOutput>

    val create:
        customStatementHandler: CustomStatementHandler<'Text,'Label,
                                                     'CustomStatement,
                                                     'CustomStatementArg,
                                                     'CustomStatementOutput> ->
        scenario: Scenario<'Text,'Label,'CustomStatement> ->
        gameState: IfEngine.State<'Text,'Label,'CustomStatement> ->
        Result<Engine<'Text,'Label,'CustomStatement,'CustomStatementArg,
                      'CustomStatementOutput>,string> when 'Label: comparison

    val getCurrentOutputMsg:
        engine: Engine<'Text,'Label,'CustomStatement,'CustomStatementArg,'CustomStatementOutput> ->
        OutputMsg<'Text,'CustomStatementOutput>

    val update:
        msg: InputMsg<'CustomStatementArg> ->
        engine: Engine<'Text,'Label,'CustomStatement,'CustomStatementArg,
                       'CustomStatementOutput> ->
        Result<Engine<'Text,'Label,'CustomStatement,'CustomStatementArg,
                      'CustomStatementOutput>,string>
