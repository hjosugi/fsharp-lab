// Run with: dotnet fsi labs/00-fsharp-basics.fsx

let square value = value * value

let squares =
    [ 1..10 ]
    |> List.map square

let evenSquares =
    squares
    |> List.filter (fun value -> value % 2 = 0)

let total = evenSquares |> List.sum

printfn "Squares: %A" squares
printfn "Even squares: %A" evenSquares
printfn "Total: %d" total

// Output task:
// 1. Add cube.
// 2. Compose square and string conversion.
// 3. Explain the type signature of every function.

