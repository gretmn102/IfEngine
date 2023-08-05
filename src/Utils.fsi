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
    pred: (VarsContainer -> bool) ->
    thenBody: Block<'a,'b,'c> ->
    elseBody: Block<'a,'b,'c> -> Stmt<'a,'b,'c>

val createNumVar:
    varName: string ->
    value: int ->
    vars: Map<string,Var> ->
    (Map<string,Var> -> int) *
    ((int -> int) -> Stmt<'a,'b,'c>) *
    Map<string,Var>

val switch:
    thenBodies: list<(VarsContainer -> bool) * Block<'Text,'Label,'CustomStatement>> ->
    elseBody  : Block<'Text,'Label,'CustomStatement>
             -> Block<'Text,'Label,'CustomStatement>

/// The `:=` operator can be used instead of this function.
val assign:
    var     : 'Var ->
    newValue: 'Value
           -> Stmt<'Text,'Label,'CustomStatement>
    when 'Var :> IVar<'Value>

val (:=):
    var     : 'Var ->
    newValue: 'Value
           -> Stmt<'Text,'Label,'CustomStatement>
    when 'Var :> IVar<'Value>

/// The `==` operator can be used instead of this function.
val equals:
    var: 'Var ->
    otherValue: 'Value -> varsContainer: VarsContainer -> bool
    when 'Var :> IVar<'Value> and 'Value : equality

val (==):
    var: 'Var ->
    otherValue: 'Value -> varsContainer: VarsContainer -> bool
    when 'Var :> IVar<'Value> and 'Value: equality

val update:
    var     : 'Var ->
    mapping : ('Value -> 'Value)
           -> Stmt<'Text,'Label,'CustomStatement>
    when 'Var :> IVar<'Value>
