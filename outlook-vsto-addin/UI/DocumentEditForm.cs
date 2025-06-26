using System;
using System.Drawing;
using System.Windows.Forms;
using EffyDocOutlookAddin.Services;
using EffyDocOutlookAddin.Models;

namespace EffyDocOutlookAddin.UI
{
    public partial class DocumentEditForm : Form
    {
        private DocumentContent documentContent;
        private ApiService apiService;
        private bool hasChanges = false;

        public DocumentEditForm(DocumentContent documentContent, ApiService apiService)
        {
            InitializeComponent();
            this.documentContent = documentContent;
            this.apiService = apiService;
            LoadDocumentForEditing();
        }

        private void LoadDocumentForEditing()
        {
            try
            {
                // Set form title
                this.Text = $"Edit Document - {documentContent.title}";

                // Load document title
                txtDocumentTitle.Text = documentContent.title;

                // Load pages into list
                listPages.Items.Clear();
                for (int i = 0; i < documentContent.pages.Count; i++)
                {
                    var page = documentContent.pages[i];
                    listPages.Items.Add($"Page {page.page_number}: {page.title}");
                }

                // Select first page
                if (listPages.Items.Count > 0)
                {
                    listPages.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading document for editing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Save current page if there are changes
            SaveCurrentPageIfChanged();

            // Load selected page
            if (listPages.SelectedIndex >= 0 && listPages.SelectedIndex < documentContent.pages.Count)
            {
                var selectedPage = documentContent.pages[listPages.SelectedIndex];
                LoadPageForEditing(selectedPage);
            }
        }

        private void LoadPageForEditing(DocumentPage page)
        {
            try
            {
                txtPageTitle.Text = page.title;
                
                // Extract text content from HTML for editing
                var textContent = ExtractTextFromHtml(page.content);
                txtPageContent.Text = textContent;

                hasChanges = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading page for editing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ExtractTextFromHtml(string htmlContent)
        {
            if (string.IsNullOrEmpty(htmlContent))
                return "";

            // Simple HTML tag removal for basic editing
            var text = htmlContent;
            text = System.Text.RegularExpressions.Regex.Replace(text, @"<[^>]+>", "");
            text = System.Web.HttpUtility.HtmlDecode(text);
            return text.Trim();
        }

        private string ConvertTextToHtml(string textContent)
        {
            if (string.IsNullOrEmpty(textContent))
                return "";

            // Simple text to HTML conversion
            var html = System.Web.HttpUtility.HtmlEncode(textContent);
            html = html.Replace("\r\n", "<br>").Replace("\n", "<br>");
            
            return $@"
            <div style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
                       line-height: 1.6; 
                       white-space: pre-wrap; 
                       text-align: left;
                       width: 100%;
                       padding: 20px;"">
                {html}
            </div>";
        }

        private void SaveCurrentPageIfChanged()
        {
            if (hasChanges && listPages.SelectedIndex >= 0)
            {
                var currentPage = documentContent.pages[listPages.SelectedIndex];
                currentPage.title = txtPageTitle.Text;
                currentPage.content = ConvertTextToHtml(txtPageContent.Text);
            }
        }

        private void txtPageTitle_TextChanged(object sender, EventArgs e)
        {
            hasChanges = true;
        }

        private void txtPageContent_TextChanged(object sender, EventArgs e)
        {
            hasChanges = true;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Save current page
                SaveCurrentPageIfChanged();

                btnSave.Enabled = false;
                btnSave.Text = "Saving...";

                // Update document title
                documentContent.title = txtDocumentTitle.Text;

                // Save document via API
                var updateData = new
                {
                    title = documentContent.title,
                    pages = documentContent.pages
                };

                await apiService.GetType().GetMethod("PutAsync").MakeGenericMethod(typeof(object))
                    .Invoke(apiService, new object[] { $"/api/outlook/documents/{documentContent.id}/content", updateData });

                MessageBox.Show("Document saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                hasChanges = false;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving document: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSave.Enabled = true;
                btnSave.Text = "Save";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (hasChanges)
            {
                var result = MessageBox.Show("You have unsaved changes. Are you sure you want to cancel?", 
                    "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                
                if (result != DialogResult.Yes)
                    return;
            }

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void DocumentEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hasChanges && this.DialogResult != DialogResult.OK)
            {
                var result = MessageBox.Show("You have unsaved changes. Save before closing?", 
                    "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                
                if (result == DialogResult.Yes)
                {
                    btnSave_Click(sender, e);
                    if (hasChanges) // If save failed
                        e.Cancel = true;
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }

    public partial class DocumentEditForm : Form
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtDocumentTitle;
        private Label lblDocumentTitle;
        private ListBox listPages;
        private TextBox txtPageTitle;
        private TextBox txtPageContent;
        private Label lblPageTitle;
        private Label lblPageContent;
        private Button btnSave;
        private Button btnCancel;
        private SplitContainer splitContainer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblDocumentTitle = new Label();
            this.txtDocumentTitle = new TextBox();
            this.splitContainer = new SplitContainer();
            this.listPages = new ListBox();
            this.lblPageTitle = new Label();
            this.txtPageTitle = new TextBox();
            this.lblPageContent = new Label();
            this.txtPageContent = new TextBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            
            // lblDocumentTitle
            this.lblDocumentTitle.AutoSize = true;
            this.lblDocumentTitle.Location = new Point(12, 15);
            this.lblDocumentTitle.Name = "lblDocumentTitle";
            this.lblDocumentTitle.Size = new Size(89, 13);
            this.lblDocumentTitle.TabIndex = 0;
            this.lblDocumentTitle.Text = "Document Title:";
            
            // txtDocumentTitle
            this.txtDocumentTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.txtDocumentTitle.Location = new Point(110, 12);
            this.txtDocumentTitle.Name = "txtDocumentTitle";
            this.txtDocumentTitle.Size = new Size(500, 20);
            this.txtDocumentTitle.TabIndex = 1;
            
            // splitContainer
            this.splitContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.splitContainer.Location = new Point(12, 45);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Size = new Size(760, 470);
            this.splitContainer.SplitterDistance = 200;
            this.splitContainer.TabIndex = 2;
            
            // splitContainer.Panel1
            this.splitContainer.Panel1.Controls.Add(this.listPages);
            
            // splitContainer.Panel2
            this.splitContainer.Panel2.Controls.Add(this.txtPageContent);
            this.splitContainer.Panel2.Controls.Add(this.lblPageContent);
            this.splitContainer.Panel2.Controls.Add(this.txtPageTitle);
            this.splitContainer.Panel2.Controls.Add(this.lblPageTitle);
            
            // listPages
            this.listPages.Dock = DockStyle.Fill;
            this.listPages.FormattingEnabled = true;
            this.listPages.Location = new Point(0, 0);
            this.listPages.Name = "listPages";
            this.listPages.Size = new Size(200, 470);
            this.listPages.TabIndex = 0;
            this.listPages.SelectedIndexChanged += new EventHandler(this.listPages_SelectedIndexChanged);
            
            // lblPageTitle
            this.lblPageTitle.AutoSize = true;
            this.lblPageTitle.Location = new Point(10, 10);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new Size(61, 13);
            this.lblPageTitle.TabIndex = 0;
            this.lblPageTitle.Text = "Page Title:";
            
            // txtPageTitle
            this.txtPageTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.txtPageTitle.Location = new Point(80, 7);
            this.txtPageTitle.Name = "txtPageTitle";
            this.txtPageTitle.Size = new Size(470, 20);
            this.txtPageTitle.TabIndex = 1;
            this.txtPageTitle.TextChanged += new EventHandler(this.txtPageTitle_TextChanged);
            
            // lblPageContent
            this.lblPageContent.AutoSize = true;
            this.lblPageContent.Location = new Point(10, 40);
            this.lblPageContent.Name = "lblPageContent";
            this.lblPageContent.Size = new Size(47, 13);
            this.lblPageContent.TabIndex = 2;
            this.lblPageContent.Text = "Content:";
            
            // txtPageContent
            this.txtPageContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.txtPageContent.Location = new Point(10, 60);
            this.txtPageContent.Multiline = true;
            this.txtPageContent.Name = "txtPageContent";
            this.txtPageContent.ScrollBars = ScrollBars.Vertical;
            this.txtPageContent.Size = new Size(540, 400);
            this.txtPageContent.TabIndex = 3;
            this.txtPageContent.TextChanged += new EventHandler(this.txtPageContent_TextChanged);
            
            // btnSave
            this.btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnSave.Location = new Point(616, 525);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(75, 25);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            
            // btnCancel
            this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(697, 525);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(75, 25);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            
            // DocumentEditForm
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new Size(784, 562);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.txtDocumentTitle);
            this.Controls.Add(this.lblDocumentTitle);
            this.MinimumSize = new Size(600, 400);
            this.Name = "DocumentEditForm";
            this.ShowIcon = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Edit Document - effyDOC";
            this.FormClosing += new FormClosingEventHandler(this.DocumentEditForm_FormClosing);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}