using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(
    IPaymentsRepository paymentsRepository) : ControllerBase
{

    /// <summary>
    /// Retrieves a previously processed payment by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the payment.</param>
    /// <returns>
    /// The payment details if found; otherwise returns <c>404 Not Found</c>.
    /// </returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = paymentsRepository.Get(id);

        return payment == null 
            ? NotFound() 
            : Ok(payment);
    }
}