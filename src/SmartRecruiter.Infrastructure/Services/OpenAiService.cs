using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using SmartRecruiter.Domain.DTOs;
using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Interfaces;

namespace SmartRecruiter.Infrastructure.Services;

public class OpenAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAi:ApiKey"] ?? throw new ArgumentNullException("ApiKey is missing");
    }

    public async Task<AiAnalysisResult> EvaluateCandidateAsync(JobVacancy vacancy, string resumeText)
    {
        string prompt = $$"""
                        Analyze this candidate for the vacancy: {{vacancy.Title}}.
                        Requirements: {{vacancy.AiPromptTemplate}}

                        DOSSIER:
                        {{resumeText}}

                        RETURN JSON ONLY:
                        {
                            "firstName": "Extracted First Name",
                            "lastName": "Extracted Last Name",
                            "score": 85,
                            "skills": ["C#", "SQL"],
                            "pros": ["Point 1"],
                            "cons": ["Point 2"],
                            "summary": "English summary"
                        }
                        """;

        var requestBody = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new { role = "system", content = "You are a recruitment assistant that outputs JSON." },
                new { role = "user", content = prompt }
            },
            temperature = 0.1,
            response_format = new { type = "json_object" }
        };

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        var response = await _httpClient.PostAsJsonAsync("https://api.groq.com/openai/v1/chat/completions", requestBody);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"AI Error: {error}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonNode = JsonNode.Parse(responseString);
        var content = jsonNode?["choices"]?[0]?["message"]?["content"]?.ToString();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<AiAnalysisResult>(content!, options)!;
    }
}