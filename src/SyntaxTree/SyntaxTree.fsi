namespace IfEngine.SyntaxTree

[<RequireQualifiedAccess>]
type Var =
    | String of string
    | Bool of bool
    | Num of int

type VarsContainer = Map<string,Var>

type IVar<'Value> =
    abstract Get: VarsContainer -> 'Value

    abstract GetVarName: unit -> string

    abstract Set: 'Value -> VarsContainer -> VarsContainer

    abstract Update: ('Value -> 'Value) -> VarsContainer -> VarsContainer

module Var =
    val get: var: #IVar<'Value> -> varsContainer: VarsContainer -> 'Value

    val set:
      var: #IVar<'Value> ->
        newValue: 'Value -> varsContainer: VarsContainer -> VarsContainer

    val update:
      var: #IVar<'Value> ->
        mapping: ('Value -> 'Value) ->
        varsContainer: VarsContainer -> VarsContainer

    val equals:
        var: 'Var ->
        otherValue: 'Value ->
        varsContainer: VarsContainer -> bool
        when 'Var :> IVar<'Value> and 'Value : equality

    val getVarName: var: #IVar<'Value> -> string

type NumVar =
    interface IVar<int>
    new: varName: string -> NumVar

type StringVar =
    interface IVar<string>
    new: varName: string -> StringVar

type BoolVar =
    interface IVar<bool>
    new: varName: string -> BoolVar

type EnumVar<'T when 'T: enum<int32>> =
    interface IVar<'T>
    new: varName: string -> EnumVar<'T>

module VarsContainer =
    val empty: VarsContainer

    val createNum: varName: string -> NumVar

    val createEnum: varName: string -> EnumVar<'T> when 'T: enum<int32>

    val createString: varName: string -> StringVar

    val createBool: varName: string -> BoolVar

type Block<'Text,'Label,'CustomStatement> =
    Stmt<'Text,'Label,'CustomStatement> list

and Choice<'Text,'Label,'CustomStatement> = string * Block<'Text,'Label,'CustomStatement>

and Choices<'Text,'Label,'CustomStatement> = Choice<'Text,'Label,'CustomStatement> list

and Stmt<'Text,'Label,'CustomStatement> =
    | Say of 'Text
    | InterpolationSay of (VarsContainer -> 'Text)
    | Jump of 'Label
    | Menu of 'Text * Choices<'Text,'Label,'CustomStatement>
    | If of (VarsContainer -> bool) * Block<'Text,'Label,'CustomStatement> * Block<'Text,'Label,'CustomStatement>
    | ChangeVars of (VarsContainer -> VarsContainer)
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
