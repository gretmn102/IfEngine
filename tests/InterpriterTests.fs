module InterpriterTests
open Fuchu

open IfEngine
open IfEngine.SyntaxTree
open IfEngine.SyntaxTree.Helpers

open Tests.SyntaxTree.Helpers

[<Tests>]
let InterpTests =
    let mock: Block<_,_,unit,unit> =
        [
            say "0"
            say "1"
            menu "2" [
                choice "0" [
                    say "0"
                    say "1"
                ]
                choice "1" [
                    say "0"
                    if' (fun _ -> true) [
                        say "0"
                        say "1"
                        say "2"
                    ] [
                        say "0"
                    ]
                ]
            ]
            say "3"
        ]

    testList "InterpTests" [
        testCase "base" <| fun () ->
            let act =
                [
                    StatementIndexInBlock.BlockStatement(2, 1)
                    StatementIndexInBlock.BlockStatement(1, 0)
                    StatementIndexInBlock.SimpleStatement 10
                ]
                |> List.rev
                |> BlockStack.ofStack (fun subIndex x -> failwithf "handleCustomStatement %A" x) mock
                |> Result.map (
                    BlockStack.next
                    >> Option.get
                    >> BlockStack.toStack
                    >> List.rev
                )

            let exp: Stack = [StatementIndexInBlock.SimpleStatement 3]

            Assert.Equal("", Ok exp, act)
    ]
