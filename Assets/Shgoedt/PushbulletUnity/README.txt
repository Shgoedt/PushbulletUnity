/// Author:
/// John de Jonker
/// http://www.twitter.com/Shgoedt

Instructions:

Getting Started
_______________

1. Register a free account at https://www.pushbullet.com/signin

2. Copy and paste your API Key from https://www.pushbullet.com/account > Access Token

3. Open Pushbullet.cs and enter your own API Key ( private string apiKey = "your_key_here" )


Using the library
_________________

Call Pushbullet.Push( title:, body: ) from any runtime script to send a note to all of
your devices.

Call Pushbullet.Push( title:, body:, device: ) from any runtime script to send a note to 
that specific device.

If you imported the optional scripts you can use the TestPushbullet.cs script,
which has some parameters and can either send to all or a single device after pressing
Return/Enter.

