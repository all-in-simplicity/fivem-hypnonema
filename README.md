<h1>Hypnonema 🎬 🎥🍿</h1>
<h5>FiveM Cinema Resource</h5>

Hypnonema is a Cinema Resource for FiveM.

[![Hypnonema](https://i.imgur.com/fgkUFzN.png)](https://youtu.be/SkuMk_0vPp8 "Hypnonema")
## Table of Contents

- [Table of Contents](#table-of-contents)
- [Features](#features)
- [Requirements](#requirements)
- [Setup](#setup)
  - [ACE-Permissions](#ace-permissions)
  - [HTTP-Server](#http-server)
  - [(Optional) Example Map](#optional-example-map)
- [Usage](#usage)
- [Customization](#customization)
  - [Change Splash-Screen (video-poster)](#change-splash-screen-video-poster)
  - [Change command](#change-command)
- [License](#license)
- [Support](#support)

## Features
- Synchronized
- Plays YouTube Videos
- Volume calculation configurable (Attenuation, Min-Distance for now)
- NUI Frontend
- ACE-Permissions
- **(advanced)** Scaleform placement configurable
## Requirements
- [FiveM](https://fivem.net)
- HTTP-Server*
  
**Hint: There is a minimal http server for testing purposes included.*

## Setup

Copy the **index.html** inside **wwwroot** to a directory on your webserver and edit the __resource.lua so that hypnonema_url points to the location on your webserver.
```
hypnonema_url 'https://WEBSERVERADDRESS/hypnonema'
```

Place "hypnonema" folder inside your fivem resources directory and append following line to your server.cfg
```bash
start hypnonema
```

### ACE-Permissions
By default only admins are allowed to execute the ```/hypnonema``` command. To permit additional users to make use of the command make sure to edit your **server.cfg**
```
add_ace identifier.steam:steamidhere "command.hypnonema" allow
```

### HTTP-Server
**Do not use this for production!**
If you are working locally or just for testing purposes, you may enable the minimal HTTP-Server with editing **__resource.lua**
```lua
-- the url to your webserver containing the corresponding html file for use in hypnonema (the file inside wwwroot)
hypnonema_url 'http://localhost:9414/'

-- whether to use the built-in http server or not. WARNING: FOR TESTING PURPOSES ONLY!
hypnonema_http_server 'true'
hypnonema_listen_addr '0.0.0.0'
hypnonema_http_port '9414'

```
### (Optional) Example Map
There is an example map included, just copy the folder **hypnonema-map** to your resource directory and add *start hypnonema-map* to your server.cfg.
## Usage
Type ```/hypnonema``` in chat to open the NUI-Frontend, paste your video link and choose either "auto" if its normal html5 video input eg. a mp4 or "youTube" if its a youTube link.

## Customization
### Change Splash-Screen (video-poster)
Open the ```index.html``` and change either splash_image or pause_image.
### Change command
If you want to use your own command just edit the __resource.lua. Don't add preceding slashes or any special characters
```
hypnonema_command_name 'mynewcommand'
```
## License

This project is licensed under the MIT License - see [LICENSE](LICENSE.md) file for details

## Support
Please [open an issue](https://github.com/thiago-dev/fivem-hypnonema/issues/new) for support.