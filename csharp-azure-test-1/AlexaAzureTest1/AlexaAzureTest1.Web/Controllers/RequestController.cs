using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlexaAzureTest1.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/Request")]
    public class RequestController : Controller
    {
        // POST api/request
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]SkillRequest request)
        {
            var requestType = request.GetRequestType();

            if (requestType == typeof(IntentRequest))
            {
                return HandleIntent(request.Request as IntentRequest);
            }
            else if (requestType == typeof(Alexa.NET.Request.Type.LaunchRequest))
            {
                return GetPromptForInputResponse("OK let's start with what would you like to do?", "Sorry, I didn't hear you say anything. What would you like to do?");
            }
            else if (requestType == typeof(AudioPlayerRequest))
            {
                return GetSpeechResponse("Doing audio stuff");
            }
            else
            {
                return GetSpeechResponse("Couldn't work out request type so I'm saying these words.");
            }
        }

        private IActionResult HandleIntent(IntentRequest intentRequest)
        {
            switch(intentRequest.Intent.Name)
            {
                case "Something":
                    return GetSpeechResponse("Roger that, doing something.");
                    break;
                case "Nothing":
                    return GetSpeechResponse("Roger that, doing nothing.");
                    break;
                default:
                    return GetSpeechResponse("Hmm, couldn't work out what you want sorry.");
                    break;
            }
        }

        private IActionResult GetSpeechResponse(string text)
        {
            var speech = new Alexa.NET.Response.SsmlOutputSpeech();
            speech.Ssml = $"<speak>{text}</speak>";
            var finalResponse = ResponseBuilder.Tell(speech);
            return Ok(finalResponse);
        }

        private IActionResult GetPromptForInputResponse(string prompt, string reprompt)
        {
            var speech = new Alexa.NET.Response.SsmlOutputSpeech();
            speech.Ssml = $"<speak>{prompt}</speak>";

            var repromptMessage = new Alexa.NET.Response.PlainTextOutputSpeech();
            repromptMessage.Text = reprompt;
            var repromptBody = new Alexa.NET.Response.Reprompt();
            repromptBody.OutputSpeech = repromptMessage;

            var finalResponse = ResponseBuilder.Ask(speech, repromptBody);
            return Ok(finalResponse);

        }
    }
}