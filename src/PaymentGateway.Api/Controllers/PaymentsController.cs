namespace PaymentGateway.Api.Controllers;

/// <summary>
/// Exposes endpoints for processing payments and retrieving previously processed payment details.
/// </summary>
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
        var payment = paymentsRepository.Get(id);

        return payment == null
            ? NotFound()
            : Ok(payment.ToResponse());
    }

    /// <summary>
    /// Processes a payment request.
    /// </summary>
    /// <param name="request">The payment details.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// The processed payment result if the request is valid; otherwise an error response.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<PaymentResponse>> ProcessPaymentAsync(
          [FromBody] PostPaymentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(new ValidationProblemDetails(ModelState));

        try
        {
            var payment = await paymentService.ProcessAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetPayment),
                new { id = payment.Id },
                payment);
        }
        catch (PaymentValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                var memberNames = error.MemberNames.Any()
                    ? error.MemberNames
                    : [string.Empty];

                foreach (var memberName in memberNames)
                {
                    ModelState.AddModelError(memberName, error.ErrorMessage ?? "The payment request is invalid.");
                }
            }

            return UnprocessableEntity(new ValidationProblemDetails(ModelState));
        }
        catch (InvalidOperationException)
        {
            return StatusCode(StatusCodes.Status502BadGateway);
        }
    }
}
