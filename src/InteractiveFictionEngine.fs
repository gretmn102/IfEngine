module InteractiveFictionEngine

open Feliz

type Var =
    | String of string
    | Bool of bool
type Vars = Map<string, Var>

type 'LabelName Stmt =
    | Say of Fable.React.ReactElement list
    | Jump of 'LabelName
    | Menu of Fable.React.ReactElement list * (string * 'LabelName Stmt list) list
    | If of (Vars -> bool) * 'LabelName Stmt list * 'LabelName Stmt list
    | ChangeVars of (Vars -> Vars)
    | StartFoxEscapeGame of 'LabelName Stmt list * 'LabelName Stmt list
type 'LabelName Label = 'LabelName * Stmt<'LabelName> list
let label (labelName:'LabelName) (stmts:Stmt<_> list) =
    labelName, stmts
    : _ Label

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

let choice (caption:string) (body:'LabelName Stmt list) = caption, body
let menu caption xs = Menu(caption, xs)
let if' pred thenBody elseBody =
    If(pred, thenBody, elseBody)
type 'LabelName Scenario when 'LabelName : comparison =
    Map<'LabelName, Label<'LabelName>>


open FsharpMyExtension.ListZipper

type 'LabelName State =
    {
        LabelState: ListZ<'LabelName Stmt> list
        Vars: Vars
    }

type 'LabelName T =
    | Print of Fable.React.ReactElement list * (unit -> 'LabelName T)
    | Choices of Fable.React.ReactElement list * string list * (int -> 'LabelName T)
    | End
    | FoxEscapeGame of (bool -> 'LabelName T)
    | NextState of 'LabelName State

let interp (scenario:'LabelName Scenario) (state:'LabelName State) =
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
        | StartFoxEscapeGame(winBody, loseBody) ->
            FoxEscapeGame(fun isWin ->
                let f body =
                    if List.isEmpty body then
                        next id stack
                    else
                        { state with
                            LabelState =
                                ListZ.ofList body::stack }
                        |> NextState
                if isWin then
                    f winBody
                else
                    f loseBody
            )
        | ChangeVars f ->
            stack
            |> next (fun state -> { state with Vars = f state.Vars })
    | [] -> End


