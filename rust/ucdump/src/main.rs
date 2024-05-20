use std::env;
use std::fs;
use charname;

fn main() {
    let args: Vec<String> = env::args().collect();
    let file_path = &args[1];
    let contents = fs::read_to_string(file_path)
        .expect("Should have been able to read the file");
    for (position, character) in contents.char_indices() {
        println!("{:08}: U+{:06X} {}",
            position,
            character as u32,
            charname::get_name(character as u32));
    }
}
