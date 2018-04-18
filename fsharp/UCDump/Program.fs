open System
open System.Unicode

let codepoint (c: char) = int c

let characterName c = 
    let cp = codepoint c
    let info = UnicodeInfo.GetCharInfo cp
    info.Name

let characterNames s = 
    List.map characterName

let explode (s: string) = [for c in s -> c]

[<EntryPoint>]
let main argv =
    let s = "F#"
    explode s |> Seq.iter (fun ch -> printfn "%c = %s" ch (characterName ch))
    
    0 // return an integer exit code
