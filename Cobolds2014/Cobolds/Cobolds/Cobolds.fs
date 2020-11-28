module Cobolds

type ItemId = string
type Recipe = { Name:ItemId; Prod:int; Ingrs:(ItemId * int) list }
type Recipes = Map<ItemId, Recipe>

type Stock = Map<ItemId, int>
let consumeToMake (st:Stock) (trgRcp:Recipe) =
    // let trgRcp = Map.find trgName recipes
    trgRcp.Ingrs
    |> List.fold (fun st (name, countNeed) ->
            match Map.tryFind name st with
            | None -> failwithf "item '%s' not found in stock" name
            | Some countAvail ->
                let countRes = countAvail - countNeed
                if countRes < 0 then
                    failwithf "item '%s' not need '%d', but has '%d'" name countNeed countAvail
                else
                    Map.add name countRes st
        ) st

let make (st:Stock) (trgRcp:Recipe) =
    // let trgRcp = Map.find trgName recipes
    consumeToMake st trgRcp |> Map.add trgRcp.Name trgRcp.Prod
open FsharpMyExtension.Either
open FsharpMyExtension.FSharpExt

let foldEither fn (st:'State) =
    let rec f st = function
        | x::xs ->
            let y = fn (Either.get st) x
            match y with
            | Right _ -> f y xs
            | _ -> y
        | [] -> st
    f (Right st)
let makeMaybe (recipes:Recipes) (st:Stock) (trgName:ItemId) =
    let trgRcp = Map.find trgName recipes
    trgRcp.Ingrs
    |> foldEither (fun st (name, countNeed) ->
            match Map.tryFind name st with
            | None -> Left <| sprintf "item '%s' not found in stock" name
            | Some countAvail ->
                let countRes = countAvail - countNeed
                if countRes < 0 then
                    Left <| sprintf "item '%s' not need '%d', but has '%d'" name countNeed countAvail
                else
                    Map.add name countRes st
                    |> Right
        ) st
    |> Either.map (Map.add trgName trgRcp.Prod)
    : Either<_, Stock>

let avail (recipes:Recipes) (stock:Stock) =
    recipes
    |> Map.fold (fun st name recept ->
            let min =
                recept.Ingrs
                |> List.map (fun (name,count) ->
                    Map.tryFind name stock
                    |> Option.map (fun x -> x / count)
                    |> Option.defaultValue 0)
                |> List.min
            if min > 0 then (name, min) :: st
            else st
            )
        []

type Queue = Map<ItemId, int>
open FsharpMyExtension.Map
let cancel (stock:Stock) (queue:Queue) (recipe:Recipe) =
    let count =
        Map.tryFind recipe.Name queue
        |> Option.defaultWith (fun () -> failwithf "%A not found in queue" recipe.Name)
    recipe.Ingrs |> List.fold (fun st (name, x) ->
        let v = x * count
        Map.addOrMod ((+) v) v name st
    ) stock : Stock

let recipes =
    { Name = ""; Ingrs = []; Prod = 1; }


