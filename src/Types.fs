module IfEngine.Types

type Var =
    | String of string
    | Bool of bool
    | Num of int

type Vars = Map<string, Var>

type Stmt<'Text, 'LabelName, 'Addon> =
    | Say of 'Text
    | Jump of 'LabelName
    | Menu of 'Text * (string * Stmt<'Text, 'LabelName, 'Addon> list) list
    | If of (Vars -> bool) * Stmt<'Text, 'LabelName, 'Addon> list * Stmt<'Text, 'LabelName, 'Addon> list
    | ChangeVars of (Vars -> Vars)
    | Addon of 'Addon

type Label<'Text, 'LabelName, 'Addon> = 'LabelName * Stmt<'Text, 'LabelName, 'Addon> list

type Scenario<'Text, 'LabelName, 'Addon> when 'LabelName : comparison =
    Map<'LabelName, Label<'Text, 'LabelName, 'Addon>>
