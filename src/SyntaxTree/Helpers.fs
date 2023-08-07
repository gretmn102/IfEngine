module IfEngine.SyntaxTree.Helpers
open IfEngine.SyntaxTree

let label (labelName: 'Label) (stmts: Stmt<'Text, 'Label, 'CustomStatement> list) =
    labelName, stmts
    : NamedBlock<'Text, 'Label, 'CustomStatement>

let jump (labelName: 'Label) =
    Jump labelName

let choice (caption: string) (body: Stmt<'Text, 'Label, 'CustomStatement> list) = caption, body

let menu caption xs = Menu(caption, xs)

let if' pred thenBody elseBody =
    If(pred, thenBody, elseBody)

let switch
    (thenBodies: ((VarsContainer -> bool) * Block<'Text,'Label,'CustomStatement>) list)
    (elseBody: Block<'Text,'Label,'CustomStatement>) =

    List.foldBack
        (fun ((pred: VarsContainer -> bool), (thenBody: Block<'Text,'Label,'CustomStatement>)) elseBody ->
            [If(pred, thenBody, elseBody)]
        )
        thenBodies
        elseBody

let assign (var: #IVar<_>) newValue =
    ChangeVars (Var.set var newValue)

let (:=) (var: #IVar<_>) newValue =
    assign var newValue

let equals (var: #IVar<_>) otherValue varsContainer =
    Var.equals var otherValue varsContainer

let (==) (var: #IVar<_>) otherValue varsContainer =
    Var.equals var otherValue varsContainer

let update (var: #IVar<_>) mapping =
    ChangeVars (Var.update var mapping)
