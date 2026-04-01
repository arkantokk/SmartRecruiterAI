using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Interfaces;

public class GoogleOAuthClient : IOAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public GoogleOAuthClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public string GetAuthorizationUrl(string state)
    {
        var clientId = _config["GoogleAuth:ClientId"];
        var redirectUri = _config["GoogleAuth:RedirectUri"];

        return $"https://accounts.google.com/o/oauth2/v2/auth?" +
               $"client_id={clientId}&" +
               $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
               $"response_type=code&" +
               $"scope=https://mail.google.com/&" +
               $"access_type=offline&" +
               $"prompt=consent&" +
               $"state={state}";
    }

    public async Task<OAuthTokenResponse> ExchangeCodeAsync(string code)
    {
        var clientId = _config["GoogleAuth:ClientId"];
        var clientSecret = _config["GoogleAuth:ClientSecret"];
        var redirectUri = _config["GoogleAuth:RedirectUri"];

        var tokenRequest = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "code", code },
            { "grant_type", "authorization_code" },
            { "redirect_uri", redirectUri }
        };

        var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenRequest));
        response.EnsureSuccessStatusCode();
        var tokenData = await response.Content.ReadFromJsonAsync<GoogleTokenResponseInternal>();

        var request = new HttpRequestMessage(HttpMethod.Get, "https://gmail.googleapis.com/gmail/v1/users/me/profile");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenData.access_token);
        
        var profileResponse = await _httpClient.SendAsync(request);
        profileResponse.EnsureSuccessStatusCode();
        var profileData = await profileResponse.Content.ReadFromJsonAsync<GmailProfileResponseInternal>();
        return new OAuthTokenResponse(
            tokenData.access_token,
            tokenData.refresh_token,
            tokenData.expires_in,
            profileData.emailAddress
        );
    }

    public async Task<TokenRefreshResponse> RefreshTokenAsync(string refreshToken)
    {
        var clientId = _config["GoogleAuth:ClientId"];
        var clientSecret = _config["GoogleAuth:ClientSecret"];
        var tokenRequest = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "refresh_token", refreshToken },
            { "grant_type", "refresh_token" }
        };

        var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenRequest));
        response.EnsureSuccessStatusCode();
        var tokenData = await response.Content.ReadFromJsonAsync<GoogleTokenResponseInternal>();
        if (tokenData.refresh_token.IsNullOrEmpty())
        {
            return new TokenRefreshResponse(tokenData.access_token,
                refreshToken,
                tokenData.expires_in);
        }
        return new TokenRefreshResponse(
            tokenData.access_token,
            tokenData.refresh_token,
            tokenData.expires_in);
    }
    
    
    
    private class GoogleTokenResponseInternal {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
    }

    private class GmailProfileResponseInternal {
        public string emailAddress { get; set; }
    }
}