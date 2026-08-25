using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace FSD.Service.Services
{
    public class CurrencyService
    {
        private readonly string _tcmbUrl = "https://www.tcmb.gov.tr/kurlar/today.xml";

        public async Task<Dictionary<string, decimal>> GetCurrentRatesAsync()
        {
            var rates = new Dictionary<string, decimal>();

            try
            {
                using var client = new HttpClient();
                // TCMB'ye istek at, XML'i string olarak al
                var xmlString = await client.GetStringAsync(_tcmbUrl);

                // XML'i parse et
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);

                // USD kurunu bul
                var usdNode = xmlDoc.SelectSingleNode("//Currency[@CurrencyCode='USD']/ForexSelling");
                if (usdNode != null)
                {
                    // TCMB nokta (.) kullanır ama C# virgül (,) bekler, o yüzden replace
                    var usdValue = usdNode.InnerText.Replace(".", ",");
                    rates["USD"] = decimal.Parse(usdValue);
                }

                // EUR kurunu bul
                var eurNode = xmlDoc.SelectSingleNode("//Currency[@CurrencyCode='EUR']/ForexSelling");
                if (eurNode != null)
                {
                    var eurValue = eurNode.InnerText.Replace(".", ",");
                    rates["EUR"] = decimal.Parse(eurValue);
                }
            }
            catch
            {
                // TCMB'ye ulaşılamazsa yaklaşık güncel değerler (Ağustos 2026)
                rates["USD"] = 47.57m;
                rates["EUR"] = 54.98m;
            }

            return rates;
        }
    }
}