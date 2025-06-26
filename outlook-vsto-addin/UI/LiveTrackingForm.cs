using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EffyDocOutlookAddin.Services;
using EffyDocOutlookAddin.Models;
using Newtonsoft.Json;

namespace EffyDocOutlookAddin.UI
{
    public partial class LiveTrackingForm : UserControl
    {
        private ApiService apiService;
        private WebSocketService webSocketService;
        private string selectedDocumentId;
        private Timer refreshTimer;

        public LiveTrackingForm(ApiService apiService, WebSocketService webSocketService)
        {
            InitializeComponent();
            this.apiService = apiService;
            this.webSocketService = webSocketService;
            
            InitializeTracking();
        }

        private void InitializeTracking()
        {
            // Initialize refresh timer
            refreshTimer = new Timer();
            refreshTimer.Interval = 30000; // Refresh every 30 seconds
            refreshTimer.Tick += RefreshTimer_Tick;

            // Hook into WebSocket events
            webSocketService.MessageReceived += WebSocketService_MessageReceived;
            webSocketService.Connected += WebSocketService_Connected;
            webSocketService.Disconnected += WebSocketService_Disconnected;

            UpdateConnectionStatus();
        }

        private void WebSocketService_Connected(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                UpdateConnectionStatus();
            });
        }

        private void WebSocketService_Disconnected(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                UpdateConnectionStatus();
            });
        }

        private void WebSocketService_MessageReceived(object sender, string message)
        {
            try
            {
                var wsMessage = JsonConvert.DeserializeObject<WebSocketMessage>(message);
                
                this.Invoke((MethodInvoker)delegate
                {
                    HandleWebSocketMessage(wsMessage);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error processing WebSocket message: {ex.Message}");
            }
        }

        private void HandleWebSocketMessage(WebSocketMessage message)
        {
            switch (message.type)
            {
                case "tracking_update":
                    if (message.document_id == selectedDocumentId)
                    {
                        AddTrackingEvent($"Real-time: {message.data}");
                    }
                    break;
                case "document_activity":
                    AddTrackingEvent($"Activity: {message.data}");
                    break;
                case "subscription_confirmed":
                    lblStatus.Text = $"Tracking document: {message.document_id}";
                    lblStatus.ForeColor = Color.Green;
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine($"Unknown WebSocket message type: {message.type}");
                    break;
            }
        }

        private void UpdateConnectionStatus()
        {
            if (webSocketService.IsConnected)
            {
                lblConnectionStatus.Text = "Connected";
                lblConnectionStatus.ForeColor = Color.Green;
                btnConnect.Text = "Disconnect";
            }
            else
            {
                lblConnectionStatus.Text = "Disconnected";
                lblConnectionStatus.ForeColor = Color.Red;
                btnConnect.Text = "Connect";
            }
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (webSocketService.IsConnected)
                {
                    await webSocketService.DisconnectAsync();
                    refreshTimer.Stop();
                }
                else
                {
                    // Show login dialog if needed
                    var sessionInfo = await apiService.GetSessionInfoAsync();
                    await webSocketService.ConnectAsync(sessionInfo.user_email, "token"); // TODO: Get actual token
                    refreshTimer.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSelectDocument_Click(object sender, EventArgs e)
        {
            try
            {
                var documentSelector = new DocumentSelectorForm(apiService);
                if (documentSelector.ShowDialog() == DialogResult.OK)
                {
                    selectedDocumentId = documentSelector.SelectedDocumentId;
                    txtSelectedDocument.Text = documentSelector.SelectedDocumentTitle;
                    
                    await LoadDocumentTracking();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting document: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadDocumentTracking()
        {
            if (string.IsNullOrEmpty(selectedDocumentId)) return;

            try
            {
                lblStatus.Text = "Loading tracking data...";
                lblStatus.ForeColor = Color.Blue;

                // Subscribe to document updates via WebSocket
                if (webSocketService.IsConnected)
                {
                    await webSocketService.SubscribeToDocumentAsync(selectedDocumentId);
                }

                // Load current metrics
                var metrics = await apiService.GetLiveTrackingMetricsAsync(selectedDocumentId);
                DisplayMetrics(metrics);

                // Load analytics
                var analytics = await apiService.GetDocumentAnalyticsAsync(selectedDocumentId);
                DisplayAnalytics(analytics);

                lblStatus.Text = "Tracking data loaded";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading tracking data";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Failed to load tracking data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayMetrics(LiveTrackingMetrics metrics)
        {
            // Update current readers
            listCurrentReaders.Items.Clear();
            foreach (var reader in metrics.current_readers)
            {
                var item = new ListViewItem(reader.email);
                item.SubItems.Add($"Page {reader.page}");
                item.SubItems.Add($"{reader.duration}s");
                item.SubItems.Add(reader.since);
                listCurrentReaders.Items.Add(item);
            }

            // Update today's stats
            lblEmailsOpened.Text = $"Emails Opened: {metrics.today_stats.emails_opened}";
            lblLinksClicked.Text = $"Links Clicked: {metrics.today_stats.links_clicked}";
            lblPageViews.Text = $"Page Views: {metrics.today_stats.page_views}";
            lblUniqueViewers.Text = $"Unique Viewers: {metrics.today_stats.unique_viewers}";

            // Update recent activity
            listRecentActivity.Items.Clear();
            foreach (var activity in metrics.recent_activity.Take(10))
            {
                var item = new ListViewItem(activity.event_type);
                item.SubItems.Add(activity.recipient_email ?? "Unknown");
                item.SubItems.Add(activity.page_number?.ToString() ?? "-");
                item.SubItems.Add(activity.timestamp.ToString("HH:mm:ss"));
                listRecentActivity.Items.Add(item);
            }
        }

        private void DisplayAnalytics(DocumentAnalytics analytics)
        {
            // Update summary statistics
            lblTotalEmailsSent.Text = $"Total Emails: {analytics.summary.total_emails_sent}";
            lblTotalOpens.Text = $"Total Opens: {analytics.summary.total_opens}";
            lblTotalClicks.Text = $"Total Clicks: {analytics.summary.total_clicks}";
            lblOpenRate.Text = $"Open Rate: {analytics.summary.open_rate:F1}%";
            lblClickRate.Text = $"Click Rate: {analytics.summary.click_rate:F1}%";
            lblUniqueViewersTotal.Text = $"Unique Viewers: {analytics.summary.unique_viewers}";
        }

        private void AddTrackingEvent(string eventDescription)
        {
            var item = new ListViewItem("Real-time");
            item.SubItems.Add(eventDescription);
            item.SubItems.Add("-");
            item.SubItems.Add(DateTime.Now.ToString("HH:mm:ss"));
            item.ForeColor = Color.Blue;
            
            listRecentActivity.Items.Insert(0, item);
            
            // Keep only last 20 items
            while (listRecentActivity.Items.Count > 20)
            {
                listRecentActivity.Items.RemoveAt(listRecentActivity.Items.Count - 1);
            }
        }

        private async void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedDocumentId))
            {
                try
                {
                    var metrics = await apiService.GetLiveTrackingMetricsAsync(selectedDocumentId);
                    DisplayMetrics(metrics);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error refreshing metrics: {ex.Message}");
                }
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDocumentTracking();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                refreshTimer?.Stop();
                refreshTimer?.Dispose();
                
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}