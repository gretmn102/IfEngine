namespace IfEngine.SyntaxTree

type Narrator =
    {
        Name: string
        AvatarUrl: string
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Narrator =
    val create: name: string -> avatarUrl: string -> Narrator

type NarratorCommonContent =
    {
        Narrator: Narrator option
        Content: CommonContent.Content
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module NarratorCommonContent =
    val create: narrator: Narrator option -> content: CommonContent.Content -> NarratorCommonContent

    val createSay: content: CommonContent.Content -> Stmt<NarratorCommonContent, 'Label, 'CustomStatement>

    val createNarratorSay:
        narrator: Narrator -> content: CommonContent.Content -> Stmt<NarratorCommonContent, 'Label, 'CustomStatement>

    val createMenu:
        caption: CommonContent.Content ->
        choices: Choices<NarratorCommonContent, 'Label, 'CustomStatement> ->
            Stmt<NarratorCommonContent, 'Label, 'CustomStatement>

    val createNarratorMenu:
        narrator: Narrator ->
        caption: CommonContent.Content ->
        choices: Choices<NarratorCommonContent, 'Label, 'CustomStatement> ->
            Stmt<NarratorCommonContent, 'Label, 'CustomStatement>
