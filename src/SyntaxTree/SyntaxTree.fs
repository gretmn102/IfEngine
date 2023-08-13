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

type Block<'Content, 'Label, 'CustomStatement> = Stmt<'Content, 'Label, 'CustomStatement> list
and Choice<'Content, 'Label, 'CustomStatement> = string * Block<'Content, 'Label, 'CustomStatement>
and Choices<'Content, 'Label, 'CustomStatement> = Choice<'Content, 'Label, 'CustomStatement> list
and Stmt<'Content, 'Label, 'CustomStatement> =
    | Say of 'Content
    | InterpolationSay of (VarsContainer -> 'Content)
    | Jump of 'Label
    | Menu of 'Content * Choices<'Content, 'Label, 'CustomStatement>
    | If of (VarsContainer -> bool) * Block<'Content, 'Label, 'CustomStatement> * Block<'Content, 'Label, 'CustomStatement>
    | ChangeVars of (VarsContainer -> VarsContainer)
    | Addon of 'CustomStatement

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Stmt =
    let equals customEquals (leftStatement: Stmt<'Content, 'Label, 'CustomStatement>) (rightStatement: Stmt<'Content, 'Label, 'CustomStatement>) =
        let rec f x =
            let blockEquals (l: Block<'Content, 'Label, 'CustomStatement>) (r: Block<'Content, 'Label, 'CustomStatement>) =
                l.Length = r.Length
                && List.forall2 (fun l r -> f (l, r)) l r

            match x with
            | Say l, Say r ->
                l = r
            | Jump l, Jump r ->
                l = r
            | Menu(lDesc, lChoices), Menu(rDesc, rChoices) ->
                let choicesEquals (l: Choices<'Content, 'Label, 'CustomStatement>) (r: Choices<'Content, 'Label, 'CustomStatement>) =
                    l.Length = r.Length
                    && List.forall2
                        (fun ((lDesc, lBlock): Choice<'Content, 'Label, 'CustomStatement>) ((rDesc, rBlock): Choice<'Content, 'Label, 'CustomStatement>) ->
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

type NamedBlock<'Content, 'Label, 'CustomStatement> = 'Label * Block<'Content, 'Label, 'CustomStatement>

type Scenario<'Content, 'Label, 'CustomStatement> when 'Label : comparison =
    Map<'Label, NamedBlock<'Content, 'Label, 'CustomStatement>>