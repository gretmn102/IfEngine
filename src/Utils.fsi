module IfEngine.Utils
open IfEngine.Types

val label:
    labelName: 'Label ->
    stmts: Stmt<'Text,'Label,'CustomStatement> list ->
    NamedBlock<'Text,'Label,'CustomStatement>

val jump: labelName: 'Label -> Stmt<'a,'Label,'b>

val choice:
    caption: string ->
    body: Stmt<'Text,'Label,'CustomStatement> list ->
    string * Stmt<'Text,'Label,'CustomStatement> list

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

