namespace UI

/// This is the main module of your application
/// here you handle all of your child pages as well as their
/// messages and their updates, useful to update multiple parts
/// of your application, Please refer to the `view` function
/// to see how to handle different kinds of "*child*" controls
module Shell =
    open System.Timers
    open Elmish
    open Avalonia
    open Avalonia.Controls
    open Avalonia.Input
    open Avalonia.FuncUI.DSL
    open Avalonia.FuncUI
    open Avalonia.FuncUI.Builder
    open Avalonia.FuncUI.Components.Hosts
    open Avalonia.FuncUI.Elmish


    type State =
        /// store the child state in your main state
        { boardState: Board.State; timer: Timer }

    type Msg =
        | BoardMsg of Board.Msg


    module Subscriptions =
        let playing (timer: Timer) =
            let sub dispatch =
                timer.Elapsed.AddHandler(fun _ _ -> dispatch (BoardMsg Board.Msg.Next))
                
            Cmd.ofSub sub

    let init timer =
        let boardState = Board.init
        { boardState = boardState; timer = timer; },
        /// If your children controls don't emit any commands
        /// in the init function, you can just return Cmd.none
        /// otherwise, you can use a batch operation on all of them
        /// you can add more init commands as you need
        Cmd.none
        // Cmd.batch [ aboutCmd ]

    let update (msg: Msg) (state: State): State * Cmd<_> =
        match msg with
        | BoardMsg boardMsg ->
            let boardState = Board.update boardMsg state.boardState
            
            match boardMsg with
            | Board.Msg.Play _ ->
                state.timer.Interval <- double boardState.interval
                state.timer.Enabled <- true
            | Board.Msg.Pause ->
                state.timer.Enabled <- false
            | _ -> ()

            { state with boardState = boardState }, Cmd.none

    let view (state: State) (dispatch) =
        DockPanel.create [
            DockPanel.children [
                (Board.view state.boardState (BoardMsg >> dispatch))
            ]
        ]

    /// This is the main window of your application
    /// you can do all sort of useful things here like setting heights and widths
    /// as well as attaching your dev tools that can be super useful when developing with
    /// Avalonia
    type MainWindow() as this =
        inherit HostWindow()
        do
            base.Title <- "Conway's Game of Life"
            base.Width <- 1024.0
            base.Height <- 768.0
            base.MinWidth <- 800.0
            base.MinHeight <- 600.0

            //this.VisualRoot.VisualRoot.Renderer.DrawFps <- true
            //this.VisualRoot.VisualRoot.Renderer.DrawDirtyRects <- true

            let timer = new Timer()
            timer.AutoReset <- true
            timer.Enabled <- false

            Elmish.Program.mkProgram (fun () -> init timer) update view
            |> Program.withHost this
            |> Program.withSubscription (fun _ -> Subscriptions.playing timer)
            |> Program.run
