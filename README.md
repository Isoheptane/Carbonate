# Carbonate
A very simple real-time chatting protocol and its implement.

### Transmission ###

​	All of the packets are actually a UTF-8 encoded JSON string with a ``'\0'`` indicator at the ending of the packet. JSON comments are not supported.

### Client Command-line Interface (ClientCLI) ###

​	The "Carbonate Client Command-line Interface" is a official implement of Carbonate Client.

#### Configure file ####

​	ClientCLI will check the file `default_user.json`. If it is not exist, program will create one.

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

### Expandability ###

​	Server plugin is not supported (cause I'm lazy and trash). However, you can use `Carbonate.Client` project to create bots.

