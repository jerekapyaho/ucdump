open System
open System.Unicode

let codepoint (c: char) = int c

let characterName c = 
    let cp = codepoint c
    let info = UnicodeInfo.GetCharInfo cp
    info.Name

[<EntryPoint>]
let main argv =
    let ch = '#'
    printfn "%c = %s" ch (characterName ch)

    0 // return an integer exit code
