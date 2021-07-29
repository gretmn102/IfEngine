module Craft.Index
open Craft
open FsharpMyExtension.Either

type Page =
    | StockPage
    | RecipesPage
    | ProductionPage

type State =
    {
        Stock: Core.Stock
        CurrentPage: Page
        ShowOnlyAvailableGoods: bool
    }

type CraftMsg =
    | Increment of Core.ItemName
    | Decrement of Core.ItemName
    | Make of Core.ItemName
    | ChangeShowOnlyAvailableGoods of bool

type Msg =
    | ChangePage of Page
    | CraftMsg of CraftMsg

let init () =
    {
        Stock =
            Core.recipes
            |> Map.map (fun _ _ -> 0)
        CurrentPage = StockPage
        ShowOnlyAvailableGoods = false
    }

open Elmish
let update (msg: Msg) (state: State) =
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
    | ChangePage x ->
        { state with
            CurrentPage = x }, Cmd.none

open Zanaptak.TypedCssClasses
type Bulma = CssClasses<"https://cdnjs.cloudflare.com/ajax/libs/bulma/0.9.1/css/bulma.min.css", Naming.PascalCase>

open Fable.React
open Fable.React.Props

open Fable.FontAwesome
open Fulma

open Feliz

let view (state:State) (dispatch: Msg -> unit) =
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
                        Html.li [
                            prop.className [
                                if state.CurrentPage = RecipesPage then
                                    Bulma.IsActive
                            ]
                            prop.children [
                                Html.a [
                                    prop.children [
                                        Html.text "Recipes"
                                    ]
                                    if state.CurrentPage <> RecipesPage then
                                        prop.onClick (fun _ -> dispatch (ChangePage RecipesPage))
                                ]
                            ]
                        ]
                        Html.li [
                            prop.className [
                                if state.CurrentPage = StockPage then
                                    Bulma.IsActive
                            ]
                            prop.children [
                                Html.a [
                                    prop.children [
                                        Html.text "Stock"
                                    ]
                                    if state.CurrentPage <> StockPage then
                                        prop.onClick (fun _ -> dispatch (ChangePage StockPage))
                                ]
                            ]
                        ]
                        Html.li [
                            prop.className [
                                if state.CurrentPage = ProductionPage then
                                    Bulma.IsActive
                            ]
                            prop.children [
                                Html.a [
                                    prop.children [
                                        Html.text "Production"
                                    ]
                                    if state.CurrentPage <> ProductionPage then
                                        prop.onClick (fun _ -> dispatch (ChangePage ProductionPage))
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
            // | MenuPage ->
            //     menuPageRender state dispatch
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
                                            Fa.i [ Fa.Solid.ArrowRight ] []
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
                                                            Fa.i [ Fa.Solid.CaretUp ] []
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
                                                            Fa.i [ Fa.Solid.CaretUp ] []
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
                                                            Fa.i [ Fa.Solid.Check ] []
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