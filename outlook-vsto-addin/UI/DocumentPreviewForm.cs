using System;
using System.Drawing;
using System.Windows.Forms;
using EffyDocOutlookAddin.Services;
using EffyDocOutlookAddin.Models;

namespace EffyDocOutlookAddin.UI
{
    public partial class DocumentPreviewForm : Form
    {
        private DocumentContent documentContent;
        private ApiService apiService;

        public DocumentPreviewForm(DocumentContent documentContent, ApiService apiService)
        {
            InitializeComponent();
            this.documentContent = documentContent;
            this.apiService = apiService;
            LoadDocumentContent();
        }

        private void LoadDocumentContent()
        {
            try
            {
                // Set form title
                this.Text = $"Document Preview - {documentContent.title}";

                // Display document info
                lblDocumentTitle.Text = documentContent.title;
                lblDocumentType.Text = documentContent.type;
                lblTotalPages.Text = $"Pages: {documentContent.total_pages}";

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
                MessageBox.Show($"Error loading document: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listPages.SelectedIndex >= 0 && listPages.SelectedIndex < documentContent.pages.Count)
            {
                var selectedPage = documentContent.pages[listPages.SelectedIndex];
                DisplayPageContent(selectedPage);
            }
        }

        private void DisplayPageContent(DocumentPage page)
        {
            try
            {
                // Clear previous content
                webBrowserContent.DocumentText = "";

                // Create HTML content for display
                var htmlContent = $@"
                <html>
                <head>
                    <title>{page.title}</title>
                    <style>
                        body {{ 
                            font-family: Arial, sans-serif; 
                            margin: 20px; 
                            line-height: 1.6; 
                        }}
                        h1 {{ color: #333; }}
                        .page-content {{ 
                            background: white; 
                            padding: 20px; 
                            border-radius: 5px; 
                        }}
                    </style>
                </head>
                <body>
                    <h1>{page.title}</h1>
                    <div class='page-content'>
                        {page.content}
                    </div>
                </body>
                </html>";

                webBrowserContent.DocumentText = htmlContent;
            }
            catch (Exception ex)
            {
                webBrowserContent.DocumentText = $"<html><body><h3>Error loading page content:</h3><p>{ex.Message}</p></body></html>";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public partial class DocumentPreviewForm : Form
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblDocumentTitle;
        private Label lblDocumentType;
        private Label lblTotalPages;
        private ListBox listPages;
        private WebBrowser webBrowserContent;
        private Button btnClose;
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
            this.lblDocumentType = new Label();
            this.lblTotalPages = new Label();
            this.listPages = new ListBox();
            this.webBrowserContent = new WebBrowser();
            this.btnClose = new Button();
            this.splitContainer = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            
            // lblDocumentTitle
            this.lblDocumentTitle.Dock = DockStyle.Top;
            this.lblDocumentTitle.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            this.lblDocumentTitle.Location = new Point(0, 0);
            this.lblDocumentTitle.Name = "lblDocumentTitle";
            this.lblDocumentTitle.Padding = new Padding(10);
            this.lblDocumentTitle.Size = new Size(800, 40);
            this.lblDocumentTitle.TabIndex = 0;
            this.lblDocumentTitle.Text = "Document Title";
            
            // lblDocumentType
            this.lblDocumentType.Dock = DockStyle.Top;
            this.lblDocumentType.Location = new Point(0, 40);
            this.lblDocumentType.Name = "lblDocumentType";
            this.lblDocumentType.Padding = new Padding(10, 0, 10, 5);
            this.lblDocumentType.Size = new Size(800, 20);
            this.lblDocumentType.TabIndex = 1;
            this.lblDocumentType.Text = "Document Type";
            
            // lblTotalPages
            this.lblTotalPages.Dock = DockStyle.Top;
            this.lblTotalPages.Location = new Point(0, 60);
            this.lblTotalPages.Name = "lblTotalPages";
            this.lblTotalPages.Padding = new Padding(10, 0, 10, 10);
            this.lblTotalPages.Size = new Size(800, 25);
            this.lblTotalPages.TabIndex = 2;
            this.lblTotalPages.Text = "Pages: 0";
            
            // splitContainer
            this.splitContainer.Dock = DockStyle.Fill;
            this.splitContainer.Location = new Point(0, 85);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Size = new Size(800, 465);
            this.splitContainer.SplitterDistance = 200;
            this.splitContainer.TabIndex = 3;
            
            // splitContainer.Panel1
            this.splitContainer.Panel1.Controls.Add(this.listPages);
            
            // splitContainer.Panel2
            this.splitContainer.Panel2.Controls.Add(this.webBrowserContent);
            
            // listPages
            this.listPages.Dock = DockStyle.Fill;
            this.listPages.FormattingEnabled = true;
            this.listPages.Location = new Point(0, 0);
            this.listPages.Name = "listPages";
            this.listPages.Size = new Size(200, 465);
            this.listPages.TabIndex = 0;
            this.listPages.SelectedIndexChanged += new EventHandler(this.listPages_SelectedIndexChanged);
            
            // webBrowserContent
            this.webBrowserContent.Dock = DockStyle.Fill;
            this.webBrowserContent.Location = new Point(0, 0);
            this.webBrowserContent.MinimumSize = new Size(20, 20);
            this.webBrowserContent.Name = "webBrowserContent";
            this.webBrowserContent.Size = new Size(596, 465);
            this.webBrowserContent.TabIndex = 0;
            
            // btnClose
            this.btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnClose.Location = new Point(713, 560);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(75, 25);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new EventHandler(this.btnClose_Click);
            
            // DocumentPreviewForm
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 600);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.lblTotalPages);
            this.Controls.Add(this.lblDocumentType);
            this.Controls.Add(this.lblDocumentTitle);
            this.Controls.Add(this.btnClose);
            this.Name = "DocumentPreviewForm";
            this.ShowIcon = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Document Preview - effyDOC";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}