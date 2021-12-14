<h1>Hypnonema 🎬 🎥🍿</h1>
<h5>Cinema Resource for FiveM</h5>

![GitHub Repo stars](https://img.shields.io/github/stars/thiago-dev/fivem-hypnonema?style=social)
![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/thiago-dev/fivem-hypnonema?include_prereleases&style=flat-square)
![GitHub all releases](https://img.shields.io/github/downloads/thiago-dev/fivem-hypnonema/total?style=flat-square)
[![CC BY-NC-SA 4.0][cc-by-nc-sa-shield]][cc-by-nc-sa]
![GitHub issues](https://img.shields.io/github/issues-raw/thiago-dev/fivem-hypnonema?style=flat-square)


[![Hypnonema](https://raw.githubusercontent.com/thiago-dev/fivem-hypnonema/gh-pages/HypnonemaTitle.jpg)](https://raw.githubusercontent.com/thiago-dev/fivem-hypnonema/gh-pages/HypnonemaTitle.jpg "Hypnonema")
## Table of Contents

- [Table of Contents](#table-of-contents)
- [Features](#features)
- [Demo](#demo)
- [Requirements](#requirements)
  - [ACE-Permissions](#ace-permissions)
  - [(Optional) Example Map](#optional-example-map)
- [Customization](#customization)
  - [Change command](#change-command)
- [License](#license)
- [Support](#support)

## Features
- Synchronized
- All HTML5 supported audio and video types including HLS/DASH plus Twitch / YouTube / DailyMotion / Facebook / Vimeo / Streamable / Vidme / Wistia / SoundCloud
- Multi-Screen capable (watch multiple movies at once)
- 3D Spatialized Audio
- NUI Frontend
- ACE-Permissions
- Live-Edit Feature (simplifies scaleform placement)

## Demo
[Click me](https://youtu.be/JckYo8bKdnE)

## Requirements
- [FiveM](https://fivem.net)

### ACE-Permissions
To permit users to make use of the command make sure to edit your `server.cfg`
```
add_ace identifier.steam:steamidhere "command.hypnonema" allow
```

### (Optional) Example Map
There is an example map included, just copy the folder **hypnonema-map** to your resource directory and paste following line into your `server.cfg`
```
start hypnonema-map
``` 


## Customization
### Change command
If you want to use your own command just edit the `fxmanifest.lua`. Don't add preceding slashes or any special characters.

**Hint:** Dont forget to adjust the ACE-Permissions (in case of change) to match your new command name!
```
hypnonema_command_name 'mynewcommand'
```
## License
This work is licensed under a
[Creative Commons Attribution-NonCommercial 4.0 International License][cc-by-nc-sa]

[![CC BY-NC 4.0][cc-by-nc-sa-image]][cc-by-nc-sa]

## Support
Please use the [fivem-thread](https://forum.fivem.net/t/release-hypnonema-a-cinema-resource-update-now-with-twitch-support-c/783324) for support.

[cc-by-nc-sa-image]: https://licensebuttons.net/l/by-nc-sa/4.0/88x31.png

[cc-by-nc-sa]: http://creativecommons.org/licenses/by-nc-sa/4.0/
[cc-by-nc-sa-shield]: https://img.shields.io/badge/License-CC%20BY--NC--SA%204.0-lightgrey.svg
