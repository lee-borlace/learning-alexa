# Running Locally
1. Install ngrok and get it set up for your ngrok account.
2. Run this project.
3. Run ```ngrok http -bind-tls=true -host-header=rewrite 61979``` (if you're using VS2017) or ```ngrok http -bind-tls=true -host-header=rewrite 5000``` (if you're using VS Code)
4. The above will give you a URL in the ngrok domain e.g. ```https://eb6554ac.ngrok.io```. This will change every time you run that step!
5. Plug that URL into the endpoint section of the Alexa app with ```/api/request``` on the end e.g. ```https://eb6554ac.ngrok.io/api/request```.
6. Talk to the Alexa app!