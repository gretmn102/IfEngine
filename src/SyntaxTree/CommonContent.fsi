namespace IfEngine.SyntaxTree.CommonContent
open IfEngine
open Farkdown.Experimental.SyntaxTree

type Content = Document

type Block<'Label, 'VarsContainer, 'CustomStatement> = SyntaxTree.Block<Content, 'Label, 'VarsContainer, 'CustomStatement>

type Choice<'Label, 'VarsContainer, 'CustomStatement> = SyntaxTree.Choice<Content, 'Label, 'VarsContainer, 'CustomStatement>

type Choices<'Label, 'VarsContainer, 'CustomStatement> = SyntaxTree.Choices<Content, 'Label, 'VarsContainer, 'CustomStatement>

type Stmt<'Label, 'VarsContainer, 'CustomStatement> = SyntaxTree.Stmt<Content, 'Label, 'VarsContainer, 'CustomStatement>

type Scenario<'Label, 'VarsContainer, 'CustomStatement> when 'Label : comparison =
    SyntaxTree.Scenario<Content, 'Label, 'VarsContainer, 'CustomStatement>

module Helpers =
    open IfEngine.SyntaxTree

    val say: Content -> Stmt<'Label, 'VarsContainer, 'CustomStatement>

    val interSay:
        getContent: ('VarsContainer -> Content) ->
            Stmt<'Label, 'VarsContainer, 'CustomStatement>
