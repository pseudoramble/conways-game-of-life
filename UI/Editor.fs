namespace UI

module Editor =
  open GameOfLife
  open Avalonia.Controls
  open Avalonia.Controls.Primitives
  open Avalonia.FuncUI.DSL
  open Avalonia.FuncUI.Types

  type State = {
    board: GameOfLife.BoardState [,]
    name: string
    size: int
  }

  type Msg = 
  | Load of string
  | Save
  | SetName of string
  | SetSize of int
  | Toggle of int * int *GameOfLife.BoardState

  let cleanupBoard board =
    let trimmedWidth = (Array2D.length1 board) - 2
    let trimmedHeight = (Array2D.length2 board) - 2
    Array2D.initBased 1 1 trimmedWidth trimmedHeight (fun x y -> board.[x, y])

  let init =
    let size = 25
    {
      size = size
      board = GameOfLife.setupInitialBoard size
      name = "new-board"
    }

  let update (message: Msg) (state: State) : State =
    match message with
    | Load name ->
      let board = GameOfLife.loadBoard name
      { state with board = board; size = (Array2D.length1 board) - 2 }
    | SetName name -> { state with name = name }
    | SetSize s -> { state with size = s; board = GameOfLife.setupInitialBoard s }
    | Save -> 
      GameOfLife.storeNewBoard state.name state.board
      
      state
    | Toggle (i, j, cellState) ->
      let nextCellState = 
        match cellState with
        | GameOfLife.BoardState.Alive -> GameOfLife.BoardState.Dead
        | GameOfLife.BoardState.Dead -> GameOfLife.BoardState.Alive
        | _ -> GameOfLife.BoardState.Dead

      let nextBoard = Array2D.copy state.board
      Array2D.set nextBoard i j nextCellState

      { state with board = nextBoard }
  
  let toBoardCell dispatch i j (cell: GameOfLife.BoardState) =
    let cellText =
      match cell with
      | GameOfLife.BoardState.Alive -> "*"
      | _ -> ""

    Button.create [
      Grid.row i
      Grid.column j
      Button.width 20.0
      Button.height 20.0
      Button.margin 2.0
      Button.content cellText
      Button.onClick ((fun _ -> dispatch (Toggle (i, j, cell))), SubPatchOptions.Always)
      Button.verticalAlignment Avalonia.Layout.VerticalAlignment.Center
      Button.horizontalAlignment Avalonia.Layout.HorizontalAlignment.Center
    ]

  let view (state: State) (gameList) (dispatch) =
    let toEditorCell = toBoardCell dispatch
    let boardCells =
      state.board
      |> cleanupBoard
      |> Array2D.mapi toEditorCell
      |> Seq.cast<IView>
    
    StackPanel.create [
      StackPanel.dock Dock.Top
      StackPanel.children [
        TextBlock.create [
          TextBlock.margin 15.0
          TextBlock.text "The grid below shows the state of each cell. A star '*' means the cell is Alive, and otherwise it's Dead. Click on a cell to change its initial state. \nWhen finished, pick a file name and click 'Save'"
        ]
        Grid.create [
          Grid.columnDefinitions "0.5*,*"
          Grid.rowDefinitions "Auto"
          Grid.margin 10.0
          Grid.children [
            TextBlock.create [
              Grid.column 0
              TextBlock.text "Template: "
            ]
            ComboBox.create [
              Grid.column 1
              ComboBox.dataItems gameList
              ComboBox.onSelectedItemChanged (fun i -> dispatch (Load (string i)))
            ]
          ]
        ]
        UniformGrid.create [
          UniformGrid.rows state.size
          UniformGrid.columns state.size
          UniformGrid.dock Dock.Top
          UniformGrid.children (boardCells |> Seq.toList)
          UniformGrid.maxWidth 1024.0
          UniformGrid.maxHeight 1024.0
        ]
        Grid.create [
          Grid.margin 5.0
          Grid.columnDefinitions "0.5*,*,0.5*,*"
          Grid.rowDefinitions "Auto,Auto"
          Grid.children [
            TextBlock.create [
              Grid.row 0
              Grid.column 0
              TextBlock.text "Game name: "
              TextBlock.verticalAlignment Avalonia.Layout.VerticalAlignment.Center
            ]
            TextBox.create [
              Grid.row 0
              Grid.column 1
              Grid.columnSpan 3
              TextBox.text state.name
              TextBox.onTextChanged (fun name -> if name <> "" then dispatch (SetName name))
            ]
            Button.create [
              Grid.row 1
              Grid.columnSpan 4
              Button.margin 5.0
              Button.content "Save"
              Button.onClick (fun _ -> dispatch Save)
            ]
          ]
        ]
      ]
    ]