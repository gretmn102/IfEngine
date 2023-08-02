module IfEngine.Utils
open IfEngine.Types

val label:
    labelName: 'Label ->
    stmts: Stmt<'Text,'Label,'Addon> list ->
    Label<'Text,'Label,'Addon>

val jump: labelName: 'Label -> Stmt<'a,'Label,'b>

val choice:
    caption: string ->
    body: Stmt<'Text,'Label,'Addon> list ->
    string * Stmt<'Text,'Label,'Addon> list

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

