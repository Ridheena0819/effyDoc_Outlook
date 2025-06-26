namespace EffyDocOutlookAddin.UI
{
    partial class DocumentLibraryForm
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabMyLibrary = new System.Windows.Forms.TabPage();
            this.tabContentHub = new System.Windows.Forms.TabPage();
            this.listViewDocuments = new System.Windows.Forms.ListView();
            this.columnHeaderTitle = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderType = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderPages = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderSize = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderViews = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderCreated = new System.Windows.Forms.ColumnHeader();
            this.imageListDocuments = new System.Windows.Forms.ImageList(this.components);
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnAttachSelected = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.tabControl.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.panelFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabMyLibrary);
            this.tabControl.Controls.Add(this.tabContentHub);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl.Location = new System.Drawing.Point(0, 45);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(350, 25);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabMyLibrary
            // 
            this.tabMyLibrary.Location = new System.Drawing.Point(4, 22);
            this.tabMyLibrary.Name = "tabMyLibrary";
            this.tabMyLibrary.Padding = new System.Windows.Forms.Padding(3);
            this.tabMyLibrary.Size = new System.Drawing.Size(342, 0);
            this.tabMyLibrary.TabIndex = 0;
            this.tabMyLibrary.Text = "My Library";
            this.tabMyLibrary.UseVisualStyleBackColor = true;
            // 
            // tabContentHub
            // 
            this.tabContentHub.Location = new System.Drawing.Point(4, 22);
            this.tabContentHub.Name = "tabContentHub";
            this.tabContentHub.Padding = new System.Windows.Forms.Padding(3);
            this.tabContentHub.Size = new System.Drawing.Size(342, 0);
            this.tabContentHub.TabIndex = 1;
            this.tabContentHub.Text = "Content Hub";
            this.tabContentHub.UseVisualStyleBackColor = true;
            // 
            // listViewDocuments
            // 
            this.listViewDocuments.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderTitle,
            this.columnHeaderType,
            this.columnHeaderPages,
            this.columnHeaderSize,
            this.columnHeaderViews,
            this.columnHeaderCreated});
            this.listViewDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDocuments.FullRowSelect = true;
            this.listViewDocuments.GridLines = true;
            this.listViewDocuments.HideSelection = false;
            this.listViewDocuments.Location = new System.Drawing.Point(0, 0);
            this.listViewDocuments.MultiSelect = false;
            this.listViewDocuments.Name = "listViewDocuments";
            this.listViewDocuments.Size = new System.Drawing.Size(350, 335);
            this.listViewDocuments.SmallImageList = this.imageListDocuments;
            this.listViewDocuments.TabIndex = 1;
            this.listViewDocuments.UseCompatibleStateImageBehavior = false;
            this.listViewDocuments.View = System.Windows.Forms.View.Details;
            this.listViewDocuments.DoubleClick += new System.EventHandler(this.listViewDocuments_DoubleClick);
            // 
            // columnHeaderTitle
            // 
            this.columnHeaderTitle.Text = "Title";
            this.columnHeaderTitle.Width = 120;
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "Type";
            this.columnHeaderType.Width = 60;
            // 
            // columnHeaderPages
            // 
            this.columnHeaderPages.Text = "Pages";
            this.columnHeaderPages.Width = 50;
            // 
            // columnHeaderSize
            // 
            this.columnHeaderSize.Text = "Size";
            this.columnHeaderSize.Width = 50;
            // 
            // columnHeaderViews
            // 
            this.columnHeaderViews.Text = "Views";
            this.columnHeaderViews.Width = 50;
            // 
            // columnHeaderCreated
            // 
            this.columnHeaderCreated.Text = "Created";
            this.columnHeaderCreated.Width = 80;
            // 
            // imageListDocuments
            // 
            this.imageListDocuments.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListDocuments.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListDocuments.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(50, 12);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 20);
            this.txtSearch.TabIndex = 2;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(8, 15);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(41, 13);
            this.lblSearch.TabIndex = 3;
            this.lblSearch.Text = "Search:";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(260, 10);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnAttachSelected
            // 
            this.btnAttachSelected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAttachSelected.Location = new System.Drawing.Point(0, 0);
            this.btnAttachSelected.Name = "btnAttachSelected";
            this.btnAttachSelected.Size = new System.Drawing.Size(350, 30);
            this.btnAttachSelected.TabIndex = 5;
            this.btnAttachSelected.Text = "Attach Selected Document";
            this.btnAttachSelected.UseVisualStyleBackColor = true;
            this.btnAttachSelected.Click += new System.EventHandler(this.btnAttachSelected_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Location = new System.Drawing.Point(0, 430);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(350, 20);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.lblSearch);
            this.panelHeader.Controls.Add(this.txtSearch);
            this.panelHeader.Controls.Add(this.btnRefresh);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(350, 45);
            this.panelHeader.TabIndex = 7;
            // 
            // panelContent
            // 
            this.panelContent.Controls.Add(this.listViewDocuments);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 70);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(350, 335);
            this.panelContent.TabIndex = 8;
            // 
            // panelFooter
            // 
            this.panelFooter.Controls.Add(this.btnAttachSelected);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 405);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(350, 30);
            this.panelFooter.TabIndex = 9;
            // 
            // DocumentLibraryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.lblStatus);
            this.Name = "DocumentLibraryForm";
            this.Size = new System.Drawing.Size(350, 450);
            this.tabControl.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelContent.ResumeLayout(false);
            this.panelFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabMyLibrary;
        private System.Windows.Forms.TabPage tabContentHub;
        private System.Windows.Forms.ListView listViewDocuments;
        private System.Windows.Forms.ColumnHeader columnHeaderTitle;
        private System.Windows.Forms.ColumnHeader columnHeaderType;
        private System.Windows.Forms.ColumnHeader columnHeaderPages;
        private System.Windows.Forms.ColumnHeader columnHeaderSize;
        private System.Windows.Forms.ColumnHeader columnHeaderViews;
        private System.Windows.Forms.ColumnHeader columnHeaderCreated;
        private System.Windows.Forms.ImageList imageListDocuments;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnAttachSelected;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Panel panelFooter;
    }
}