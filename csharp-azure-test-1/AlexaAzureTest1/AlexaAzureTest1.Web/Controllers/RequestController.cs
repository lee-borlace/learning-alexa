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
                return GetSpeechResponse("Handling intent.");
            }
            else if (requestType == typeof(Alexa.NET.Request.Type.LaunchRequest))
            {
                return GetSpeechResponse("Launched.");
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

        private IActionResult GetSpeechResponse(string text)
        {
            var speech = new Alexa.NET.Response.SsmlOutputSpeech();
            speech.Ssml = $"<speak>{text}</speak>";
            var finalResponse = ResponseBuilder.Tell(speech);
            return Ok(finalResponse);
        }
    }
}