module IfEngine.SyntaxTree.Helpers
open IfEngine.SyntaxTree

let label (labelName: 'Label) (stmts: Stmt<'Content,'Label,'VarsContainer,'CustomStatement> list) =
    labelName, stmts
    : NamedBlock<'Content,'Label,'VarsContainer,'CustomStatement>

let jump (labelName: 'Label) =
    Jump labelName

let choice (caption: string) (body: Stmt<'Content,'Label,'VarsContainer,'CustomStatement> list) =
    {
        Predicate = None
        Caption = caption
        Body = body
    }

let pchoice predicate (caption: string) (body: Stmt<'Content,'Label,'VarsContainer,'CustomStatement> list) =
    {
        Predicate = Some predicate
        Caption = caption
        Body = body
    }

let menu caption xs = Menu(caption, xs)

let imenu getContent xs = InterpolatedMenu(getContent, xs)

let if' pred thenBody elseBody =
    If(pred, thenBody, elseBody)

let switch
    (thenBodies: (('V -> bool) * Block<'C,'L,'V,'CS>) list)
    (elseBody: Block<'C,'L,'V,'CS>) =

    List.foldBack
        (fun ((pred: 'V -> bool), (thenBody: Block<'C,'L,'V,'CS>)) elseBody ->
            [If(pred, thenBody, elseBody)]
        )
        thenBodies
        elseBody

let assign (var: #IVar<_,_>) newValue =
    ChangeVars (Var.set var newValue)

let (:=) (var: #IVar<_,_>) newValue =
    assign var newValue

let equals (var: #IVar<_,_>) otherValue varsContainer =
    Var.equals var otherValue varsContainer

let (==) (var: #IVar<_,_>) otherValue varsContainer =
    Var.equals var otherValue varsContainer

let update (var: #IVar<_,_>) mapping =
    ChangeVars (Var.update var mapping)
