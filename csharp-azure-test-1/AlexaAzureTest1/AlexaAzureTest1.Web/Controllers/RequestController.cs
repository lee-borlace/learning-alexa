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
            var speech = new Alexa.NET.Response.SsmlOutputSpeech();
            speech.Ssml = "<speak>Hello World!</speak>";
            var finalResponse = ResponseBuilder.Tell(speech);
            return Ok(finalResponse);

            // check what type of a request it is like an IntentRequest or a LaunchRequest
            //var requestType = request.GetRequestType();

            //if (requestType == typeof(IntentRequest))
            //{
            //    var speech = new Alexa.NET.Response.SsmlOutputSpeech();
            //    speech.Ssml = "<speak>Hello World!</speak>";
            //    var finalResponse = ResponseBuilder.Tell(speech);
            //    return Ok(finalResponse);
            //}
            //else if (requestType == typeof(Alexa.NET.Request.Type.LaunchRequest))
            //{
            //    ResponseBuilder.
            //}
            //else if (requestType == typeof(AudioPlayerRequest))
            //{
            //    // do some audio response stuff
            //}


        }
    }
}