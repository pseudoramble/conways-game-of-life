# Conway's Game of Life

A classic CS game to explore cellular automata. The inventor of Game of Life, John Conway, passed away on April 11th, 2020. This is my very primitive implementation of said game.

![Simple glider demo](./demo.gif)

## Running

Install .NET Core 3.x. Clone this repo. From the command line:

    dotnet run

If you want to try another sample, specify it as the 1st argument:

    dotnet run -- samples/blinker.txt

Feel free to add your own samples into the samples folder.

## Assumptions/Limitations

1. The code assumes the inputs will be perfectly square (you can't have a longer dimension).
2. Printing is slow. It's controlled by clearing the console and pausing the thread for a bit.
3. No input validation. You can give it junk values and it will probably just explode.
4. When the cell state goes off the edge of the board, it can get locked into a state where some cells are alive and never change or go off the board.
5. No tests. The inputs I have seem to work, but it's not very good.