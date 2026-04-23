using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.Services;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Monetary;

public interface IValueExchangeService : IRequireInit
{
    ICollection<ValueExchangeRate> ValueExchangeRates { get; set; }
    Task RefreshExchangeRates();
    RegionInfo[] RegionInfos { get; }
    decimal Exchange(Tuple<string, decimal> fromValue, string toValue);
    decimal ConvertToEuro(decimal value, string currencyCode);
    decimal ConvertFromEuro(decimal value, string currencyCode);
}

[SingletonService(typeof(IValueExchangeService))]
public class ValueExchangeService : RequireInit, IValueExchangeService
{
    private readonly ILogger<ValueExchangeService> _appLogger;

    public ValueExchangeService(ILogger<ValueExchangeService> appLogger)
    {
        _appLogger = appLogger;
        ValueExchangeRates = [];
    }

    public RegionInfo[] RegionInfos { get; set; }

    public async Task RefreshExchangeRates()
    {
        try
        {
            ValueExchangeRates.Clear();
            var httpClient = new HttpClient();
            var euroExchangeRates =
                await httpClient.GetAsync("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");
            euroExchangeRates.EnsureSuccessStatusCode();
            var xmlParser = new XmlDocument();
            xmlParser.LoadXml(await euroExchangeRates.Content.ReadAsStringAsync());
            var nodes = xmlParser.SelectNodes("//*[@currency]");
            var dateNode = xmlParser.SelectNodes("//*[@time]");

            var date = DateTime.ParseExact(
                dateNode.OfType<XmlElement>().FirstOrDefault().GetAttribute("time"),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture);

            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    var rate = new ValueExchangeRate()
                    {
                        IsoName = node.Attributes["currency"].Value,
                        ExchangeRate = Decimal.Parse(node.Attributes["rate"].Value, NumberStyles.Any, new CultureInfo("en-Us")),
                        Date = date
                    };
                    ValueExchangeRates.Add(rate);
                }
            }
            ValueExchangeRates.Add(new ValueExchangeRate()
            {
                ExchangeRate = 1m,
                IsoName = "EUR"
            });

            //ValueExchangeRates.AddRange(rates.Select(e => new ValueExchangeRate
            //{
            //	Date = date,
            //	Name = e.Key,
            //	IsoName = e.Key,
            //	ExchangeRate = e.Value
            //}));
        }
        catch (Exception)
        {
            _appLogger.LogError("Error due Refresh of Exchange rates");
        }
    }

    public ICollection<ValueExchangeRate> ValueExchangeRates { get; set; }

    public decimal Exchange(Tuple<string, decimal> fromValue, string toValue)
    {
        var fromInEur = ConvertToEuro(fromValue.Item2, fromValue.Item1);

        var fromCurrencyExchangeRate = ValueExchangeRates.FirstOrDefault(e =>
            e.IsoName.Equals(toValue, StringComparison.OrdinalIgnoreCase));
        if (fromCurrencyExchangeRate != null)
        {
            return fromInEur / fromCurrencyExchangeRate.ExchangeRate;
        }

        throw new ArgumentException("Unsupported currency code " + toValue);
    }

    public decimal ConvertToEuro(decimal value, string currencyCode)
    {
        var fromCurrencyExchangeRate = ValueExchangeRates.FirstOrDefault(e =>
            e.IsoName.Equals(currencyCode, StringComparison.OrdinalIgnoreCase));
        if (fromCurrencyExchangeRate != null)
        {
            return value / fromCurrencyExchangeRate.ExchangeRate;
        }

        throw new ArgumentException("Unsupported currency code " + currencyCode);
    }


    public decimal ConvertFromEuro(decimal value, string currencyCode)
    {
        var fromCurrencyExchangeRate = ValueExchangeRates.FirstOrDefault(e =>
            e.IsoName.Equals(currencyCode, StringComparison.OrdinalIgnoreCase));
        if (fromCurrencyExchangeRate != null)
        {
            return value * fromCurrencyExchangeRate.ExchangeRate;
        }

        throw new ArgumentException("Unsupported currency code" + currencyCode);
    }

    public override async ValueTask InitAsync()
    {
        RegionInfos = CultureInfo.GetCultures(CultureTypes.NeutralCultures).Select(e =>
            {
                if (e.ToString() == "" || e.IsNeutralCulture)
                {
                    return null;
                }
                try
                {
                    return new RegionInfo(e.ToString());
                }
                catch (Exception)
                {
                    return null;
                }
            }).Where(e => e != null)
            //.DistinctBy(e => e.ISOCurrencySymbol)
            .ToArray();
        //await RefreshExchangeRates();
    }
}

public class ValueExchangeRate
{
    public DateTime Date { get; set; }
    public string IsoName { get; set; }
    public string Name { get; set; }
    public decimal ExchangeRate { get; set; }
}