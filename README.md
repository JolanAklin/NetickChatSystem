![Presentation image](/Readme-images/main-image.png)
# NetickChatSystem
Chat system with adjustable scopes for Netick
## How the demo works
![Presentation image](/Readme-images/scopes.png)
Red underline : to which scope the message will be sent to

Blue underline : to scope you belong to

You can send messages from any scope to Everyone, from Teams to the red and blue team and from red or blue team to Teams.

In the demo, there is three files that you can edit :
* Chat.cs. This script shows what the client receives.
* EventHandler.cs. It registers the scopes, spawn the client and send message such has connection and disconnection.
* Cient.cs. Is reponsible for changing scope and sending messages.

## How to work with the chat system
Ensure that Netick uses ChatLiteNetTransport.

You **must** have one EventHandler that inherits from ChatNetworkEventListener. You must not override some of it's function without calling the base function. See the list below.
```
OnStartup
OnShutdown
OnConnectedToServer
OnClientConnected
OnClientDisconnected
RemoveNetConnection
```
### Sending messages
Messages are sent from the client to the server, then the server dispatches the message to the right clients. Messages are dispatched automaticly.

From the server :
```
ChatMessenger.SendChatMessageToOne(string message, NetworkConnection client)
ChatMessenger.SendChatMessageToScope(string message, Scope target)
```
eg:
```
[...]
  _chat = GetComponent<ChatMessenger>();
[...]
public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
{
  _chat.SendChatMessageToOne($"Welcome to the server Client {client.Id}", client);
  _chat.SendChatMessageToScope($"Client {client.Id} connected", Scope.Everyone);
}
```
From the client :
```
ChatMessenger.SendToServer(string message, Scope target)
ChatMessenger.SendToServer(string message, int targetClient)
```
eg:
```
[...]
_messenger = Sandbox.FindGameObjectWithTag("NetController").GetComponent<ChatMessenger>();
[...]
_messenger.SendToServer(message, ScopeManager.Instance.GetScope("Blue team"));
```
For more examples, see the demo. Script of interest are EventHandler.cs, Client.cs, Chat.cs.

### Custom chat styling
To customize chat looks, you can create a new scripts that inherit from SenderStyler. See DefaultStyle.cs for implementation.
## License
MIT License

Copyright (c) 2022 Jolan Aklin

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
## Important notes
Only Scripts with the MIT License are the one that have the MIT License header on it. Other scripts are the property of their creator (Chat.cs, Client.cs, EventHandler.cs are created by me. But you can use them as you will).
### Disclaimer
This project is not affiliated nor endorsed by Netick. 
