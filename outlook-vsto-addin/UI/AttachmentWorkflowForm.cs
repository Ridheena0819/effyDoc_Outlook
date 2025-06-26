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
using Outlook = Microsoft.Office.Interop.Outlook;
using System.IO;

namespace EffyDocOutlookAddin.UI
{
    public partial class AttachmentWorkflowForm : Form
    {
        private ApiService apiService;
        private Outlook.Application outlookApp;
        private DocumentInfo selectedDocument;
        private DocumentContent documentContent;

        public AttachmentWorkflowForm(ApiService apiService, Outlook.Application outlookApp)
        {
            InitializeComponent();
            this.apiService = apiService;
            this.outlookApp = outlookApp;
            
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set form properties
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = SystemIcons.Application;
            
            // Initialize attachment type selection
            radioTrackableAttachment.Checked = true;
            UpdateAttachmentOptions();
        }

        public void SetSelectedDocument(DocumentInfo document)
        {
            selectedDocument = document;
            
            if (selectedDocument != null)
            {
                txtDocumentTitle.Text = selectedDocument.title;
                txtDocumentType.Text = selectedDocument.type;
                txtDocumentPages.Text = selectedDocument.total_pages.ToString();
                
                _ = LoadDocumentContent();
            }
        }

        private async Task LoadDocumentContent()
        {
            try
            {
                lblStatus.Text = "Loading document content...";
                lblStatus.ForeColor = Color.Blue;
                
                documentContent = await apiService.GetDocumentContentAsync(selectedDocument.id);
                
                // Enable preview and edit buttons
                btnPreviewDocument.Enabled = true;
                btnEditDocument.Enabled = documentContent.can_edit;
                
                lblStatus.Text = "Document loaded successfully";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading document";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Failed to load document content: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void radioTrackableAttachment_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAttachmentOptions();
        }

        private void radioRegularAttachment_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAttachmentOptions();
        }

        private void UpdateAttachmentOptions()
        {
            if (radioTrackableAttachment.Checked)
            {
                groupTrackingOptions.Enabled = true;
                lblAttachmentDescription.Text = "This document will be attached as a trackable HTML file with embedded tracking links. Recipients' interactions will be monitored in real-time.";
                lblAttachmentDescription.ForeColor = Color.DarkGreen;
                
                btnPreviewDocument.Enabled = selectedDocument != null;
                btnEditDocument.Enabled = selectedDocument != null && documentContent?.can_edit == true;
            }
            else
            {
                groupTrackingOptions.Enabled = false;
                lblAttachmentDescription.Text = "This document will be attached as a regular file without tracking capabilities. No interaction data will be collected.";
                lblAttachmentDescription.ForeColor = Color.DarkOrange;
                
                btnPreviewDocument.Enabled = false;
                btnEditDocument.Enabled = false;
            }
        }

        private async void btnSelectDocument_Click(object sender, EventArgs e)
        {
            try
            {
                var documentSelector = new DocumentSelectorForm(apiService);
                if (documentSelector.ShowDialog() == DialogResult.OK)
                {
                    var libraryResponse = await apiService.GetMyLibraryAsync();
                    selectedDocument = libraryResponse.documents.FirstOrDefault(d => d.id == documentSelector.SelectedDocumentId);
                    
                    if (selectedDocument != null)
                    {
                        SetSelectedDocument(selectedDocument);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting document: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPreviewDocument_Click(object sender, EventArgs e)
        {
            if (documentContent != null)
            {
                var previewForm = new DocumentPreviewForm(documentContent, apiService);
                previewForm.Show();
            }
        }

        private void btnEditDocument_Click(object sender, EventArgs e)
        {
            if (documentContent != null && documentContent.can_edit)
            {
                var editForm = new DocumentEditForm(documentContent, apiService);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // Reload document content after editing
                    _ = LoadDocumentContent();
                }
            }
        }

        private async void btnAttachToEmail_Click(object sender, EventArgs e)
        {
            if (selectedDocument == null)
            {
                MessageBox.Show("Please select a document first.", "No Document Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                lblStatus.Text = "Attaching document to email...";
                lblStatus.ForeColor = Color.Blue;
                
                if (radioTrackableAttachment.Checked)
                {
                    await AttachTrackableDocument();
                }
                else
                {
                    await AttachRegularDocument();
                }
                
                lblStatus.Text = "Document attached successfully";
                lblStatus.ForeColor = Color.Green;
                
                // Show success message and close form
                MessageBox.Show("Document has been attached to your email successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error attaching document";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Failed to attach document: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task AttachTrackableDocument()
        {
            // Generate trackable attachment data
            var attachmentOptions = new
            {
                include_tracking = true,
                notify_on_open = chkNotifyOnOpen.Checked,
                notify_on_click = chkNotifyOnClick.Checked,
                track_page_views = chkTrackPageViews.Checked,
                track_time_spent = chkTrackTimeSpent.Checked
            };

            var attachmentData = await apiService.GenerateAttachmentDataAsync(selectedDocument.id, attachmentOptions);

            // Create temporary HTML file
            var tempDir = Path.GetTempPath();
            var fileName = Path.Combine(tempDir, attachmentData.filename);
            
            await File.WriteAllTextAsync(fileName, attachmentData.content);

            // Get the current mail item (compose window)
            var mailItem = GetCurrentMailItem();
            if (mailItem != null)
            {
                // Add attachment with tracking metadata
                var attachment = mailItem.Attachments.Add(fileName, Outlook.OlAttachmentType.olByValue, 1, attachmentData.filename);
                
                // Add custom properties for tracking
                attachment.PropertyAccessor.SetProperty("http://schemas.microsoft.com/mapi/string/{00020386-0000-0000-C000-000000000046}/effyDocId", selectedDocument.id);
                attachment.PropertyAccessor.SetProperty("http://schemas.microsoft.com/mapi/string/{00020386-0000-0000-C000-000000000046}/effyDocTrackingLink", attachmentData.tracking_link);
                
                // Clean up temp file
                try { File.Delete(fileName); } catch { }
                
                // Add tracking information to email body
                if (chkAddTrackingInfo.Checked)
                {
                    var trackingInfo = $"\n\n---\nThis document is powered by effyDOC and includes engagement tracking.\nView online: {attachmentData.tracking_link}";
                    mailItem.Body += trackingInfo;
                }
            }
            else
            {
                throw new Exception("No active email compose window found. Please open a new email first.");
            }
        }

        private async Task AttachRegularDocument()
        {
            // For regular attachment, we would need the original file
            // This is a simplified implementation
            MessageBox.Show("Regular file attachment is not yet implemented. Please use trackable attachment.", "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private Outlook.MailItem GetCurrentMailItem()
        {
            try
            {
                // Try to get the active inspector
                if (outlookApp.ActiveInspector() != null)
                {
                    var inspector = outlookApp.ActiveInspector();
                    if (inspector.CurrentItem is Outlook.MailItem mailItem)
                    {
                        return mailItem;
                    }
                }

                // If no active inspector, try to create a new mail item
                return outlookApp.CreateItem(Outlook.OlItemType.olMailItem) as Outlook.MailItem;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to access email: {ex.Message}");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void AttachmentWorkflowForm_Load(object sender, EventArgs e)
        {
            // Form load initialization
            lblStatus.Text = "Ready to attach document";
            lblStatus.ForeColor = Color.Black;
        }
    }
}