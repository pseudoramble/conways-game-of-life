open System

type BoardState =
    Alive
    | Dead
    | Edge // Used as a sentinel in the board. Makes less tricky math!

let nextState cell neighbors =
    let totalLivingNeighbors =  Seq.sumBy (fun n -> if n = Alive then 1 else 0) neighbors 
    
    match cell with
    | Alive ->
        match totalLivingNeighbors with
        | 2 | 3 -> Alive
        | _ -> Dead
    | Dead ->
        match totalLivingNeighbors with
        | 3 -> Alive
        | _ -> Dead
    | Edge -> Edge

let nextBoard (board: BoardState [,]) =
    Array2D.mapi (fun i j cell -> 
        match cell with
        | Edge -> Edge
        | _ -> 
            let neighbors = 
                [|
                    Array2D.get board (i - 1) (j - 1)
                    Array2D.get board (i - 1) (j)
                    Array2D.get board (i - 1) (j + 1)
                    Array2D.get board (i) (j - 1)
                    Array2D.get board (i) (j + 1)
                    Array2D.get board (i + 1) (j - 1)
                    Array2D.get board (i + 1) (j)
                    Array2D.get board (i + 1) (j + 1)
                |]
                |> Seq.filter (fun n -> n <> Edge)
            nextState cell neighbors
    ) board

let printBoard board boardSize =
    Array2D.iteri (fun i j cell ->
        let cellPrint =
            match cell with
            | Edge -> " "
            | Dead -> " "
            | Alive -> "*"
        
        if j % boardSize = 0
        then printfn "%4s" cellPrint
        else printf "%4s" cellPrint
    ) board

[<EntryPoint>]
let main argv =
    let blinkerInitBoard = array2D [|
        [| Edge; Edge; Edge; Edge; Edge; Edge; Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Edge |]
        [| Edge; Dead; Alive; Alive; Alive; Dead; Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Edge |]
        [| Edge; Edge; Edge; Edge; Edge; Edge; Edge |]
    |]
    let toadInitBoard = array2D [|
        [| Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Dead; Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Dead; Edge |]
        [| Edge; Dead; Dead; Alive; Alive; Alive; Dead; Edge |]
        [| Edge; Dead; Alive; Alive; Alive; Dead; Dead; Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Dead; Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Dead; Edge |]
        [| Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge |]
    |]
    let gliderInitBoard = array2D  [|
        [| Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge |]
        [| Edge; Dead; Dead; Alive; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Edge |]
        [| Edge; Dead; Dead; Dead; Alive; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Edge |]
        [| Edge; Dead; Alive; Alive; Alive; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead;Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead;Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead;Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead;Edge |]
        [| Edge; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Dead; Edge |]
        [| Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge; Edge |]
    |]
    
    let mutable board = gliderInitBoard
    let boardSize = Array2D.length1 board

    while true do
        System.Console.Clear()
        printBoard board boardSize
        board <- nextBoard board
        Threading.Thread.Sleep(500)

    0