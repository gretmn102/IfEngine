module IfEngine.Types

type Var =
    | String of string
    | Bool of bool
    | Num of int

type Vars = Map<string,Var>

type Block<'Text,'Label,'Addon> = Stmt<'Text,'Label,'Addon> list

and Choice<'Text,'Label,'Addon> = string * Block<'Text,'Label,'Addon>

and Choices<'Text,'Label,'Addon> = Choice<'Text,'Label,'Addon> list

and Stmt<'Text,'Label,'Addon> =
    | Say of 'Text
    | Jump of 'Label
    | Menu of 'Text * Choices<'Text,'Label,'Addon>
    | If of (Vars -> bool) * Block<'Text,'Label,'Addon> * Block<'Text,'Label,'Addon>
    | ChangeVars of (Vars -> Vars)
    | Addon of 'Addon

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Stmt =
    val equals:
      customEquals: ('Addon -> 'Addon -> bool) ->
        leftStatement: Stmt<'Text,'Label,'Addon> ->
        rightStatement: Stmt<'Text,'Label,'Addon> -> bool
        when 'Text: equality and 'Label: equality

type Label<'Text,'Label,'Addon> =
    'Label * Block<'Text,'Label,'Addon>

type Scenario<'Text,'Label,'Addon when 'Label: comparison> =
    Map<'Label,Label<'Text,'Label,'Addon>>
