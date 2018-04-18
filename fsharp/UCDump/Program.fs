open System
open System.Unicode

let codepoint (c: char) = int c

let characterName c = 
    let cp = codepoint c
    let info = UnicodeInfo.GetCharInfo cp
    info.Name

let characterNames s = 
    List.map characterName

let octetCount cp =
    match cp with
    | c when c >= 0x000000 && c <= 0x00007f -> 1
    | c when c >= 0x000080 && c <= 0x0007ff -> 2
    | c when c >= 0x000800 && c <= 0x00ffff -> 3
    | c when c >= 0x010000 && c <= 0x10ffff -> 4
    | _ -> 0

let octetCounts s =
    let codepoints = List.map codepoint s
    List.map octetCount codepoints

let explode (s: string) = [for c in s -> c]

[<EntryPoint>]
let main argv =
    let s = "I \u2764 F#"
    explode s |> Seq.iter (fun ch -> printfn "%c = %s (%i octets)" ch (characterName ch) (octetCount (codepoint ch)))
    
    0 // return an integer exit code
