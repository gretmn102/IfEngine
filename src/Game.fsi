module IfEngine.Game

type State<'Text,'Label,'Addon,'Arg> =
    {
        Game: IfEngine.AbstractEngine<'Text,'Label,'Addon,'Arg>
        GameState: IfEngine.State<'Text,'Label,'Addon>
        SavedGameState: IfEngine.State<'Text,'Label,'Addon>
    }

type Msg<'CustomStatement,'CustomStatementArg> =
    | Next
    | Choice of int
    | HandleCustomState of ('CustomStatement -> 'CustomStatementArg)
    | Save
    | Load
    | NewGame

val update:
    interp: (IfEngine.State<'Text,'Label,'Addon> -> IfEngine.AbstractEngine<'Text,'Label,'Addon,'Arg>) ->
    scenarioInit: IfEngine.State<'Text,'Label,'Addon> ->
    msg: Msg<'Addon,'Arg> ->
    state: State<'Text,'Label,'Addon,'Arg> ->
    State<'Text,'Label,'Addon,'Arg>
