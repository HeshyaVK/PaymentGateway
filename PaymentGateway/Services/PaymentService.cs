
using PayPal.Api;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using PaymentGateway.Models;
using System;

public class PaymentService
{
    private readonly PayPalSettings _payPalSettings;

    public PaymentService(IOptions<PayPalSettings> payPalSettings)
    {
        _payPalSettings = payPalSettings.Value;
    }

    private APIContext GetAPIContext()
    {
        var config = new Dictionary<string, string>
        {
            { "mode", _payPalSettings.Mode }
        };

        var accessToken = new OAuthTokenCredential(_payPalSettings.ClientId, _payPalSettings.ClientSecret, config).GetAccessToken();
        return new APIContext(accessToken)
        {
            Config = config
        };
    }

    public Payment CreatePayment(decimal amount, string returnUrl, string cancelUrl)
    {
        var apiContext = GetAPIContext();

        var payment = new Payment
        {
            intent = "sale",
            payer = new Payer { payment_method = "paypal" },
            transactions = new List<Transaction>
            {
                new Transaction
                {
                    description = "Transaction description",
                    amount = new Amount { currency = "USD", total = amount.ToString("F2") }
                }
            },
            redirect_urls = new RedirectUrls
            {
                return_url = returnUrl,
                cancel_url = cancelUrl
            }
        };

        return payment.Create(apiContext);
    }
}

