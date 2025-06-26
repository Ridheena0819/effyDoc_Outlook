import React, { useState, useEffect } from 'react';
import { documentsAPI, trackingAPI } from '../services/api';
import { 
  FileText, 
  Send, 
  Eye, 
  Clock, 
  Users, 
  TrendingUp,
  Loader2
} from 'lucide-react';
import toast from 'react-hot-toast';

interface BaseDocument {
  id: string;
  title: string;
  type: string;
  created_at: string;
  total_pages: number;
  share_link: string;
  is_trackable: boolean;
}

interface LibraryDocument extends BaseDocument {
  file_size: number;
  tracking_stats: {
    total_views: number;
    total_shares: number;
    last_viewed: string | null;
  };
}

interface HubDocument extends BaseDocument {
  owner_name: string;
  description: string;
  tags: string[];
  is_template: boolean;
}

interface DocumentLibraryProps {
  onDocumentSelect?: (document: BaseDocument) => void;
}

const DocumentLibrary: React.FC<DocumentLibraryProps> = ({ onDocumentSelect }) => {
  const [documents, setDocuments] = useState<LibraryDocument[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [sendingDocument, setSendingDocument] = useState<string | null>(null);

  useEffect(() => {
    loadDocuments();
  }, []);

  const loadDocuments = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await documentsAPI.getMyLibrary();
      setDocuments(response.documents || []);
    } catch (error: any) {
      console.error('Failed to load documents:', error);
      setError('Failed to load your documents. Please try again.');
      toast.error('Failed to load documents');
    } finally {
      setLoading(false);
    }
  };

  const formatFileSize = (bytes: number): string => {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i];
  };

  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  const sendTrackableDocument = async (document: LibraryDocument) => {
    try {
      setSendingDocument(document.id);
      
      // Generate trackable link
      const linkResponse = await documentsAPI.generateTrackableLink(document.id);
      
      // Get current email recipients (from Outlook context)
      const recipients = await getEmailRecipients();
      
      if (!recipients || recipients.length === 0) {
        toast.error('Please add recipients to your email first');
        return;
      }

      // Insert trackable link into email body
      const emailBody = `
I'm sharing this trackable document with you: ${document.title}

📄 View Document: ${linkResponse.full_url}

This document includes real-time tracking so I can see when you've viewed it and provide any assistance you might need.

Best regards
      `.trim();

      // Use Office.js to modify the email
      if (Office.context.mailbox.item) {
        // Set email body
        Office.context.mailbox.item.body.setAsync(
          emailBody,
          { coercionType: Office.CoercionType.Text },
          (result) => {
            if (result.status === Office.AsyncResultStatus.Succeeded) {
              toast.success('Document added to email! 📧');
            } else {
              console.error('Failed to set email body:', result.error);
              toast.error('Failed to add document to email');
            }
          }
        );

        // Track the email send
        await trackingAPI.trackEmailSent({
          document_id: document.id,
          recipients: recipients,
          subject: await getEmailSubject(),
          email_body: emailBody
        });

        toast.success(`Tracking started for ${recipients.length} recipient(s)`);
      } else {
        // Fallback: copy to clipboard
        await navigator.clipboard.writeText(linkResponse.full_url);
        toast.success('Trackable link copied to clipboard! 📋');
      }

    } catch (error: any) {
      console.error('Error sending trackable document:', error);
      toast.error('Failed to send trackable document');
    } finally {
      setSendingDocument(null);
    }
  };

  const getEmailRecipients = async (): Promise<string[]> => {
    return new Promise((resolve) => {
      if (Office.context.mailbox.item) {
        const item = Office.context.mailbox.item;
        
        // Get TO recipients
        if (item.to) {
          item.to.getAsync((result) => {
            if (result.status === Office.AsyncResultStatus.Succeeded) {
              const emails = result.value.map(recipient => recipient.emailAddress);
              resolve(emails);
            } else {
              resolve([]);
            }
          });
        } else {
          resolve([]);
        }
      } else {
        resolve([]);
      }
    });
  };

  const getEmailSubject = async (): Promise<string> => {
    return new Promise((resolve) => {
      if (Office.context.mailbox.item && Office.context.mailbox.item.subject) {
        Office.context.mailbox.item.subject.getAsync((result) => {
          if (result.status === Office.AsyncResultStatus.Succeeded) {
            resolve(result.value || 'Document Shared');
          } else {
            resolve('Document Shared');
          }
        });
      } else {
        resolve('Document Shared');
      }
    });
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center py-8">
        <Loader2 className="w-6 h-6 animate-spin text-blue-500" />
        <span className="ml-2 text-gray-600">Loading your documents...</span>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-4 bg-red-50 border border-red-200 rounded-lg">
        <p className="text-red-600 text-sm">{error}</p>
        <button 
          onClick={loadDocuments}
          className="mt-2 px-3 py-1 bg-red-600 text-white text-xs rounded hover:bg-red-700"
        >
          Retry
        </button>
      </div>
    );
  }

  if (documents.length === 0) {
    return (
      <div className="text-center py-8">
        <FileText className="w-12 h-12 text-gray-400 mx-auto mb-3" />
        <h3 className="text-lg font-medium text-gray-900 mb-2">No Documents Found</h3>
        <p className="text-gray-500 text-sm">
          Create your first trackable document in effyDOC to get started.
        </p>
      </div>
    );
  }

  return (
    <div className="space-y-3">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-lg font-semibold text-gray-900">My Library</h2>
        <span className="text-sm text-gray-500">{documents.length} documents</span>
      </div>

      {documents.map((document) => (
        <div
          key={document.id}
          className="bg-white border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow cursor-pointer"
          onClick={() => onDocumentSelect?.(document)}
        >
          <div className="flex items-start justify-between">
            <div className="flex-1 min-w-0">
              <div className="flex items-center mb-2">
                <FileText className="w-4 h-4 text-blue-500 mr-2 flex-shrink-0" />
                <h3 className="text-sm font-medium text-gray-900 truncate">
                  {document.title}
                </h3>
              </div>
              
              <div className="space-y-1">
                <div className="flex items-center text-xs text-gray-500">
                  <Clock className="w-3 h-3 mr-1" />
                  {formatDate(document.created_at)}
                </div>
                
                <div className="flex items-center text-xs text-gray-500">
                  <FileText className="w-3 h-3 mr-1" />
                  {document.total_pages} pages • {formatFileSize(document.file_size)}
                </div>
                
                {document.tracking_stats.total_views > 0 && (
                  <div className="flex items-center text-xs text-green-600">
                    <Eye className="w-3 h-3 mr-1" />
                    {document.tracking_stats.total_views} views • {document.tracking_stats.total_shares} shares
                  </div>
                )}
              </div>
            </div>
          </div>

          <div className="mt-3 pt-3 border-t border-gray-100">
            <button
              onClick={(e) => {
                e.stopPropagation();
                sendTrackableDocument(document);
              }}
              disabled={sendingDocument === document.id}
              className="w-full bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white text-sm font-medium py-2 px-3 rounded-md flex items-center justify-center transition-colors"
            >
              {sendingDocument === document.id ? (
                <>
                  <Loader2 className="w-4 h-4 animate-spin mr-2" />
                  Adding to Email...
                </>
              ) : (
                <>
                  <Send className="w-4 h-4 mr-2" />
                  Send Trackable
                </>
              )}
            </button>
          </div>
        </div>
      ))}
      
      <div className="mt-4 p-3 bg-blue-50 border border-blue-200 rounded-lg">
        <div className="flex items-center text-sm text-blue-700">
          <TrendingUp className="w-4 h-4 mr-2" />
          <span className="font-medium">Track every interaction</span>
        </div>
        <p className="text-xs text-blue-600 mt-1">
          See when recipients open, click, and read your documents in real-time.
        </p>
      </div>
    </div>
  );
};

export default DocumentLibrary;