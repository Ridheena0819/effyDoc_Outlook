using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EffyDocOutlookAddin.Services;
using EffyDocOutlookAddin.Models;

namespace EffyDocOutlookAddin.UI
{
    public partial class DocumentSelectorForm : Form
    {
        private ApiService apiService;
        private List<DocumentInfo> documents;

        public string SelectedDocumentId { get; private set; }
        public string SelectedDocumentTitle { get; private set; }

        public DocumentSelectorForm(ApiService apiService)
        {
            InitializeComponent();
            this.apiService = apiService;
            LoadDocuments();
        }

        private async void LoadDocuments()
        {
            try
            {
                lblStatus.Text = "Loading documents...";
                lblStatus.ForeColor = Color.Blue;

                var response = await apiService.GetMyLibraryAsync();
                documents = response.documents;

                PopulateDocumentList();
                
                lblStatus.Text = $"Loaded {documents.Count} documents";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading documents";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Failed to load documents: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateDocumentList()
        {
            listViewDocuments.Items.Clear();

            foreach (var doc in documents)
            {
                var item = new ListViewItem(doc.title);
                item.SubItems.Add(doc.type);
                item.SubItems.Add(doc.total_pages.ToString());
                item.SubItems.Add(doc.tracking_stats.total_views.ToString());
                item.Tag = doc;
                listViewDocuments.Items.Add(item);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (listViewDocuments.SelectedItems.Count > 0)
            {
                var selectedDoc = (DocumentInfo)listViewDocuments.SelectedItems[0].Tag;
                SelectedDocumentId = selectedDoc.id;
                SelectedDocumentTitle = selectedDoc.title;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a document.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void listViewDocuments_DoubleClick(object sender, EventArgs e)
        {
            btnOK_Click(sender, e);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (documents == null) return;

            var searchText = txtSearch.Text.ToLower();
            var filteredDocs = documents.Where(d => 
                d.title.ToLower().Contains(searchText) ||
                d.type.ToLower().Contains(searchText)
            ).ToList();

            PopulateFilteredDocuments(filteredDocs);
        }

        private void PopulateFilteredDocuments(List<DocumentInfo> filteredDocuments)
        {
            listViewDocuments.Items.Clear();

            foreach (var doc in filteredDocuments)
            {
                var item = new ListViewItem(doc.title);
                item.SubItems.Add(doc.type);
                item.SubItems.Add(doc.total_pages.ToString());
                item.SubItems.Add(doc.tracking_stats.total_views.ToString());
                item.Tag = doc;
                listViewDocuments.Items.Add(item);
            }
        }
    }

    public partial class DocumentSelectorForm : Form
    {
        private System.ComponentModel.IContainer components = null;
        private ListView listViewDocuments;
        private Button btnOK;
        private Button btnCancel;
        private Label lblStatus;
        private TextBox txtSearch;
        private Label lblSearch;

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
            this.listViewDocuments = new ListView();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.lblStatus = new Label();
            this.txtSearch = new TextBox();
            this.lblSearch = new Label();
            this.SuspendLayout();
            
            // lblSearch
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new Point(12, 15);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new Size(44, 13);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search:";
            
            // txtSearch
            this.txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.txtSearch.Location = new Point(62, 12);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new Size(300, 20);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
            
            // listViewDocuments
            this.listViewDocuments.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.listViewDocuments.FullRowSelect = true;
            this.listViewDocuments.GridLines = true;
            this.listViewDocuments.HideSelection = false;
            this.listViewDocuments.Location = new Point(12, 45);
            this.listViewDocuments.MultiSelect = false;
            this.listViewDocuments.Name = "listViewDocuments";
            this.listViewDocuments.Size = new Size(460, 250);
            this.listViewDocuments.TabIndex = 2;
            this.listViewDocuments.UseCompatibleStateImageBehavior = false;
            this.listViewDocuments.View = View.Details;
            this.listViewDocuments.DoubleClick += new EventHandler(this.listViewDocuments_DoubleClick);
            
            this.listViewDocuments.Columns.Add("Title", 200);
            this.listViewDocuments.Columns.Add("Type", 100);
            this.listViewDocuments.Columns.Add("Pages", 60);
            this.listViewDocuments.Columns.Add("Views", 60);
            
            // btnOK
            this.btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnOK.Location = new Point(316, 310);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(75, 25);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.btnOK_Click);
            
            // btnCancel
            this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(397, 310);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(75, 25);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            
            // lblStatus
            this.lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.lblStatus.Location = new Point(12, 315);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(200, 15);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Ready";
            
            // DocumentSelectorForm
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new Size(484, 347);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.listViewDocuments);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblSearch);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(400, 300);
            this.Name = "DocumentSelectorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Select Document - effyDOC";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}