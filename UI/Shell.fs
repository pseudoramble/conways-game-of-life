namespace UI

/// This is the main module of your application
/// here you handle all of your child pages as well as their
/// messages and their updates, useful to update multiple parts
/// of your application, Please refer to the `view` function
/// to see how to handle different kinds of "*child*" controls
module Shell =
  open System.Timers
  open Elmish
  open Avalonia.Controls
  open Avalonia.FuncUI.DSL
  open Avalonia.FuncUI
  open Avalonia.FuncUI.Components.Hosts
  open Avalonia.FuncUI.Elmish

  type State = {
    editorState: Editor.State;
    gameList: string array
    playerState: Player.State;
    timer: Timer
  }

  type Msg =
  | EditorMsg of Editor.Msg
  | PlayerMsg of Player.Msg

  module Subscriptions =
    let playing (timer: Timer) =
      let sub dispatch =
        timer.Elapsed.AddHandler(fun _ _ -> dispatch (PlayerMsg Player.Msg.Next))
        
      Cmd.ofSub sub

  let init timer =
    let gameList = GameOfLife.GameOfLife.getGameList ()
    let firstGame = if (Array.isEmpty gameList) then "toad" else Array.head gameList
    let state = {
      editorState = Editor.init;
      gameList = gameList;
      playerState = Player.init firstGame;
      timer = timer;
    }

    (state, Cmd.none) // Cmd.batch [ aboutCmd ]

  let update (msg: Msg) (state: State): State * Cmd<_> =
    match msg with
    | EditorMsg editorMsg ->
      let editorState = Editor.update editorMsg state.editorState

      { 
        state with
          editorState = editorState;
          gameList = GameOfLife.GameOfLife.getGameList ()
      }, Cmd.none
    | PlayerMsg boardMsg ->
      let playerState = Player.update boardMsg state.playerState
    
      match boardMsg with
      | Player.Msg.Play _ ->
        state.timer.Interval <- double playerState.interval
        state.timer.Enabled <- true
      | Player.Msg.Pause ->
        state.timer.Enabled <- false
      | _ -> ()

      { state with playerState = playerState }, Cmd.none

  let view (state: State) (dispatch) =
    DockPanel.create [
      DockPanel.verticalAlignment Avalonia.Layout.VerticalAlignment.Stretch
      DockPanel.children [
        TabControl.create [
          TabControl.viewItems [
            TabItem.create [
              TabItem.header "Player"
              TabItem.content (
                Player.view state.playerState state.gameList (PlayerMsg >> dispatch)
              )
            ]
            TabItem.create [
              TabItem.header "Editor"
              TabItem.content (
                Editor.view state.editorState state.gameList (EditorMsg >> dispatch)
              )
            ]
          ]
        ]
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
      base.Width <- 1000.0
      base.Height <- 800.0
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