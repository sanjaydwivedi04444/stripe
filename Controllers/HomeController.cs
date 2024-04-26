using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using StripePaymentGateway.Models;
using System.Diagnostics;

namespace StripePaymentGateway.Controllers
{
    //https://www.youtube.com/watch?v=g7meqnNquOE
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StripeSettings _stripeSettings;
        public HomeController(ILogger<HomeController> logger, IOptions<StripeSettings> stripeSettings)
        {
            _logger = logger;
            _stripeSettings = stripeSettings.Value;
        }

        public IActionResult Index()
        {
            var product = new Products();
            product.Id = 1;product.ProductName= "Pen Drive";product.Quantity = 1;product.Price =5M;
            var productList = new List<Products>();
            productList.Add(product);
            return View(productList);
        }
        public IActionResult Cart()
        {
            var product = new Products();
            product.Id = 1; product.ProductName = "Pen Drive"; product.Quantity = 1; product.Price = 5M;
            var productList = new List<Products>();
            productList.Add(product);
            return View(productList);
        }
        public IActionResult Checkout(string amount)
        {
            var currency = "usd"; // Currency code
            var successUrl = "https://localhost:7126/Home/Success";
            var cancelUrl = "https://localhost:7126/Home/Cancel";
            Stripe.StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                  "card"
                },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = currency,
                                UnitAmount = Convert.ToInt32(amount) * 100, // Amount in smallest currency unit (e.g., cents)
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Product Name",
                                Description = "Product Description"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };
            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }

        public async Task<IActionResult> Success()
        {
            return View("Index");
        }
        public IActionResult Cancel()
        {
            return View("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
