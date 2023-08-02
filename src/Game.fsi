module IfEngine.Game

type State<'Text,'LabelName,'Addon,'Arg> =
    {
        Game: IfEngine.AbstractEngine<'Text,'LabelName,'Addon,'Arg>
        GameState: IfEngine.State<'Text,'LabelName,'Addon>
        SavedGameState: IfEngine.State<'Text,'LabelName,'Addon>
    }

type Msg<'CustomStatement,'CustomStatementArg> =
    | Next
    | Choice of int
    | HandleCustomState of ('CustomStatement -> 'CustomStatementArg)
    | Save
    | Load
    | NewGame

val update:
    interp: (IfEngine.State<'Text,'LabelName,'Addon> -> IfEngine.AbstractEngine<'Text,'LabelName,'Addon,'Arg>) ->
    scenarioInit: IfEngine.State<'Text,'LabelName,'Addon> ->
    msg: Msg<'Addon,'Arg> ->
    state: State<'Text,'LabelName,'Addon,'Arg> ->
    State<'Text,'LabelName,'Addon,'Arg>
