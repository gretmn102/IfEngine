module Program
open System
open System.Windows.Forms

open Abstract

type FrmType = { BtnAdd:Button; Name:Label; Avail:Label; Crafting:Label; CraftAvail:Label; IngrReq:Label }
type Frm() =
    let frm = new Forms.MainForm()
    let m = core.initState() |> core.mail
    let frmElems =
        let st = match m core.GetStock with core.Stock s -> s | _ -> failwith ""
        st |> stock.map
                    (fun _ _ -> { BtnAdd = new Button()
                                  Name = new Label(BorderStyle = BorderStyle.FixedSingle)
                                  Avail = new Label(BorderStyle = BorderStyle.FixedSingle)
                                  Crafting = new Label(BorderStyle = BorderStyle.FixedSingle)
                                  CraftAvail = new Label(BorderStyle = BorderStyle.FixedSingle)
                                  IngrReq = new Label(BorderStyle = BorderStyle.FixedSingle)})
    do
        let space = 25
        let frmElems = frmElems |> Map.toList

        frmElems |> List.iter (fun (name, x) -> x.Name.Text <- name; x.Name.Size <- Drawing.Size(66, 13)  ) //66; 13
        frmElems |> List.iter (fun (_, {BtnAdd = b}) -> b.Text <- "+"; b.Size <- new System.Drawing.Size(10, 13))

        let f func (frmElem:Control) =
            let f (x, y) (c:Control) =
                c.Location <- Drawing.Point(x, y);
                (x, y + space)
            frmElems
            |> List.fold (fun loc (_, b) -> f loc (func b) ) (frmElem.Location.X, frmElem.Location.Y + space)
            |> ignore
        f (fun {BtnAdd = x} -> x) frm.lbAdd
        f (fun {Name = x} -> x) frm.lbName
        f (fun {Avail = x} -> x) frm.lbAvail
        f (fun {Crafting = x} -> x) frm.lbCrafting
        f (fun {IngrReq = x} -> x) frm.lbIngrsReq
        f (fun {CraftAvail = x}-> x) frm.lbCraftAvail
        let f func = frm.Controls.AddRange (frmElems |> List.map (snd >> (fun x -> (func x) :> Control)) |> Array.ofList)
        f (fun {BtnAdd = x} -> x)
        f (fun {Name = x} -> x)
        f (fun {Avail = x} -> x)
        f (fun {Crafting = x} -> x)
        f (fun {IngrReq = x} -> x)
        f (fun {CraftAvail = x}-> x)
    let updateAvail () =
        let st = match m core.GetStock with core.Stock s -> s | _ -> failwith ""
        stock.iter (fun name count ->
            let {Avail = x} = Map.find name frmElems
            x.Text <- count.ToString() ) st
    let updateCraftAvail () =
        frmElems |> Map.iter (fun _ x -> x.CraftAvail.Text <- "0")
        let avail s = match m (core.PossibleMake s) with core.PossibleMakeResp s -> s | x -> failwithf "%A" x
        frmElems |> Map.iter (fun name {CraftAvail = x; BtnAdd = b} ->
            match avail name with
            | 0 -> x.Text <- "0"; b.Enabled <- false
            | n -> x.Text <- string n; b.Enabled <- true )
    do
        let update() = updateAvail(); updateCraftAvail();
        update()
        let f name =
            m <| core.Make(name, 1) |> ignore
            update()
        frmElems |> Map.iter (fun name {BtnAdd = b} -> b.Click.Add(fun _ -> f name))
        frm.btnNextTurn.Click.Add(fun _ ->  m core.NextTurn |> ignore; update())
    member __.Form = frm

[<EntryPoint>]
let main _ =
    let frm = new Frm()
    Application.EnableVisualStyles();
    Application.Run frm.Form
    0