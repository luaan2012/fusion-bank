using fusion.bank.core.Enum;
using fusion.bank.creditcard.domain.DTO;
using fusion.bank.creditcard.domain.Interfaces;

namespace fusion.bank.creditcard.services
{
    public class GenerateCreditCardService() : IGenerateCreditCardService
    {
        public async Task<CreditCardDTO> GenerateCreditCard(CreditCardFlag creditCardType)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://www.4devs.com.br/ferramentas_online.php");

            request.Headers.Add("Server", "nginx");
            request.Headers.Add("Strict-Transport-Security", "max-age=63072000; includeSubDomains; preload");
            request.Headers.Add("Vary", "Accept-Encoding, User-Agent");
            request.Headers.Add("X-Content-Type-Options", "nosniff");
            request.Headers.Add("X-Frame-Options", "SAMEORIGIN");

            var bandeira = creditCardType switch
            {
                CreditCardFlag.MASTERCARD => "master",
                CreditCardFlag.AMERICANEXPRESS => "amex",
                CreditCardFlag.HIPERCARD => "hiper",
                CreditCardFlag.VISA => "visa16",
                CreditCardFlag.DISCOVER => "discover",
            };

            var content = new MultipartFormDataContent();
            content.Add(new StringContent("gerar_cc"), "acao");   
            content.Add(new StringContent("pontuacao"), "S");     
            content.Add(new StringContent(bandeira), "bandeira"); 

            request.Content = content;

            var response = await client.SendAsync(request);

            string responseContent = await response.Content.ReadAsStringAsync();

            string numero = ExtractValue(responseContent, "cartao_numero");
            string dataValidade = ExtractValue(responseContent, "data_validade");
            string cvv = ExtractValue(responseContent, "codigo_seguranca");

            var creditCard = new CreditCardDTO
            {
                CreditCardNumber = numero,
                CreditCardCode = cvv,
                CreditCardValidity = dataValidade
            };

            return creditCard;
        }

        private string ExtractValue(string html, string id)
        {
            string startTag = $"id=\"{id}\" onclick=\"fourdevs.selectText(this)\" class=\"output-txt output-txt-active\">";
            string endTag = "<span class=\"clipboard-copy\"></span>";

            int startIndex = html.IndexOf(startTag) + startTag.Length;
            int endIndex = html.IndexOf(endTag, startIndex);

            if (startIndex < 0 || endIndex < 0 || endIndex <= startIndex)
                return "Não encontrado";

            return html.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}
