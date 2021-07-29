module IfEngine.Core

open Feliz

type Var =
    | String of string
    | Bool of bool
    | Num of int
type Vars = Map<string, Var>

type Stmt<'LabelName, 'Addon> =
    | Say of Fable.React.ReactElement list
    | Jump of 'LabelName
    | Menu of Fable.React.ReactElement list * (string * Stmt<'LabelName, 'Addon> list) list
    | If of (Vars -> bool) * Stmt<'LabelName, 'Addon> list * Stmt<'LabelName, 'Addon> list
    | ChangeVars of (Vars -> Vars)
    | Addon of 'Addon

type Label<'LabelName, 'Addon> = 'LabelName * Stmt<'LabelName, 'Addon> list
let label (labelName:'LabelName) (stmts:Stmt<_, _> list) =
    labelName, stmts
    : Label<_,_>

let divCenter (xs:seq<Fable.React.ReactElement>) =
    Html.div [
        prop.style [
            style.justifyContent.center
            style.display.flex
        ]

        prop.children xs
    ]

let say (txt:string) =
    Html.p [
        prop.style [
            // style.justifyContent.center
            // style.display.flex
        ]
        prop.text txt
    ]
    |> List.singleton
    |> Say
let says (xs:string list) =
    xs
    |> List.map (fun str ->
        Html.p [
            prop.style [
                // style.justifyContent.center
                // style.display.flex
            ]
            prop.children [
                Html.text str
            ]
        ]
    )
    |> Say
let jump (labelName:'LabelName) =
    Jump labelName

let choice (caption:string) (body:Stmt<'LabelName, 'Addon> list) = caption, body
let menu caption xs = Menu(caption, xs)
let if' pred thenBody elseBody =
    If(pred, thenBody, elseBody)
type Scenario<'LabelName, 'Addon> when 'LabelName : comparison =
    Map<'LabelName, Label<'LabelName, 'Addon>>


open FsharpMyExtension.ListZipper

type State<'LabelName, 'Addon> =
    {
        LabelState: ListZ<Stmt<'LabelName, 'Addon>> list
        Vars: Vars
    }

type T<'LabelName, 'Addon, 'Arg> =
    | Print of Fable.React.ReactElement list * (unit -> T<'LabelName, 'Addon, 'Arg>)
    | Choices of Fable.React.ReactElement list * string list * (int -> T<'LabelName, 'Addon, 'Arg>)
    | End
    | AddonAct of 'Addon * ('Arg -> T<'LabelName, 'Addon, 'Arg>)
    | NextState of State<'LabelName, 'Addon>

let interp addon (scenario: Scenario<'LabelName,'Addon>) (state:State<'LabelName, 'Addon>) =
    let next changeState stack =
        let rec next = function
            | x::xs ->
                match ListZ.next x with
                | Some x ->
                    { state with
                        LabelState = x::xs }
                    |> Some
                | None -> next xs
            | [] -> None
        match next stack with
        | Some state ->
            NextState (changeState state)
        | None -> End
    match state.LabelState with
    | headStack::tailStack as stack ->
        let x = ListZ.hole headStack
        match x with
        | Jump x ->
            { state with
                LabelState =
                    match snd scenario.[x] with
                    | [] -> []
                    | xs ->
                        [ ListZ.ofList xs ] }
            |> NextState
        | Say x ->
            Print(x, fun () ->
                next id stack
            )
        | Menu(caption, xs) ->
            let labels = xs |> List.map fst
            Choices(caption, labels, fun i ->
                let _, body = xs.[i]

                if List.isEmpty body then
                    next id stack
                else
                    { state with
                        LabelState =
                            ListZ.ofList body::stack }
                    |> NextState
            )
        | If(pred, thenBody, elseBody) ->
            let f body =
                if List.isEmpty body then
                    next id stack
                else
                    { state with
                        LabelState =
                            ListZ.ofList body::stack }
                    |> NextState
            if pred state.Vars then
                f thenBody
            else
                f elseBody
        | Addon addonArg ->
            AddonAct(addonArg, fun res ->
                addon next state res addonArg
            )
        | ChangeVars f ->
            stack
            |> next (fun state -> { state with Vars = f state.Vars })
    | [] -> End


