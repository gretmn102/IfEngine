namespace IfEngine.SyntaxTree.CommonContent
open IfEngine
open Farkdown.Experimental.SyntaxTree

type Content = Document

type Block<'Label,'CustomStatement> = SyntaxTree.Block<Content,'Label,'CustomStatement>

type Choice<'Label,'CustomStatement> = SyntaxTree.Choice<Content,'Label,'CustomStatement>

type Choices<'Label,'CustomStatement> = SyntaxTree.Choices<Content,'Label,'CustomStatement>

type Stmt<'Label,'CustomStatement> = SyntaxTree.Stmt<Content,'Label,'CustomStatement>

type Scenario<'Label, 'CustomStatement> when 'Label : comparison =
    SyntaxTree.Scenario<Content, 'Label, 'CustomStatement>

module Helpers =
    open IfEngine.SyntaxTree

    val say: Content -> Stmt<'Label, 'CustomStatement>

    val interSay: getContent: (VarsContainer -> Content) -> Stmt<'Label,'CustomStatement>
