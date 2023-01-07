module IfEngine.Types

type Var =
    | String of string
    | Bool of bool
    | Num of int

type Vars = Map<string, Var>

type Block<'Text, 'LabelName, 'Addon> = Stmt<'Text, 'LabelName, 'Addon> list
and Choice<'Text, 'LabelName, 'Addon> = string * Block<'Text, 'LabelName, 'Addon>
and Choices<'Text, 'LabelName, 'Addon> = Choice<'Text, 'LabelName, 'Addon> list
and Stmt<'Text, 'LabelName, 'Addon> =
    | Say of 'Text
    | Jump of 'LabelName
    | Menu of 'Text * Choices<'Text, 'LabelName, 'Addon>
    | If of (Vars -> bool) * Block<'Text, 'LabelName, 'Addon> * Block<'Text, 'LabelName, 'Addon>
    | ChangeVars of (Vars -> Vars)
    | Addon of 'Addon

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Stmt =
    let equals customEquals (leftStatement: Stmt<'Text, 'LabelName, 'Addon>) (rightStatement: Stmt<'Text, 'LabelName, 'Addon>) =
        let rec f x =
            let blockEquals (l: Block<'Text, 'LabelName, 'Addon>) (r: Block<'Text, 'LabelName, 'Addon>) =
                l.Length = r.Length
                && List.forall2 (fun l r -> f (l, r)) l r

            match x with
            | Say l, Say r ->
                l = r
            | Jump l, Jump r ->
                l = r
            | Menu(lDesc, lChoices), Menu(rDesc, rChoices) ->
                let choicesEquals (l: Choices<'Text, 'LabelName, 'Addon>) (r: Choices<'Text, 'LabelName, 'Addon>) =
                    l.Length = r.Length
                    && List.forall2
                        (fun ((lDesc, lBlock): Choice<'Text, 'LabelName, 'Addon>) ((rDesc, rBlock): Choice<'Text, 'LabelName, 'Addon>) ->
                            lDesc = rDesc
                            && blockEquals lBlock rBlock
                        )
                        l
                        r

                lDesc = rDesc
                && choicesEquals lChoices rChoices

            | If(_, lThenBlock, lElseBlock), If(_, rThenBlock, rElseBlock) ->
                blockEquals lThenBlock rThenBlock
                && blockEquals lElseBlock rElseBlock
            | ChangeVars _, ChangeVars _ ->
                true
            | Addon l, Addon r ->
                customEquals l r
            | _ ->
                false

        f (leftStatement, rightStatement)

type Label<'Text, 'LabelName, 'Addon> = 'LabelName * Block<'Text, 'LabelName, 'Addon>

type Scenario<'Text, 'LabelName, 'Addon> when 'LabelName : comparison =
    Map<'LabelName, Label<'Text, 'LabelName, 'Addon>>
