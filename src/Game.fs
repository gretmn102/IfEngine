module IfEngine.Game

type State<'Text, 'LabelName, 'Addon, 'Arg> =
    {
        Game: Interpreter.Command<'Text, 'LabelName, 'Addon, 'Arg>
        GameState: Interpreter.State<'Text, 'LabelName, 'Addon>

        SavedGameState: Interpreter.State<'Text, 'LabelName, 'Addon>
    }

type Msg =
    | Next
    | Choice of int
    | NextState
    | Save
    | Load
    | NewGame

let update interp scenarioInit (msg: Msg) (state: State<'Text, 'LabelName, 'Addon, 'Arg>) =
    let nextState x =
        let rec nextState gameState = function
            | Interpreter.NextState newGameState ->
                nextState newGameState (interp newGameState)
            | game ->
                { state with
                    GameState = gameState
                    Game = game }
        nextState state.GameState x

    match msg with
    | Next ->
        match state.Game with
        | Interpreter.Print(_, f) ->
            nextState (f ())
        | Interpreter.NextState x ->
            failwith "nextNextState"
        | Interpreter.End
        | Interpreter.Choices _
        | Interpreter.AddonAct _ ->
            state
    | Choice i ->
        match state.Game with
        | Interpreter.Choices(_, _, f)->
            nextState (f i)
        | Interpreter.Print(_, f) ->
            nextState (f ())
        | Interpreter.NextState x ->
            failwith "choiceNextState"
        | Interpreter.End
        | Interpreter.AddonAct _ -> state
    | NextState ->
        nextState state.Game
    | Save ->
        let state =
            { state with
                SavedGameState = state.GameState }
        state
    | Load ->
        let state =
            let gameState = state.SavedGameState
            { state with
                Game = interp gameState
                GameState = gameState }
        state
    | NewGame ->
        let state =
            { state with
                Game = interp scenarioInit }
        state
