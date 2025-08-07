using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CVMatchPro.Models;

namespace CVMatchPro.Services
{
    public class IAParserService
    {
        private readonly HttpClient _httpClient;

        public IAParserService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<CV> ExtractDataFromTextAsync(string text)
        {
            var json = JsonSerializer.Serialize(new { text });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:5000/parse", content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<CVNLPResult>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var cv = new CV
            {
                NomExtrait = result.Nom,
                EmailExtrait = result.Email,
                Telephone = result.Telephone,
                Formation = result.Formations,
                Experience = result.Experiences,
                CompetencesExtraites = result.Competences,
                CentresInteret = result.CentresInteret
            };

            return cv;
        }

        private class CVNLPResult
        {
            public string Nom { get; set; }
            public string Email { get; set; }
            public string Telephone { get; set; }
            public string Formations { get; set; }
            public string Experiences { get; set; }
            public string Competences { get; set; }
            public string CentresInteret { get; set; }
        }
    }
}
