module IfEngine.Game

type State<'Text,'Label,'CustomStatement,'Arg> =
    {
        Game: IfEngine.AbstractEngine<'Text,'Label,'CustomStatement,'Arg>
        GameState: IfEngine.State<'Text,'Label,'CustomStatement>
        SavedGameState: IfEngine.State<'Text,'Label,'CustomStatement>
    }

type Msg<'CustomStatement,'CustomStatementArg> =
    | Next
    | Choice of int
    | HandleCustomState of ('CustomStatement -> 'CustomStatementArg)
    | Save
    | Load
    | NewGame

val update:
    interp: (IfEngine.State<'Text,'Label,'CustomStatement> -> IfEngine.AbstractEngine<'Text,'Label,'CustomStatement,'Arg>) ->
    scenarioInit: IfEngine.State<'Text,'Label,'CustomStatement> ->
    msg: Msg<'CustomStatement,'Arg> ->
    state: State<'Text,'Label,'CustomStatement,'Arg> ->
    State<'Text,'Label,'CustomStatement,'Arg>
