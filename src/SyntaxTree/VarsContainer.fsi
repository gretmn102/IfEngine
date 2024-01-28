namespace IfEngine.SyntaxTree

[<RequireQualifiedAccess>]
type Var<'Custom> =
    | String of string
    | Bool of bool
    | Num of int
    | Custom of 'Custom

type VarsContainer<'Custom> = Map<string, Var<'Custom>>
type VarsContainer = Map<string, Var<unit>>

type IVar<'Custom, 'Value> =
    abstract Get: VarsContainer<'Custom> -> 'Value
    abstract Set: 'Value -> VarsContainer<'Custom> -> VarsContainer<'Custom>
    abstract Update: ('Value -> 'Value) -> VarsContainer<'Custom> -> VarsContainer<'Custom>
    abstract GetVarName: unit -> string

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Var =
    val get:
        var: #IVar<'Custom, 'Value> ->
        varsContainer: VarsContainer<'Custom> ->
            'Value

    val set:
        var: #IVar<'Custom, 'Value> ->
        newValue: 'Value ->
        varsContainer: VarsContainer<'Custom> ->
            VarsContainer<'Custom>

    val update:
        var: #IVar<'Custom, 'Value> ->
        mapping: ('Value -> 'Value) ->
        varsContainer: VarsContainer<'Custom> ->
            VarsContainer<'Custom>

    val equals:
        var: 'Var ->
        otherValue: 'Value ->
        varsContainer: VarsContainer<'Custom> -> bool
        when 'Var :> IVar<'Custom, 'Value> and 'Value : equality

    val getVarName: var: #IVar<'Custom, 'Value> -> string

type NumVar<'Custom> =
    interface IVar<'Custom, int>
    new: varName: string -> NumVar<'Custom>

type NumVar = NumVar<unit>

type StringVar<'Custom> =
    interface IVar<'Custom, string>
    new: varName: string -> StringVar<'Custom>

type StringVar = StringVar<unit>

type BoolVar<'Custom> =
    interface IVar<'Custom, bool>
    new: varName: string -> BoolVar<'Custom>

type BoolVar = BoolVar<unit>

type EnumVar<'Custom, 'T when 'T: enum<int32>> =
    interface IVar<'Custom, 'T>
    new: varName: string -> EnumVar<'Custom, 'T>

type EnumVar<'T when 'T: enum<int32>> = EnumVar<unit, 'T>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module VarsContainer =
    val empty: VarsContainer<'Custom>

    val createNum: varName: string -> NumVar<'Custom>

    val createEnum: varName: string -> EnumVar<'Custom, 'T> when 'T: enum<int32>

    val createString: varName: string -> StringVar<'Custom>

    val createBool: varName: string -> BoolVar<'Custom>
