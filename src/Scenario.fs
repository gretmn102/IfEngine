module Scenario
open IfEngine.Core
open Feliz
type LabelName =
    | MainMenu
    | Prelude
    | Forest
    | Forest2
    | FoxEscape
    | FoxEscape2
    | Еpilogue
let mushroom = "mushroom"
let getmushroom (x:Map<_,_>) =
    match x.[mushroom] with
    | Bool x -> x
    | _ -> failwith ""
let counter = "counter"
let getCounter (x:Map<_,_>) =
    match x.[counter] with
    | Num x -> x
    | _ -> failwith ""
let counterSet fn =
    ChangeVars (fun vars ->
        match Map.tryFind counter vars with
        | Some (Num x) ->
            Map.add counter (Num (fn x)) vars
        | _ -> Map.add counter (Num (fn 0)) vars
    )

let princessInTowerScenario : list<Label<LabelName,obj>> =
    [
        label Prelude [
            say "Король отдаст полцарства тому, кто спасет принцессу из башни дракона."
            say "Наш герой — один из тех, кто взялся за это славное задание!"

            menu
                [Html.text "Кем будет наш герой?"]
                [
                    choice "[Храбрым] воином с [легендарным мечом]" [
                        ChangeVars (fun vars ->
                            Map.add mushroom (Bool true) vars
                        )
                    ]
                    choice "[Мудрым] волшебником с [жезлом молний]." [
                        ChangeVars (fun vars ->
                            Map.add mushroom (Bool true) vars
                        )
                    ]
                    choice "[Бдительным] лучником с [верным волком]." [
                        ChangeVars (fun vars ->
                            Map.add mushroom (Bool true) vars
                        )
                    ]
                ]
        ]
    ]

let someScenario : list<Label<LabelName,obj>> =
    [
        label Prelude [
            say "Валера каким-то чудом устроился на стажировку в крупной компании — звезды сошлись, не иначе."
            // say "Он любовался новым в"
            // say "Куратор на перерыве сказал что нужно решить  отправил по электронной почте письмо, сказал его прочесть, а сам ушел  Валера пристально пялился в монитор, но письмо упорно отказывалось открываться."

            menu [ Html.text "Тогда он решил:" ] [
                choice "Уволиться" []
                choice "Вскрыть письмо ножом" [
                    say "А что, идея показалась вполне себе здравая — он вооружился канцелярским ножом и поднес его к монитору."

                    menu [ Html.text "— И что это ты удумал? — раздалось невесть откуда." ] [
                        choice "— Вскрыть письмо, конечно же!" [

                        ]
                        choice "— Ты кто?!" [
                            say "— Ну, точно не твоя совесть. Подумать только: венец эволюции, homo sapiens, а собрался тыкаь в меня ножом."
                        ]

                    ]
                ]
                choice "Расспросить среди знакомых" []
            ]
        ]
    ]

let hedgehogScenario : list<Label<LabelName,obj>> =
    [
        label Prelude [
            jump Forest
        ]

        label Forest [
            Say [
                divCenter [
                    Html.img [
                        prop.src "https://giffs.ru/wp-content/uploads/2019/12/Gz3FM-gap.jpg"
                    ]
                ]

                Html.div [
                    prop.text "Вы шли по лесу и столкнулись с йожигом, существом, крайне напоминающим ежика, но с бесконечно умными глазами."
                ]
            ]

            menu [ Html.text "Что будете делать?" ]
                [
                    choice "Заговорить с йожигом" [
                    ]
                ]
            say "Едва вы попытались заговорить о своей нелегкой судьбе, как тот указал на ближайший пень, вполне удобный, чтобы на него сесть."
            menu [] [
                choice "Сесть на пень и опять попытаться заговорить" [
                    say "ksdjf"
                ]
            ]
            say "На пень-то сели, вот только йожиг вновь опередил вас: протянул гриб, облизнулся и погладил животик."
            menu [] [
                choice "Взять гриб" [
                    ChangeVars (fun vars ->
                        Map.add mushroom (Bool true) vars
                    )
                ]
            ]
            if' (getmushroom)
                [
                    say "у меня есть гриб"
                ]
                [
                    say "у меня нет гриба"
                ]
            jump Forest2
        ]
        label Forest2 [
            say "и ничего не случилось"
            menu [] [
                choice "А может..." [
                    say "one"
                ]
                choice "А может и нет" [
                    menu [] [
                        choice "А может и нет" [
                            say "two"
                        ]
                        choice "А может и нет2" [
                            say "two"
                        ]
                    ]
                    say "three"
                ]
            ]
            say "конец"
        ]
    ]

type Addon =
    | StartFoxEscapeGame of speed:float * Stmt<LabelName, Addon> list * Stmt<LabelName, Addon> list option

let scenario =
    [
        label MainMenu [

            menu [
                Html.h1 [
                    prop.style [
                        style.justifyContent.center
                        style.display.flex
                    ]

                    prop.text "Ведьмочка и кот"
                ]
                Html.div [ Html.text "Малюсенький интерактивный рассказ." ]
                Html.div [
                    prop.style [
                        style.justifyContent.flexEnd
                        style.display.flex
                    ]
                    prop.text "v1.01"
                ]
            ] [
                choice "Начать" [ jump Prelude ]
                choice "Мини игра" [
                    menu [
                        Html.text "По мере прохождения вы столкнетесь с игрой в догонялки. Если интересно, то в такое можно сыграть отдельно по "
                        Html.a [
                            prop.href "https://gretmn102.github.io/public/FoxEscape/index.html"
                            prop.target "_blank"
                            prop.text "этой ссылке"
                        ]
                        Html.text "."
                    ] [
                        choice "Назад" [ jump MainMenu ]
                    ]
                ]
                choice "Авторов!" [
                    menu [
                        Html.text "Ну, кто мог сочинить такое безобразие? Конечно же, кот с ведьмочкой!"
                    ] [
                        choice "Назад" [ jump MainMenu ]
                    ]
                ]
            ]
        ]
        label Prelude [
            // jump FoxEscape
            // Сцена: неожиданная встреча
            // say "Кот бродит по лесу и натыкается на ведьмочку."
            say "Ведьмочка ходит по лесу, травы собирает, пока однажды не натыкается на кота."
            let attack =
                choice "Припугнуть" [
                ]

            let defence xs =
                choice "Защититься" [
                    say "Да он вроде и не нападает, хотя стоит и смотрит, как вкопанный."
                    menu [
                        divCenter [Html.text "Что делать?"]
                    ] xs
                ]
            let ignoreHim xs =
                choice "Не обращать внимания" [
                    say "Тяжело заниматься своими делами, когда на тебя смотрят."
                    menu [
                        divCenter [Html.text "Что делать?"]
                    ] xs
                ]

            menu [
                divCenter [Html.text "Что делать?"]

            ] [
                attack
                defence [
                    attack
                    ignoreHim [
                        attack
                    ]
                ]
                ignoreHim [
                    attack
                    defence [
                        attack
                    ]
                ]
            ]

            say "— Кыш, кыш, пушистый! — топает ведьмочка и пытается припугнуть кота."
            says [
                "Однако кот пугаться не спешит, да еще и говорит:"
                "— Чего это? Т.е. мяу?"
            ]
            say "— Так ты еще и говорящий?! Ну и ну."
            say "— Конечно, а что такого?"

            menu [
                Html.div [
                    prop.text "Он оглядывает ее (будто до этого не насмотрелся!) и спрашивает:"
                ]
                Html.div [
                    prop.text "— А чем это ты занимаешься?"
                ]
            ] [
                choice "Рассказать" [
                    say "— Собираю травку для очень важного..."
                ]
                choice "Увильнуть" [
                    say "— Что-то ты слишком любопытен, как для кота."
                    say "— Может, я смогу чем-то помочь, у меня знаешь какой нюх? То-то же."

                    say "\"Ладно, что плохого может случиться?\", — думает ведьмочка и отвечает: — Собираю травку для очень важного..."
                ]
            ]

            say "— Валерьянку?! — кот оживляется и бежит к ведьмочке."
            say "— Ай, ты чего?! Ну вот, рассыпал всё, помощничек."
            say "— Извини, — говорит кот и поджимает уши."
            say "— Ладно, страстный любитель валерьянки, с кем не бывает."

            say "Она собирает травы, прощается с котом и отправляется домой."

            // ***
            say "Впрочем, на этом странная встреча не закончилась: кот то и дело попадался ей в лесу и пытался блеснуть каким-нибудь талантом. Такие встречи тяжело назвать случайными."
            say "Однажды, когда она собирала хворост, он приволок ей цветы и как-то странно на нее посмотрел."

            say "— Случайные встречи, цветы, взгляды... Так-так, всё панятна."
            say "— И н-ничего не понятно!"
            say "— А чего мордочку прячешь? Землю исследуешь?"
            say "— Да так..."
            say "— Кот, ты ведь кот, пусть и говорящий. Нашел бы себе кошку, а то на ведьм засматриваешься. Совсем глупый, что ли?"
            say "— А я могу в человека превращаться!"
            say "— Ну-ка, ну-ка."
            say "— Раньше мог, — грустно говорит кот и никнет."

            say "— Что ж, хвостатый, — она собирает свои пожитки и бросает напоследок: — Мне пора домой, прощай."
            say "И уходит без оглядки."

            // ***
            menu [
                Html.text "Ведьмочка приходит домой, только отчего-то беспокойно на душе было: правильно ли она поступила?"
            ] [
                choice "Задуматься" [
                    // menu [] [
                    //     choice "С одной стороны..." [
                            say "Он — кот, всего лишь кот, хоть и говорящий. Это глупо, бесконечно глупо. Нельзя давать ложную надежду — она правильно поступила."
                            // say "И всё же..."
                        // ]

                        // choice "А с другой..." [
                            // say "Глупый, глупый кот. Интересно, чем он сейчас занимается?"
                            say "Интересно, чем он сейчас занимается?"
                        // ]
                    // ]
                ]
                choice "Заняться делами" [
                    say "Да, самое верное решение. Нельзя давать каким-то котам нарушать душевный покой. Тем более, что нужно зелье приготовить, да и по дому дел невпроворот. Кому нужны эти бестолковые коты... наверное."
                    say "Какое-то время занимается своими делами, а всякие лишние мысли отметает прочь. Проходит мимо окна и..."
                ]
            ]

            // ***
            say "— Эй, да он в окно подсматривает!"

            say "Кот пугается, падает. Она подбегает к окну, распахивает и видит, как тот вылезает из кустов."
            say "— И не стыдно подсматривать?"
            say "— Да я тут, это, мимо проходил..."
            say "— И решил в окно подсмотреть?"
            say "— Да, то есть нет, кхм, — он подходит к кустам и вытаскивает рыбу."
            say "— Ты с каких допотопных времен ее достал?! Вот же глупый!"
            say "Кот грустно вешает хвост."

            say "— Наверное, думал, что я воскликну: \"Эй, да это же редкая рыба stultus cattus, которая используется для приготовления легендарного зелья!\""
            say "— Что, пра... Т.е. да, это именно сутулый кактус, да!"

            says [
                "Ведьмочка смеется, а затем говорит:"
                "— Эх ты, пройдоха. Свалился на мою голову. Ты ведь меня совсем не знаешь. Может, я вредная или еще чего."
            ]
            say "— Так и я не подар... То есть я — самый лучший кот на свете, вот! А вредность меня совсем не страшит."
            say "— Да? Что ж, есть у меня одна идея, пошли."

            // ***
            says [
                "Она приводит кота на полянку, чертит огромный круг и велит ему, чтобы в круг не смел заступать. Сама же заходит в круг и говорит:"
                "— Вот сможешь меня поймать, тогда поцелую; а коль сумею сбежать, что ж, не судьба."
            ]
            menu [
                Html.text "Ведьмочка знала, что кот бегает гораздо быстрее нее (то-то он стоит и довольно ухмыляется), но она бы не предлагала игру, в которой не сумела бы победить."
            ] [
                choice "Начать игру" []
            ]

            jump FoxEscape
        ]

        let startGame winBody loseBody = Addon(StartFoxEscapeGame(3.5, winBody, loseBody))
        label FoxEscape [
            startGame [
                jump Еpilogue
            ] (Some [
                say "Ну уж нет, нельзя давать этим котам побеждать — надо попробовать еще раз!"
                counterSet (fun _ -> 0)
                jump FoxEscape2
            ])
        ]
        label FoxEscape2 [
            if' (fun vars -> getCounter vars < 3) [
                startGame [
                    jump Еpilogue
                ] (Some [
                    counterSet ((+) 1)
                    jump FoxEscape2
                ])
            ] [
                menu [
                    Html.text "Что ж, если не получается, ничего страшного. Должно быть, кот слишком сильно хочет добиться своего, потому так быстро бегает. Его можно победить, честное мяу, но если не хватает сил, можно просто пропустить."
                ] [
                    choice "Попробовать еще раз!" [
                        startGame [
                            jump Еpilogue
                        ] (Some [
                            counterSet (fun _ -> 0)
                            jump FoxEscape2
                        ])
                    ]
                    choice "Пропустить" [
                        jump Еpilogue
                    ]
                ]
            ]
        ]

        label Еpilogue [
            // say "Кот пытается ее поймать и так, и этак, но совсем выбивается из сил — ложится на травку и смотрит на закат."
            // say "Кот пытался ее поймать и так, и этак, но ведьмочка то и дело проворно от него сбегала. Совсем выбился из сил — лег на травку и уставился на закат."
            say "— Фух, это было весело, — говорит ведьмочка и смотрит на кота. — Ну что, видишь, какая я вредная?"
            says [
                "Тот молча ложится под дерево и грустно смотрит на закат. Она садится рядом и говорит:"
                "— Эх ты, так быстро сдался. Кстати, хвостатое, ты правда умеешь в человека превращаться? Может, ты заколдован или еще чего?"
            ]
            say "— Спросила ведьма, — буркает кот и отворачивается."
            say "— Не язви. Если бы могла понять природу волшебства, уже бы расколдовала."
            say "Неожиданно для самой себя она наклоняется над ним и чмокает в щеку. Тот оживляется и довольно мурлычет, пока она отплевывает шерсть."
            say "— Тьфу, не помогло. Ладно, попробовать стоило. Чего ухмыляешься? Ах ты ж хитрюга!"
            say "— Но я правда..."
            say "Теперь пришел черед коту убегать от разозленной ведьмочки."
            say "Продолжение следует :)"
        ]
    ]

open FsharpMyExtension.ListZipper
let beginLoc = MainMenu
let start () =
    let scenario =
        // hedgehogScenario
        scenario
        |> List.map (fun (labelName, body) -> labelName, (labelName, body))
        |> Map.ofList
        : Scenario<_,_>
    let init =
        {
            LabelState =
                [ ListZ.ofList (snd scenario.[beginLoc]) ]
            Vars = Map.empty
        }
    {|
        Scenario = scenario
        Init = init
    |}
let interp gameState =
    let x = start ()
    gameState
    |> interp
        (fun next state isWin addon ->
            match addon with
            | StartFoxEscapeGame(speed, winBody, loseBody) ->
                let f body =
                    let stack = state.LabelState
                    if List.isEmpty body then
                        next id stack
                    else
                        { state with
                            LabelState =
                                ListZ.ofList body::stack }
                        |> NextState
                if isWin then
                    f winBody
                else
                    match loseBody with
                    | Some loseBody -> f loseBody
                    | None -> NextState state
        )
        x.Scenario
