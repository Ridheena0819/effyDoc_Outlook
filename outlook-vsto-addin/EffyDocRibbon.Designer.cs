namespace EffyDocOutlookAddin
{
    partial class EffyDocRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public EffyDocRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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
            this.tab1 = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.btnDocumentLibrary = this.Factory.CreateRibbonButton();
            this.btnLiveTracking = this.Factory.CreateRibbonButton();
            this.btnAttachDocument = this.Factory.CreateRibbonButton();
            this.btnHideTaskPanes = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.group1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.group1);
            this.tab1.Label = "effyDOC";
            this.tab1.Name = "tab1";
            // 
            // group1
            // 
            this.group1.Items.Add(this.btnDocumentLibrary);
            this.group1.Items.Add(this.btnLiveTracking);
            this.group1.Items.Add(this.btnAttachDocument);
            this.group1.Items.Add(this.btnHideTaskPanes);
            this.group1.Label = "Document Tracking";
            this.group1.Name = "group1";
            // 
            // btnDocumentLibrary
            // 
            this.btnDocumentLibrary.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnDocumentLibrary.Image = global::EffyDocOutlookAddin.Properties.Resources.DocumentLibrary;
            this.btnDocumentLibrary.Label = "Document Library";
            this.btnDocumentLibrary.Name = "btnDocumentLibrary";
            this.btnDocumentLibrary.ShowImage = true;
            this.btnDocumentLibrary.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnDocumentLibrary_Click);
            // 
            // btnLiveTracking
            // 
            this.btnLiveTracking.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnLiveTracking.Image = global::EffyDocOutlookAddin.Properties.Resources.LiveTracking;
            this.btnLiveTracking.Label = "Live Tracking";
            this.btnLiveTracking.Name = "btnLiveTracking";
            this.btnLiveTracking.ShowImage = true;
            this.btnLiveTracking.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnLiveTracking_Click);
            // 
            // btnAttachDocument
            // 
            this.btnAttachDocument.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnAttachDocument.Image = global::EffyDocOutlookAddin.Properties.Resources.AttachDocument;
            this.btnAttachDocument.Label = "Attach Document";
            this.btnAttachDocument.Name = "btnAttachDocument";
            this.btnAttachDocument.ShowImage = true;
            this.btnAttachDocument.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnAttachDocument_Click);
            // 
            // btnHideTaskPanes
            // 
            this.btnHideTaskPanes.Label = "Hide Panels";
            this.btnHideTaskPanes.Name = "btnHideTaskPanes";
            this.btnHideTaskPanes.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnHideTaskPanes_Click);
            // 
            // EffyDocRibbon
            // 
            this.Name = "EffyDocRibbon";
            this.RibbonType = "Microsoft.Outlook.Mail.Compose, Microsoft.Outlook.Mail.Read";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.EffyDocRibbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnDocumentLibrary;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnLiveTracking;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnAttachDocument;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnHideTaskPanes;
    }
}