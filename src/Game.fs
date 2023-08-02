module IfEngine.Game

type State<'Text, 'Label, 'CustomStatement, 'Arg> =
    {
        Game: AbstractEngine<'Text, 'Label, 'CustomStatement, 'Arg>
        GameState: State<'Text, 'Label, 'CustomStatement>

        SavedGameState: State<'Text, 'Label, 'CustomStatement>
    }

type Msg<'CustomStatement, 'CustomStatementArg> =
    | Next
    | Choice of int
    | HandleCustomState of ('CustomStatement -> 'CustomStatementArg)
    | Save
    | Load
    | NewGame

let update interp scenarioInit (msg: Msg<'CustomStatement, 'Arg>) (state: State<'Text, 'Label, 'CustomStatement, 'Arg>) =
    let nextState x =
        let rec nextState gameState = function
            | AbstractEngine.NextState(newGameState, next) ->
                nextState newGameState (next ())
            | game ->
                { state with
                    GameState = gameState
                    Game = game }
        nextState state.GameState x

    match msg with
    | Next ->
        match state.Game with
        | AbstractEngine.Print(_, f) ->
            nextState (f ())
        | AbstractEngine.NextState(x, _) ->
            failwith "nextNextState"
        | AbstractEngine.End
        | AbstractEngine.Choices _
        | AbstractEngine.AddonAct _ ->
            state
    | Choice i ->
        match state.Game with
        | AbstractEngine.Choices(_, _, f)->
            nextState (f i)
        | AbstractEngine.Print(_, f) ->
            nextState (f ())
        | AbstractEngine.NextState(x, _) ->
            failwith "choiceNextState"
        | AbstractEngine.End
        | AbstractEngine.AddonAct _ -> state
    | HandleCustomState handler ->
        match state.Game with
        | AbstractEngine.AddonAct(customStatement, f) ->
            f (handler customStatement)
            |> nextState
        | x ->
            failwithf "Expected: AddonAct\nActual: %A" x
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
