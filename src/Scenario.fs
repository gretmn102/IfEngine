module Scenario
open InteractiveFictionEngine
open Feliz
type LabelName =
    | Prelude
    | Forest
    | Forest2
    | FoxEscape
    | Еpilogue
let mushroom = "mushroom"
let getmushroom (x:Map<_,_>) =
    match x.[mushroom] with
    | Bool x -> x
    | _ -> failwith ""

let princessInTowerScenario =
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

let someScenario =
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

let hedgehogScenario =
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

let scenario =
    [
        label Prelude [

            say "Кот бродит по лесу и натыкается на ведьмочку Макси."

            says [
                "Пытается за ней приударить, но она лишь хихикает:"
                "— Ты ведь кот, пусть и говорящий. Нашел бы себе кошку, а то на ведьм засматриваешься. Совсем глупый, что ли?"
            ]

            say "— А я могу в человека превращаться!"
            say "— Ну-ка, ну-ка."
            say "— Раньше мог, — грустно говорит кот и никнет."

            say "Она дразнит его, за нос водит, а когда кот совсем отчаивается, она предлагает сыграть в игру."

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

        label FoxEscape [
            let startGame winBody loseBody = StartFoxEscapeGame(winBody, loseBody)
            startGame [
                jump Еpilogue
            ] [
                say "todo: Если кот ее всё же словит, то нужно придумать какое-то толковое оправдание, ведь поезд must go on по рельсам."
                jump FoxEscape
            ]
        ]

        label Еpilogue [
            say "Кот пытается ее поймать и так, и этак, но совсем выбивается из сил — ложится на травку и смотрит на закат."
            say "— Эй, хвостатое, ты, правда, умеешь в человека превращаться? Может, ты заколдован или еще чего?"
            say "— Спросила ведьма."
            say "— Не язви. Если бы могла понять природу волшебства, уже бы расколдовала."
            say "Неожиданно она наклоняется над котом и чмокает. Тот довольно мурлыкает, пока она отплевывает шерсть."
            say "— Тьфу, не помогло. Ладно, попробовать стоило. Чего ухмыляешься? Ах ты ж хитрюга!"
            say "— Но я правда..."
            say "Теперь пришел черед коту убегать от разозленной ведьмочки."
            say "Продолжение следует :)"
        ]
    ]

open FsharpMyExtension.ListZipper
let beginLoc = Prelude
let start () =
    let scenario =
        hedgehogScenario
        |> List.map (fun (labelName, body) -> labelName, (labelName, body))
        |> Map.ofList
        : _ Scenario
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