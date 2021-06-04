# Carbonate
A very simple real-time chatting protocol and its implement.

### Transmission ###

​	All of the packets are actually a UTF-8 encoded JSON string with a 4-byte-long string length indicator at the beginning of the packet. JSON comments are not supported.

### Client Command-line Interface (ClientCLI) ###

​	The "Carbonate Client Command-line Interface" is a official implement of Carbonate Client.

#### Configure file ####

​	ClientCLI will check the file `client_config.json`. If it is not exist, program will create one.

​	It should be like this:

```json
{
    "username": "Username", 
    "nickname": "Nickname", 
    "password": "PlaintextPassword"
}
```

​	The password hash is calculated using SHA256 Hash Algorithm.

​	JSON comments are not supported.

#### Commands ####

​	Commands starting with `!` are offline commands, and commands starting with `/` are online commands.

| Offline Command Usage | Function |
| ------: | -------- |
|`!register <Address>[:Port]` | Ping the indicated server. Returns the server information.|
|`!connect <Address>[:Port]` | Connect to the indicated server using the loaded configure file.|
|`!register <Address>[:Port]` | Register at the indicated server using the loaded configure file.|
|`!disconnect` | Force disconnect from the connected remote server.|
|`!clear` | Clear the screen.|
|`!exit` | Exit the program.|



| Online Command Usage | Function |
| ------: | -------- |
|`/say <Message>` | Say something.|
|`/me <Message>`  | Send a action message.|
|`/tell <User> <Message>` | Send a whisper message to the indicated user.|
|`/changename <Nickname>` | Change the nickname of the user.|
|`/list` | List all online users.|
|`/keep-alive` | Send a client keep-alive packet. Automatically sent by every 2000ms.|
|`/disconnect` | Send a disconnect message to the server. Then the server will send a disconnect message to the client.|

### Expandability ###

​	Server plugin is not supported (cause I'm lazy and trash). However, you can use `Carbonate.Client` project to create bots.

