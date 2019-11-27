<h1>Hypnonema 🎬 🎥🍿</h1>
<h5>FiveM Cinema Resource</h5>

Hypnonema is a Cinema Resource for FiveM.

[![Hypnonema](https://raw.githubusercontent.com/thiago-dev/fivem-hypnonema/gh-pages/HypnonemaTitle.jpg)](https://raw.githubusercontent.com/thiago-dev/fivem-hypnonema/gh-pages/HypnonemaTitle.jpg "Hypnonema")
## Table of Contents

- [Table of Contents](#table-of-contents)
- [Features](#features)
- [Demo](#demo)
- [Requirements](#requirements)
  - [ACE-Permissions](#ace-permissions)
  - [(Optional) Example Map](#optional-example-map)
    - [Required Screen values](#required-screen-values)
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
[Click me](https://youtu.be/NQZf-pyKeNg)

## Requirements
- [FiveM](https://fivem.net)

### ACE-Permissions
To permit users to make use of the command make sure to edit your **server.cfg**
```
add_ace identifier.steam:steamidhere "command.hypnonema" allow
```

### (Optional) Example Map
There is an example map included, just copy the folder **hypnonema-map** to your resource directory and add *start hypnonema-map* to your server.cfg.
#### Required Screen values
The corresponding values for the screen at the example-map are as follows:
```
positionX: -1678.949

positionY: -928.3431

positionZ: 20.6290932

rotationX: 0

rotationY: 0

rotationZ: -140.0

scaleX: 0.969999969

scaleY: 0.484999985

scaleZ: -0.1
```

## Customization
### Change command
If you want to use your own command just edit the __resource.lua. Don't add preceding slashes or any special characters.
```
hypnonema_command_name 'mynewcommand'
```
## License

This project is licensed under the MIT License - see [LICENSE](LICENSE.md) file for details

## Support
Please use the [fivem-thread](https://forum.fivem.net/t/release-hypnonema-a-cinema-resource-update-now-with-twitch-support-c/783324) for support.