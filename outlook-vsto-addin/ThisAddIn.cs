using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools;
using Microsoft.Office.Tools.Outlook;
using System.Windows.Forms;
using EffyDocOutlookAddin.Services;
using EffyDocOutlookAddin.UI;

namespace EffyDocOutlookAddin
{
    public partial class ThisAddIn
    {
        private ApiService apiService;
        private WebSocketService webSocketService;
        private DocumentLibraryForm documentLibraryForm;
        private LiveTrackingForm liveTrackingForm;
        private AttachmentWorkflowForm attachmentWorkflowForm;
        private CustomTaskPane documentLibraryTaskPane;
        private CustomTaskPane liveTrackingTaskPane;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            try
            {
                // Initialize services
                InitializeServices();

                // Create task panes
                CreateTaskPanes();

                // Add ribbon
                AddRibbon();

                // Hook into Outlook events
                HookOutlookEvents();

                MessageBox.Show("effyDOC Outlook Add-in loaded successfully!", "effyDOC", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading effyDOC Add-in: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeServices()
        {
            // Initialize API service with backend URL
            string backendUrl = System.Configuration.ConfigurationManager.AppSettings["BackendUrl"] ?? "https://f70e6bf0-40a7-454d-8960-1649b6f10c4c.preview.emergentagent.com";
            apiService = new ApiService(backendUrl);

            // Initialize WebSocket service for real-time tracking
            webSocketService = new WebSocketService(backendUrl.Replace("https://", "wss://").Replace("http://", "ws://") + "/api/outlook/ws");
        }

        private void CreateTaskPanes()
        {
            // Create Document Library Task Pane
            documentLibraryForm = new DocumentLibraryForm(apiService);
            documentLibraryTaskPane = this.CustomTaskPanes.Add(documentLibraryForm, "effyDOC Library");
            documentLibraryTaskPane.Width = 350;
            documentLibraryTaskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionRight;

            // Create Live Tracking Task Pane
            liveTrackingForm = new LiveTrackingForm(apiService, webSocketService);
            liveTrackingTaskPane = this.CustomTaskPanes.Add(liveTrackingForm, "effyDOC Tracking");
            liveTrackingTaskPane.Width = 350;
            liveTrackingTaskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionRight;

            // Initially hide both task panes
            documentLibraryTaskPane.Visible = false;
            liveTrackingTaskPane.Visible = false;
        }

        private void AddRibbon()
        {
            // The ribbon is added automatically via the EffyDocRibbon class
        }

        private void HookOutlookEvents()
        {
            // Hook into mail send events to track when documents are sent
            this.Application.ItemSend += Application_ItemSend;
        }

        private void Application_ItemSend(object Item, ref bool Cancel)
        {
            try
            {
                if (Item is Outlook.MailItem mailItem)
                {
                    // Check if this email contains trackable attachments
                    var trackableAttachments = GetTrackableAttachments(mailItem);
                    
                    if (trackableAttachments.Count > 0)
                    {
                        // Track email send event
                        TrackEmailSent(mailItem, trackableAttachments);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't cancel send
                System.Diagnostics.Debug.WriteLine($"Error tracking email send: {ex.Message}");
            }
        }

        private List<string> GetTrackableAttachments(Outlook.MailItem mailItem)
        {
            var trackableAttachments = new List<string>();
            
            foreach (Outlook.Attachment attachment in mailItem.Attachments)
            {
                // Check if attachment is from effyDOC (contains tracking metadata)
                if (attachment.DisplayName.Contains("effyDOC") || 
                    attachment.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/string/{00020386-0000-0000-C000-000000000046}/effyDocId") != null)
                {
                    trackableAttachments.Add(attachment.DisplayName);
                }
            }
            
            return trackableAttachments;
        }

        private async void TrackEmailSent(Outlook.MailItem mailItem, List<string> trackableAttachments)
        {
            try
            {
                var recipients = new List<string>();
                foreach (Outlook.Recipient recipient in mailItem.Recipients)
                {
                    recipients.Add(recipient.Address);
                }

                var trackingData = new
                {
                    document_id = GetDocumentIdFromAttachments(mailItem),
                    recipients = recipients,
                    subject = mailItem.Subject,
                    email_body = mailItem.Body,
                    attachments = trackableAttachments
                };

                await apiService.TrackEmailSentAsync(trackingData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error tracking email: {ex.Message}");
            }
        }

        private string GetDocumentIdFromAttachments(Outlook.MailItem mailItem)
        {
            // Extract document ID from attachment metadata
            foreach (Outlook.Attachment attachment in mailItem.Attachments)
            {
                try
                {
                    var documentId = attachment.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/string/{00020386-0000-0000-C000-000000000046}/effyDocId");
                    if (documentId != null)
                    {
                        return documentId.ToString();
                    }
                }
                catch
                {
                    // Continue checking other attachments
                }
            }
            return null;
        }

        // Public methods to show/hide task panes (called from ribbon)
        public void ShowDocumentLibrary()
        {
            documentLibraryTaskPane.Visible = true;
            liveTrackingTaskPane.Visible = false;
        }

        public void ShowLiveTracking()
        {
            liveTrackingTaskPane.Visible = true;
            documentLibraryTaskPane.Visible = false;
        }

        public void ShowAttachmentWorkflow()
        {
            if (attachmentWorkflowForm == null || attachmentWorkflowForm.IsDisposed)
            {
                attachmentWorkflowForm = new AttachmentWorkflowForm(apiService, this.Application);
            }
            attachmentWorkflowForm.Show();
        }

        public void HideAllTaskPanes()
        {
            documentLibraryTaskPane.Visible = false;
            liveTrackingTaskPane.Visible = false;
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            try
            {
                // Cleanup resources
                webSocketService?.Dispose();
                
                if (documentLibraryForm != null && !documentLibraryForm.IsDisposed)
                    documentLibraryForm.Dispose();
                
                if (liveTrackingForm != null && !liveTrackingForm.IsDisposed)
                    liveTrackingForm.Dispose();
                
                if (attachmentWorkflowForm != null && !attachmentWorkflowForm.IsDisposed)
                    attachmentWorkflowForm.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during shutdown: {ex.Message}");
            }
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}