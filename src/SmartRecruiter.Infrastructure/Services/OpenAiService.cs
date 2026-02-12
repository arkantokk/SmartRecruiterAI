using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Interfaces;
using SmartRecruiter.Domain.ValueObjects;

namespace SmartRecruiter.Infrastructure.Services;

public class OpenAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAi:ApiKey"]; // Тут має бути ключ 'gsk_...'
    }

    public async Task<CandidateEvaluation> EvaluateCandidateAsync(Candidate candidate, JobVacancy vacancy, string resumeText)
    {
        // 1. Промпт (залишаємо як є)
        // ... у методі EvaluateCandidateAsync ...
        string prompt = $$"""
                          You are a professional Talent Acquisition Specialist.
                          Evaluate the candidate for the following vacancy.

                          JOB VACANCY: {{vacancy.Title}}

                          CANDIDATE DATA:
                          ---
                          {{resumeText}}
                          ---

                          INSTRUCTIONS:
                          1. Extract the candidate's real First Name and Last Name from the text.
                          2. Provide a match score (0-100).
                          3. List tech skills, pros, and cons.

                          OUTPUT FORMAT (STRICT JSON):
                          {
                              "firstName": "Extracted Name",
                              "lastName": "Extracted Surname",
                              "score": 85,
                              "skills": ["C#", "SQL"],
                              "pros": ["Experience..."],
                              "cons": ["Lacks..."],
                              "summary": "Summary text..."
                          }
                          """;

        // 2. Тіло запиту
        // 👇 ЗМІНА 1: Використовуємо найновішу модель Groq
        var requestBody = new
        {
            model = "llama-3.3-70b-versatile", 
            messages = new[]
            {
                new { role = "system", content = "You are a helpful assistant designed to output JSON." },
                new { role = "user", content = prompt }
            },
            temperature = 0.1, // Трохи зменшив для стабільності
            response_format = new { type = "json_object" }
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // 3. Авторизація
        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        // 4. Відправка (GROQ URL)
        var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", httpContent);
        
        // 👇 ЗМІНА 2: Читаємо текст помилки, якщо щось пішло не так!
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            // Цей текст вилетить у консоль червоним, і ми зрозуміємо причину
            throw new Exception($"🛑 GROQ ERROR ({response.StatusCode}): {errorBody}");
        }

        // 5. Успіх - розбираємо відповідь
        var responseString = await response.Content.ReadAsStringAsync();
        
        try 
        {
            var jsonNode = JsonNode.Parse(responseString);
            var aiContentString = jsonNode?["choices"]?[0]?["message"]?["content"]?.ToString();

            if (string.IsNullOrEmpty(aiContentString))
                throw new Exception("Groq returned empty content.");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var evaluation = JsonSerializer.Deserialize<CandidateEvaluation>(aiContentString, options);

            return evaluation;
        }
        catch (Exception ex)
        {
            // Якщо Groq повернув не JSON, а просто текст
            throw new Exception($"Failed to parse AI response: {ex.Message}. Response was: {responseString}");
        }
    }
}