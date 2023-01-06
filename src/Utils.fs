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

let createNumVar varName value vars =
    let vars = Map.add varName (Num value) vars

    let get (vars: Map<_, _>) =
        match vars.[varName] with
        | Num x -> x
        | _ -> failwithf "expected Num _ but %s" varName

    let update fn =
        ChangeVars (fun vars ->
            match Map.tryFind varName vars with
            | Some (Num x) ->
                Map.add varName (Num (fn x)) vars
            | _ ->
                Map.add varName (Num (fn 0)) vars
        )

    get, update, vars
