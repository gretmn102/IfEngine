module Abstract


(*
Все предметы для своего создания требуют нечто. Это может быть такое нечто, которое, как только
создастся предмет они тут же удаляются. Также и может быть и обратное. К примеру x требует для своего
создания Y одноразовых (т.е. после того, как завершится создание x, такие элементы будут удалены) и Z
многоразовых (после создания, такие элементы вернутся туда, откуда были взяты). Пример мог навести на мысль,
что одноразовость или многоразовость - свойство самого элемента, но это не так. Именно в рецепте элемента
указано то, вернется ли данный элемент после создания или нет.
Более реальный пример:
    Steel,
    { multi = [ Blacksmith, 1; Forge, 1; ]
       single = [ IronOre, 2 ]}
    Smelter,
    { single = [ Hammer, 1 ]; multi = [] } *)


let hasrecipes =
    let xs = ["steel"; "smelter"] |> Set.ofList
    (fun x -> Set.contains x xs)

module elements =
    type Prop = { Single: (string * int) list; Multi: (string * int) list}
    type Elements = Elements of Map<string, Prop>
    let initSample() =
        [( "steel", { Multi = [ "blacksmith", 1; "forge", 1; ]; Single = [ "iron ore", 2 ]});
         ( "smelter", { Single = [ "hammer", 1 ]; Multi = [] })]
        |> Map.ofList |> Elements
    let get x (Elements e) = e.[x]
    let iter f (Elements e) = Map.iter f e

let items =
    ["steel"; "blacksmith"; "forge"; "iron ore"; "smelter"; "hammer"; "mushroom"]

let unitType = set ["blacksmith"; "smelter"]
let eatType = "mushroom"

module Map =
    let incr x n p =
        let init = match Map.tryFind x p with Some curr -> curr | None -> 0
        Map.add x (n + init) p
    let decr x n s =
        let curr = Map.find x s
        let n = let n = curr - n in if n < 0 then 0 else n
        Map.add x n s

module stock =
    type Stock = Stock of Map<string, int>
    let init xs =
        let init = items |> List.map (fun x -> x, 0)
        xs |> List.fold (fun state (key, v) -> Map.add key v state ) (Map.ofList init) |> Stock
    let get x (Stock s) = s.[x]
    let incr x n (Stock p) =
        let curr = Map.find x p
        let n = curr + n
        Map.add x n p |> Stock
    let decr x n (Stock s) =
        let curr = Map.find x s
        let n = let n = curr - n in if n < 0 then 0 else n
        Map.add x n s |> Stock
    let map f (Stock s) = Map.map f s
    let iter f (Stock s) = Map.iter f s

module make =
    let recipes = elements.initSample()
    let possibleMakes x stock' =
        let r = elements.get x recipes
        let res = List.map (fun (res,count) -> stock.get res stock' / count) (r.Single @ r.Multi)
        List.min res
    let make (x, count) stock' =
        let {elements.Prop.Single = single; elements.Prop.Multi = multi} = elements.get x recipes
        let multiple = List.map (fun (res, n) -> res, n * count)
        let multi = multiple multi
        //let all = multiple single @ multi
        let foldStock = List.fold (fun state (res, count) -> stock.decr res count state ) //stock' all
        let stock' = foldStock stock' (multiple single) |> (fun state -> foldStock state multi)
        multi, stock'
    //let unmake (x, count)

module crafter =
    type OnNextTurn = { ResMakes: Map<string, int>; ResTemp: Map<string, int>}
    type State = { Stock:stock.Stock; NextTurn:OnNextTurn }
    let possibleMakes { Stock = st; } reqElement =
        if hasrecipes reqElement then make.possibleMakes reqElement st
        else 0
    let make { Stock = st; NextTurn = next } reqElem count =
#if DEBUG
        if make.possibleMakes reqElem st < count then
            failwith "запрашиваемой продукции больше чем ингредиентов для нее"
#endif
        let multi, st = make.make (reqElem, count) st
        let resTemp = List.fold (fun state (x, count) -> Map.incr x count state ) next.ResTemp multi
        let nt = { ResMakes = Map.incr reqElem count next.ResMakes; ResTemp = resTemp }
        { Stock = st; NextTurn = nt }
    let nextTurn { Stock = st; NextTurn = next } =
        let incr = Map.fold (fun state elem count -> stock.incr elem count state )
        let st = incr st next.ResMakes |> (fun state -> incr state next.ResTemp)
        //{ Stock = st; NextTurn = {next with ResMakes = Map.empty; ResTemp = Map.empty}}
        { Stock = st; NextTurn = {ResMakes = Map.empty; ResTemp = Map.empty}}
    let unmake _ = failwith "unmake not impl"
    let initState () =
        { Stock = stock.init ["blacksmith", 10; "forge", 10; "iron ore", 5; "hammer", 1 ]
          NextTurn = { ResMakes = Map.empty; ResTemp = Map.empty} }
module core =
    type Req =
        | PossibleMake of string
        | Make of string * int
        | Unmake of string * int
        | NextTurn
        | PopulationCount
        | GetStock
        | SetGiveEat of int
    type Resp =
        | PossibleMakeResp of int
        | Stock of stock.Stock
        | PopulationCountResp of int
        | NoneResp
        | Success
        | Fail
    //type OnNextTurn = { ResMakes: Map<string, int>; ResTemp: Map<string, int>}
    type State = { StateCraft:crafter.State; GiveEat:int }

    let interp ({StateCraft = stateCraft; GiveEat = giveEat} as state) = function
        | PossibleMake reqElement ->
            crafter.possibleMakes stateCraft reqElement |> PossibleMakeResp, None
        | Make(reqElem, count) ->
            let st = crafter.make stateCraft reqElem count
            NoneResp, Some {state with StateCraft = st}
        | NextTurn ->
            let st = crafter.nextTurn stateCraft
            NoneResp, Some {state with StateCraft = st}
        | GetStock -> Stock stateCraft.Stock, None
        | Unmake _ -> failwith "unmake not impl"
        | PopulationCount ->
            let notIdle = unitType |> Set.fold (fun state x -> state + stock.get x stateCraft.Stock) 0
            let idle =
                stateCraft.NextTurn.ResTemp
                |> Map.fold (fun state name count ->
                    if Set.contains name unitType then count else 0
                    + state) 0
            idle + notIdle |> PopulationCountResp, None
        | SetGiveEat n ->
            let eats = stock.get eatType stateCraft.Stock
#if DEBUG
            if n > eats then failwith "кол-во запрашиваемой еды больше чем есть на складе"
#endif
            //NoneResp, {Stock = stock.decr eatType n st; NextTurn = {next with GiveEat = n} } |> Some
            let st = stock.decr eatType n stateCraft.Stock
            let stc = { stateCraft with crafter.Stock = st; }
            NoneResp, { StateCraft = stc; GiveEat = n } |> Some
    let mail start =
        let mail start = MailboxProcessor.Start(fun inbox ->
            let rec loop gs = async {
                let! (req, reply:AsyncReplyChannel<Resp>) = inbox.Receive()
                match interp gs req with
                | resp, None -> reply.Reply resp; return! loop gs
                | resp, Some gsnew -> reply.Reply resp; return! loop gsnew }
            loop start)
        let m = mail start
        (fun r -> m.PostAndReply(fun x -> r, x))
    let initState () = {StateCraft = crafter.initState(); GiveEat = 0}

module analyze =
    open System.Text.RegularExpressions
    open System.IO
    let str = File.ReadAllText @"items.txt"
    let matches = Regex.Matches(str, "\"(.+?)\"" )
    matches |> Seq.cast<Match> |> Seq.map (fun x -> (x.Groups.Item 1).Value) |> Seq.distinct |>  List.ofSeq
    |> printfn "%A"