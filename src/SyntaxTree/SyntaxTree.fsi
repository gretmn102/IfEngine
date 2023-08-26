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

type Block<'Content,'Label,'CustomStatement> =
    Stmt<'Content,'Label,'CustomStatement> list

and Choice<'Content,'Label,'CustomStatement> = string * Block<'Content,'Label,'CustomStatement>

and Choices<'Content,'Label,'CustomStatement> = Choice<'Content,'Label,'CustomStatement> list

and Stmt<'Content,'Label,'CustomStatement> =
    | Say of 'Content
    | InterpolationSay of (VarsContainer -> 'Content)
    | Jump of 'Label
    | Menu of 'Content * Choices<'Content,'Label,'CustomStatement>
    | If of (VarsContainer -> bool) * Block<'Content,'Label,'CustomStatement> * Block<'Content,'Label,'CustomStatement>
    | ChangeVars of (VarsContainer -> VarsContainer)
    | Addon of 'CustomStatement

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Choice =
    val mapLabel:
        blockMapLabel: ('a -> Block<'b,'OldLabel,'c> -> Block<'d,'NewLabel,'e>) ->
        labelMapping: 'a ->
        string * Block<'b,'OldLabel,'c> -> Choice<'d,'NewLabel,'e>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Choices =
    val mapLabel:
        blockMapLabel: ('a -> Block<'b,'OldLabel,'c> -> Block<'d,'NewLabel,'e>) ->
        labelMapping: 'a ->
        choices: Choices<'b,'OldLabel,'c> -> Choices<'d,'NewLabel,'e>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Stmt =
    val equals:
        customEquals: ('CustomStatement -> 'CustomStatement -> bool) ->
        leftStatement: Stmt<'Content,'Label,'CustomStatement> ->
        rightStatement: Stmt<'Content,'Label,'CustomStatement> -> bool
        when 'Content: equality and 'Label: equality

    val mapLabel:
        blockMapLabel: (('OldLabel -> 'NewLabel) -> Block<'a,'OldLabel,'b> -> Block<'a,'NewLabel,'b>) ->
        labelMapping: ('OldLabel -> 'NewLabel) ->
        statement: Stmt<'a,'OldLabel,'b> -> Stmt<'a,'NewLabel,'b>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Block =
    val mapLabel:
        labelMapping: ('OldLabel -> 'NewLabel) ->
        block: Block<'a,'OldLabel,'b> -> Block<'a,'NewLabel,'b>

type NamedBlock<'Content,'Label,'CustomStatement> =
    'Label * Block<'Content,'Label,'CustomStatement>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module NamedBlock =
    val mapLabel:
        labelMapping: ('OldLabel -> 'NewLabel) ->
        'OldLabel * Block<'a,'OldLabel,'b> -> NamedBlock<'a,'NewLabel,'b>

type Scenario<'Content,'Label,'CustomStatement when 'Label: comparison> =
    Map<'Label,NamedBlock<'Content,'Label,'CustomStatement>>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Scenario =
    val mapLabel:
        labelMapping: ('OldLabel -> 'NewLabel) ->
        scenario: Scenario<'a,'OldLabel,'b> -> Scenario<'a,'NewLabel,'b>
        when 'OldLabel: comparison and 'NewLabel: comparison
