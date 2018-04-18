open System.IO
open System.Unicode

type OffsetCharacterPair = { Offset: int; Character: char }

let codepoint (c: char) = int c

let characterName c = 
    let info = UnicodeInfo.GetCharInfo (codepoint c)
    info.Name

let octetCount cp =
    match cp with
    | c when c >= 0x000000 && c <= 0x00007f -> 1
    | c when c >= 0x000080 && c <= 0x0007ff -> 2
    | c when c >= 0x000800 && c <= 0x00ffff -> 3
    | c when c >= 0x010000 && c <= 0x10ffff -> 4
    | _ -> 0

let octetCounts s = List.map octetCount (List.map codepoint s)

let explode (s: string) = [for c in s -> c]

let zipMap f a b = Seq.zip a b |> Seq.map (fun (x, y) -> f x y)

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
    match argv with
    | [|filename|] -> 
        let fileContents = File.ReadAllText(filename)
        let result = characterLines fileContents
        result |> Seq.iter (fun line -> printfn "%s" line)
    | _ -> printf "Need a filename"
    
    0 // return an integer exit code
