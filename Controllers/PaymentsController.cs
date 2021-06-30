using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using SyllabusZip.Common.Data;
using SyllabusZip.Configuration;

namespace SyllabusZip.Controllers
{
    public class PaymentsController : Controller
    {
        // Set your secret key. Remember to switch to your live secret key in production.
        // See your keys here: https://dashboard.stripe.com/account/apikeys
        //StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("SqlConnectionString");
        private readonly IStripeClient client;
        public readonly IOptions<StripeOptions> options;

        private ApplicationDbContext Database { get; }

        public IActionResult Index()
        {
            return View();
        }

        public PaymentsController(IOptions<StripeOptions> options, ApplicationDbContext db)
        {
            this.options = options;
            this.client = new StripeClient(this.options.Value.SecretKey);
            Database = db;
        }

        public class CreateCheckoutSessionRequest
        {
            [JsonProperty("priceId")]
            public string PriceId { get; set; }

            [JsonProperty("selection")]
            public string Selection { get; set; }
        }

        public class CreateCheckoutSessionResponse
        {
            [JsonProperty("sessionId")]
            public string SessionId { get; set; }
        }

        public class CustomerPortalRequest
        {
            [JsonProperty("sessionId")]
            public string SessionId { get; set; }
        }

        [HttpPost("create-checkout-session")]
        [Authorize]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest req)
        {
            var options = new SessionCreateOptions
            {
                // See https://stripe.com/docs/api/checkout/sessions/create
                // for additional parameters to pass.
                // {CHECKOUT_SESSION_ID} is a string literal; do not change it!
                // the actual Session ID is returned in the query parameter when your customer
                // is redirected to the success page.
                SuccessUrl = $"https://{Request.Host}/payments/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"https://example.com/payments/canceled",
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                Mode = req.Selection == "semester" ? "payment" : "subscription",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = req.PriceId,
                        // For metered billing, do not pass quantity
                        Quantity = 1,
                    },
                },
                CustomerEmail = User.FindFirstValue(ClaimTypes.Name)
            };
            var service = new SessionService(this.client);
            try
            {
                var session = await service.CreateAsync(options);
                return Ok(new CreateCheckoutSessionResponse
                {
                    SessionId = session.Id,
                });
            }
            catch (StripeException e)
            {
                Console.WriteLine(e.StripeError.Message);
                return BadRequest(new
                {
                    ErrorMessage = new
                    {
                        Message = e.StripeError.Message,
                    }
                });
            }
        }

        [HttpPost("customer-portal")]
        public async Task<IActionResult> CustomerPortal([FromBody] CustomerPortalRequest req)
        {
            // For demonstration purposes, we're using the Checkout session to retrieve the customer ID.
            // Typically this is stored alongside the authenticated user in your database.
            var checkoutSessionId = req.SessionId;
            var checkoutService = new SessionService(this.client);
            var checkoutSession = await checkoutService.GetAsync(checkoutSessionId);

            // This is the URL to which your customer will return after
            // they are done managing billing in the Customer Portal.
            var returnUrl = this.options.Value.Domain;

            var options = new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = checkoutSession.CustomerId,
                ReturnUrl = returnUrl,
            };
            var service = new Stripe.BillingPortal.SessionService(this.client);
            var session = await service.CreateAsync(options);

            return Ok(session);
        }

        [HttpGet]
        public async Task<IActionResult> Success(string session_id, [FromServices] ApplicationDbContext db)
        {
            // This is loosely based off of step 6 and 7 at https://stripe.com/docs/billing/subscriptions/checkout#send-to-portal
            // Stripe's implementation is extra-complicated due to an assumed split between client-side app and server-side
            // app. Since we're all server-side rendered, we don't need the excess back-and-forth

            var service = new SessionService(this.client);
            var session = await service.GetAsync(session_id);

            // Need to store customerId somewhere...
            var user = db.Users.FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            user.CustomerId = session.CustomerId;
            db.SaveChanges();

            return View();
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            Event stripeEvent;
            try
            {
                var webhookSecret = options.Value.WebhookSecret;
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    webhookSecret
                );
                Console.WriteLine($"Webhook notification with type: {stripeEvent.Type} found for {stripeEvent.Id}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something failed {e}");
                return BadRequest();
            }

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                    // Payment is successful and the subscription is created.
                    // You should provision the subscription.
                    await HandleSessionCompeleted(stripeEvent.Data.Object as Session);
                    break;
                case "customer.subscription.created":
                    HandleSubscriptionCreated(stripeEvent.Data.Object as Subscription);
                    break;
                case "invoice.paid":
                    // Continue to provision the subscription as payments continue to be made.
                    // Store the status in your database and check when a user accesses your service.
                    // This approach helps you avoid hitting rate limits.
                    break;
                case "invoice.payment_failed":
                    // The payment failed or the customer does not have a valid payment method.
                    // The subscription becomes past_due. Notify your customer and send them to the
                    // customer portal to update their payment information.
                    break;
                default:
                    // Unhandled event type
                    break;
            }

            return Ok();
        }

        public async Task HandleSessionCompeleted(Session session)
        {
            if (session != null)
            {
                if (session.PaymentStatus == "paid")
                {
                    if (!string.IsNullOrEmpty(session.SubscriptionId))
                    {
                        // Start the subscription
                        var subscription = session.Subscription;
                        if (subscription is null)
                        {
                            //Let's go get it
                            subscription = await new SubscriptionService(client).GetAsync(session.SubscriptionId);
                        }
                        HandleSubscriptionCreated(subscription);
                    }
                    else
                    {
                        Console.WriteLine($"Session {session.Id} does not contain a subscription");
                    }
                }
                else
                {
                    Console.WriteLine($"Session {session.Id} is not yet paid");
                }
            }
            else
            {
                Console.WriteLine($"Event did not contain a session or it was null");
            }
        }

        public void HandleSubscriptionCreated(Subscription subscription)
        {
            Console.WriteLine($"Subscription now ends at {subscription.CurrentPeriodEnd}");

            // Save to user's record
            var user = Database.Users.FirstOrDefault(u => u.CustomerId == subscription.CustomerId);
            if (user != null)
            {
                user.Expiration = subscription.CurrentPeriodEnd;
                Database.SaveChanges();
            }
            else
            {
                Console.WriteLine($"No user found with customer ID {subscription.CustomerId}");
            }
        }
    }




}
