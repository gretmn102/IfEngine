module File3

open FsharpMyExtension.FSharpExt
open FsharpMyExtension.Show

let ran = System.Random()

type T =
    | Acts of (string * (unit -> T)) seq
    | Print of string * (unit -> T)
    | PrintPopulationCount of int * (unit -> T)
    | GetCountPopulation of (int -> T)
    | GetFreePeoples of (int -> T)
    | GetWorkInField of (int -> T)
    | AddWorkInField of (unit -> T)
    | RemoveWorkInField of (unit -> T)
    | GetFood of (int -> T)
    /// Произвольно умерщвляет указанное количество населения возвращая список в виде: профессия * количество умерших
    | RemovePeople of int * ((string * int) list -> T)
    /// Засеять поле
    | ToPlant of (unit -> T)
    /// количество не засеянных участков
    | GetNotSowPlantCount of (int -> T)
    /// количество засеянных участков
    | GetSowPlantCount of (int -> T)
    | GetSeedCount of (int -> T)

let rec main () =
    GetCountPopulation(fun pcount ->
    PrintPopulationCount(pcount, (fun () ->
    GetFreePeoples(fun free ->
//    let fn () =
//        GetFreePeoples(fun free ->
//        if free > 0 then
//            sprintf "послать одного на вольные хлеба (свободно %i)" free, fun () -> AddWorkInField main
//        else failwith "")
    GetWorkInField(fun fieldWorkers ->
    Acts (seq {
        if free > 0 then
            yield sprintf "послать одного на вольные хлеба (свободно %i)" free, fun () ->
            AddWorkInField main
        if fieldWorkers > 0 then
            yield sprintf "отозвать одного с вольных хлебов (работает %i)" fieldWorkers, fun () ->
            RemoveWorkInField main
        yield "следующий ход", fun () ->
            GetFood(fun food ->
                let fn peopleCount food =
                    /// для выживания одного человека нужно 2 еды.
                    let foodConsHuman = 2
                    let survived = food / foodConsHuman
                    let dfoodh = peopleCount - survived
                    if dfoodh > 0 then dfoodh, survived * foodConsHuman
                    else 0, peopleCount * foodConsHuman
                let removePeople death next =
                    if death > 0 then next ""
                    else
                        RemovePeople(death, fun xs ->
                        let hungerDeath = function
                        | [] -> ""
                        | xs -> sprintf "Погибло от голода %i:\n%s" death
                                    (List.map (curry <| sprintf "\t%s: %i" ) xs |> join "\n")
                        hungerDeath xs |> next)
                let foodEats foodInit foodEats next =
                    match foodInit - foodEats with
                    | 0 ->
                        sprintf "Вся еда съедена!"
                match fn pcount food with
                | 0, foodEats -> failwith ""
                | death, foodEats ->
                    RemovePeople(death, fun xs ->
                    let hungerDeath = function
                        | [] -> ""
                        | xs ->
                            sprintf "Погибло от голода %i:\n%s" death
                                (List.map (curry <| sprintf "\t%s: %i" ) xs |> join "\n")
                    let madeFood = sprintf "Сделано еды: %i" fieldWorkers
                    sprintf "Засеяно полей: %i" <| failwith ""
                    failwith ""
                    ))
        })
    )))))

type AsmPlace =
    | Field
type Spec =
    | Unskilled
    | Sower
type Resource =
    | Seed
    | Food
    | Bag
type AcademyType =
    | HomeEduc

type 'a Reciple = { Name: 'a; Resources:(Resource*int) list }
type ResReciple = { Reciple:Reciple<Resource>; OutputCount:int; AsmPlace:AsmPlace; }
type CreatureEduc = { Reciple:Reciple<Spec>; AcademyType:AcademyType }
type PlaceType = { Reciple:Reciple<AsmPlace>; CountMaxWorkers:int; SpecNeed:Spec; }

type Reciples = Map<Resource, ResReciple>
type CreatureEducs = Map<Spec, CreatureEduc>
type PlacesType = Map<AsmPlace, PlaceType>

type State = { Creatures:Map<Spec,int>; Stock: Map<Resource,int> }
let food =
    { AsmPlace = Field
      Reciple = { Name = Food; Resources = [ Seed, 2 ] }
      OutputCount = 2 }
let sower =
    { AcademyType = HomeEduc
      Reciple = { Name = Sower; Resources = [ Bag, 1 ] } }
let field =
    { CountMaxWorkers = 2
      Reciple = { Name = Field; Resources = failwith "Not implemented yet" }
      SpecNeed = Sower }

module Ab =
    type T =
        | ExistResources of (Resource*int) list * (bool -> T)
        | FreeWorkerSpec of Spec * (bool -> T)
        | FreeExistPlace of AsmPlace * (bool -> T)
        | DoWork of Spec * (unit -> T)
        | RemoveRes of (Resource*int) list * (unit -> T)
        | TakePlace of AsmPlace * (unit -> T)
        | AddToNext of ResReciple
    let make item =
        FreeWorkerSpec(item.SpecNeed, function
            | true ->
                FreeExistPlace(item.AsmPlace, function
                    | true ->
                        ExistResources(item.Resources, function
                            | true ->
                                DoWork(item.SpecNeed, fun () ->
                                RemoveRes(item.Resources, fun () ->
                                TakePlace(item.AsmPlace, fun () ->
                                AddToNext item
                    ))))))