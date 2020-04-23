namespace UI

module Board =
    open GameOfLife
    open Avalonia.Controls
    open Avalonia.Controls.Primitives
    open Avalonia.FuncUI.DSL
    open Avalonia.FuncUI.Types

    type State = { board: GameOfLife.BoardState [,] }

    type Msg = | Next

    let init =
        let initialBoardFilename = "../samples/glider.txt"
        { board = GameOfLife.setupInitialBoard initialBoardFilename }

    let update (message: Msg) (state: State) : State =
        match message with
        | Next -> { state with board = GameOfLife.nextBoard state.board }
    
    let toBoardCell i j (cell: GameOfLife.BoardState) =
        let cellText =
            match cell with
            | GameOfLife.BoardState.Alive -> "*"
            | _ -> ""
        
        Canvas.create [
            Grid.row i
            Grid.column j
            Button.width 40.0
            Button.height 40.0
            Canvas.verticalAlignment Avalonia.Layout.VerticalAlignment.Stretch
            Canvas.horizontalAlignment Avalonia.Layout.HorizontalAlignment.Center
            Canvas.children [
                TextBlock.create [
                    TextBlock.text cellText
                ]
            ]
        ]

    let view (state: State) (dispatch) =
        let trimmedWidth = (Array2D.length1 state.board) - 2
        let trimmedHeight = (Array2D.length2 state.board) - 2
        let boardCells =
            Array2D.initBased 1 1 trimmedWidth trimmedHeight (fun x y -> state.board.[x, y])
            |> Array2D.mapi toBoardCell
            |> Seq.cast<IView>

        DockPanel.create [
            DockPanel.dock Dock.Top
            DockPanel.children [
                UniformGrid.create [
                    UniformGrid.columns trimmedWidth
                    UniformGrid.rows trimmedHeight
                    UniformGrid.dock Dock.Top
                    UniformGrid.children (boardCells |> Seq.toList)
                ]
                Button.create [
                    Button.dock Dock.Bottom
                    Button.content "Next ->"
                    Button.onClick (fun _ -> dispatch Next)
                ]
            ]
        ]