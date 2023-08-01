module IfEngine.Engine
[<RequireQualifiedAccess>]
type InputMsg<'CustomStatementArg> =
    | Next
    | Choice of int
    | HandleCustomStatement of 'CustomStatementArg

[<RequireQualifiedAccess>]
type OutputMsg<'Text, 'CustomStatement> =
    | Print of 'Text
    | Choices of 'Text * string list
    | End
    | CustomStatement of 'CustomStatement
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module OutputMsg =
    let ofAbstractEngine (abstractEngine: AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>) =
        match abstractEngine with
        | AbstractEngine.Print(text, _) ->
            OutputMsg.Print text
        | AbstractEngine.Choices(title, choices, _) ->
            OutputMsg.Choices(title, choices)
        | AbstractEngine.AddonAct(x, _) ->
            OutputMsg.CustomStatement(x)
        | AbstractEngine.End ->
            OutputMsg.End
        | AbstractEngine.NextState(_, _) ->
            failwith "NextState is not implemented"

type CustomStatementHandler<'Text, 'Label, 'CustomStatement, 'CustomStatementArg> =
    {
        Handle: State<'Text,'Label,'CustomStatement> -> BlockStack<'Text,'Label,'CustomStatement> -> 'CustomStatementArg -> 'CustomStatement -> (State<'Text,'Label,'CustomStatement> -> AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>) -> AbstractEngine<'Text,'Label,'CustomStatement,'CustomStatementArg>
        RestoreBlockFromStack: int -> 'CustomStatement -> Result<Types.Block<'Text,'Label,'CustomStatement>, string>
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module CustomStatementHandler =
    let empty : CustomStatementHandler<'Text, 'Label, 'CustomStatement, 'CustomStatementArg> =
        {
            Handle =
                fun state blockStack customStatementArg customStatement continues ->
                    failwithf "CustomStatementHandler.Handle not implemented"
            RestoreBlockFromStack =
                fun index customStatementArg ->
                    failwithf "CustomStatementHandler.RestoreBlockFromStack not implemented"
        }

type Engine<'Text, 'Label, 'CustomStatement, 'CustomStatementArg> =
    {
        AbstractEngine: AbstractEngine<'Text, 'Label, 'CustomStatement, 'CustomStatementArg>
        GameState: State<'Text, 'Label, 'CustomStatement>
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Engine =
    let create
        (customStatementHandler: CustomStatementHandler<'Text, 'Label, 'CustomStatement, 'CustomStatementArg>)
        (scenario: Types.Scenario<'Text, 'Label, 'CustomStatement>)
        (gameState: State<'Text, 'Label, 'CustomStatement>)
        : Result<Engine<'Text, 'Label, 'CustomStatement, 'CustomStatementArg>, string> =

        AbstractEngine.interp
            (customStatementHandler.Handle, customStatementHandler.RestoreBlockFromStack)
            scenario
            gameState
        |> Result.map (fun abstractEngine ->
            {
                AbstractEngine =
                    abstractEngine
                GameState =
                    gameState
            }
        )

    let getCurrentOutputMsg (engine: Engine<'Text, 'Label, 'CustomStatement, 'CustomStatementArg>) : OutputMsg<'Text, 'CustomStatement> =
        OutputMsg.ofAbstractEngine engine.AbstractEngine

    let update
        (msg: InputMsg<'CustomStatementArg>)
        (engine: Engine<'Text, 'Label, 'CustomStatement, 'CustomStatementArg>)
        : Result<Engine<'Text, 'Label, 'CustomStatement, 'CustomStatementArg>, string> =

        let nextState abstractEngine =
            let rec nextState gameState = function
                | AbstractEngine.NextState(newGameState, next) ->
                    nextState newGameState (next ())
                | abstractEngine ->
                    { engine with
                        GameState = gameState
                        AbstractEngine = abstractEngine
                    }

            nextState engine.GameState abstractEngine |> Ok

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
