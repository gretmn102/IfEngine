namespace IfEngine.SyntaxTree

type Narrator =
    {
        Name: string
        AvatarUrl: string
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Narrator =
    let create name avatarUrl : Narrator =
        {
            Name = name
            AvatarUrl = avatarUrl
        }

type NarratorCommonContent =
    {
        Narrator: Narrator option
        Content: CommonContent.Content
    }
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module NarratorCommonContent =
    let create narrator content =
        {
            Narrator = narrator
            Content = content
        }

    let createSay content : Stmt<NarratorCommonContent, 'Label, 'V, 'CustomStatement> =
        Say (create None content)

    let createNarratorSay narrator content : Stmt<NarratorCommonContent, 'L, 'V, 'CS> =
        Say (create (Some narrator) content)

    let createMenu caption choices : Stmt<NarratorCommonContent, 'L, 'V, 'CS> =
        Menu(
            create None caption,
            choices
        )

    let createNarratorMenu narrator caption choices : Stmt<NarratorCommonContent, 'L, 'V, 'CS> =
        Menu(
            (create
                (Some narrator)
                caption
            ),
            choices
        )
