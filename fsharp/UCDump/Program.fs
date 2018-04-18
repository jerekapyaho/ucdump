open System.IO
open System.Unicode

/// Record type to hold a character and its offset in the source
type OffsetCharacterPair = { Offset: int; Character: char }

/// Returns the Unicode codepoint of the character c
let codepoint (c: char) = int c

/// Gets the canonical name of the Unicode character c
let characterName c = 
    let info = UnicodeInfo.GetCharInfo (codepoint c)
    info.Name

/// Returns the UTF-8 octet count of the Unicode codepoint cp
let octetCount cp =
    match cp with
    | c when c >= 0x000000 && c <= 0x00007f -> 1
    | c when c >= 0x000080 && c <= 0x0007ff -> 2
    | c when c >= 0x000800 && c <= 0x00ffff -> 3
    | c when c >= 0x010000 && c <= 0x10ffff -> 4
    | _ -> 0

/// Returns a list of the octet counts in the string s
let octetCounts s = List.map octetCount (List.map codepoint s)

/// Explodes the string s into a list of characters
let explode (s: string) = [for c in s -> c]

/// Zips two lists a and b and applies the function f to the result
let zipMap f a b = Seq.zip a b |> Seq.map (fun (x, y) -> f x y)

/// Makes an OffsetCharacterPair record
let makePair offset ch = { Offset = offset; Character = ch }

/// Formats a string with the offset, Unicode code point and the character name
let characterLine pair = 
    sprintf "%08d: U+%06X %s" pair.Offset (codepoint pair.Character) (characterName pair.Character)

/// Returns a list of lines describing each character in the string s
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
