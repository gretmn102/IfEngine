module IfEngine.Engine
[<RequireQualifiedAccess>]
type InputMsg<'CustomStatementArg> =
    | Next
    | Choice of int
    | HandleCustomStatement of 'CustomStatementArg

type CustomStatementTransformer<'CustomStatementInput, 'CustomStatementOutput> =
    'CustomStatementInput -> 'CustomStatementOutput

[<RequireQualifiedAccess>]
type OutputMsg<'Content, 'CustomStatement> =
    | Print of 'Content
    | Choices of 'Content * (int * string) list
    | End
    | CustomStatement of 'CustomStatement
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module OutputMsg =
    let ofAbstractEngine
        (customStatementTransformer: CustomStatementTransformer<'CustomStatement, 'CustomStatementOutput>)
        (abstractEngine: AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg>) =

        match abstractEngine with
        | AbstractEngine.Print(text, _) ->
            OutputMsg.Print text
        | AbstractEngine.Choices(title, choices, _) ->
            OutputMsg.Choices(title, choices)
        | AbstractEngine.AddonAct(x, _) ->
            OutputMsg.CustomStatement(customStatementTransformer x)
        | AbstractEngine.End ->
            OutputMsg.End
        | AbstractEngine.NextState(_, _) ->
            failwith "NextState is not implemented"

type CustomStatementHandler<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput> =
    {
        Handle: AbstractEngine.CustomStatementHandle<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg>
        RestoreBlockFromStack: AbstractEngine.CustomStatementRestore<'Content,'Label,'VarsContainer,'CustomStatement>
        Transformer: 'CustomStatement -> 'CustomStatementOutput
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module CustomStatementHandler =
    let empty : CustomStatementHandler<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput> =
        {
            Handle =
                fun state blockStack customStatementArg customStatement continues ->
                    failwithf "CustomStatementHandler.Handle not implemented"
            RestoreBlockFromStack =
                fun index customStatementArg ->
                    failwithf "CustomStatementHandler.RestoreBlockFromStack not implemented"
            Transformer =
                fun customStatement ->
                    failwithf "CustomStatementHandler.Transformer Not implemented"
        }

type Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput> =
    {
        AbstractEngine: AbstractEngine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg>
        GameState: State<'Content, 'Label, 'VarsContainer>
        CustomStatementTransformer: CustomStatementTransformer<'CustomStatement, 'CustomStatementOutput>
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Engine =
    let nextState
        abstractEngine
        (engine: Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>) =

        let rec nextState gameState = function
            | AbstractEngine.NextState(newGameState, next) ->
                nextState newGameState (next ())
            | abstractEngine ->
                { engine with
                    GameState = gameState
                    AbstractEngine = abstractEngine
                }

        nextState engine.GameState abstractEngine

    let create
        (customStatementHandler: CustomStatementHandler<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>)
        (scenario: SyntaxTree.Scenario<'Content,'Label,'VarsContainer,'CustomStatement>)
        (gameState: State<'Content, 'Label, 'VarsContainer>)
        : Result<Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>, string> =

        AbstractEngine.create
            (customStatementHandler.Handle, customStatementHandler.RestoreBlockFromStack)
            scenario
            gameState
        |> Result.map (fun abstractEngine ->
            {
                AbstractEngine =
                    abstractEngine
                GameState =
                    gameState
                CustomStatementTransformer =
                    customStatementHandler.Transformer
            }
            |> nextState abstractEngine
        )

    let getCurrentOutputMsg
        (engine: Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>)
        : OutputMsg<'Content, 'CustomStatementOutput> =

        OutputMsg.ofAbstractEngine engine.CustomStatementTransformer engine.AbstractEngine

    let update
        (msg: InputMsg<'CustomStatementArg>)
        (engine: Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>)
        : Result<Engine<'Content, 'Label, 'VarsContainer, 'CustomStatement, 'CustomStatementArg, 'CustomStatementOutput>, string> =

        let nextState abstractEngine = nextState abstractEngine engine |> Ok

        match msg with
        | InputMsg.Next ->
            match engine.AbstractEngine with
            | AbstractEngine.Print(_, f) ->
                nextState (f ())
            | AbstractEngine.NextState(x, _) ->
                failwith "nextNextState"
            | AbstractEngine.End
            | AbstractEngine.Choices _
            | AbstractEngine.AddonAct _ ->
                Error (sprintf "Next —> Print but %A" engine.AbstractEngine)

        | InputMsg.Choice i ->
            match engine.AbstractEngine with
            | AbstractEngine.Choices(_, _, f)->
                nextState (f i)
            | AbstractEngine.Print(_, f) ->
                nextState (f ())
            | AbstractEngine.NextState(x, _) ->
                failwith "choiceNextState"
            | AbstractEngine.End
            | AbstractEngine.AddonAct _ ->
                Error (sprintf "Next —> Choices but %A" engine.AbstractEngine)

        | InputMsg.HandleCustomStatement customStatementArg ->
            match engine.AbstractEngine with
            | AbstractEngine.AddonAct(_, f) ->
                f customStatementArg
                |> nextState
            | x ->
                Error (sprintf "Next —> CustomStatement but %A" x)
