module IfEngine.Utils
open IfEngine.Types

val label:
    labelName: 'LabelName ->
    stmts: Stmt<'Text,'LabelName,'Addon> list ->
    Label<'Text,'LabelName,'Addon>

val jump: labelName: 'LabelName -> Stmt<'a,'LabelName,'b>

val choice:
    caption: string ->
    body: Stmt<'Text,'LabelName,'Addon> list ->
    string * Stmt<'Text,'LabelName,'Addon> list

val menu:
    caption: 'a ->
    xs: Choices<'a,'b,'c> -> Stmt<'a,'b,'c>

val if':
    pred: (Vars -> bool) ->
    thenBody: Block<'a,'b,'c> ->
    elseBody: Block<'a,'b,'c> -> Stmt<'a,'b,'c>

val createNumVar:
    varName: string ->
    value: int ->
    vars: Map<string,Var> ->
    (Map<string,Var> -> int) *
    ((int -> int) -> Stmt<'a,'b,'c>) *
    Map<string,Var>

