# msz-dialogue-skip

Skip dialogue with space.

Doesn't skip animations currently.

Report bugs on the [issues](https://github.com/Gameknight963/msz-dialogue-skip/issues) page

## Installation

 - Download Melonloader installer from [here](https://melonwiki.xyz/).
 - Point it to the folder containing your msz installation
 - Once it's installed, put it in the Mods/ folder (if it doesn't exist, create it)

## Configuration

Space key may conflict with other mods

You can change the key to any key you like by editing the file at 
'Mods/msz dialogue skip/skip_key.cfg'. Type in the name of the key you want, eg: "A" or "Backspace."

It's generated if it doesn't exist, read automatically at initialization, and after that,
watched with a `FileSystemWatcher` so changes apply right away
