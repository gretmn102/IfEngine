module IfEngine.Types

type Var =
    | String of string
    | Bool of bool
    | Num of int

type Vars = Map<string, Var>

type Block<'Text, 'Label, 'CustomStatement> = Stmt<'Text, 'Label, 'CustomStatement> list
and Choice<'Text, 'Label, 'CustomStatement> = string * Block<'Text, 'Label, 'CustomStatement>
and Choices<'Text, 'Label, 'CustomStatement> = Choice<'Text, 'Label, 'CustomStatement> list
and Stmt<'Text, 'Label, 'CustomStatement> =
    | Say of 'Text
    | Jump of 'Label
    | Menu of 'Text * Choices<'Text, 'Label, 'CustomStatement>
    | If of (Vars -> bool) * Block<'Text, 'Label, 'CustomStatement> * Block<'Text, 'Label, 'CustomStatement>
    | ChangeVars of (Vars -> Vars)
    | Addon of 'CustomStatement

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Stmt =
    let equals customEquals (leftStatement: Stmt<'Text, 'Label, 'CustomStatement>) (rightStatement: Stmt<'Text, 'Label, 'CustomStatement>) =
        let rec f x =
            let blockEquals (l: Block<'Text, 'Label, 'CustomStatement>) (r: Block<'Text, 'Label, 'CustomStatement>) =
                l.Length = r.Length
                && List.forall2 (fun l r -> f (l, r)) l r

            match x with
            | Say l, Say r ->
                l = r
            | Jump l, Jump r ->
                l = r
            | Menu(lDesc, lChoices), Menu(rDesc, rChoices) ->
                let choicesEquals (l: Choices<'Text, 'Label, 'CustomStatement>) (r: Choices<'Text, 'Label, 'CustomStatement>) =
                    l.Length = r.Length
                    && List.forall2
                        (fun ((lDesc, lBlock): Choice<'Text, 'Label, 'CustomStatement>) ((rDesc, rBlock): Choice<'Text, 'Label, 'CustomStatement>) ->
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

type Label<'Text, 'Label, 'CustomStatement> = 'Label * Block<'Text, 'Label, 'CustomStatement>

type Scenario<'Text, 'Label, 'CustomStatement> when 'Label : comparison =
    Map<'Label, Label<'Text, 'Label, 'CustomStatement>>
