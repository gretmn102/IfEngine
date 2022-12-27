module IfEngine.Types

type Var =
    | String of string
    | Bool of bool
    | Num of int

type Vars = Map<string, Var>

type Stmt<'LabelName, 'Addon> =
    | Say of Fable.React.ReactElement list
    | Jump of 'LabelName
    | Menu of Fable.React.ReactElement list * (string * Stmt<'LabelName, 'Addon> list) list
    | If of (Vars -> bool) * Stmt<'LabelName, 'Addon> list * Stmt<'LabelName, 'Addon> list
    | ChangeVars of (Vars -> Vars)
    | Addon of 'Addon

type Label<'LabelName, 'Addon> = 'LabelName * Stmt<'LabelName, 'Addon> list

type Scenario<'LabelName, 'Addon> when 'LabelName : comparison =
    Map<'LabelName, Label<'LabelName, 'Addon>>
