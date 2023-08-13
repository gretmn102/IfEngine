module IfEngine.SyntaxTree.Helpers
open IfEngine.SyntaxTree

val label:
    labelName: 'Label ->
    stmts: Stmt<'Content,'Label,'CustomStatement> list ->
    NamedBlock<'Content,'Label,'CustomStatement>

val jump: labelName: 'Label -> Stmt<'a,'Label,'b>

val choice:
    caption: string ->
    body: Stmt<'Content,'Label,'CustomStatement> list ->
    string * Stmt<'Content,'Label,'CustomStatement> list

val menu:
    caption: 'a ->
    xs: Choices<'a,'b,'c> -> Stmt<'a,'b,'c>

val if':
    pred: (VarsContainer -> bool) ->
    thenBody: Block<'a,'b,'c> ->
    elseBody: Block<'a,'b,'c> -> Stmt<'a,'b,'c>

val switch:
    thenBodies: list<(VarsContainer -> bool) * Block<'Content,'Label,'CustomStatement>> ->
    elseBody  : Block<'Content,'Label,'CustomStatement>
             -> Block<'Content,'Label,'CustomStatement>

/// The `:=` operator can be used instead of this function.
val assign:
    var     : 'Var ->
    newValue: 'Value
           -> Stmt<'Content,'Label,'CustomStatement>
    when 'Var :> IVar<'Value>

val (:=):
    var     : 'Var ->
    newValue: 'Value
           -> Stmt<'Content,'Label,'CustomStatement>
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
           -> Stmt<'Content,'Label,'CustomStatement>
    when 'Var :> IVar<'Value>
