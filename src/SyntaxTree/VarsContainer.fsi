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
