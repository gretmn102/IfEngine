module App

open Elmish
open Browser

type Deferred<'t> =
    | HasNotStartedYet
    | InProgress
    | Resolved of 't

type Img = Result<Types.HTMLImageElement, unit>
type ImgState =
    {
        InputImageSrc: string
        ImageSrc: string

        Img:Deferred<Img>
        IsRounded: bool
    }

type State =
    {
        IfEngineState: IfEngine.Index.State<Scenario.LabelName,Scenario.Addon,bool>

        FoxImgState: ImgState
        DuckImgState: ImgState
    }

type FoxEscapeMsg =
    | GameOver of bool
    | UpdateFoxState of Img
    | UpdateDuckState of Img

type Msg =
    | IfEngineMsg of IfEngine.Index.Msg
    | FoxEscapeMsg of FoxEscapeMsg

let scenario = Scenario.start()

let init () =
    let imgStateEmpty =
        {
            InputImageSrc = ""
            ImageSrc = ""
            Img = HasNotStartedYet
            IsRounded = true
        }

    let st =
        {
            IfEngineState =
                {
                    IfEngine.Index.Game =
                        Scenario.interp scenario.Init
                    IfEngine.Index.GameState = scenario.Init
                    IfEngine.Index.SavedGameState = scenario.Init
                }

            FoxImgState =
                { imgStateEmpty with
                    ImageSrc = "https://thumbs.dreamstime.com/z/black-cat-icon-halloween-symbol-domestic-pet-black-cat-icon-halloween-symbol-domestic-pet-silhouette-animal-isolated-flat-158661525.jpg"
                }
            DuckImgState =
                { imgStateEmpty with
                    ImageSrc = "https://thumbs.dreamstime.com/z/magic-witch-hat-surrounded-stars-vector-helloween-symbol-evil-costume-wand-logo-magical-cloting-wizard-logo-magic-witch-hat-152581714.jpg"
                }
        }
    st, Cmd.none

let update (msg: Msg) (state: State) =
    match msg with
    | IfEngineMsg msg ->
        let gameState, cmd =
            IfEngine.Index.update Scenario.interp scenario.Init msg state.IfEngineState
        let state =
            { state with
                IfEngineState = gameState
            }
        state, cmd |> Cmd.map IfEngineMsg

    | FoxEscapeMsg res ->
        match res with
        | GameOver isWin ->
            match state.IfEngineState.Game with
            | IfEngine.Core.AddonAct(x, f) ->
                match x with
                | Scenario.StartFoxEscapeGame(foxSpeed, winBody, loseBody) ->
                    FoxEscape.foxSpeed <- foxSpeed
                    if isWin then
                        FoxEscape.restart()
                    let state =
                        { state with
                            IfEngineState =
                                { state.IfEngineState with
                                    Game = f isWin }
                        }
                    state, Cmd.none
            | IfEngine.Core.NextState x ->
                let state =
                    { state with
                        IfEngineState =
                            { state.IfEngineState with
                                Game = Scenario.interp x }
                    }
                state, Cmd.none
            | IfEngine.Core.Print _
            | IfEngine.Core.Choices _
            | IfEngine.Core.End _ ->
                state, Cmd.none
        | UpdateFoxState img ->
            let state =
                { state with
                    FoxImgState =
                        { state.FoxImgState with
                            Img = Resolved img } }
            match img with
            | Ok img ->
                FoxEscape.updateFoxSprite state.FoxImgState.IsRounded img
            | _ -> ()
            state, Cmd.none
        | UpdateDuckState img ->
            let state =
                { state with
                    DuckImgState =
                        { state.DuckImgState with
                            Img = Resolved img } }
            match img with
            | Ok img ->
                FoxEscape.updateDuckSprite state.DuckImgState.IsRounded img
            | _ -> ()
            state, Cmd.none

open Zanaptak.TypedCssClasses
type Bulma = CssClasses<"https://cdnjs.cloudflare.com/ajax/libs/bulma/0.9.1/css/bulma.min.css", Naming.PascalCase>

open Fable.React
open Fable.React.Props

open Fable.FontAwesome
open Fulma

open Feliz

let view (state:State) (dispatch: Msg -> unit) =
    // Craft.Index.view state.CraftState (CraftMsg >> dispatch)

    IfEngine.Index.view
        (fun (x:Scenario.Addon) state2 dispatch2 ->
            let foxSpeed =
                match x with
                | Scenario.Addon.StartFoxEscapeGame(foxSpeed, _, _) ->
                    foxSpeed
            do
                let f (state:ImgState) update =
                    match state.Img with
                    | HasNotStartedYet
                    | Resolved (Error _) ->
                        if not <| System.String.IsNullOrEmpty state.ImageSrc then
                            let img = document.createElement "img" :?> Types.HTMLImageElement

                            img.src <- state.ImageSrc

                            img.onload <- fun e ->
                                if isNull e then ()
                                else
                                    let imgElement = e.currentTarget :?> Types.HTMLImageElement
                                    dispatch (update (Ok imgElement))

                            img.onerror <- fun e ->
                                dispatch (update (Error ()))
                    | _ -> ()
                f state.FoxImgState (UpdateFoxState >> FoxEscapeMsg)
                f state.DuckImgState (UpdateDuckState >> FoxEscapeMsg)

            let gameRender =
                Html.canvas [
                    prop.style [
                        // Feliz.style.border(1, borderStyle.solid, "grey")
                    ]
                    prop.tabIndex -1
                    prop.ref (fun canvas ->
                        if isNull canvas then ()
                        else
                            let canvas = canvas :?> Types.HTMLCanvasElement
                            let updateSize () =
                                match canvas.parentElement with
                                | null -> ()
                                | x ->
                                    let w = x.offsetWidth // - 50.
                                    canvas.width <- w
                                    canvas.height <- w
                                    FoxEscape.updateSize w w

                                    FoxEscape.speedMult <- w * 0.20 / 540.
                            updateSize ()

                            window.onresize <- fun x ->
                                updateSize ()

                            FoxEscape.foxSpeed <- foxSpeed
                            let x =
                                FoxEscape.start canvas (fun isWin ->
                                    Mainloop.mainloop.stop () |> ignore
                                    dispatch (FoxEscapeMsg (GameOver isWin))
                                )
                            FoxEscape.m_winMsg <-
                                {|
                                    Color = "grey"
                                    Msg = "Сбежала!"
                                |}
                            FoxEscape.m_LoseMsg <-
                                {|
                                    Color = "pink"
                                    Msg = "Кусь!"
                                |}

                            Mainloop.mainloop.setUpdate (fun delta -> x.Update delta) |> ignore
                            Mainloop.mainloop.setDraw (fun _ -> x.Draw ()) |> ignore
                            Mainloop.mainloop.setEnd (fun fps panic ->
                                // TODO: fpsCounter.textContent <- sprintf "%A FPS" (round fps)
                                if panic then
                                    let discardedTime = round(Mainloop.mainloop.resetFrameDelta())
                                    printfn "Main loop panicked, probably because the browser tab was put in the background. Discarding %A ms" discardedTime
                            ) |> ignore
                            Mainloop.mainloop.start () |> ignore
                    )
                ]
            Html.div [
                prop.style [
                        style.justifyContent.center
                        style.display.flex
                ]
                prop.children gameRender
            ]
        )
        state.IfEngineState
        (IfEngineMsg >> dispatch)

open Elmish.React

Program.mkProgram init update view
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.run