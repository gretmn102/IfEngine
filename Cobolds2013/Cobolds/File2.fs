module File2

//#nowarn "40"


type MainWindow () =
    let form = new Forms.MainForm()
    let txb = form.richTextBox1
    let btn1 = form.button1
    let btn2 = form.button2
    do
        form.richTextBox1.TextChanged.Add
            (fun _ -> txb.SelectionStart <- txb.Text.Length; txb.ScrollToCaret())
    member __.Form = form
    member __.Print format = Printf.kprintf (fun s -> txb.AppendText (s + Environment.NewLine)) format

    member this.AddAction1 s f =
        btn1.Text <- s
        btn1.Click
        |> Observable.subscribe(fun _ -> f(); this.Print "instant 1")
    member this.AddAction2 s f =
        btn2.Text <- s
        btn2.Click
        |> Observable.subscribe(fun _ -> f(); this.Print "instant 2")


module rand =
    let ran = System.Random()
    let r n max = ran.Next(n, max)
    let equiprob xs =
        let n = ran.Next(0, Seq.length xs)
        Seq.nth n xs

type Resource =
    | None = 0
    | Eat = 1
    | Mushroom = 1

    | Hammer = 2
    | Spear = 3
    | Pickaxe = 4

type Works =
    | Field = 0
    | Idler = 1
    | Workshop = 2

module population =
    type Unit = Unit of int * string
    type Pop = Population of Map<Works, Set<Unit>>
    //let population = [ Field, Set.empty; Idler, Set.empty ] |> Map.ofList
    let sendTo from where count (Population p) =
        let send from where n =
            let rec f (idlers, workers) =
                let r = rand.equiprob idlers
                Set.remove r idlers, Set.add r workers
            List.fold (fun state _ -> f state) (from, where) [1..n]
        let fromVal = Map.find from p
        let whereVal = Map.find from p
        if Set.count fromVal = 0 then
            Map.add from (Set.union fromVal whereVal) p |> Map.add where Set.empty
        else
            let fromVal, whereVal = send fromVal (Map.find where p) count
            Map.add from fromVal p
            |> Map.add where whereVal
        |> Population
    /// <summary>Добавляет населению одного бездельника</summary>
    let add x (Population p) =
        let rec f n =
            let curr = Unit(n,x)
            if p |> Map.exists(fun _ v -> Set.exists ((=)curr) v) then f (n + 1) else curr
        let currVal = Map.find Works.Idler p
        Map.add Works.Idler (Set.add (f 0) currVal) p |> Population
    let init xs =
        let empty = System.Enum.GetValues(typeof<Works>) |> Seq.cast<Works>
                    |> Seq.map (fun x -> x, Set.empty<Unit>) |> Map.ofSeq //.ofList [Field, Set.empty<unit'>; Idler, Set.empty<unit'>; Workshop, Set.empty<unit'>]
        List.fold (fun state x -> add x state) (Population empty) xs
    let getval x (Population p) = Map.find x p
    let count x (Population p) = Map.find x p |> Seq.length
    let summary (Population p) = p |> Map.fold (fun state _ value -> Set.count value + state) 0

module resources =
    type Res = Resources of Map<Resource, int>
    let init xs =
        let empty = System.Enum.GetValues(typeof<Resource>) |> Seq.cast<Resource>
                    |> Seq.map (fun x -> x, 0) |> Map.ofSeq
        List.fold (fun state (res, count) -> Map.add res count state) empty xs
        |> Resources
    let getval x (Resources p) = Map.find x p
    let decr r n (Resources p) =
        let curr = Map.find r p
        let n = let n = curr - n in if n < 0 then 0 else n
        Map.add r n p |> Resources
    let incr r n (Resources p) =
        let curr = Map.find r p
        let n = curr + n
        Map.add r n p |> Resources

type Req =
    | GetPopulation
    | GetResources
    //| SendToWorkOnField of int
    | SendTo of Works * Works * int
    | NextTurn
type Resp =
    | State
    | PopulationCount of population.Pop
    | Resources of resources.Res
    | Success
    | Error of string

type Msg = Post of Req * AsyncReplyChannel<Resp>
type Gamestate = { PopDiscontent:int; Population: population.Pop; Res: resources.Res }
let mail r =
    let interp ({PopDiscontent = popDisc; Gamestate.Population = pop; Res = r} as gs) = function
        | GetPopulation -> PopulationCount pop, None
        | GetResources -> Resources r, None
        | SendTo(from, where, n) ->
            if population.count from pop < n then Error "народа маловато", None
            else Success, {gs with Population = population.sendTo from where n pop} |> Some
        | NextTurn ->
            let eatIncr n gs = {gs with Res = resources.incr Resource.Eat n gs.Res}
            let eatDecr n gs = {gs with Res = resources.decr Resource.Eat n gs.Res}
            let res =
                let summ = population.summary pop
                let state =
                    if resources.getval Resource.Eat r < summ then
                        let popDisc = popDisc + rand.r 0 10
                        {gs with PopDiscontent = popDisc}
                    else
                        let randName = rand.r 0 100 |> sprintf "%d"
                        {gs with Population = population.add randName pop}
                eatDecr summ gs
            let eatProduct =
                let workersCount = population.count Works.Field pop
                workersCount*2
            let res = eatIncr eatProduct res
            //failwith "not impl"
            Success, Some res

 (*
сделать ход
    if населению не хватает на всех еды then растет недовольство
    else растет население

    в зависимости от коэфф недовольства -> население (работников или бездельников или тех и других) перебегает в соседний лагерь
    производство пищи зависит от того, сколько населения занимается выращиванием и кол-вом семян для посадки *)
        | _ -> Error "cmd not impl", None
    MailboxProcessor.Start(fun inbox ->
        let rec loop gs = async {
            let! (Post(req, reply)) = inbox.Receive()
            match interp gs req with
            | resp, None -> reply.Reply resp; return! loop gs
            | resp, Some gsnew -> reply.Reply resp; return! loop gsnew }
        loop r)

assert
    let startunit = List.init 5 (fun x -> x.ToString())
    let startPopulation = population.init startunit
    //population.add "1" startPopulation
    let startRes = [ Resource.Eat, 10 ] |> resources.init
    let m = mail {PopDiscontent = 0; Population = startPopulation; Res = startRes}
    let post cmd = m.PostAndReply(fun x -> Post(cmd, x))
    Req.SendTo(Works.Idler, Works.Field, 5) |> post |> ignore
    Req.GetPopulation |> post
    |> ignore

    Req.SendTo(Works.Idler, Works.Workshop, 5) |> post |> ignore
    Req.SendTo(Works.Field, Works.Idler, 5) |> post |> ignore
    Req.GetPopulation |> post
    |> ignore
    true
