using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace EffyDocOutlookPlugin.Services
{
    public class EffyDocApiService
    {
        private readonly HttpClient httpClient;
        private readonly string baseUrl = "https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com/api";
        private string authToken;

        public EffyDocApiService()
        {
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            
            // Load saved auth token if available
            LoadAuthToken();
        }

        private void LoadAuthToken()
        {
            try
            {
                // Load token from registry or user settings
                // For now, we'll use a placeholder - in production, implement proper token storage
                authToken = GetStoredAuthToken();
                
                if (!string.IsNullOrEmpty(authToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading auth token: {ex.Message}");
            }
        }

        private string GetStoredAuthToken()
        {
            try
            {
                // In production, implement secure token storage
                // For now, return null to prompt for login
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            try
            {
                var loginData = new { email = email, password = password };
                var json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{baseUrl}/auth/login", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(responseContent);
                    
                    authToken = result.access_token;
                    httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                    
                    // Store token securely
                    SaveAuthToken(authToken);
                    
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Authentication error: {ex.Message}");
                return false;
            }
        }

        private void SaveAuthToken(string token)
        {
            try
            {
                // In production, implement secure token storage
                // For now, just store in memory
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving auth token: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Models.DocumentModel>> GetUserDocumentsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(authToken))
                {
                    // Prompt for authentication
                    throw new UnauthorizedAccessException("Please login to effyDOC first");
                }

                var response = await httpClient.GetAsync($"{baseUrl}/outlook/documents/my-library");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(json);
                    
                    var documents = new List<Models.DocumentModel>();
                    
                    foreach (var doc in result.documents)
                    {
                        documents.Add(new Models.DocumentModel
                        {
                            Id = doc.id,
                            Title = doc.title,
                            Type = doc.type,
                            UpdatedAt = DateTime.Parse(doc.updated_at.ToString()),
                            TotalPages = doc.total_pages ?? 1,
                            TotalViews = doc.tracking_stats?.total_views ?? 0,
                            Description = doc.description,
                            TrackingLink = $"{baseUrl.Replace("/api", "")}/view/{doc.id}?source=outlook"
                        });
                    }
                    
                    return documents;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Please login to effyDOC first");
                }
                
                return new List<Models.DocumentModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting documents: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GenerateTrackableHtmlAsync(string documentId)
        {
            try
            {
                if (string.IsNullOrEmpty(authToken))
                {
                    throw new UnauthorizedAccessException("Please login to effyDOC first");
                }

                var response = await httpClient.PostAsync($"{baseUrl}/outlook/documents/{documentId}/attachment-data", 
                    new StringContent("{}", Encoding.UTF8, "application/json"));
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(json);
                    
                    return result.content;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating trackable HTML: {ex.Message}");
                return null;
            }
        }

        public async Task TrackEmailSentAsync(Outlook.MailItem mailItem)
        {
            try
            {
                if (string.IsNullOrEmpty(authToken) || mailItem == null)
                {
                    return;
                }

                // Extract document IDs from email content
                var documentIds = ExtractDocumentIds(mailItem.HTMLBody);
                
                if (documentIds.Any())
                {
                    var recipients = new List<string>();
                    
                    // Get recipients
                    foreach (Outlook.Recipient recipient in mailItem.Recipients)
                    {
                        recipients.Add(recipient.Address);
                    }

                    var trackingData = new
                    {
                        document_id = documentIds.First(), // Track first document for now
                        recipients = recipients,
                        subject = mailItem.Subject,
                        email_body = mailItem.HTMLBody
                    };

                    var json = JsonConvert.SerializeObject(trackingData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    await httpClient.PostAsync($"{baseUrl}/outlook/tracking/email-sent", content);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error tracking email sent: {ex.Message}");
            }
        }

        private List<string> ExtractDocumentIds(string htmlContent)
        {
            var documentIds = new List<string>();
            
            try
            {
                // Look for data-effydoc-document attributes in the HTML
                var startTag = "data-effydoc-document=\"";
                var index = 0;
                
                while ((index = htmlContent.IndexOf(startTag, index)) != -1)
                {
                    index += startTag.Length;
                    var endIndex = htmlContent.IndexOf("\"", index);
                    
                    if (endIndex > index)
                    {
                        var documentId = htmlContent.Substring(index, endIndex - index);
                        if (!documentIds.Contains(documentId))
                        {
                            documentIds.Add(documentId);
                        }
                    }
                    
                    index = endIndex;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error extracting document IDs: {ex.Message}");
            }
            
            return documentIds;
        }

        public async Task<dynamic> GetDocumentAnalyticsAsync(string documentId)
        {
            try
            {
                if (string.IsNullOrEmpty(authToken))
                {
                    throw new UnauthorizedAccessException("Please login to effyDOC first");
                }

                var response = await httpClient.GetAsync($"{baseUrl}/outlook/tracking/document-analytics/{documentId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject(json);
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting analytics: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}