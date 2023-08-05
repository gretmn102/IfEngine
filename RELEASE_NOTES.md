## 1.2.0
* feat: add `Utils.switch`
* breaking: rename `Types.Vars` to `Types.VarsContainer`
* breaking: make `Types.Var` require access qualified access
* fix: scroll `NextState`'s in `Engine.create`
* feat: add the `IVar` interface and create var types using it

## 1.1.1
* fix: fable: .fsi files missing

## 1.1.0
* breaking: rename `Interpreter.Command` to `Interpreter.AbstractEngine`
* breaking: move `Interpreter.next`, `Interpreter.down` and `Interpreter.interp` to `AbstractEngine`
* breaking: make `Interpreter.AbstractEngine` require access qualified access
* breaking: make `Interpreter.StatementIndexInBlock` require access qualified access
* breaking: move all from `IfEngine.Interpreter` to outside
* breaking: rename `StackStatements` to `BlockStack`
* breaking: rename `LabelState` to `NamedStack`
* breaking: remove `changeState` arg in `AbstractEngine.next` function
* breaking: loop `AbstractEngine` via `NextState of 'State -> (unit -> AbstractEngine)`
* add `Engine` as a replacement for `IfEngine.Game`
* breaking: rename `Label` on `NamedBlock`

## 1.0.1
* make FSharp.Core and FSharpMyExt any version restriction

## 1.0.0
* release
