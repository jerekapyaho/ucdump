use std::env;
use std::fs;
use std::io::Read;
use charname;

fn main() {
    let args: Vec<String> = env::args().collect();
    let file_path = &args[1];
    if let Some(buffer) = read_file(file_path) {
        println!("Got {} bytes of data from '{}'", buffer.len(), file_path);

        let mut offset = 0;
        let mut processed_count = 0;
        while offset < buffer.len() {
            print!("{:08X}: ", offset);
            let b = buffer[offset];
            if let Some(octet_count) = get_octet_count(b as u32) {
                let codepoint: u32 = match octet_count {
                    1 => b as u32,
                    2 => u32::from_be_bytes([0x00, 0x00, b, buffer[offset + 1]]),
                    3 => u32::from_be_bytes([0x00, b, buffer[offset + 1], buffer[offset + 2]]),
                    4 => u32::from_be_bytes([b, buffer[offset + 1], buffer[offset + 3], buffer[offset + 4]]),
                };
                print!("{:04X}", codepoint);
                offset += octet_count;
            }
            else {
                panic!("Invalid UTF-8 character in file at offset {:08X}", offset);
            }
        }
    }
}

fn read_file(name: &String) -> Option<Vec<u8>> {
    match fs::File::open(&name) {
        Ok(mut f) => {
            let mut buffer = Vec::new();
            match f.read_to_end(&mut buffer) {
                Ok(_) => Some(buffer),
                Err(_) => None
            }
        },
        Err(_) => {
            eprintln!("Unable to open file {}", &name);
            None
        }
    }
}

// Determines the length of a Unicode codepoint when encoded in UTF-8.
// See RFC 3629 for the details.
fn get_octet_count(codepoint: u32) -> Option<usize> {
    match codepoint {
        0x000000 ..= 0x00007F => Some(1),
        0x000080 ..= 0x0007FF => Some(2),
        0x000800 ..= 0x00FFFF => Some(3),
        0x010000 ..= 0x10FFFF => Some(4),
        _ => None
    }
}
