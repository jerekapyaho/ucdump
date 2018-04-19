open System.IO
open System.Unicode

/// <summary>Gets the Unicode codepoint of the character <c>c</c>.</summary>
/// <param name="c">The character.</param>
/// <returns>The codepoint of the character.</returns>
let codepoint (c: char) = int c

/// <summary>Gets the canonical name of the Unicode character <c>c</c>.</summary>
/// <param name="c">The character.</param>
/// <returns>The name of the character.</returns>
let characterName c = 
    let info = UnicodeInfo.GetCharInfo (codepoint c)
    info.Name

/// <summary>Gets the UTF-8 octet count of the Unicode codepoint <c>cp</c>.</summary>
/// <remarks>See RFC 3629 for details.</remarks>
/// <param name="cp">The codepoint.</param>
/// <returns>The octet count based on the codepoint value.</returns>
let octetCount cp =
    match cp with
    | c when c >= 0x000000 && c <= 0x00007f -> 1
    | c when c >= 0x000080 && c <= 0x0007ff -> 2
    | c when c >= 0x000800 && c <= 0x00ffff -> 3
    | c when c >= 0x010000 && c <= 0x10ffff -> 4
    | _ -> 0

/// <summary>Returns a list of the octet counts in the string <c>s</c>.</summary>
/// <param name="s">The string.</param>
/// <returns>A list of the octet counts.</returns>
let octetCounts s = List.map octetCount (List.map codepoint s)

/// <summary>Explodes the string <c>s</c> into a list of characters.</summary>
/// <param name="s">The string to explode.</param>
/// <returns>A list containing the characters in the string.</returns>
let explode (s: string) = [for c in s -> c]

/// <summary>Zips two lists and applies a function f to the result.</summary>
/// <param name="f">The function to apply.</param>
/// <param name="a">The first list.</param>
/// <param name="b">The second list.</param>
/// <returns>A list.</returns>
let zipMap f a b = Seq.zip a b |> Seq.map (fun (x, y) -> f x y)

/// <summary>Formats a string with the offset, Unicode code point and the character name.</summary>
/// <param name="pair">The <c>OffsetCharacterPair</code> record with the data.</param>
/// <returns>A string describing the character.</returns>
let characterLine pair = 
    let offset, ch = pair
    sprintf "%08d: U+%06X %s" offset (codepoint ch) (characterName ch)

let pair offset ch = (offset, ch)

/// <summary>Returns a list of lines describing each character in the string <c>s</c>.</summary>
/// <param name="s">The string.</param>
/// <returns>A list of line descriptions.</returns>
let characterLines s =
    let counts = octetCounts (explode s)
    let offsets = List.scan (+) 0 counts  // make a list of cumulative offsets
    let pairs = zipMap pair offsets (Seq.toList s)
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
