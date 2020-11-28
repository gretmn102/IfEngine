module File1
(*
let agriculture = 5 // ?
let geology = 1
let depletion = 50 // истощение (ресурсов)
let beerLimit = 100500

module territory =
    let dens = 10
    let stock = 0
    let fort = 0
    let smithy = 1
    let brewery = 0
    let stables = 0
    let arsenal = 1
    let granary = 1
module population =
    let toddlers = 0 // мелкие
    let kids = 0 // не оч мелкие
    let youth = 0 // юнцы
    let hippy = 0 // хиппи
    let serfs = 6 // рабов
    // медники. Мастер, занимающийся изготовлением или починкой медных изделий, посуды.
    let tinkers = 1 // лудильщики. Покрывать полудой для предохранения чего-л. от окисления. полуда - слой олова
    let miners = 3
    let marauders = 0
module food =
    let shrooms = 20
    let raw_meat = 0
    let beer = 0
module resources =
    let coal = 1
    let ore = 1
    let steel = 0
    let salt = 0
    let copper = 0
    let silver = 0
    let gold = 0
module equips =
    let tools = 0
    let wpn = 0
    let armor = 0
module nationPower =
    let dwarf_power = 100
    let elf_power = 120
    let goblin_power = 100
    let ant_power = 100
module exploredReserves =
    let ore_deposit = 32
    let copper_deposit = 27
    let silver_deposit = 13
    let gold_deposit = 9
    *)
//let useless = (((toddlers + kids) + youth) + hippy)
//let usefull = (((serfs + tinkers) + miners) + marauders)
//let workers = (((miners + marauders) + tinkers) + useless)
//let kobolds = (workers + serfs)

type Skill =
    | Unskilled
    | Miner
    ///<summary>Литейщик</summary>
    | Smelter
    | Warrior

type Status =
    | Idle
    | Make of string

type Resource =
    | None = 0

    | Mushroom = 1

    | Hammer = 2
    | Spear = 3
    | Pickaxe = 4

    | Steel = 5
    | Wood = 6
    | MushroomSpore = 7

    | IronOre = 8
(*
type MapCountClass<'a when 'a:comparison and 'a :> System.Enum> =
    | Something of Map<'a, int>
    static member init xs =
        let empty = System.Enum.GetValues(typeof<'a>) |> Seq.cast<'a>
                    |> Seq.map (fun x -> x, 0) |> Map.ofSeq
        List.fold (fun state (res, count) -> Map.add res count state) empty xs
        |> Something
    static member get (x:'a) (Something s) = s.[x]
    static member incr (x:'a) n (Something p) =
        let curr = Map.find x p
        let n = curr + n
        Map.add x n p |> Something
    static member decr (x:'a) n (Something s) =
        let curr = Map.find x s
        let n = let n = curr - n in if n < 0 then 0 else n
        Map.add x n s |> Something *)



module mapCount =
    type Something<'a when 'a:comparison and 'a :> System.Enum> = Something of Map<'a, int>
    let init xs =
        let empty = System.Enum.GetValues(typeof<'a>) |> Seq.cast<'a>
                    |> Seq.map (fun x -> x, 0) |> Map.ofSeq
        List.fold (fun state (res, count) -> Map.add res count state) empty xs
        |> Something
    let get (x:'a) (Something s) = s.[x]
    let incr (x:'a) n (Something p) =
        let curr = Map.find x p
        let n = curr + n
        Map.add x n p |> Something
    let decr (x:'a) n (Something s) =
        let curr = Map.find x s
        let n = let n = curr - n in if n < 0 then 0 else n
        Map.add x n s |> Something




type Unit = { Name: string; Skill: Skill; Busy: bool }

type BuildingTypes =
    /// <summary>Литейная</summary>
    | Foundry = 0
    /// <summary>Грибница</summary>
    | Mycelium = 1
    | IronMine = 2
// все производится за один ход если выполнены условия
// для производства одного молотка нужен незанятый кузнец, свободная кузня, 1 сталь и 2 дерева
// для производства 10 грибов нужен незанятый разнорабочий, свободная грибница(mycelium) и 1 семя гриба
// в любую шахту помещается сколь угодно шахтеров
// для добычи 1 xРуды нужен шахтер и xШахта



type Opt<'a> =
    | Some of 'a
    | None of string

module population =
    type Pop = Pop of Unit list
    // Чтобы получить литейщика нужно взять одного разнорабочего и добавить к нему молоток
    let reqSkills x =
        let reqSkills =
            [(Unskilled, Resource.None);
             (Miner, Resource.Pickaxe);
             (Smelter, Resource.Hammer);
             (Warrior, Resource.Pickaxe)] |> Map.ofList
        reqSkills.[x]
    (*
    let train from where (Pop pop) stock =
        match List.tryFind (fun {Skill = s} -> s = from) pop with
        | option.Some currUnit ->
            let req = reqSkills where
            if mapCount.get req stock = 0 then None "недостаточно ресурса"
            else
                (mapCount.decr req 1 stock, {currUnit with Skill = from}) |> Some
        | option.None -> None "некого обучать" *)
    let init() =
        [{ Name = "1"; Skill = Skill.Miner; Busy = false};
         { Name = "2"; Skill = Skill.Smelter; Busy = false};
         { Name = "3"; Skill = Skill.Unskilled; Busy = false};
         { Name = "4"; Skill = Skill.Warrior; Busy = false}] |> Pop
    let train2 from where (Pop pop) stock =
        let rec f acc = function
        | [] -> None "некого обучать"
        | ({Skill = s; Busy = false} as h)::t ->
            if s = from then List.rev({h with Skill = where}::acc) @ t |> Some
            else f (h::acc) t
        | h::t -> f (h::acc) t
        let req = reqSkills where
        if mapCount.get req stock = 0 then None "недостаточно ресурса"
        else
            match f [] pop with
            | None x -> None x
            | Some x -> (mapCount.decr req 1 stock, Pop x) |> Some
    let trainCount from where population stock =
        let units = List.filter (fun {Skill = s} -> s = from) population |> List.length
        let toolsAvail = let req = reqSkills where in mapCount.get req stock
        min units toolsAvail
    let idleCount skill (Pop pop) =
        List.filter (fun {Skill = s; Busy = b} -> s = skill && not b) pop |> List.length
    let hasIdle skill (Pop pop) = List.exists (fun {Skill = s; Busy = b} -> s = skill && not b) pop
    let doWork reqSkill (Pop pop) =
        //let u = List.find (fun {Status = Idle} -> true | _ -> false) pop
        let rec f acc = function
            | [] -> failwith "все заняты"
            | ({Skill = s; Busy = false} as h)::t ->
                if s = reqSkill then List.rev({h with Busy = true}::acc) @ t
                else f (h::acc) t
            | h::t -> f (h::acc) t
        f [] pop |> Pop
assert
    let sample = population.init()
    population.doWork Skill.Miner sample
    |> ignore
    assert
        population.idleCount Skill.Miner sample = 1
    population.hasIdle Skill.Warrior sample

module buildings =
    type Prop = { Free:int; Busy:int }
    type Buildings = Buildings of Map<BuildingTypes, Prop>
    let init xs =
        let empty = System.Enum.GetValues(typeof<BuildingTypes>) |> Seq.cast<BuildingTypes>
                    |> Seq.map (fun x -> x, { Free = 0; Busy = 0 }) |> Map.ofSeq
        List.fold (fun state (res, count) -> Map.add res { Free = count; Busy = 0 } state) empty xs
        |> Buildings
    let get x (Buildings s) = let curr = s.[x] in curr
    let incr x n (Buildings p) =
        let ({ Free = freeCurr; } as curr) = Map.find x p
        let curr = {curr with Free = freeCurr + n}
        Map.add x curr p |> Buildings
    let decr x n (Buildings s) =
        let ({ Free = freeCurr; } as curr) = Map.find x s
        let diff = freeCurr - n
        if diff < 0 then failwithf "такого количества свободных строений нету"
        else
            let curr = {curr with Free = diff}
            Map.add x curr s |> Buildings
    let release x n (Buildings b) =
        let ({ Free = freeCurr; Busy = busyCurr }) = Map.find x b
        let diff = busyCurr - n
        if diff < 0 then failwithf "not enough busy workers"
        else
            let curr = {Free = freeCurr + n; Busy = diff }
            Map.add x curr b |> Buildings
    /// <summary>
    /// занимает указанное кол-во строений
    /// </summary>
    let engage x n (Buildings b) =
        let ({ Free = freeCurr; Busy = busyCurr }) = Map.find x b
        let diff = freeCurr - n
        if diff < 0 then failwithf "not enough free workers"
        else
            let curr = {Free = diff; Busy = busyCurr + n }
            Map.add x curr b |> Buildings
assert
    let init = buildings.init [BuildingTypes.Foundry, 20]
    let s1 = buildings.engage BuildingTypes.Foundry 10 init
    let s2 = buildings.release BuildingTypes.Foundry 5 s1
    false
module item =
    type ItemReq = { Thing:Resource * int; WorkPlace:BuildingTypes; WorkerSkill:Skill; Ingrs:(Resource * int) list }

    let itemReqs =
        let itemReqs =
            [{ Thing = Resource.Hammer, 1; WorkPlace = BuildingTypes.Foundry
               Ingrs = [ Resource.Steel, 1;Resource.Wood, 2 ]
               WorkerSkill = Skill.Smelter }
             { Thing = Resource.Mushroom, 10; WorkPlace = BuildingTypes.Mycelium; Ingrs = [ Resource.MushroomSpore, 1]
               WorkerSkill = Skill.Unskilled}
             { Thing = Resource.IronOre, 1
               WorkPlace = BuildingTypes.IronMine
               WorkerSkill = Skill.Miner
               Ingrs = [] } ]
        itemReqs
        |> List.map (function {Thing = (r, _)} as x -> r, x)
        |> Map.ofList
        //itemReqs.[x]
    let make thing pop stock builds =
        let {Thing = (resReq, countMakes); WorkPlace = build; WorkerSkill = skill; Ingrs = ingrs} =
            Map.find thing itemReqs
        let pop' = population.doWork skill pop
        let stock' = List.fold (fun state (r, count) -> mapCount.decr r count state) stock ingrs
        let builds' = buildings.engage build 1 builds
        (resReq, countMakes), pop', stock', builds'
    let makeCount thing pop stock builds =
        let { WorkPlace = build; WorkerSkill = skill; Ingrs = ingrs} = Map.find thing itemReqs
        let { buildings.Prop.Free = free } = buildings.get build builds
        if free = 0 then 0
        else
            let res = List.map (fun (res,count) -> mapCount.get res stock - count) ingrs
            let min = List.min res
            if min <= 0 then 0
            else
                let skilled = population.idleCount skill pop
                if skilled = 0 then 0
                else
                    List.min [free; min; skilled]
    let tryMake thing pop stock builds =
        match Map.tryFind thing itemReqs with
        | Option.None ->  None <| sprintf "reciple %A not found" thing
        | Option.Some { WorkPlace = build; WorkerSkill = skill; Ingrs = ingrs  } ->
            let p = buildings.get build builds
            if p.Free = 0 then None "empty building not found"
            else
                if List.forall(fun (res, count) -> mapCount.get res stock >= count ) ingrs then
                    if population.hasIdle skill pop then
                        make thing pop stock builds |> Some
                    else
                        None "not enough worker"
                else
                    None "not enough ingredients"
    assert
        let reqres = Resource.Mushroom
        let itemreq = itemReqs.[Resource.Mushroom]
        let pop = population.init()
        //population.hasIdle Skill.Unskilled (population.init())
        let stock = mapCount.init [Resource.MushroomSpore, 10]
        let bds = buildings.init [BuildingTypes.Mycelium, 2]
        //makeCount reqres pop stock bds
        let m = make reqres pop stock bds
        let trym = tryMake reqres pop stock bds
        false