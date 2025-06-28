using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

namespace EffyDocOutlookPlugin
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // Initialize the add-in
            System.Diagnostics.Debug.WriteLine("effyDOC Outlook Plugin started successfully!");
            
            // Subscribe to application events
            this.Application.ItemSend += new Outlook.ApplicationEvents_11_ItemSendEventHandler(Application_ItemSend);
            this.Application.NewMail += new Outlook.ApplicationEvents_11_NewMailEventHandler(Application_NewMail);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Clean up resources
            System.Diagnostics.Debug.WriteLine("effyDOC Outlook Plugin shutting down...");
        }

        private void Application_ItemSend(object Item, ref bool Cancel)
        {
            try
            {
                if (Item is Outlook.MailItem mailItem)
                {
                    // Check if this email contains effyDOC tracked documents
                    CheckForEffyDocAttachments(mailItem);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Application_ItemSend: {ex.Message}");
            }
        }

        private void Application_NewMail()
        {
            try
            {
                // Handle new mail events if needed
                System.Diagnostics.Debug.WriteLine("New mail received");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Application_NewMail: {ex.Message}");
            }
        }

        private void CheckForEffyDocAttachments(Outlook.MailItem mailItem)
        {
            try
            {
                // Check if the email body contains effyDOC tracking links
                if (mailItem.HTMLBody.Contains("effydoc-tracking-link") || 
                    mailItem.HTMLBody.Contains("data-effydoc-document"))
                {
                    // This email contains effyDOC tracked content
                    // You could show a notification or log this event
                    System.Diagnostics.Debug.WriteLine("Email contains effyDOC tracked content");
                    
                    // Optionally, send tracking data to the backend
                    var apiService = new Services.EffyDocApiService();
                    _ = apiService.TrackEmailSentAsync(mailItem);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking effyDOC attachments: {ex.Message}");
            }
        }

        public Outlook.Application GetOutlookApplication()
        {
            return this.Application;
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}