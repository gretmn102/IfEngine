namespace IfEngine.SyntaxTree

[<RequireQualifiedAccess>]
type Var =
    | String of string
    | Bool of bool
    | Num of int

type VarsContainer = Map<string, Var>

type IVar<'Value> =
    abstract Get: VarsContainer -> 'Value
    abstract Set: 'Value -> VarsContainer -> VarsContainer
    abstract Update: ('Value -> 'Value) -> VarsContainer -> VarsContainer
    abstract GetVarName: unit -> string

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Var =
    let get (var: #IVar<'Value>) varsContainer =
        var.Get varsContainer

    let set (var: #IVar<'Value>) newValue varsContainer =
        var.Set newValue varsContainer

    let update (var: #IVar<'Value>) mapping varsContainer =
        var.Update mapping varsContainer

    let equals (var: #IVar<'Value>) otherValue varsContainer =
        (get var varsContainer) = otherValue

    let getVarName (var: #IVar<'Value>) =
        var.GetVarName()

type NumVar(varName: string) =
    interface IVar<int> with
        member __.GetVarName () =
            varName

        member __.Get varsContainer =
            match Map.tryFind varName varsContainer with
            | Some(Var.Num x) -> x
            | _ ->
                failwithf "expected Some(Num x) but %s" varName

        member __.Set newValue varsContainer =
            Map.add varName (Var.Num newValue) varsContainer

        member this.Update mapping varsContainer =
            Var.set
                this
                (mapping (Var.get this varsContainer))
                varsContainer

type StringVar(varName: string) =
    interface IVar<string> with
        member __.GetVarName () =
            varName

        member __.Get varsContainer =
            match Map.tryFind varName varsContainer with
            | Some(Var.String x) -> x
            | _ ->
                failwithf "expected Some(String x) but %s" varName

        member __.Set newValue varsContainer =
            Map.add varName (Var.String newValue) varsContainer

        member this.Update mapping varsContainer =
            Var.set
                this
                (mapping (Var.get this varsContainer))
                varsContainer

type BoolVar(varName: string) =
    interface IVar<bool> with
        member __.GetVarName () =
            varName

        member __.Get varsContainer =
            match Map.tryFind varName varsContainer with
            | Some(Var.Bool x) -> x
            | _ ->
                failwithf "expected Some(Bool x) but %s" varName

        member __.Set newValue varsContainer =
            Map.add varName (Var.Bool newValue) varsContainer

        member this.Update mapping varsContainer =
            Var.set
                this
                (mapping (Var.get this varsContainer))
                varsContainer

type EnumVar<'T when 'T: enum<int32>>(varName: string)  =
    interface IVar<'T> with
        member __.Get varsContainer =
            let enum (x: int) =
                #if FABLE_COMPILER
                unbox x : 'T
                #else
                unbox (System.Enum.ToObject(typeof<'T>, x)) : 'T
                #endif

            match Map.tryFind varName varsContainer with
            | Some(Var.Num x) -> enum x
            | _ ->
                failwithf "expected Some(Bool x) but %s" varName

        member __.GetVarName() =
            varName

        member __.Set (newValue) varsContainer =
            let newValue = int (unbox newValue)
            Map.add varName (Var.Num newValue) varsContainer

        member this.Update mapping varsContainer =
            Var.set
                this
                (mapping (Var.get this varsContainer))
                varsContainer

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module VarsContainer =
    let empty: VarsContainer = Map.empty

    let createNum varName = new NumVar(varName)

    let createEnum varName = new EnumVar<'T>(varName)

    let createString varName = new StringVar(varName)

    let createBool varName = new BoolVar(varName)
