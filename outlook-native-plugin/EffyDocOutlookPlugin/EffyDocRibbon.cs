using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;

namespace EffyDocOutlookPlugin
{
    public partial class EffyDocRibbon
    {
        private void EffyDocRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            // Initialize ribbon when loaded
            System.Diagnostics.Debug.WriteLine("effyDOC Ribbon loaded successfully!");
        }

        private void btnAttachDocument_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                // Open document selector dialog
                var documentSelector = new DocumentSelector();
                
                if (documentSelector.ShowDialog() == DialogResult.OK)
                {
                    var selectedDocument = documentSelector.SelectedDocument;
                    if (selectedDocument != null)
                    {
                        AttachEffyDocDocument(selectedDocument);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error attaching document: {ex.Message}", "effyDOC Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnViewAnalytics_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                // Open analytics dashboard
                var trackingPanel = new TrackingPanel();
                trackingPanel.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening analytics: {ex.Message}", "effyDOC Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNewProposal_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                // Create new proposal
                CreateNewDocument("proposal");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating proposal: {ex.Message}", "effyDOC Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNewContract_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                // Create new contract
                CreateNewDocument("contract");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating contract: {ex.Message}", "effyDOC Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSettings_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                // Open settings/login dialog
                MessageBox.Show("Settings dialog will open here", "effyDOC Settings", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening settings: {ex.Message}", "effyDOC Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AttachEffyDocDocument(Models.DocumentModel document)
        {
            try
            {
                // Get the current mail item
                var inspector = Globals.ThisAddIn.Application.ActiveInspector();
                if (inspector?.CurrentItem is Outlook.MailItem mailItem)
                {
                    // Generate trackable HTML content for the document
                    var apiService = new Services.EffyDocApiService();
                    var htmlContent = apiService.GenerateTrackableHtmlAsync(document.Id).Result;

                    if (!string.IsNullOrEmpty(htmlContent))
                    {
                        // Insert the trackable document into the email body
                        var documentHtml = $@"
                        <div style='border: 2px solid #4f46e5; border-radius: 8px; padding: 16px; margin: 16px 0; background: #f8fafc;'>
                            <div style='display: flex; align-items: center; margin-bottom: 12px;'>
                                <img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAAdgAAAHYBTnsmCAAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAANCSURBVFiFtZc9aBRBFMd/M7vZTWKMGoygQcHCQlsrwcJCG1sLG1sLbW0sLGwsLLSxsLGwsLGwsLGwsLGwsLGwsLGwsLGwsLGwsLGwsLGwsLGwsLGwsLGwsLGwsLGwsLGwsLGwsLGwsLGwsLGwsLGw' width='20' height='20' style='margin-right: 8px;'/>
                                <strong style='color: #4f46e5; font-size: 16px;'>📄 effyDOC Document</strong>
                            </div>
                            <h3 style='color: #1e293b; margin: 0 0 8px 0; font-size: 18px;'>{document.Title}</h3>
                            <p style='color: #64748b; margin: 0 0 12px 0; font-size: 14px;'>{document.Type} • Updated {document.UpdatedAt:MMM dd, yyyy}</p>
                            <div style='background: white; border-radius: 6px; padding: 12px; margin-bottom: 12px;'>
                                {htmlContent}
                            </div>
                            <p style='color: #4f46e5; font-size: 12px; margin: 0;'>
                                📊 This document includes tracking analytics and interactive elements
                            </p>
                            <div style='margin-top: 12px;'>
                                <a href='{document.TrackingLink}' 
                                   style='background: #4f46e5; color: white; padding: 8px 16px; text-decoration: none; border-radius: 6px; font-size: 14px; font-weight: 500;'
                                   data-effydoc-document='{document.Id}' 
                                   class='effydoc-tracking-link'>
                                   View Full Document
                                </a>
                            </div>
                        </div>";

                        // Insert into email body
                        if (string.IsNullOrEmpty(mailItem.HTMLBody))
                        {
                            mailItem.HTMLBody = documentHtml;
                        }
                        else
                        {
                            // Insert before the signature or at the end
                            var signatureIndex = mailItem.HTMLBody.LastIndexOf("</body>", StringComparison.OrdinalIgnoreCase);
                            if (signatureIndex > 0)
                            {
                                mailItem.HTMLBody = mailItem.HTMLBody.Insert(signatureIndex, documentHtml);
                            }
                            else
                            {
                                mailItem.HTMLBody += documentHtml;
                            }
                        }

                        MessageBox.Show($"Document '{document.Title}' attached successfully!", "effyDOC", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please open a new email first to attach documents.", "effyDOC", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error attaching document: {ex.Message}");
                MessageBox.Show($"Failed to attach document: {ex.Message}", "effyDOC Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateNewDocument(string documentType)
        {
            try
            {
                // Open web browser to create new document
                var baseUrl = "https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com";
                var createUrl = documentType == "proposal" 
                    ? $"{baseUrl}/rfp-builder" 
                    : $"{baseUrl}/create?type={documentType}";

                System.Diagnostics.Process.Start(createUrl);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating new document: {ex.Message}");
                MessageBox.Show($"Failed to create new {documentType}: {ex.Message}", "effyDOC Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}