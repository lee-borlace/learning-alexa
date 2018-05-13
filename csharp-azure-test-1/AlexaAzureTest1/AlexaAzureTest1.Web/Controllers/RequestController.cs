using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
                return GetPromptForInputResponse("Hi! How can I help?", "Sorry, I didn't hear you say anything. How can I help you today?");
            }
            else if (requestType == typeof(AudioPlayerRequest))
            {
                return GetSpeechResponse("Doing audio stuff");
            }
            else
            {
                return GetSpeechResponse("Hmm something has gone wrong. I got an unexpected request type.");
            }
        }

        private IActionResult HandleIntent(IntentRequest intentRequest)
        {
            switch (intentRequest.Intent.Name)
            {
                case "AMAZON.CancelIntent":
                case "AMAZON.StopIntent":
                    return GetSpeechResponse("Bye!");
                    break;
                case "AMAZON.HelpIntent":
                    return GetPromptForInputResponse("I can help you check an invoice status or create a purchase order. I can also do something. Also nothing. How can I help you?", "Sorry, I didn't hear you say anything. How can I help you?");
                    break;
                case "CreatePurchaseOrder":
                    return GetCreatePurchaseOrderResponse(intentRequest);
                case "InvoiceStatus":
                    return GetInvoiceStatusResponse(intentRequest);
                case "Something":
                    return GetSpeechResponse("Roger that, doing something.");
                    break;
                case "Nothing":
                    return GetSpeechResponse("Roger that, doing nothing.");
                    break;
                case "AMAZON.FallbackIntent":
                default:
                    return GetSpeechResponse("Hmm, I couldn't work out what you want, sorry. Have a nice day!");
                    break;
            }
        }

        private IActionResult GetCreatePurchaseOrderResponse(IntentRequest intentRequest)
        {
            // If dialog has state STARTED or IN_PROGRESS, then delegate back to Alexa to elicit and confirm the slots and confirm the whole intent.
            if (intentRequest.DialogState.Equals(DialogState.Started) || intentRequest.DialogState.Equals(DialogState.InProgress))
            {
                return Ok(ResponseBuilder.DialogDelegate());
            }
            // Otherwise, check whether the user confirmed the entire intent. Complete the request if so, otherwise fail it.
            else
            {
                string message;

                if (intentRequest.Intent.ConfirmationStatus == ConfirmationStatus.Confirmed)
                {
                    message = "Your purchase order is ready. I've emailed you a link.";
                }
                else
                {
                    message = "OK, let me know if you want to create a purchase order later.";
                }

                return GetPromptForInputResponse($"{message} What would you like to do now?", "Sorry, I didn't hear you say anything. What would you like to do now?");
            }
        }

        private IActionResult GetInvoiceStatusResponse(IntentRequest intentRequest)
        {
            // If dialog has state STARTED or IN_PROGRESS, then delegate back to Alexa to elicit and confirm the slots and confirm the whole intent.
            if (intentRequest.DialogState.Equals(DialogState.Started) || intentRequest.DialogState.Equals(DialogState.InProgress))
            {
                return Ok(ResponseBuilder.DialogDelegate());
            }
            // Otherwise, check whether the user confirmed the entire intent. Complete the request if so, otherwise fail it.
            else
            {
                var invoiceNumber = GetSlotValue("InvoiceNumber", intentRequest);
                var message = "The status of that invoice is open.";
                return GetPromptForInputResponse($"{message} How can I help now?", "Sorry, I didn't hear you say anything. How can I help now?");
            }
        }


        private string GetSlotValue(string slotName, IntentRequest intentRequest)
        {
            if (
               intentRequest.Intent != null &&
               intentRequest.Intent.Slots != null &&
               intentRequest.Intent.Slots.ContainsKey(slotName)
           )
            {
                return intentRequest.Intent.Slots[slotName].Value;
            }
            else
            {
                return null;
            }
        }


        private string ConvertInvoiceNumberForSpeech(string invoiceNumber)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < invoiceNumber.Length; i++)
            {
                sb.Append(invoiceNumber[i]);

                if (i < invoiceNumber.Length - 1)
                {
                    if (!IsNumber(invoiceNumber[i + 1])
                        || (!IsNumber(invoiceNumber[i]) && IsNumber(invoiceNumber[i + 1]))
                        )
                    {
                        sb.Append(",");
                    }
                }
            }

            return sb.ToString();
        }


        private bool IsNumber(char c)
        {
            return c >= '0' && c <= '9';
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