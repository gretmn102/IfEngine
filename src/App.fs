module App

open Elmish
open Feliz
open Browser
open Either

type Page =
    | MenuPage
    | StockPage
    | RecipesPage
    | ProductionPage

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

type 'LabelName State =
    {
        Count: Either<string, int>
        Stock: Core.Stock
        CurrentPage: Page
        ShowOnlyAvailableGoods: bool
        Game: 'LabelName InteractiveFictionEngine.T
        GameState: 'LabelName InteractiveFictionEngine.State

        SavedGameState: 'LabelName InteractiveFictionEngine.State

        FoxImgState: ImgState
        DuckImgState: ImgState
    }
type CraftMsg =
    | Increment of Core.ItemName
    | Decrement of Core.ItemName
    | Make of Core.ItemName
    | ChangeShowOnlyAvailableGoods of bool

type IfEngineMsg =
    | Next
    | Choice of int
    | NextState
    | Save
    | Load
    | NewGame

type FoxEscapeMsg =
    | GameOver of bool
    | UpdateFoxState of Img
    | UpdateDuckState of Img
type Msg =
    | ChangePage of Page
    | CraftMsg of CraftMsg
    | IfEngineMsg of IfEngineMsg
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
            Count = Right 0
            Stock =
                Core.recipes
                |> Map.map (fun _ _ -> 0)
            CurrentPage = MenuPage
            ShowOnlyAvailableGoods = false
            Game =
                InteractiveFictionEngine.interp scenario.Scenario scenario.Init
            GameState = scenario.Init
            SavedGameState = scenario.Init

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

let update (msg: Msg) (state: _ State) =
    match msg with
    | CraftMsg msg ->
        match msg with
        | Increment itemName ->
            { state with
                Stock = state.Stock |> Core.Map.addOrMod itemName 1 ((+) 1) }, Cmd.none
        | Decrement itemName ->
            { state with
                Stock = state.Stock |> Core.Map.addOrMod itemName 0 (fun i -> i - 1) }, Cmd.none
        | Make itemName ->
            { state with
                Stock = Core.make state.Stock Core.recipes.[itemName] }, Cmd.none
        | ChangeShowOnlyAvailableGoods x ->
            { state with
                ShowOnlyAvailableGoods = x }, Cmd.none
    | IfEngineMsg msg ->
        let nextState x =
            let rec nextState gameState = function
                | InteractiveFictionEngine.NextState newGameState ->
                    nextState newGameState (InteractiveFictionEngine.interp scenario.Scenario newGameState)
                | game ->
                    { state with
                        GameState = gameState
                        Game = game }
            nextState state.GameState x
        match msg with
        | Next ->
            match state.Game with
            | InteractiveFictionEngine.Print(_, f) ->
                nextState (f ()), Cmd.none
            | InteractiveFictionEngine.NextState x ->
                failwith "nextNextState"
            | InteractiveFictionEngine.End
            | InteractiveFictionEngine.Choices _
            | InteractiveFictionEngine.FoxEscapeGame _ ->
                state, Cmd.none
        | Choice i ->
            match state.Game with
            | InteractiveFictionEngine.Choices(_, _, f)->
                nextState (f i), Cmd.none
            | InteractiveFictionEngine.Print(_, f) ->
                nextState (f ()), Cmd.none
            | InteractiveFictionEngine.NextState x ->
                failwith "choiceNextState"
            | InteractiveFictionEngine.End
            | InteractiveFictionEngine.FoxEscapeGame _ -> state, Cmd.none
        | NextState ->
            nextState state.Game, Cmd.none
        | Save ->
            let state =
                { state with
                    SavedGameState = state.GameState }
            state, Cmd.none
        | Load ->
            let state =
                let gameState = state.SavedGameState
                { state with
                    Game = InteractiveFictionEngine.interp scenario.Scenario gameState
                    GameState = gameState }
            state, Cmd.none
        | NewGame ->
            let state =
                { state with
                    Game = InteractiveFictionEngine.interp scenario.Scenario scenario.Init }
            state, Cmd.none

    | ChangePage x ->
        { state with
            CurrentPage = x }, Cmd.none
    | FoxEscapeMsg res ->
        match res with
        | GameOver isWin ->
            match state.Game with
            | InteractiveFictionEngine.FoxEscapeGame(foxSpeed, f) ->
                FoxEscape.foxSpeed <- foxSpeed
                let state =
                    { state with
                        Game = f isWin }
                state, Cmd.none
            | InteractiveFictionEngine.NextState x ->
                { state with
                    Game = InteractiveFictionEngine.interp scenario.Scenario x }, Cmd.none
            | InteractiveFictionEngine.Print _
            | InteractiveFictionEngine.Choices _
            | InteractiveFictionEngine.End _ ->
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
open Fable.Core
open Fable.React
open Feliz
type Icon = CssClasses<"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.1/css/all.min.css", Naming.PascalCase>
type Bulma = CssClasses<"https://cdnjs.cloudflare.com/ajax/libs/bulma/0.9.1/css/bulma.min.css", Naming.PascalCase>
open Fable.FontAwesome
open Fulma
open Fable.React.Helpers

let menuPageRender (state:_ State) (dispatch: Msg -> unit) =
    let xs =
        let print (xs:ReactElement list) =
            Html.div [
                prop.className Bulma.Content
                prop.children xs
            ]

        match state.Game with
        | InteractiveFictionEngine.Print(xs, _) ->
            Html.div [
                prop.children [
                    print xs

                    Html.div [
                        prop.style [
                            style.justifyContent.center
                            style.display.flex
                        ]
                        prop.children [

                            Html.button [
                                prop.className [
                                    Bulma.Button
                                ]
                                prop.onClick (fun _ -> dispatch (IfEngineMsg Next))

                                prop.text "..."
                            ]
                        ]
                    ]
                ]
            ]
        | InteractiveFictionEngine.End ->
            Html.div [
                prop.style [
                    style.justifyContent.center
                    style.display.flex
                ]
                prop.text "Конец"
            ]
        | InteractiveFictionEngine.Choices(caption, choices, _) ->
            let xs =
                choices
                |> List.mapi (fun i label ->
                    Html.div [
                        prop.style [
                            style.justifyContent.center
                            style.display.flex
                        ]
                        prop.children [
                            Html.button [
                                prop.className [
                                    Bulma.Button
                                ]
                                prop.onClick (fun _ -> dispatch (IfEngineMsg (Choice i)))
                                prop.text label
                            ]
                        ]
                    ]
                )


            Html.div [
                prop.children (print caption :: xs)
            ]
        | InteractiveFictionEngine.FoxEscapeGame _ ->
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
        | InteractiveFictionEngine.NextState x ->
            Html.div [
                prop.text "NextState"
                prop.ref (fun e ->
                    dispatch (IfEngineMsg NextState)
                )
            ]

    Column.column [
        Column.Width (Screen.All, Column.Is6)
        Column.Offset (Screen.All, Column.Is3)
    ] [
        Box.box' [] [xs]
    ]

let render (state:_ State) (dispatch: Msg -> unit) =
    let nav =
        Html.div [
            prop.className [
                Bulma.Tabs
                Bulma.IsCentered
            ]
            prop.children [
                Html.ul [
                    prop.children [
                        // Html.li [
                        //     prop.className [
                        //         if state.CurrentPage = MenuPage then
                        //             Bulma.IsActive
                        //     ]
                        //     prop.children [
                        //         Html.a [
                        //             prop.children [
                        //                 Html.text "MenuPage"
                        //             ]
                        //             if state.CurrentPage <> MenuPage then
                        //                 prop.onClick (fun _ -> dispatch (ChangePage MenuPage))
                        //         ]
                        //     ]
                        // ]
                        // Html.li [
                        //     prop.className [
                        //         if state.CurrentPage = RecipesPage then
                        //             Bulma.IsActive
                        //     ]
                        //     prop.children [
                        //         Html.a [
                        //             prop.children [
                        //                 Html.text "Recipes"
                        //             ]
                        //             if state.CurrentPage <> RecipesPage then
                        //                 prop.onClick (fun _ -> dispatch (ChangePage RecipesPage))
                        //         ]
                        //     ]
                        // ]
                        // Html.li [
                        //     prop.className [
                        //         if state.CurrentPage = StockPage then
                        //             Bulma.IsActive
                        //     ]
                        //     prop.children [
                        //         Html.a [
                        //             prop.children [
                        //                 Html.text "Stock"
                        //             ]
                        //             if state.CurrentPage <> StockPage then
                        //                 prop.onClick (fun _ -> dispatch (ChangePage StockPage))
                        //         ]
                        //     ]
                        // ]
                        // Html.li [
                        //     prop.className [
                        //         if state.CurrentPage = ProductionPage then
                        //             Bulma.IsActive
                        //     ]
                        //     prop.children [
                        //         Html.a [
                        //             prop.children [
                        //                 Html.text "Production"
                        //             ]
                        //             if state.CurrentPage <> ProductionPage then
                        //                 prop.onClick (fun _ -> dispatch (ChangePage ProductionPage))
                        //         ]
                        //     ]
                        // ]
                        let makeIcon icon =
                            Html.i [
                                prop.className [
                                    Icon.Fas
                                    icon
                                ]
                                prop.style [
                                    style.marginRight 4
                                ]
                            ]

                        Html.li [
                            Html.a [
                                prop.onClick (fun _ -> dispatch (IfEngineMsg NewGame))
                                prop.children [
                                    Html.div [
                                        makeIcon Icon.FaFile

                                        Html.text "New Game"
                                    ]
                                ]
                            ]
                        ]
                        Html.li [
                            Html.a [
                                prop.onClick (fun _ -> dispatch (IfEngineMsg Save))
                                prop.children [
                                    Html.div [
                                        makeIcon Icon.FaSave

                                        Html.text "Save"
                                    ]
                                ]
                            ]
                        ]
                        Html.li [
                            Html.a [
                                prop.onClick (fun _ -> dispatch (IfEngineMsg Load))
                                prop.children [
                                    Html.div [
                                        makeIcon Icon.FaUpload

                                        Html.text "Load"
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]

    Html.section [
        prop.style [
            style.padding 20
        ]
        prop.children [
            nav
            match state.CurrentPage with
            | MenuPage ->
                menuPageRender state dispatch
            | RecipesPage ->
                let recipesRender () =
                    let recipeRender (recipe:Core.Recipe) =
                        if List.isEmpty recipe.Ingredients then
                            [
                                Html.p [
                                    prop.text (sprintf "%s" recipe.ItemName)
                                ]
                            ]
                        else
                            [
                                Html.h1 [
                                    prop.children [
                                        Html.span [
                                            prop.text recipe.ItemName
                                        ]
                                        Html.span " "
                                        Html.span [
                                            prop.className [
                                                Icon.Fa
                                                Icon.FaArrowRight
                                            ]
                                        ]
                                        Html.span " "
                                        Html.span [
                                            prop.className [

                                            ]
                                            prop.text recipe.OutputCount
                                        ]
                                    ]
                                    // prop.text (sprintf "%s -> %d" recipe.ItemName recipe.OutputCount)
                                ]
                                Html.ul [
                                    recipe.Ingredients
                                    |> List.map (fun (name, count) ->
                                        Html.li [
                                            prop.children [
                                                Html.text (sprintf "%s x %d" name count)
                                            ]
                                        ]
                                    )
                                    |> prop.children
                                ]
                            ]
                    Core.recipes
                    |> Map.toList
                    |> List.map (fun (_, x) ->

                        Html.ul [
                            prop.children [
                                Html.div [
                                    prop.className [
                                        Bulma.Box
                                    ]
                                    prop.children [
                                        Html.div [
                                            prop.className [
                                                Bulma.Columns
                                                Bulma.IsMobile
                                                Bulma.IsVcentered
                                            ]
                                            prop.children [
                                                Html.div [
                                                    prop.className [
                                                        Bulma.Column
                                                        Bulma.Content
                                                    ]
                                                    prop.children (recipeRender x)
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    )
                yield! recipesRender ()
            | StockPage ->
                let fieldWithButtons (name:string) count =
                    Html.div [
                        prop.className [
                            Bulma.Box
                        ]
                        prop.children [
                            Html.div [
                                prop.className [
                                    Bulma.Columns
                                    Bulma.IsMobile
                                    Bulma.IsVcentered
                                ]
                                prop.children [
                                    Html.div [
                                        prop.className [
                                            Bulma.Column
                                        ]
                                        prop.children [Html.text name]
                                    ]
                                    Html.div [
                                        prop.className [
                                            Bulma.Column
                                            Bulma.IsNarrow
                                        ]
                                        prop.children [
                                            Html.div [
                                                prop.className [
                                                    Bulma.Field
                                                    Bulma.HasAddons
                                                ]
                                                prop.children [
                                                    let toControl (x:ReactElement) =
                                                        Html.p [
                                                            prop.className [
                                                                Bulma.Control
                                                            ]
                                                            prop.children x
                                                        ]
                                                    Html.button [
                                                        prop.className [
                                                            Bulma.Button
                                                        ]
                                                        prop.children [
                                                            Html.i [
                                                                prop.className [
                                                                    Icon.Fa
                                                                    Icon.FaCaretUp
                                                                ]
                                                            ]
                                                        ]
                                                        prop.onClick (fun _ -> dispatch (CraftMsg (Increment name)))
                                                    ]
                                                    |> toControl
                                                    Html.button [
                                                        prop.className [
                                                            Bulma.Button
                                                            Bulma.IsStatic
                                                        ]
                                                        prop.text (count:string)
                                                    ]
                                                    |> toControl
                                                    Html.button [
                                                        prop.className [
                                                            Bulma.Button
                                                        ]
                                                        prop.children [
                                                            Html.i [
                                                                prop.className [
                                                                    Icon.Fa
                                                                    Icon.FaCaretDown
                                                                ]
                                                            ]
                                                        ]
                                                        prop.onClick (fun _ -> dispatch (CraftMsg (Decrement name)))
                                                    ]
                                                    |> toControl
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                Html.label [
                    prop.className [
                        Bulma.Checkbox
                    ]
                    prop.children [
                        Html.input [
                            prop.type'.checkbox
                            prop.isChecked state.ShowOnlyAvailableGoods
                            prop.onCheckedChange (fun x ->
                                dispatch (CraftMsg (ChangeShowOnlyAvailableGoods x))
                            )
                        ]
                        Html.text " Show only available goods"
                    ]
                ]
                let stockRender () =
                    let xs =
                        state.Stock
                        |> Map.toList
                    if state.ShowOnlyAvailableGoods then
                        xs
                        |> List.choose (fun (name, x) ->
                            if x > 0 then
                                Html.ul [
                                    prop.children [
                                        fieldWithButtons name (string x)
                                    ]
                                ]
                                |> Some
                            else None
                        )
                    else
                        xs
                        |> List.map (fun (name, x) ->
                            Html.ul [
                                prop.children [
                                    fieldWithButtons name (string x)
                                ]
                            ]
                        )
                yield! stockRender ()
            | ProductionPage ->
                let fieldWithButtons (name:string) availCount =
                    Html.div [
                        prop.className [
                            Bulma.Box
                        ]
                        prop.children [
                            Html.div [
                                prop.className [
                                    Bulma.Columns
                                    Bulma.IsMobile
                                    Bulma.IsVcentered
                                ]
                                prop.children [
                                    Html.div [
                                        prop.className [
                                            Bulma.Column
                                        ]
                                        prop.children [Html.text name]
                                    ]
                                    Html.div [
                                        prop.className [
                                            Bulma.Column
                                            Bulma.IsNarrow
                                        ]
                                        prop.children [
                                            Html.div [
                                                prop.className [
                                                    Bulma.Field
                                                    Bulma.HasAddons
                                                ]
                                                prop.children [
                                                    let toControl (x:ReactElement) =
                                                        Html.p [
                                                            prop.className [
                                                                Bulma.Control
                                                            ]
                                                            prop.children x
                                                        ]
                                                    Html.button [
                                                        prop.className [
                                                            Bulma.Button
                                                        ]
                                                        prop.children [
                                                            Html.i [
                                                                prop.className [
                                                                    Icon.Fa
                                                                    Icon.FaCheck
                                                                ]
                                                            ]
                                                        ]
                                                        prop.onClick (fun _ -> dispatch (CraftMsg (Make name)))
                                                    ]
                                                    |> toControl
                                                    Html.button [
                                                        prop.className [
                                                            Bulma.Button
                                                            Bulma.IsStatic
                                                        ]
                                                        prop.text (availCount:int)
                                                    ]
                                                    |> toControl
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                let stockRender () =
                    Core.avail Core.recipes state.Stock
                    |> List.map (fun (name, availCount) ->
                        Html.ul [
                            prop.children [
                                fieldWithButtons name availCount
                            ]
                        ]
                    )
                yield! stockRender ()
        ]
    ]
open Elmish.React


Program.mkProgram init update render
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.run