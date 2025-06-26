using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EffyDocOutlookAddin.Models;

namespace EffyDocOutlookAddin.Services
{
    public class ApiService
    {
        private readonly HttpClient httpClient;
        private readonly string baseUrl;
        private string authToken;

        public ApiService(string baseUrl)
        {
            this.baseUrl = baseUrl.TrimEnd('/');
            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Add("User-Agent", "effyDOC-Outlook-VSTO/1.0");
        }

        public void SetAuthToken(string token)
        {
            authToken = token;
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        #region Authentication Methods

        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            try
            {
                var loginData = new { email, password };
                var response = await PostAsync<LoginResponse>("/api/auth/login", loginData);
                
                if (response != null && !string.IsNullOrEmpty(response.access_token))
                {
                    SetAuthToken(response.access_token);
                }
                
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Login failed: {ex.Message}");
            }
        }

        public async Task<UserSessionInfo> GetSessionInfoAsync()
        {
            return await GetAsync<UserSessionInfo>("/api/outlook/user/session-info");
        }

        #endregion

        #region Document Library Methods

        public async Task<DocumentLibraryResponse> GetMyLibraryAsync()
        {
            return await GetAsync<DocumentLibraryResponse>("/api/outlook/documents/my-library");
        }

        public async Task<DocumentLibraryResponse> GetContentHubAsync()
        {
            return await GetAsync<DocumentLibraryResponse>("/api/outlook/documents/content-hub");
        }

        public async Task<DocumentContent> GetDocumentContentAsync(string documentId)
        {
            return await GetAsync<DocumentContent>($"/api/outlook/documents/{documentId}/content");
        }

        public async Task<TrackableLinkResponse> GenerateTrackableLinkAsync(string documentId)
        {
            return await GetAsync<TrackableLinkResponse>($"/api/outlook/documents/{documentId}/share-link");
        }

        public async Task<AttachmentDataResponse> GenerateAttachmentDataAsync(string documentId, object options)
        {
            return await PostAsync<AttachmentDataResponse>($"/api/outlook/documents/{documentId}/attachment-data", options);
        }

        #endregion

        #region Tracking Methods

        public async Task TrackEmailSentAsync(object trackingData)
        {
            await PostAsync("/api/outlook/tracking/email-sent", trackingData);
        }

        public async Task TrackDocumentEventAsync(object eventData)
        {
            await PostAsync("/api/outlook/tracking/event", eventData);
        }

        public async Task<LiveTrackingMetrics> GetLiveTrackingMetricsAsync(string documentId)
        {
            return await GetAsync<LiveTrackingMetrics>($"/api/outlook/tracking/live-metrics/{documentId}");
        }

        public async Task<DocumentAnalytics> GetDocumentAnalyticsAsync(string documentId)
        {
            return await GetAsync<DocumentAnalytics>($"/api/outlook/tracking/document-analytics/{documentId}");
        }

        #endregion

        #region HTTP Helper Methods

        private async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await httpClient.GetAsync(baseUrl + endpoint);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API request failed: {ex.Message}");
            }
        }

        private async Task<T> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await httpClient.PostAsync(baseUrl + endpoint, content);
                response.EnsureSuccessStatusCode();
                
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API request failed: {ex.Message}");
            }
        }

        private async Task PostAsync(string endpoint, object data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await httpClient.PostAsync(baseUrl + endpoint, content);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API request failed: {ex.Message}");
            }
        }

        #endregion

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}