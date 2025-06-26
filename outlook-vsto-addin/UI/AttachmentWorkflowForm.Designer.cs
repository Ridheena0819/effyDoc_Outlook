namespace EffyDocOutlookAddin.UI
{
    partial class AttachmentWorkflowForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupDocumentSelection = new System.Windows.Forms.GroupBox();
            this.btnEditDocument = new System.Windows.Forms.Button();
            this.btnPreviewDocument = new System.Windows.Forms.Button();
            this.btnSelectDocument = new System.Windows.Forms.Button();
            this.txtDocumentPages = new System.Windows.Forms.TextBox();
            this.txtDocumentType = new System.Windows.Forms.TextBox();
            this.txtDocumentTitle = new System.Windows.Forms.TextBox();
            this.lblDocumentPages = new System.Windows.Forms.Label();
            this.lblDocumentType = new System.Windows.Forms.Label();
            this.lblDocumentTitle = new System.Windows.Forms.Label();
            this.groupAttachmentType = new System.Windows.Forms.GroupBox();
            this.lblAttachmentDescription = new System.Windows.Forms.Label();
            this.radioRegularAttachment = new System.Windows.Forms.RadioButton();
            this.radioTrackableAttachment = new System.Windows.Forms.RadioButton();
            this.groupTrackingOptions = new System.Windows.Forms.GroupBox();
            this.chkAddTrackingInfo = new System.Windows.Forms.CheckBox();
            this.chkTrackTimeSpent = new System.Windows.Forms.CheckBox();
            this.chkTrackPageViews = new System.Windows.Forms.CheckBox();
            this.chkNotifyOnClick = new System.Windows.Forms.CheckBox();
            this.chkNotifyOnOpen = new System.Windows.Forms.CheckBox();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAttachToEmail = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.groupDocumentSelection.SuspendLayout();
            this.groupAttachmentType.SuspendLayout();
            this.groupTrackingOptions.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupDocumentSelection
            // 
            this.groupDocumentSelection.Controls.Add(this.btnEditDocument);
            this.groupDocumentSelection.Controls.Add(this.btnPreviewDocument);
            this.groupDocumentSelection.Controls.Add(this.btnSelectDocument);
            this.groupDocumentSelection.Controls.Add(this.txtDocumentPages);
            this.groupDocumentSelection.Controls.Add(this.txtDocumentType);
            this.groupDocumentSelection.Controls.Add(this.txtDocumentTitle);
            this.groupDocumentSelection.Controls.Add(this.lblDocumentPages);
            this.groupDocumentSelection.Controls.Add(this.lblDocumentType);
            this.groupDocumentSelection.Controls.Add(this.lblDocumentTitle);
            this.groupDocumentSelection.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupDocumentSelection.Location = new System.Drawing.Point(0, 0);
            this.groupDocumentSelection.Name = "groupDocumentSelection";
            this.groupDocumentSelection.Size = new System.Drawing.Size(500, 120);
            this.groupDocumentSelection.TabIndex = 0;
            this.groupDocumentSelection.TabStop = false;
            this.groupDocumentSelection.Text = "Document Selection";
            // 
            // btnEditDocument
            // 
            this.btnEditDocument.Enabled = false;
            this.btnEditDocument.Location = new System.Drawing.Point(280, 85);
            this.btnEditDocument.Name = "btnEditDocument";
            this.btnEditDocument.Size = new System.Drawing.Size(100, 25);
            this.btnEditDocument.TabIndex = 8;
            this.btnEditDocument.Text = "Edit Document";
            this.btnEditDocument.UseVisualStyleBackColor = true;
            this.btnEditDocument.Click += new System.EventHandler(this.btnEditDocument_Click);
            // 
            // btnPreviewDocument
            // 
            this.btnPreviewDocument.Enabled = false;
            this.btnPreviewDocument.Location = new System.Drawing.Point(170, 85);
            this.btnPreviewDocument.Name = "btnPreviewDocument";
            this.btnPreviewDocument.Size = new System.Drawing.Size(100, 25);
            this.btnPreviewDocument.TabIndex = 7;
            this.btnPreviewDocument.Text = "Preview";
            this.btnPreviewDocument.UseVisualStyleBackColor = true;
            this.btnPreviewDocument.Click += new System.EventHandler(this.btnPreviewDocument_Click);
            // 
            // btnSelectDocument
            // 
            this.btnSelectDocument.Location = new System.Drawing.Point(400, 20);
            this.btnSelectDocument.Name = "btnSelectDocument";
            this.btnSelectDocument.Size = new System.Drawing.Size(80, 25);
            this.btnSelectDocument.TabIndex = 6;
            this.btnSelectDocument.Text = "Select...";
            this.btnSelectDocument.UseVisualStyleBackColor = true;
            this.btnSelectDocument.Click += new System.EventHandler(this.btnSelectDocument_Click);
            // 
            // txtDocumentPages
            // 
            this.txtDocumentPages.Location = new System.Drawing.Point(320, 50);
            this.txtDocumentPages.Name = "txtDocumentPages";
            this.txtDocumentPages.ReadOnly = true;
            this.txtDocumentPages.Size = new System.Drawing.Size(60, 20);
            this.txtDocumentPages.TabIndex = 5;
            // 
            // txtDocumentType
            // 
            this.txtDocumentType.Location = new System.Drawing.Point(170, 50);
            this.txtDocumentType.Name = "txtDocumentType";
            this.txtDocumentType.ReadOnly = true;
            this.txtDocumentType.Size = new System.Drawing.Size(100, 20);
            this.txtDocumentType.TabIndex = 4;
            // 
            // txtDocumentTitle
            // 
            this.txtDocumentTitle.Location = new System.Drawing.Point(60, 20);
            this.txtDocumentTitle.Name = "txtDocumentTitle";
            this.txtDocumentTitle.ReadOnly = true;
            this.txtDocumentTitle.Size = new System.Drawing.Size(320, 20);
            this.txtDocumentTitle.TabIndex = 3;
            // 
            // lblDocumentPages
            // 
            this.lblDocumentPages.AutoSize = true;
            this.lblDocumentPages.Location = new System.Drawing.Point(280, 53);
            this.lblDocumentPages.Name = "lblDocumentPages";
            this.lblDocumentPages.Size = new System.Drawing.Size(40, 13);
            this.lblDocumentPages.TabIndex = 2;
            this.lblDocumentPages.Text = "Pages:";
            // 
            // lblDocumentType
            // 
            this.lblDocumentType.AutoSize = true;
            this.lblDocumentType.Location = new System.Drawing.Point(130, 53);
            this.lblDocumentType.Name = "lblDocumentType";
            this.lblDocumentType.Size = new System.Drawing.Size(34, 13);
            this.lblDocumentType.TabIndex = 1;
            this.lblDocumentType.Text = "Type:";
            // 
            // lblDocumentTitle
            // 
            this.lblDocumentTitle.AutoSize = true;
            this.lblDocumentTitle.Location = new System.Drawing.Point(15, 23);
            this.lblDocumentTitle.Name = "lblDocumentTitle";
            this.lblDocumentTitle.Size = new System.Drawing.Size(30, 13);
            this.lblDocumentTitle.TabIndex = 0;
            this.lblDocumentTitle.Text = "Title:";
            // 
            // groupAttachmentType
            // 
            this.groupAttachmentType.Controls.Add(this.lblAttachmentDescription);
            this.groupAttachmentType.Controls.Add(this.radioRegularAttachment);
            this.groupAttachmentType.Controls.Add(this.radioTrackableAttachment);
            this.groupAttachmentType.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupAttachmentType.Location = new System.Drawing.Point(0, 120);
            this.groupAttachmentType.Name = "groupAttachmentType";
            this.groupAttachmentType.Size = new System.Drawing.Size(500, 100);
            this.groupAttachmentType.TabIndex = 1;
            this.groupAttachmentType.TabStop = false;
            this.groupAttachmentType.Text = "Attachment Type";
            // 
            // lblAttachmentDescription
            // 
            this.lblAttachmentDescription.Location = new System.Drawing.Point(15, 50);
            this.lblAttachmentDescription.Name = "lblAttachmentDescription";
            this.lblAttachmentDescription.Size = new System.Drawing.Size(470, 40);
            this.lblAttachmentDescription.TabIndex = 2;
            this.lblAttachmentDescription.Text = "This document will be attached as a trackable HTML file with embedded tracking l" +
    "inks. Recipients\' interactions will be monitored in real-time.";
            // 
            // radioRegularAttachment
            // 
            this.radioRegularAttachment.AutoSize = true;
            this.radioRegularAttachment.Location = new System.Drawing.Point(200, 25);
            this.radioRegularAttachment.Name = "radioRegularAttachment";
            this.radioRegularAttachment.Size = new System.Drawing.Size(151, 17);
            this.radioRegularAttachment.TabIndex = 1;
            this.radioRegularAttachment.Text = "Regular File (No Tracking)";
            this.radioRegularAttachment.UseVisualStyleBackColor = true;
            this.radioRegularAttachment.CheckedChanged += new System.EventHandler(this.radioRegularAttachment_CheckedChanged);
            // 
            // radioTrackableAttachment
            // 
            this.radioTrackableAttachment.AutoSize = true;
            this.radioTrackableAttachment.Checked = true;
            this.radioTrackableAttachment.Location = new System.Drawing.Point(15, 25);
            this.radioTrackableAttachment.Name = "radioTrackableAttachment";
            this.radioTrackableAttachment.Size = new System.Drawing.Size(148, 17);
            this.radioTrackableAttachment.TabIndex = 0;
            this.radioTrackableAttachment.TabStop = true;
            this.radioTrackableAttachment.Text = "Trackable Document (effyDOC)";
            this.radioTrackableAttachment.UseVisualStyleBackColor = true;
            this.radioTrackableAttachment.CheckedChanged += new System.EventHandler(this.radioTrackableAttachment_CheckedChanged);
            // 
            // groupTrackingOptions
            // 
            this.groupTrackingOptions.Controls.Add(this.chkAddTrackingInfo);
            this.groupTrackingOptions.Controls.Add(this.chkTrackTimeSpent);
            this.groupTrackingOptions.Controls.Add(this.chkTrackPageViews);
            this.groupTrackingOptions.Controls.Add(this.chkNotifyOnClick);
            this.groupTrackingOptions.Controls.Add(this.chkNotifyOnOpen);
            this.groupTrackingOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupTrackingOptions.Location = new System.Drawing.Point(0, 220);
            this.groupTrackingOptions.Name = "groupTrackingOptions";
            this.groupTrackingOptions.Size = new System.Drawing.Size(500, 120);
            this.groupTrackingOptions.TabIndex = 2;
            this.groupTrackingOptions.TabStop = false;
            this.groupTrackingOptions.Text = "Tracking Options";
            // 
            // chkAddTrackingInfo
            // 
            this.chkAddTrackingInfo.AutoSize = true;
            this.chkAddTrackingInfo.Checked = true;
            this.chkAddTrackingInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAddTrackingInfo.Location = new System.Drawing.Point(15, 90);
            this.chkAddTrackingInfo.Name = "chkAddTrackingInfo";
            this.chkAddTrackingInfo.Size = new System.Drawing.Size(187, 17);
            this.chkAddTrackingInfo.TabIndex = 4;
            this.chkAddTrackingInfo.Text = "Add tracking info to email signature";
            this.chkAddTrackingInfo.UseVisualStyleBackColor = true;
            // 
            // chkTrackTimeSpent
            // 
            this.chkTrackTimeSpent.AutoSize = true;
            this.chkTrackTimeSpent.Checked = true;
            this.chkTrackTimeSpent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTrackTimeSpent.Location = new System.Drawing.Point(15, 70);
            this.chkTrackTimeSpent.Name = "chkTrackTimeSpent";
            this.chkTrackTimeSpent.Size = new System.Drawing.Size(168, 17);
            this.chkTrackTimeSpent.TabIndex = 3;
            this.chkTrackTimeSpent.Text = "Track time spent on document";
            this.chkTrackTimeSpent.UseVisualStyleBackColor = true;
            // 
            // chkTrackPageViews
            // 
            this.chkTrackPageViews.AutoSize = true;
            this.chkTrackPageViews.Checked = true;
            this.chkTrackPageViews.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTrackPageViews.Location = new System.Drawing.Point(15, 50);
            this.chkTrackPageViews.Name = "chkTrackPageViews";
            this.chkTrackPageViews.Size = new System.Drawing.Size(149, 17);
            this.chkTrackPageViews.TabIndex = 2;
            this.chkTrackPageViews.Text = "Track page-wise interactions";
            this.chkTrackPageViews.UseVisualStyleBackColor = true;
            // 
            // chkNotifyOnClick
            // 
            this.chkNotifyOnClick.AutoSize = true;
            this.chkNotifyOnClick.Checked = true;
            this.chkNotifyOnClick.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNotifyOnClick.Location = new System.Drawing.Point(15, 30);
            this.chkNotifyOnClick.Name = "chkNotifyOnClick";
            this.chkNotifyOnClick.Size = new System.Drawing.Size(133, 17);
            this.chkNotifyOnClick.TabIndex = 1;
            this.chkNotifyOnClick.Text = "Notify on link clicks";
            this.chkNotifyOnClick.UseVisualStyleBackColor = true;
            // 
            // chkNotifyOnOpen
            // 
            this.chkNotifyOnOpen.AutoSize = true;
            this.chkNotifyOnOpen.Checked = true;
            this.chkNotifyOnOpen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNotifyOnOpen.Location = new System.Drawing.Point(15, 10);
            this.chkNotifyOnOpen.Name = "chkNotifyOnOpen";
            this.chkNotifyOnOpen.Size = new System.Drawing.Size(141, 17);
            this.chkNotifyOnOpen.TabIndex = 0;
            this.chkNotifyOnOpen.Text = "Notify when document opened";
            this.chkNotifyOnOpen.UseVisualStyleBackColor = true;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Controls.Add(this.btnAttachToEmail);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 360);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(500, 50);
            this.panelButtons.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(415, 15);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAttachToEmail
            // 
            this.btnAttachToEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAttachToEmail.Location = new System.Drawing.Point(310, 15);
            this.btnAttachToEmail.Name = "btnAttachToEmail";
            this.btnAttachToEmail.Size = new System.Drawing.Size(100, 30);
            this.btnAttachToEmail.TabIndex = 0;
            this.btnAttachToEmail.Text = "Attach to Email";
            this.btnAttachToEmail.UseVisualStyleBackColor = true;
            this.btnAttachToEmail.Click += new System.EventHandler(this.btnAttachToEmail_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Location = new System.Drawing.Point(0, 340);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(500, 20);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AttachmentWorkflowForm
            // 
            this.AcceptButton = this.btnAttachToEmail;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(500, 410);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.groupTrackingOptions);
            this.Controls.Add(this.groupAttachmentType);
            this.Controls.Add(this.groupDocumentSelection);
            this.Controls.Add(this.panelButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AttachmentWorkflowForm";
            this.ShowInTaskbar = false;
            this.Text = "effyDOC - Attach Document";
            this.Load += new System.EventHandler(this.AttachmentWorkflowForm_Load);
            this.groupDocumentSelection.ResumeLayout(false);
            this.groupDocumentSelection.PerformLayout();
            this.groupAttachmentType.ResumeLayout(false);
            this.groupAttachmentType.PerformLayout();
            this.groupTrackingOptions.ResumeLayout(false);
            this.groupTrackingOptions.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupDocumentSelection;
        private System.Windows.Forms.GroupBox groupAttachmentType;
        private System.Windows.Forms.GroupBox groupTrackingOptions;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.TextBox txtDocumentTitle;
        private System.Windows.Forms.Label lblDocumentTitle;
        private System.Windows.Forms.Button btnSelectDocument;
        private System.Windows.Forms.TextBox txtDocumentPages;
        private System.Windows.Forms.TextBox txtDocumentType;
        private System.Windows.Forms.Label lblDocumentPages;
        private System.Windows.Forms.Label lblDocumentType;
        private System.Windows.Forms.RadioButton radioRegularAttachment;
        private System.Windows.Forms.RadioButton radioTrackableAttachment;
        private System.Windows.Forms.Label lblAttachmentDescription;
        private System.Windows.Forms.CheckBox chkNotifyOnOpen;
        private System.Windows.Forms.CheckBox chkNotifyOnClick;
        private System.Windows.Forms.CheckBox chkTrackPageViews;
        private System.Windows.Forms.CheckBox chkTrackTimeSpent;
        private System.Windows.Forms.CheckBox chkAddTrackingInfo;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAttachToEmail;
        private System.Windows.Forms.Button btnEditDocument;
        private System.Windows.Forms.Button btnPreviewDocument;
        private System.Windows.Forms.Label lblStatus;
    }
}