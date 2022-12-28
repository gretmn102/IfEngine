module IfEngine.Utils
open IfEngine.Types

let label (labelName: 'LabelName) (stmts: Stmt<'Text, 'LabelName, 'Addon> list) =
    labelName, stmts
    : Label<'Text, 'LabelName, 'Addon>

let jump (labelName: 'LabelName) =
    Jump labelName

let choice (caption: string) (body: Stmt<'Text, 'LabelName, 'Addon> list) = caption, body

let menu caption xs = Menu(caption, xs)

let if' pred thenBody elseBody =
    If(pred, thenBody, elseBody)
