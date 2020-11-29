module Either

[<Struct>]
type Either<'a,'b> =
    | Left of Left : 'a
    | Right of Right : 'b

module Either =
    let empty = Right

    let map fn = function
        | Right x -> Right(fn x)
        | Left x -> Left x
    /// `either (f >> Left) (g >> Right)`
    let mapBoth f g = function
        | Right x -> Right(g x)
        | Left x -> Left(f x)
    let mapLeft f = function Left x -> Left(f x) | Right x -> Right x
    let iter fn = function
        | Right x -> fn x
        | Left _ -> ()
    let fold fn state = function
        | Right x -> fn state x
        | _ -> state
    let bind fn = function
        | Right x -> fn x
        | Left x -> Left x
    let liftA2 fn x y =
        match x, y with
        | Right x, Right y -> fn x y |> Right
        | Left x, _ -> Left x
        | _, Left x -> Left x
    let ap (f : Either<'a, ('b -> 'c)>) xs =
        match f, xs with
        | Right x, Right y -> Right(x y)
        | Left x, _ -> Left x
        | _, Left x -> Left x
    /// Case analysis for the Either type. If the value is Left a, apply the first function to a; if it is Right b, apply the second function to b.
    let either f g = function
        | Right x -> g x
        | Left x -> f x

    let ofOption s = function None -> Left s | Some x -> Right x
    let ofOptionWith s = function None -> Left(s()) | Some x -> Right x

    /// непохоже на sequenceA, ибо `f<t, f<x>> -> t<f<x>>` и результат не равен `fmap concat . sequenceA`
    let seqEitherPseudo xs = either (Left >> Seq.singleton) id xs

    let listEitherPseudo xs = either (Left >> List.singleton) id xs
    let collect f = map f >> seqEitherPseudo

    let getOrDef x = function Right x -> x | _ -> x
    let getOrElse f = function
        | Right x -> x
        | Left _ -> f()
    let getOrDef' fn = function Right x -> x | Left x -> fn x
    let orElse x = function
        | Right x -> Right x
        | Left _ -> x
    let orElseWith f = function
        | Right x -> Right x
        | Left _ -> f()
    let get = function Right x -> x | x -> failwithf "try get right, but '%A'" x
    let getLeft = function Left x -> x | x -> failwithf "try get left, but '%A'" x
    let isRight = function Right _ -> true | _ -> false
    let isLeft = function Left _ -> true | _ -> false

    let concat x = bind id x

    let travOpt (rf: 'a -> option<'b>) (x : Either<'c,'a>) : option<Either<'c,'b>> =
        match x with
        | Left x -> Some (Left x)
        | Right x ->  (rf >> Option.map Right) x
    let seqOpt x = travOpt id x
