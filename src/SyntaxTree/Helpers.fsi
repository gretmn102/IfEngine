module IfEngine.SyntaxTree.Helpers
open IfEngine.SyntaxTree

val label:
    labelName: 'L ->
    stmts: Stmt<'C,'L,'V,'CS> list ->
        NamedBlock<'C,'L,'V,'CS>

val jump: labelName: 'L -> Stmt<'C,'L, 'V, 'CS>

val choice:
    caption: string ->
    body: Stmt<'C,'L,'V,'CS> list ->
        Choice<'C,'L,'V,'CS>

val menu:
    caption: 'C ->
    xs: Choices<'C,'L,'V,'CS> ->
        Stmt<'C,'L,'V,'CS>

val imenu:
    getContent: ('V -> 'C) ->
    xs: Choices<'C,'L,'V,'CS> ->
        Stmt<'C,'L,'V,'CS>

val if':
    pred: ('V -> bool) ->
    thenBody: Block<'C,'L,'V,'CS> ->
    elseBody: Block<'C,'L,'V,'CS> ->
        Stmt<'C,'L,'V,'CS>

val switch:
    thenBodies: list<('V -> bool) * Block<'C,'L,'V,'CS>> ->
    elseBody: Block<'C,'L,'V,'CS> ->
        Block<'C,'L,'V,'CS>

/// The `:=` operator can be used instead of this function.
val assign:
    var: 'Var ->
    newValue: 'Value ->
        Stmt<'C,'L,VarsContainer<'Custom> ,'CS>
        when 'Var :> IVar<'Custom, 'Value>

val (:=):
    var: 'Var ->
    newValue: 'Value ->
        Stmt<'C,'L,VarsContainer<'Custom> ,'CS>
        when 'Var :> IVar<'Custom, 'Value>

/// The `==` operator can be used instead of this function.
val equals:
    var: 'Var ->
    otherValue: 'Value ->
    varsContainer: VarsContainer<'Custom>  ->
        bool
        when 'Var :> IVar<'Custom, 'Value> and 'Value : equality

val (==):
    var: 'Var ->
    otherValue: 'Value ->
    varsContainer: VarsContainer<'Custom>  ->
        bool
        when 'Var :> IVar<'Custom, 'Value> and 'Value: equality

val update:
    var: 'Var ->
    mapping: ('Value -> 'Value) ->
        Stmt<'C,'L,VarsContainer<'Custom> ,'CS>
        when 'Var :> IVar<'Custom, 'Value>
