using System;
using Microsoft.Office.Tools.Ribbon;

namespace EffyDocOutlookAddin
{
    public partial class EffyDocRibbon
    {
        private void EffyDocRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            // Initialize ribbon
        }

        private void btnDocumentLibrary_Click(object sender, RibbonControlEventArgs e)
        {
            var addIn = Globals.ThisAddIn;
            addIn.ShowDocumentLibrary();
        }

        private void btnLiveTracking_Click(object sender, RibbonControlEventArgs e)
        {
            var addIn = Globals.ThisAddIn;
            addIn.ShowLiveTracking();
        }

        private void btnAttachDocument_Click(object sender, RibbonControlEventArgs e)
        {
            var addIn = Globals.ThisAddIn;
            addIn.ShowAttachmentWorkflow();
        }

        private void btnHideTaskPanes_Click(object sender, RibbonControlEventArgs e)
        {
            var addIn = Globals.ThisAddIn;
            addIn.HideAllTaskPanes();
        }
    }
}