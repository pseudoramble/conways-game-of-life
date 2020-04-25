namespace UI

module Player =
  open GameOfLife
  open Avalonia.Controls
  open Avalonia.Controls.Primitives
  open Avalonia.FuncUI.DSL
  open Avalonia.FuncUI.Types

  type State = {
    board: GameOfLife.BoardState [,]
    file: string
    interval: int
    isPlaying: bool
  }

  type Msg = 
  | Load of string
  | Next
  | Play
  | Pause
  | SetInterval of int

  let init initialBoard =
    {
        board = GameOfLife.loadBoard initialBoard
        file = initialBoard
        isPlaying = false
        interval = 200
    }

  let update (message: Msg) (state: State) : State =
    match message with
    | Next -> { state with board = GameOfLife.nextBoard state.board }
    | Play -> { state with isPlaying = true; }
    | Pause -> { state with isPlaying = false }
    | Load f -> { state with file = f; board = GameOfLife.loadBoard f }
    | SetInterval time -> { state with interval = time }
  
  let toBoardCell i j (cell: GameOfLife.BoardState) =
    let cellText =
      match cell with
      | GameOfLife.BoardState.Alive -> "*"
      | _ -> ""
    
    Canvas.create [
      Grid.row i
      Grid.column j
      Canvas.width 26.0
      Canvas.height 26.0
      Canvas.verticalAlignment Avalonia.Layout.VerticalAlignment.Stretch
      Canvas.horizontalAlignment Avalonia.Layout.HorizontalAlignment.Center
      Canvas.children [
        TextBlock.create [
          TextBlock.text cellText
        ]
      ]
    ]

  let view (state: State) (gameList) (dispatch) =
    let trimmedWidth = (Array2D.length1 state.board) - 2
    let trimmedHeight = (Array2D.length2 state.board) - 2
    let boardCells =
      Array2D.initBased 1 1 trimmedWidth trimmedHeight (fun x y -> state.board.[x, y])
      |> Array2D.mapi toBoardCell
      |> Seq.cast<IView>

    DockPanel.create [
      DockPanel.verticalAlignment Avalonia.Layout.VerticalAlignment.Stretch
      DockPanel.children [
        UniformGrid.create [
          UniformGrid.dock Dock.Top
          UniformGrid.columns trimmedWidth
          UniformGrid.rows trimmedHeight
          UniformGrid.children (boardCells |> Seq.toList)
        ]
        StackPanel.create [
          StackPanel.dock Dock.Bottom
          StackPanel.children [
              Button.create [
                Button.dock Dock.Bottom
                Button.margin 1.0
                Button.content "Next >"
                Button.onClick (fun _ -> dispatch Next)
              ]
              if state.isPlaying
              then
                Button.create [
                  Button.dock Dock.Bottom
                  Button.content "Pause ||"
                  Button.onClick (fun _ -> dispatch Pause)
                ]
              else
                Button.create [
                  Button.dock Dock.Bottom
                  Button.content "Play |>"
                  Button.onClick (fun _ -> dispatch Play)
                ]
              TextBox.create [
                TextBox.text "200"
                TextBox.onTextChanged (fun t ->
                  let time =
                    if t <> "" && not (isNull t)
                    then int t
                    else 200

                  dispatch (SetInterval time)
                )
              ]
              ComboBox.create [
                ComboBox.dataItems gameList
                ComboBox.selectedIndex 0
                ComboBox.onSelectedItemChanged (fun o -> dispatch (Load (string o)))
              ]
          ]
        ]
      ]
    ]