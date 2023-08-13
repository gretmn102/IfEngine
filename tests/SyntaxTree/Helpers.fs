module Tests.SyntaxTree.Helpers
open IfEngine.SyntaxTree

let say (txt: string) =
    Say txt

let interSay (getText: _ -> string) =
    InterpolationSay getText
