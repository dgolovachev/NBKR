using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;

namespace NBKR
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class NBKRRate : IDisposable
    {
        private readonly HttpClient _httpClient;
        private const string _nbkrUrl = @"http://www.nbkr.kg/XML/daily.xml";

        /// <summary>
        /// Initializes a new instance of the <see cref="NBKRRate"/> class.
        /// </summary>
        /// <param name="webProxy">The web proxy.</param>
        public NBKRRate(IWebProxy webProxy = null)
        {
            if (webProxy != null)
            {
                var httpClientHander = new HttpClientHandler
                {
                    Proxy = webProxy,
                    UseProxy = true
                };

                _httpClient = new HttpClient(httpClientHander);
            }
            else
                _httpClient = new HttpClient();
        }

        /// <summary>
        /// Ежедневные официальные курсы по USD, EUR, RUB, KZT
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public CurrencyRates GetDailyRate()
        {
            try
            {
                DateTime date;
                double usd = 0, eur = 0, kzt = 0, rub = 0;
                var response = _httpClient.GetAsync(_nbkrUrl).Result;
                var xDocument = XDocument.Parse(response.Content.ReadAsStringAsync().Result);

                date = DateTime.ParseExact(xDocument.Root.Attribute("Date").Value, "dd.mm.yyyy", CultureInfo.InvariantCulture);

                var currencyNodes = xDocument.Descendants("Currency");
                foreach (var currency in currencyNodes)
                {
                    if (currency.Attribute("ISOCode").Value == "USD") usd = double.Parse(currency.Element("Value").Value, new CultureInfo("Ru-Ru"));
                    if (currency.Attribute("ISOCode").Value == "EUR") eur = double.Parse(currency.Element("Value").Value, new CultureInfo("Ru-Ru"));
                    if (currency.Attribute("ISOCode").Value == "KZT") kzt = double.Parse(currency.Element("Value").Value, new CultureInfo("Ru-Ru"));
                    if (currency.Attribute("ISOCode").Value == "RUB") rub = double.Parse(currency.Element("Value").Value, new CultureInfo("Ru-Ru"));
                }

                return new CurrencyRates(usd, eur, kzt, rub, date);

            }
            catch (Exception e)
            {
                throw new Exception($"Error in GetDailyRate: {e.Message}", e);
            }

        }

        /// <summary>
        /// Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых ресурсов.
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    public class CurrencyRates
    {
        public DateTime Date { get; }
        public double USD { get; }
        public double EUR { get; }
        public double KZT { get; }
        public double RUB { get; }

        public CurrencyRates(double usd, double eur, double kzt, double rub, DateTime date)
        {
            USD = usd;
            EUR = eur;
            KZT = kzt;
            RUB = rub;
            Date = date;
        }

        public override string ToString()
        {
            return $"Date: {Date}\nUSD: {USD}\nEUR: {EUR}\nKZT: {KZT}\nRUB:{RUB}";
        }
    }

}
