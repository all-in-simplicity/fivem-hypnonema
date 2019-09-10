<h1>HTTP-Server Setup Tutorial - Caddy</h1>

```
"Caddy is the HTTP/2 web server with automatic HTTPS"
```
[caddyserver.com](https://caddyserver.com)

Caddy as a webserver can be setup hassle-free and if you are using it for production it can even provide you with a SSL/TLS certificate from LetsEncrypt.
<h2>Table of Contents</h2>

- [Setup](#setup)
  - [Download](#download)
  - [Installation](#installation)
  - [Using this on a vps/dedicated machine](#using-this-on-a-vpsdedicated-machine)
## Setup
### Download

* [Windows 64-bit](https://caddyserver.com/download/windows/amd64?license=personal&telemetry=off)

* [Linux 64-bit](https://caddyserver.com/download/linux/amd64?license=personal&telemetry=off)

* [More Options](https://caddyserver.com/download)

### Installation
1. Extract the .zip file to a location of your choice which you can easy get at. For example ```C:\Caddy```

Hint:
**"By default, Caddy will use the current directory (the directory it is being executed from, not the folder where the binary lives) as the root of the site. This makes it easy to work on sites locally!"**

2. Open a terminal or the command-line and change to the folder where the index.html of hypnonema is. Eg. ```cd E:\FiveM\cfx-server-data\resources\hypnonema\wwwroot```

3. Run Caddy. If you put the .exe in C:\Caddy, run:
   ```C:\Caddy\caddy.exe```

4. Load http://localhost:2015 in your browser. If you see a 404 error, then Caddy is working but you are probably in the wrong directory because index.html is missing.

5. If everything is working you should see [curtains](https://imgur.com/a/VdeNS17) with Hypnonema sign in the center. You are almost there, just change the url inside __resource.lua to http://localhost:2015 and you are finally done!


### Using this on a vps/dedicated machine
The steps are basically the same, just make sure that you replace localhost with the ip of your server. If you have any firewalls enabled make sure to open the 2015 tcp port. 

## Final Notes
I recommend reading the [documentation](https://caddyserver.com/docs) if you have need of a more fine tuned setup.
