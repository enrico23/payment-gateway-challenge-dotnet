namespace PaymentGateway.Api.Controllers;

/// <summary>
/// Handles payment requests.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class PaymentsController(
    IPaymentsRepository paymentsRepository,
    IPaymentService paymentService)
    : ControllerBase
{
    /// <summary>
    /// Gets a previously processed payment.
    /// </summary>
    /// <param name="id">Payment identifier.</param>
    /// <returns>The payment if it exists.</returns>
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
    /// Processes a payment.
    /// </summary>
    /// <param name="request">Payment details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The processed payment result.</returns>
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
