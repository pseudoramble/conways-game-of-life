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
            | Edge -> ""
            | Dead -> " "
            | Alive -> "*"

        if j % boardSize = 0
        then printfn "%4s" cellPrint
        else printf "%4s" cellPrint
    ) board

let setupInitialBoard fileName =
    let fileLines = System.IO.File.ReadAllLines fileName
    let edgeRow = Array.init (Array.length fileLines + 2) (fun _ -> "e") |> (fun er -> String.Join(",", er))
    let inputRows =
        fileLines
        |> Array.map (fun ir -> sprintf "e,%s,e" ir)
        |> Array.append [| edgeRow |]
        |> Array.rev
        |> Array.append [| edgeRow |]
        |> Array.rev
        |> Array.map (fun r -> r.Split [| ',' |])

    inputRows
    |> array2D
    |> Array2D.map (fun cell -> match cell with | "a" | "A" -> Alive | "d" | "D" -> Dead | _ -> Edge)

[<EntryPoint>]
let main argv =
    let initialBoardFilename = 
        if Array.length argv > 0
        then argv.[0]
        else "samples/glider.txt"
    printfn "%s" initialBoardFilename

    let mutable board = setupInitialBoard initialBoardFilename
    let boardSize = Array2D.length1 board

    while true do
        System.Console.Clear()
        printBoard board boardSize
        board <- nextBoard board
        Threading.Thread.Sleep(100)

    0