namespace GameOfLife

module GameOfLife =
  open System
  open System.IO

  type BoardState =
  | Alive
  | Dead
  | Edge // Used as a sentinel in the board. Makes less tricky math!

  let nextState cell neighbors =
    let totalLivingNeighbors = 
      Seq.sumBy (fun n -> if n = Alive then 1 else 0) neighbors 
    
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

  let loadBoard fileName =
    let fileLines = System.IO.File.ReadAllLines (sprintf "./samples/%s" fileName)
    let edgeRow =
      Array.init (Array.length fileLines + 2) (fun _ -> "e")
      |> (fun er -> String.Join(",", er))
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
    |> Array2D.map (fun cell ->
      match cell with| "a" | "A" -> Alive | "d" | "D" -> Dead | _ -> Edge)

  let getGameList () = 
    Directory.GetFiles("./samples")
    |> Array.map Path.GetFileName
      
  let setupInitialBoard size =
    let actualSize = size + 2
    Array2D.init actualSize actualSize (fun i j ->
      if i = 0 || i = (actualSize - 1)
      then Edge
      elif j = 0 || j = (actualSize - 1)
      then Edge
      else Dead
    )

  let storeNewBoard name board =
    let fileName = sprintf "./samples/%s" name
    let fileContents = new StringWriter()
    let cols = Array2D.length1 board
    let stringyBoard =
      Array2D.map (fun cell -> match cell with Alive -> "a" | Dead -> "d" | _ -> "") board
    Array2D.iteri (fun _ j cell ->
      if cell <> ""
      then
        let contentToWrite = 
          if j = cols - 2
          then sprintf "%s\n" cell
          else sprintf "%s," cell

        fileContents.Write(contentToWrite)
    ) stringyBoard

    File.WriteAllText(fileName, fileContents.ToString().Trim())

  // [<EntryPoint>]
  // let main argv =
  //     let customFileIndex =
  //         Array.tryFindIndex (fun arg -> arg = "--file" || arg = "-f") argv
  //         |> Option.defaultValue -1
  //     let initialBoardFilename = 
  //         if customFileIndex > -1
  //         then argv.[customFileIndex + 1]
  //         else "samples/glider.txt"
  //     let isInteractive = Array.exists (fun arg -> arg = "-i" || arg = "--interactive") argv

  //     let mutable board = loadBoard initialBoardFilename
  //     let boardSize = Array2D.length1 board

  //     while true do
  //         System.Console.Clear()
  //         printBoard board boardSize
  //         board <- nextBoard board

  //         if isInteractive
  //         then System.Console.ReadLine() |> ignore
  //         else Threading.Thread.Sleep(100)

  //     0