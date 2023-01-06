module InterpriterTests
open Fuchu

open IfEngine.Types
open IfEngine.Interpreter
open IfEngine.Utils

let say (txt: int) =
    Say txt

[<Tests>]
let InterpTests =
    let mock: Block<_, unit, unit> =
        [
            say 0
            say 1
            menu 2 [
                choice "0" [
                    say 0
                    say 1
                ]
                choice "1" [
                    say 0
                    if' (fun _ -> true) [
                        say 0
                        say 1
                        say 2
                    ] [
                        say 0
                    ]
                ]
            ]
            say 3
        ]

    testList "InterpTests" [
        testCase "base" <| fun () ->
            let act =
                [
                    BlockStatement(2, 1)
                    BlockStatement(1, 0)
                    SimpleStatement 10
                ]
                |> List.rev
                |> StackStatements.ofStack (fun subIndex x -> failwithf "handleCustomStatement %A" x) mock
                |> Result.map (
                    StackStatements.next
                    >> Option.get
                    >> StackStatements.toStack
                    >> List.rev
                )

            let exp: Stack = [SimpleStatement 3]

            Assert.Equal("", Ok exp, act)
    ]
