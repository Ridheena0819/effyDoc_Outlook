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

namespace EffyDocOutlookAddin.UI
{
    public partial class DocumentLibraryForm : UserControl
    {
        private ApiService apiService;
        private List<DocumentInfo> currentDocuments;
        private bool isLoggedIn = false;

        public DocumentLibraryForm(ApiService apiService)
        {
            InitializeComponent();
            this.apiService = apiService;
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                await ShowLoginIfNeeded();
                await LoadDocuments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing Document Library: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ShowLoginIfNeeded()
        {
            if (!isLoggedIn)
            {
                var loginForm = new LoginForm(apiService);
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    isLoggedIn = true;
                    lblStatus.Text = "Connected to effyDOC";
                    lblStatus.ForeColor = Color.Green;
                }
                else
                {
                    lblStatus.Text = "Not connected";
                    lblStatus.ForeColor = Color.Red;
                    return;
                }
            }
        }

        private async Task LoadDocuments()
        {
            if (!isLoggedIn) return;

            try
            {
                lblStatus.Text = "Loading documents...";
                lblStatus.ForeColor = Color.Blue;

                DocumentLibraryResponse response;
                if (tabControl.SelectedTab == tabMyLibrary)
                {
                    response = await apiService.GetMyLibraryAsync();
                }
                else
                {
                    response = await apiService.GetContentHubAsync();
                }

                currentDocuments = response.documents;
                PopulateDocumentList(currentDocuments);

                lblStatus.Text = $"Loaded {currentDocuments.Count} documents";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading documents";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Failed to load documents: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateDocumentList(List<DocumentInfo> documents)
        {
            listViewDocuments.Items.Clear();

            foreach (var doc in documents)
            {
                var item = new ListViewItem(doc.title);
                item.SubItems.Add(doc.type);
                item.SubItems.Add(doc.total_pages.ToString());
                item.SubItems.Add(FormatFileSize(doc.file_size));
                item.SubItems.Add(doc.tracking_stats.total_views.ToString());
                item.SubItems.Add(doc.created_at);
                item.Tag = doc;

                // Add icon based on document type
                if (doc.type.Contains("PDF"))
                    item.ImageIndex = 0;
                else if (doc.type.Contains("Word") || doc.type.Contains("DOCX"))
                    item.ImageIndex = 1;
                else
                    item.ImageIndex = 2;

                listViewDocuments.Items.Add(item);
            }
        }

        private string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024} KB";
            return $"{bytes / (1024 * 1024)} MB";
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDocuments();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            _ = LoadDocuments();
        }

        private void listViewDocuments_DoubleClick(object sender, EventArgs e)
        {
            if (listViewDocuments.SelectedItems.Count > 0)
            {
                var selectedDoc = (DocumentInfo)listViewDocuments.SelectedItems[0].Tag;
                ShowDocumentPreview(selectedDoc);
            }
        }

        private async void ShowDocumentPreview(DocumentInfo document)
        {
            try
            {
                var content = await apiService.GetDocumentContentAsync(document.id);
                var previewForm = new DocumentPreviewForm(content, apiService);
                previewForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load document preview: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnAttachSelected_Click(object sender, EventArgs e)
        {
            if (listViewDocuments.SelectedItems.Count > 0)
            {
                var selectedDoc = (DocumentInfo)listViewDocuments.SelectedItems[0].Tag;
                await AttachDocumentToEmail(selectedDoc);
            }
            else
            {
                MessageBox.Show("Please select a document to attach.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task AttachDocumentToEmail(DocumentInfo document)
        {
            try
            {
                var attachmentForm = new AttachmentWorkflowForm(apiService, Globals.ThisAddIn.Application);
                attachmentForm.SetSelectedDocument(document);
                attachmentForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to attach document: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterDocuments();
        }

        private void FilterDocuments()
        {
            if (currentDocuments == null) return;

            var searchText = txtSearch.Text.ToLower();
            var filteredDocs = currentDocuments.Where(d => 
                d.title.ToLower().Contains(searchText) ||
                d.type.ToLower().Contains(searchText) ||
                (d.tags != null && d.tags.Any(t => t.ToLower().Contains(searchText)))
            ).ToList();

            PopulateDocumentList(filteredDocs);
        }
    }
}