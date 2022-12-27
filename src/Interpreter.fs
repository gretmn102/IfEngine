module IfEngine.Interpreter
open FsharpMyExtension.ListZipper

open IfEngine.Types

type State<'Text, 'LabelName, 'Addon> =
    {
        LabelState: ListZ<Stmt<'Text, 'LabelName, 'Addon>> list
        Vars: Vars
    }

type Command<'Text, 'LabelName, 'Addon, 'Arg> =
    | Print of 'Text * (unit -> Command<'Text, 'LabelName, 'Addon, 'Arg>)
    | Choices of 'Text * string list * (int -> Command<'Text, 'LabelName, 'Addon, 'Arg>)
    | End
    | AddonAct of 'Addon * ('Arg -> Command<'Text, 'LabelName, 'Addon, 'Arg>)
    | NextState of State<'Text, 'LabelName, 'Addon>

let interp addon (scenario: Scenario<'Text, 'LabelName, 'Addon>) (state: State<'Text, 'LabelName, 'Addon>) =
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
