using events.Models.API;
using events.Models.Comands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace events.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventsController(ILogger<EventsController> logger, IMediator mediator) : ControllerBase
{
    [HttpGet("health")]
    public ActionResult<HealthApiResponse> Health() => Ok(new HealthApiResponse { Status = true });

    [HttpPost("movie")]
    public async Task<ActionResult<EventApiResponse>> CreateMovieEvent([FromBody] MovieEventApiRequest movieApiRequest)
    {
        logger.LogInformation("Movie event received");
        return await Send($"{movieApiRequest.MovieId}", "movie", movieApiRequest);
    }

    [HttpPost("user")]
    public async Task<ActionResult<EventApiResponse>> CreateUserEvent([FromBody] UserEventApiRequest userApiRequest)
    {
        logger.LogInformation("User event received");
        return await Send($"{userApiRequest.UserId}", "user", userApiRequest);
    }

    [HttpPost("payment")]
    public async Task<ActionResult<EventApiResponse>> CreatePaymentEvent([FromBody] PaymentEventApiRequest paymentEventApiRequest)
    {
        logger.LogInformation("Payment event received");
        return await Send($"{paymentEventApiRequest.PaymentId}", "payment", paymentEventApiRequest);
    }
    
    private async Task<ActionResult<EventApiResponse>> Send(string id, string type, IEventRequest request)
    {
        var result = await mediator.Send(new ProduceEventRequest(request));

        if (result.Status == "failed")
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return StatusCode(StatusCodes.Status201Created, new EventApiResponse
        {
            Status = "success",
            Partition = result.Partition,
            Offset = result.Offset,

            Event = new Event
            {
                Id = id,
                Type = type,
                Timestamp = DateTime.Now,
                Payload = request
            }
        });
    }
}
