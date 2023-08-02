module IfEngine.Types

type Var =
    | String of string
    | Bool of bool
    | Num of int

type Vars = Map<string,Var>

type Block<'Text,'Label,'CustomStatement> = Stmt<'Text,'Label,'CustomStatement> list

and Choice<'Text,'Label,'CustomStatement> = string * Block<'Text,'Label,'CustomStatement>

and Choices<'Text,'Label,'CustomStatement> = Choice<'Text,'Label,'CustomStatement> list

and Stmt<'Text,'Label,'CustomStatement> =
    | Say of 'Text
    | Jump of 'Label
    | Menu of 'Text * Choices<'Text,'Label,'CustomStatement>
    | If of (Vars -> bool) * Block<'Text,'Label,'CustomStatement> * Block<'Text,'Label,'CustomStatement>
    | ChangeVars of (Vars -> Vars)
    | Addon of 'CustomStatement

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Stmt =
    val equals:
      customEquals: ('CustomStatement -> 'CustomStatement -> bool) ->
        leftStatement: Stmt<'Text,'Label,'CustomStatement> ->
        rightStatement: Stmt<'Text,'Label,'CustomStatement> -> bool
        when 'Text: equality and 'Label: equality

type NamedBlock<'Text,'Label,'CustomStatement> =
    'Label * Block<'Text,'Label,'CustomStatement>

type Scenario<'Text,'Label,'CustomStatement when 'Label: comparison> =
    Map<'Label,NamedBlock<'Text,'Label,'CustomStatement>>
