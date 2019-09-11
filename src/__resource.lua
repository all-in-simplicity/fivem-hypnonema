--[[
 _   _                                                    
| | | |                                                   
| |_| |_   _ _ __  _ __   ___  _ __   ___ _ __ ___   __ _ 
|  _  | | | | '_ \| '_ \ / _ \| '_ \ / _ \ '_ ` _ \ / _` |
| | | | |_| | |_) | | | | (_) | | | |  __/ | | | | | (_| |
\_| |_/\__, | .__/|_| |_|\___/|_| |_|\___|_| |_| |_|\__,_|
        __/ | |                                           
       |___/|_|                                           

Version: 1.1

]]--

-- the url to your webserver containing the corresponding html file for use in hypnonema (the index.html inside wwwroot)
hypnonema_url 'http://localhost:9414/'

-- the command someone needs to enter for opening the menu
-- Hint: no spaces, no special characters!
hypnonema_command_name 'hypnonema'

-- whether to use the built-in http server or not. WARNING: FOR TESTING PURPOSES ONLY!
hypnonema_http_server 'false'
hypnonema_listen_addr '127.0.0.1'
hypnonema_http_port '9414'
-- --------------------------------------------

resource_manifest_version '44febabe-d386-4d18-afbe-5e627f4af937'

server_script 'server/Hypnonema.Server.net.dll'
ui_page 'client/html/index.html'
client_script 'client/Hypnonema.Client.net.dll'

files {
    'client/Newtonsoft.json.dll',
    'client/html/index.html',
    'client/html/main-es5.js',
    'client/html/main-es2015.js',
    'client/html/polyfills-es5.js',
    'client/html/polyfills-es2015.js',
    'client/html/runtime-es2015.js',
    'client/html/runtime-es5.js',
    'client/html/styles-es2015.js',
    'client/html/styles-5.js'
}


-- Only change if you know what you are doing!
hypnonema_position { '-1678.949', '-928.3431', '20.6290932' }
hypnonema_rotation { '0.0', '0.0', '-140.0' }
hypnonema_scale { '0.969999969', '0.484999985', '-0.1' }

hypnonema_width '1280'
hypnonema_height '720'
