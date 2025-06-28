using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EffyDocOutlookPlugin
{
    public partial class TrackingPanel : Form
    {
        private Services.EffyDocApiService apiService;
        private Timer refreshTimer;

        public TrackingPanel()
        {
            InitializeComponent();
            apiService = new Services.EffyDocApiService();
            
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600);
            
            InitializeRefreshTimer();
        }

        private void InitializeRefreshTimer()
        {
            refreshTimer = new Timer();
            refreshTimer.Interval = 30000; // Refresh every 30 seconds
            refreshTimer.Tick += RefreshTimer_Tick;
        }

        private async void TrackingPanel_Load(object sender, EventArgs e)
        {
            await LoadDocumentAnalytics();
        }

        private async void RefreshTimer_Tick(object sender, EventArgs e)
        {
            await LoadDocumentAnalytics();
        }

        private async Task LoadDocumentAnalytics()
        {
            try
            {
                lblStatus.Text = "Loading analytics...";
                lblStatus.Visible = true;
                
                // Clear existing data
                lstDocuments.Items.Clear();
                lstRecentActivity.Items.Clear();
                
                // Load user documents
                var documents = await apiService.GetUserDocumentsAsync();
                
                if (documents != null && documents.Any())
                {
                    foreach (var doc in documents.Take(10)) // Show top 10 documents
                    {
                        var item = new ListViewItem(doc.Title);
                        item.SubItems.Add(doc.Type);
                        item.SubItems.Add(doc.TotalViews.ToString());
                        item.SubItems.Add(doc.UpdatedAt.ToString("MMM dd, HH:mm"));
                        item.Tag = doc;
                        lstDocuments.Items.Add(item);
                        
                        // Load analytics for this document
                        var analytics = await apiService.GetDocumentAnalyticsAsync(doc.Id);
                        if (analytics != null)
                        {
                            // Update view count with real analytics
                            item.SubItems[2].Text = analytics.summary?.total_views?.ToString() ?? "0";
                        }
                    }
                    
                    // Update summary stats
                    var totalDocs = documents.Count();
                    var totalViews = documents.Sum(d => d.TotalViews);
                    
                    lblTotalDocuments.Text = $"📄 {totalDocs} Documents";
                    lblTotalViews.Text = $"👁 {totalViews} Total Views";
                    lblActiveToday.Text = $"🔥 {documents.Count(d => d.UpdatedAt.Date == DateTime.Today)} Active Today";
                    
                    lblStatus.Text = $"Loaded analytics for {totalDocs} documents";
                }
                else
                {
                    lblStatus.Text = "No documents found. Create documents at effyDOC.com";
                    lblTotalDocuments.Text = "📄 0 Documents";
                    lblTotalViews.Text = "👁 0 Total Views";
                    lblActiveToday.Text = "🔥 0 Active Today";
                }
                
                // Start auto-refresh
                if (!refreshTimer.Enabled)
                {
                    refreshTimer.Start();
                }
                
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading analytics. Please check your connection.";
                MessageBox.Show($"Error loading analytics: {ex.Message}", "effyDOC Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void lstDocuments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDocuments.SelectedItems.Count > 0)
            {
                var selectedItem = lstDocuments.SelectedItems[0];
                var document = selectedItem.Tag as Models.DocumentModel;
                
                if (document != null)
                {
                    await LoadDocumentDetails(document);
                }
            }
            else
            {
                ClearDocumentDetails();
            }
        }

        private async Task LoadDocumentDetails(Models.DocumentModel document)
        {
            try
            {
                lblDocumentTitle.Text = $"📊 Analytics: {document.Title}";
                lblDocumentInfo.Text = $"Type: {document.Type} | Pages: {document.TotalPages} | Updated: {document.UpdatedAt:MMM dd, yyyy}";
                
                // Load detailed analytics
                var analytics = await apiService.GetDocumentAnalyticsAsync(document.Id);
                
                if (analytics != null)
                {
                    // Update analytics display
                    var summary = analytics.summary;
                    if (summary != null)
                    {
                        lblTotalOpens.Text = $"📧 {summary.total_opens ?? 0} Opens";
                        lblTotalClicks.Text = $"🖱 {summary.total_clicks ?? 0} Clicks";
                        lblUniqueViewers.Text = $"👥 {summary.unique_viewers ?? 0} Unique Viewers";
                        
                        var openRate = summary.open_rate ?? 0;
                        var clickRate = summary.click_rate ?? 0;
                        
                        lblOpenRate.Text = $"📈 {openRate:F1}% Open Rate";
                        lblClickRate.Text = $"📊 {clickRate:F1}% Click Rate";
                    }
                    
                    // Load recent activity
                    lstRecentActivity.Items.Clear();
                    
                    // Add some sample recent activity (in production, this would come from the API)
                    var recentActivities = new[]
                    {
                        new { Event = "Document Opened", User = "client@example.com", Time = DateTime.Now.AddMinutes(-5) },
                        new { Event = "Page 2 Viewed", User = "client@example.com", Time = DateTime.Now.AddMinutes(-10) },
                        new { Event = "Email Opened", User = "prospect@company.com", Time = DateTime.Now.AddMinutes(-15) },
                        new { Event = "Link Clicked", User = "manager@business.com", Time = DateTime.Now.AddMinutes(-30) }
                    };
                    
                    foreach (var activity in recentActivities)
                    {
                        var activityItem = new ListViewItem(activity.Event);
                        activityItem.SubItems.Add(activity.User);
                        activityItem.SubItems.Add(activity.Time.ToString("HH:mm"));
                        lstRecentActivity.Items.Add(activityItem);
                    }
                }
                else
                {
                    // Show basic info if detailed analytics not available
                    lblTotalOpens.Text = "📧 - Opens";
                    lblTotalClicks.Text = "🖱 - Clicks";
                    lblUniqueViewers.Text = "👥 - Unique Viewers";
                    lblOpenRate.Text = "📈 - Open Rate";
                    lblClickRate.Text = "📊 - Click Rate";
                }
                
                btnViewOnline.Enabled = true;
                btnShareDocument.Enabled = true;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading document details: {ex.Message}", "effyDOC Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearDocumentDetails()
        {
            lblDocumentTitle.Text = "📊 Select a document to view analytics";
            lblDocumentInfo.Text = "";
            lblTotalOpens.Text = "📧 - Opens";
            lblTotalClicks.Text = "🖱 - Clicks";
            lblUniqueViewers.Text = "👥 - Unique Viewers";
            lblOpenRate.Text = "📈 - Open Rate";
            lblClickRate.Text = "📊 - Click Rate";
            
            lstRecentActivity.Items.Clear();
            btnViewOnline.Enabled = false;
            btnShareDocument.Enabled = false;
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDocumentAnalytics();
        }

        private void btnViewOnline_Click(object sender, EventArgs e)
        {
            if (lstDocuments.SelectedItems.Count > 0)
            {
                var document = lstDocuments.SelectedItems[0].Tag as Models.DocumentModel;
                if (document != null)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(document.TrackingLink);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening document: {ex.Message}", "effyDOC Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnShareDocument_Click(object sender, EventArgs e)
        {
            if (lstDocuments.SelectedItems.Count > 0)
            {
                var document = lstDocuments.SelectedItems[0].Tag as Models.DocumentModel;
                if (document != null)
                {
                    try
                    {
                        // Copy tracking link to clipboard
                        Clipboard.SetText(document.TrackingLink);
                        MessageBox.Show("Document link copied to clipboard!", "effyDOC", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error copying link: {ex.Message}", "effyDOC Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TrackingPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the refresh timer
            refreshTimer?.Stop();
            refreshTimer?.Dispose();
        }

        private void chkAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoRefresh.Checked)
            {
                refreshTimer.Start();
            }
            else
            {
                refreshTimer.Stop();
            }
        }
    }
}