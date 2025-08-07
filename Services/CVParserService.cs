using System.Net.Http;
using System.Text;
using System.Text.Json;
using CVMatchPro.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace CVMatchPro.Services
{
    public class CVParserService
    {
        private readonly HttpClient _http;

        public CVParserService()
        {
            _http = new HttpClient();
        }

        // ✅ Lis le contenu texte du fichier PDF
        private string ExtractTextFromPdf(string filePath)
        {
            using var reader = new PdfReader(filePath);
            using var pdf = new PdfDocument(reader);
            var sb = new StringBuilder();
            for (int i = 1; i <= pdf.GetNumberOfPages(); i++)
            {
                sb.AppendLine(PdfTextExtractor.GetTextFromPage(pdf.GetPage(i)));
            }
            return sb.ToString();
        }

        // 🔍 Envoie le texte brut à Flask
        public async Task<CV> ExtractDataFromFileAsync(string filePath)
        {
            string text = ExtractTextFromPdf(filePath);

            var jsonContent = JsonSerializer.Serialize(new { text });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync("http://localhost:5000/parse", content);

            if (!response.IsSuccessStatusCode)
                return new CV(); // Erreur => retour d’un CV vide

            var responseString = await response.Content.ReadAsStringAsync();

            var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(responseString);

            return new CV
            {
                NomExtrait = parsed.GetValueOrDefault("nom", ""),
                EmailExtrait = parsed.GetValueOrDefault("email", ""),
                Telephone = parsed.GetValueOrDefault("telephone", ""),
                Formation = parsed.GetValueOrDefault("formations", ""),
                Experience = parsed.GetValueOrDefault("experiences", ""),
                CompetencesExtraites = parsed.GetValueOrDefault("competences", ""),
                CentresInteret = parsed.GetValueOrDefault("centresInteret", "")
            };
        }

        public string ExtractText(string filePath) => ExtractTextFromPdf(filePath);

        public CV ExtractDataFromText(string text) => new(); // Obsolète
    }
}
