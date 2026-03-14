namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(
    IPaymentsRepository paymentsRepository,
    IPaymentService paymentService) 
    : ControllerBase
{

    /// <summary>
    /// Retrieves a previously processed payment by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the payment.</param>
    /// <returns>
    /// The payment details if found; otherwise returns <c>404 Not Found</c>.
    /// </returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PaymentResponse?> GetPayment(Guid id)
    {
        PaymentResponse? payment = paymentsRepository.Get(id);

        return payment == null 
            ? NotFound() 
            : Ok(payment);
    }

    /// <summary>
    /// Processes a payment request.
    /// </summary>
    /// <param name="request">The payment details.</param>
    /// <returns>
    /// The processed payment result if the request is valid; otherwise an error response.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<PaymentResponse>> ProcessPaymentAsync(
          [FromBody] PostPaymentRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        PaymentResponse payment = await paymentService.ProcessAsync(request);

        return CreatedAtAction(
            nameof(GetPayment), 
            new { id = payment.Id }, 
            payment);
    }
}