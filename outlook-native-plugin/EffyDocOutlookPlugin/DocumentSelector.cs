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
    public partial class DocumentSelector : Form
    {
        public Models.DocumentModel SelectedDocument { get; private set; }
        private Services.EffyDocApiService apiService;

        public DocumentSelector()
        {
            InitializeComponent();
            apiService = new Services.EffyDocApiService();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private async void DocumentSelector_Load(object sender, EventArgs e)
        {
            await LoadDocuments();
        }

        private async Task LoadDocuments()
        {
            try
            {
                lblStatus.Text = "Loading your documents...";
                lblStatus.Visible = true;
                lstDocuments.Items.Clear();
                btnAttach.Enabled = false;

                var documents = await apiService.GetUserDocumentsAsync();

                if (documents != null && documents.Any())
                {
                    foreach (var doc in documents)
                    {
                        var item = new ListViewItem(doc.Title);
                        item.SubItems.Add(doc.Type);
                        item.SubItems.Add(doc.UpdatedAt.ToString("MMM dd, yyyy"));
                        item.SubItems.Add($"{doc.TotalViews} views");
                        item.Tag = doc;
                        lstDocuments.Items.Add(item);
                    }

                    lblStatus.Text = $"Found {documents.Count()} documents";
                }
                else
                {
                    lblStatus.Text = "No documents found. Create documents at effyDOC.com";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading documents. Please check your connection.";
                MessageBox.Show($"Error loading documents: {ex.Message}", "effyDOC Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstDocuments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDocuments.SelectedItems.Count > 0)
            {
                var selectedItem = lstDocuments.SelectedItems[0];
                SelectedDocument = selectedItem.Tag as Models.DocumentModel;
                btnAttach.Enabled = true;

                // Show document preview
                if (SelectedDocument != null)
                {
                    txtPreview.Text = $"Title: {SelectedDocument.Title}\n" +
                                     $"Type: {SelectedDocument.Type}\n" +
                                     $"Pages: {SelectedDocument.TotalPages}\n" +
                                     $"Last Updated: {SelectedDocument.UpdatedAt:yyyy-MM-dd HH:mm}\n" +
                                     $"Total Views: {SelectedDocument.TotalViews}\n\n" +
                                     $"Description: {SelectedDocument.Description ?? "No description available"}";
                }
            }
            else
            {
                btnAttach.Enabled = false;
                txtPreview.Clear();
            }
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            if (SelectedDocument != null)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDocuments();
        }

        private void btnCreateNew_Click(object sender, EventArgs e)
        {
            try
            {
                // Open effyDOC website to create new document
                System.Diagnostics.Process.Start("https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com/create");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening website: {ex.Message}", "effyDOC Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstDocuments_DoubleClick(object sender, EventArgs e)
        {
            if (btnAttach.Enabled)
            {
                btnAttach_Click(sender, e);
            }
        }
    }
}