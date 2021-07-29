module Craft.Core
open FsharpMyExtension.Either

type ItemName = string
type Ingredient = ItemName * int
type OutputCount = int
type Recipe =
    {
        /// Предмет, который можно изготовить по данному рецепту.
        ItemName:ItemName
        /// То, сколько получается в итоге предметов, если всё сделать по рецепту.
        OutputCount:OutputCount
        Ingredients:Ingredient list
    }
type Recipes = Map<ItemName, Recipe>

type Stock = Map<ItemName, int>
let consumeToMake (st:Stock) (targetRecipe:Recipe) =
    targetRecipe.Ingredients
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

let make (st:Stock) (targetRecipe:Recipe) =
    consumeToMake st targetRecipe
    |> Map.add targetRecipe.ItemName targetRecipe.OutputCount
    : Stock
let foldEither fn (st:'State) =
    let rec f st = function
        | x::xs ->
            let y = fn (Either.get st) x
            match y with
            | Right _ -> f y xs
            | _ -> y
        | [] -> st
    f (Right st)
let makeMaybe (recipes:Recipes) (st:Stock) (targetRecipeId:ItemName) =
    let targetRecipe = Map.find targetRecipeId recipes
    targetRecipe.Ingredients
    |> foldEither
        (fun st (name, countNeed) ->
            match Map.tryFind name st with
            | None -> Left <| sprintf "item '%s' not found in stock" name
            | Some countAvail ->
                let countRes = countAvail - countNeed
                if countRes < 0 then
                    Left <| sprintf "item '%s' not need '%d', but has '%d'" name countNeed countAvail
                else
                    Map.add name countRes st
                    |> Right
        )
        st
    |> Either.map (Map.add targetRecipeId targetRecipe.OutputCount)
    : Either<_, Stock>

let avail (recipes:Recipes) (stock:Stock) =
    recipes
    |> Map.fold
        (fun st name recipe ->
            match recipe.Ingredients with
            | [] -> st
            | ingrs ->
                let min =
                    ingrs
                    |> List.map (fun (name, count) ->
                        match Map.tryFind name stock with
                        | Some x -> x / count
                        | None -> 0
                    )
                    |> List.min
                if min > 0 then
                    (name, min) :: st
                else
                    st
        )
        []

type Queue = Map<ItemName, int>
module Map =
    let addOrMod key def modify db =
        Map.tryFind key db
        |> Option.map modify
        |> Option.defaultValue def
        |> fun x -> Map.add key x db

    let addOrModWith key def modify db =
        Map.tryFind key db
        |> Option.map modify
        |> Option.defaultWith def
        |> fun x -> Map.add key x db

let cancel (stock:Stock) (queue:Queue) (recipe:Recipe) =
    let count =
        Map.tryFind recipe.ItemName queue
        |> Option.defaultWith (fun () -> failwithf "%A not found in queue" recipe.ItemName)
    recipe.Ingredients
    |> List.fold
        (fun st (name, x) ->
            let v = x * count
            Map.addOrMod name v ((+) v) st
        )
        stock
    : Stock

let recipes =
    [
        ("алхимический двигатель",
         { ItemName = "алхимический двигатель"
           OutputCount = 1
           Ingredients =
                        [("электрическая штуковина", 2); ("доски", 4);
                         ("каменный блок", 2)] })
        ("бревно", { ItemName = "бревно"
                     OutputCount = 1
                     Ingredients = [] })
        ("бритва", { ItemName = "бритва"
                     OutputCount = 1
                     Ingredients = [("ветка", 2); ("кремень", 2)] })
        ("веревка", { ItemName = "веревка"
                      OutputCount = 1
                      Ingredients = [("трава", 3)] })
        ("ветка", { ItemName = "ветка"
                    OutputCount = 1
                    Ingredients = [] })
        ("громоотвод", { ItemName = "громоотвод"
                         OutputCount = 1
                         Ingredients = [("золото", 4); ("каменный блок", 1)] })
        ("деревянная броня", { ItemName = "деревянная броня"
                               OutputCount = 1
                               Ingredients = [("бревно", 8); ("веревка", 2)] })
        ("дождеметр", { ItemName = "дождеметр"
                        OutputCount = 1
                        Ingredients = [("доски", 2); ("золото", 2); ("веревка", 2)] })
        ("доски", { ItemName = "доски"
                    OutputCount = 1
                    Ingredients = [("бревно", 4)] })
        ("древесный уголь", { ItemName = "древесный уголь"
                              OutputCount = 1
                              Ingredients = [] })
        ("зимняя шапка", { ItemName = "зимняя шапка"
                           OutputCount = 1
                           Ingredients = [("шерсть бифало", 4); ("паутина", 4)] })
        ("золото", { ItemName = "золото"
                     OutputCount = 1
                     Ingredients = [] })
        ("казан",
         { ItemName = "казан"
           OutputCount = 1
           Ingredients = [("ветка", 6); ("древесный уголь", 6); ("каменный блок", 3)] })
        ("каменный блок", { ItemName = "каменный блок"
                            OutputCount = 1
                            Ingredients = [("камни", 3)] })
        ("камни", { ItemName = "камни"
                    OutputCount = 1
                    Ingredients = [] })
        ("кирка", { ItemName = "кирка"
                    OutputCount = 1
                    Ingredients = [("ветка", 2); ("кремень", 2)] })
        ("копье", { ItemName = "копье"
                    OutputCount = 1
                    Ingredients = [("веревка", 1); ("ветка", 2); ("кремень", 1)] })
        ("кострище", { ItemName = "кострище"
                       OutputCount = 1
                       Ingredients = [("бревно", 2); ("камни", 12)] })
        ("кремень", { ItemName = "кремень"
                      OutputCount = 1
                      Ingredients = [] })
        ("максимум",
         { ItemName = "максимум"
           OutputCount = 1
           Ingredients =
                        [("кострище", 1); ("улучшенная грядка", 3); ("сушилка", 3);
                         ("казан", 3); ("сундук", 4); ("холодильник", 2);
                         ("громоотвод", 1); ("термометр", 1); ("дождеметр", 1)] })
        ("минимум",
         { ItemName = "минимум"
           OutputCount = 1
           Ingredients =
                        [("казан", 1); ("холодильник", 1); ("громоотвод", 1);
                         ("сундук", 2); ("термометр", 1); ("кострище", 1)] })
        ("молот", { ItemName = "молот"
                    OutputCount = 1
                    Ingredients = [("ветка", 3); ("камни", 3); ("трава", 6)] })
        ("навоз", { ItemName = "навоз"
                    OutputCount = 1
                    Ingredients = [] })
        ("научная машина",
         { ItemName = "научная машина"
           OutputCount = 1
           Ingredients = [("золото", 1); ("бревно", 4); ("камни", 4)] })
        ("начало",
         { ItemName = "начало"
           OutputCount = 1
           Ingredients =
                        [("научная машина", 1); ("рюкзак", 1); ("молот", 1);
                         ("деревянная броня", 1); ("копье", 1);
                         ("алхимический двигатель", 1)] })
        ("паутина", { ItemName = "паутина"
                      OutputCount = 1
                      Ingredients = [] })
        ("простая грядка", { ItemName = "простая грядка"
                             OutputCount = 1
                             Ingredients = [("трава", 8); ("навоз", 4); ("бревно", 4)] })
        ("рюкзак", { ItemName = "рюкзак"
                     OutputCount = 1
                     Ingredients = [("ветка", 4); ("трава", 4)] })
        ("сундук", { ItemName = "сундук"
                     OutputCount = 1
                     Ingredients = [("доски", 3)] })
        ("сушилка",
         { ItemName = "сушилка"
           OutputCount = 1
           Ingredients = [("ветка", 3); ("древесный уголь", 2); ("веревка", 3)] })
        ("термальный камень",
         { ItemName = "термальный камень"
           OutputCount = 1
           Ingredients = [("кирка", 1); ("камни", 10); ("кремень", 3)] })
        ("термометр", { ItemName = "термометр"
                        OutputCount = 1
                        Ingredients = [("золото", 2); ("доски", 2)] })
        ("трава", { ItemName = "трава"
                    OutputCount = 1
                    Ingredients = [] })
        ("улучшенная грядка",
         { ItemName = "улучшенная грядка"
           OutputCount = 1
           Ingredients = [("трава", 10); ("навоз", 6); ("камни", 4)] })
        ("холодильник",
         { ItemName = "холодильник"
           OutputCount = 1
           Ingredients = [("золото", 2); ("шестеренки", 1); ("каменный блок", 1)] })
        ("шерсть бифало", { ItemName = "шерсть бифало"
                            OutputCount = 1
                            Ingredients = [] })
        ("шестеренки", { ItemName = "шестеренки"
                         OutputCount = 1
                         Ingredients = [] })
        ("электрическая штуковина",
         { ItemName = "электрическая штуковина"
           OutputCount = 1
           Ingredients = [("золото", 2); ("каменный блок", 1)] })

    ] |> Map.ofList
    : Recipes