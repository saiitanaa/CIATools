use rand::Rng;

const TITLE_ID_PRE: &str = "000400000";
const TITLE_ID_POST: &str = "00";
const TITLE_ID_MIN: u32 = 0x300;
const TITLE_ID_MAX: u32 = 0xF7FFF;

pub fn generate() -> String {
	let mut rng = rand::thread_rng();
	let game_id = rng.gen_range(TITLE_ID_MIN..=TITLE_ID_MAX);
	format!("{TITLE_ID_PRE}{game_id:05X}{TITLE_ID_POST}")
}