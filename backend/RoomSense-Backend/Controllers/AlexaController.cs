using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;
using RoomSense_Backend.Entity;
using Microsoft.EntityFrameworkCore;

namespace RoomSense_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlexaController : Controller
    {
        private readonly MonitoringContext _context;
        private readonly ILogger<AlexaController> _logger;

        public AlexaController(MonitoringContext context, ILogger<AlexaController> logger)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> HandleRequest([FromBody] JsonObject requestBody)
        {
            _logger.LogInformation("Received Alexa request: {@Request}", requestBody);
            
            // Check if the request body contains the "request" property
            if (requestBody["request"] == null)
            {
                return BadRequest("Invalid request format. Missing 'request' property.");
            }

            // Extract the request type from the request body
            var requestType = requestBody["request"]["type"]?.ToString();

            // Check if the request type is "IntentRequest"
            if (requestType != "IntentRequest")
            {
                return BadRequest("Invalid request type. Expected 'IntentRequest'.");
            }

            // Check if the request body contains the "intent" property
            if (requestBody["request"]["intent"] == null)
            {
                return BadRequest("Invalid request format. Missing 'intent' property.");
            }

            // Extract the intent name from the request body
            var intentName = requestBody["request"]["intent"]["name"]?.ToString();

            // Check if the intent name is "RecommendationIntent"
            if (intentName == "RecommendationIntent")
            {
                string roomName = null;
                string speechText;

                // Check if the "roomName" slot is present
                if (requestBody["request"]["intent"]["slots"]?["roomName"] != null)
                {
                    // Extract the room name slot value from the request body
                    roomName = requestBody["request"]["intent"]["slots"]["roomName"]["value"]?.ToString();
                }

                if (string.IsNullOrEmpty(roomName))
                {
                    // Fetch the last recommendation from the database
                    var recommendation = await _context.Recommendations.OrderByDescending(r => r.Timestamp).FirstOrDefaultAsync();
                    speechText = recommendation != null ? $"Die letzte allgemeine Empfehlung ist: {recommendation.Message}" : "Zur Zeit liegen keine allgemeinen Empfehlungen vor.";
                }
                else
                {
                    // Fetch the recommendation for the specified room from the database
                    var recommendation = await _context.Recommendations.Where(r => r.Room.Name == roomName).OrderByDescending(r => r.Timestamp).FirstOrDefaultAsync();
                    speechText = recommendation != null ? $"Die Empfehlung für {roomName} ist: {recommendation.Message}" : $"Zur Zeit liegen keine Empfehlungen für {roomName} vor.";
                }

                // https://www.developer.amazon.com/en-US/docs/alexa/custom-skills/request-and-response-json-reference.html#response-examples
                var response = new
                {
                    version = "1.0",
                    sessionAttributes = new {},
                    response = new
                    {
                        outputSpeech = new
                        {
                            type = "PlainText",
                            text = speechText
                        },
                        card = new
                        {
                            type = "Simple",
                            title = "Skill Response",
                            content = speechText
                        },
                        reprompt = new
                        {
                            outputSpeech = new
                            {
                                type = "PlainText",
                                text = "Kann ich Ihnen noch bei etwas anderem helfen?"
                            }
                        },
                        shouldEndSession = false
                    }
                };

                _logger.LogInformation("Sending Alexa response: {@Response}", response);
                return Ok(response);
            }

            _logger.LogInformation("Invalid intent. Expected 'RecommendationIntent'.");
            return BadRequest("Invalid intent. Expected 'RecommendationIntent'.");
        }
    }
}
