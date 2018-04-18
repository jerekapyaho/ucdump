open System
open System.Unicode

type OffsetCharacterPair = { Offset: int; Character: char }

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

let zipMap f a b = Seq.zip a b |> Seq.map (fun (x,y) -> f x y)

let makePair offset ch = { Offset = offset; Character = ch }

let characterLine pair = 
    sprintf "%08d: U+%06X %s" pair.Offset (codepoint pair.Character) (characterName pair.Character)

let characterLines s =
    let counts = octetCounts (explode s)
    let offsets = List.scan (+) 0 counts
    let pairs = zipMap makePair offsets (Seq.toList s)
    List.map characterLine (Seq.toList pairs)

[<EntryPoint>]
let main argv =
    let s = "I \u2764 F#"
    let result = characterLines s
    result |> Seq.iter (fun line -> printfn "%s" line)
    
    0 // return an integer exit code
