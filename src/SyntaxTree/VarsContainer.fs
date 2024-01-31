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
    let get (var: #IVar<'Custom, 'Value>) (varsContainer: VarsContainer<'Custom>) : 'Value =
        var.Get varsContainer

    let set (var: #IVar<'Custom, 'Value>) newValue varsContainer =
        var.Set newValue varsContainer

    let update (var: #IVar<'Custom, 'Value>) mapping varsContainer =
        var.Update mapping varsContainer

    let equals (var: #IVar<'Custom, 'Value>) otherValue varsContainer =
        (get var varsContainer) = otherValue

    let getVarName (var: #IVar<'Custom, 'Value>) =
        var.GetVarName()

type NumVar<'Custom>(varName: string) =
    interface IVar<'Custom, int> with
        member __.GetVarName () =
            varName

        member __.Get varsContainer =
            match Map.tryFind varName varsContainer with
            | Some(Var.Num x) -> x
            | x ->
                failwithf "expected %s = Some(Num x) but %A" varName x

        member __.Set newValue varsContainer =
            Map.add varName (Var.Num newValue) varsContainer

        member this.Update mapping varsContainer =
            Var.set
                this
                (mapping (Var.get this varsContainer))
                varsContainer

type NumVar = NumVar<unit>

type StringVar<'Custom>(varName: string) =
    interface IVar<'Custom, string> with
        member __.GetVarName () =
            varName

        member __.Get varsContainer =
            match Map.tryFind varName varsContainer with
            | Some(Var.String x) -> x
            | x ->
                failwithf "expected %s = Some(String x) but %A" varName x

        member __.Set newValue varsContainer =
            Map.add varName (Var.String newValue) varsContainer

        member this.Update mapping varsContainer =
            Var.set
                this
                (mapping (Var.get this varsContainer))
                varsContainer

type StringVar = StringVar<unit>

type BoolVar<'Custom>(varName: string) =
    interface IVar<'Custom, bool> with
        member __.GetVarName () =
            varName

        member __.Get varsContainer =
            match Map.tryFind varName varsContainer with
            | Some(Var.Bool x) -> x
            | x ->
                failwithf "expected %s = Some(Bool x) but %A" varName x

        member __.Set newValue varsContainer =
            Map.add varName (Var.Bool newValue) varsContainer

        member this.Update mapping varsContainer =
            Var.set
                this
                (mapping (Var.get this varsContainer))
                varsContainer

type BoolVar = BoolVar<unit>

type EnumVar<'Custom, 'T when 'T: enum<int32>>(varName: string)  =
    interface IVar<'Custom, 'T> with
        member __.Get varsContainer =
            let enum (x: int) =
                #if FABLE_COMPILER
                unbox x : 'T
                #else
                unbox (System.Enum.ToObject(typeof<'T>, x)) : 'T
                #endif

            match Map.tryFind varName varsContainer with
            | Some(Var.Num x) -> enum x
            | x ->
                failwithf "expected %s = Some(Bool x) but %A" varName x

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

type EnumVar<'T when 'T: enum<int32>> = EnumVar<unit, 'T>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module VarsContainer =
    let empty: VarsContainer<'Custom> = Map.empty

    let createNum varName = new NumVar<'Custom>(varName)

    let createEnum varName = new EnumVar<'Custom, 'T>(varName)

    let createString varName = new StringVar<'Custom>(varName)

    let createBool varName = new BoolVar<'Custom>(varName)
