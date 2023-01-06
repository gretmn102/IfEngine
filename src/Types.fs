module IfEngine.Types

type Var =
    | String of string
    | Bool of bool
    | Num of int

type Vars = Map<string, Var>

type Block<'Text, 'LabelName, 'Addon> = Stmt<'Text, 'LabelName, 'Addon> list
and Stmt<'Text, 'LabelName, 'Addon> =
    | Say of 'Text
    | Jump of 'LabelName
    | Menu of 'Text * (string * Block<'Text, 'LabelName, 'Addon>) list
    | If of (Vars -> bool) * Block<'Text, 'LabelName, 'Addon> * Block<'Text, 'LabelName, 'Addon>
    | ChangeVars of (Vars -> Vars)
    | Addon of 'Addon

type Label<'Text, 'LabelName, 'Addon> = 'LabelName * Block<'Text, 'LabelName, 'Addon>

type Scenario<'Text, 'LabelName, 'Addon> when 'LabelName : comparison =
    Map<'LabelName, Label<'Text, 'LabelName, 'Addon>>
