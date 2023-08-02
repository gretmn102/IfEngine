module IfEngine.Types

type Var =
    | String of string
    | Bool of bool
    | Num of int

type Vars = Map<string, Var>

type Block<'Text, 'Label, 'Addon> = Stmt<'Text, 'Label, 'Addon> list
and Choice<'Text, 'Label, 'Addon> = string * Block<'Text, 'Label, 'Addon>
and Choices<'Text, 'Label, 'Addon> = Choice<'Text, 'Label, 'Addon> list
and Stmt<'Text, 'Label, 'Addon> =
    | Say of 'Text
    | Jump of 'Label
    | Menu of 'Text * Choices<'Text, 'Label, 'Addon>
    | If of (Vars -> bool) * Block<'Text, 'Label, 'Addon> * Block<'Text, 'Label, 'Addon>
    | ChangeVars of (Vars -> Vars)
    | Addon of 'Addon

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Stmt =
    let equals customEquals (leftStatement: Stmt<'Text, 'Label, 'Addon>) (rightStatement: Stmt<'Text, 'Label, 'Addon>) =
        let rec f x =
            let blockEquals (l: Block<'Text, 'Label, 'Addon>) (r: Block<'Text, 'Label, 'Addon>) =
                l.Length = r.Length
                && List.forall2 (fun l r -> f (l, r)) l r

            match x with
            | Say l, Say r ->
                l = r
            | Jump l, Jump r ->
                l = r
            | Menu(lDesc, lChoices), Menu(rDesc, rChoices) ->
                let choicesEquals (l: Choices<'Text, 'Label, 'Addon>) (r: Choices<'Text, 'Label, 'Addon>) =
                    l.Length = r.Length
                    && List.forall2
                        (fun ((lDesc, lBlock): Choice<'Text, 'Label, 'Addon>) ((rDesc, rBlock): Choice<'Text, 'Label, 'Addon>) ->
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

type Label<'Text, 'Label, 'Addon> = 'Label * Block<'Text, 'Label, 'Addon>

type Scenario<'Text, 'Label, 'Addon> when 'Label : comparison =
    Map<'Label, Label<'Text, 'Label, 'Addon>>
