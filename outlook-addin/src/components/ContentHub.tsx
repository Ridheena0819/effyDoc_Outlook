import React, { useState, useEffect } from 'react';
import { documentsAPI } from '../services/api.ts';
import { 
  Building, 
  FileText, 
  Clock, 
  Tag, 
  Send,
  Loader2,
  Users
} from 'lucide-react';
import toast from 'react-hot-toast';

interface Document {
  id: string;
  title: string;
  type: string;
  created_at: string;
  owner_name: string;
  total_pages: number;
  description: string;
  tags: string[];
  share_link: string;
  is_template: boolean;
  is_trackable: boolean;
}

interface ContentHubProps {
  onDocumentSelect?: (document: Document) => void;
}

const ContentHub: React.FC<ContentHubProps> = ({ onDocumentSelect }) => {
  const [documents, setDocuments] = useState<Document[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [sendingDocument, setSendingDocument] = useState<string | null>(null);
  const [selectedCategory, setSelectedCategory] = useState<string>('all');

  useEffect(() => {
    loadDocuments();
  }, []);

  const loadDocuments = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await documentsAPI.getContentHub();
      setDocuments(response.documents || []);
    } catch (error: any) {
      console.error('Failed to load content hub:', error);
      setError('Failed to load shared documents. Please try again.');
      toast.error('Failed to load content hub');
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  const getUniqueCategories = (): string[] => {
    const categories = new Set<string>();
    documents.forEach(doc => {
      doc.tags.forEach(tag => categories.add(tag));
    });
    return ['all', ...Array.from(categories)];
  };

  const filteredDocuments = selectedCategory === 'all' 
    ? documents 
    : documents.filter(doc => doc.tags.includes(selectedCategory));

  const sendTrackableDocument = async (document: Document) => {
    try {
      setSendingDocument(document.id);
      
      // Generate trackable link
      const linkResponse = await documentsAPI.generateTrackableLink(document.id);
      
      // Get current email recipients
      const recipients = await getEmailRecipients();
      
      if (!recipients || recipients.length === 0) {
        toast.error('Please add recipients to your email first');
        return;
      }

      // Create email content
      const emailBody = `
I'm sharing this document from our content library: ${document.title}

${document.description ? `📝 ${document.description}` : ''}

📄 View Document: ${linkResponse.full_url}

${document.is_template ? '📋 This is a template you can use and customize.' : ''}

This document includes real-time tracking so I can see when you've viewed it and provide any assistance you might need.

Best regards
      `.trim();

      // Use Office.js to modify the email
      if (Office.context.mailbox.item) {
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

        toast.success(`Shared with ${recipients.length} recipient(s)`);
      } else {
        // Fallback: copy to clipboard
        await navigator.clipboard.writeText(linkResponse.full_url);
        toast.success('Document link copied to clipboard! 📋');
      }

    } catch (error: any) {
      console.error('Error sending document:', error);
      toast.error('Failed to share document');
    } finally {
      setSendingDocument(null);
    }
  };

  const getEmailRecipients = async (): Promise<string[]> => {
    return new Promise((resolve) => {
      if (Office.context.mailbox.item && Office.context.mailbox.item.to) {
        Office.context.mailbox.item.to.getAsync((result) => {
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
    });
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center py-8">
        <Loader2 className="w-6 h-6 animate-spin text-blue-500" />
        <span className="ml-2 text-gray-600">Loading content hub...</span>
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
        <Building className="w-12 h-12 text-gray-400 mx-auto mb-3" />
        <h3 className="text-lg font-medium text-gray-900 mb-2">No Shared Content</h3>
        <p className="text-gray-500 text-sm">
          Your admin hasn't shared any documents or templates yet.
        </p>
      </div>
    );
  }

  const categories = getUniqueCategories();

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-lg font-semibold text-gray-900">Content Hub</h2>
        <span className="text-sm text-gray-500">{filteredDocuments.length} documents</span>
      </div>

      {/* Category Filter */}
      {categories.length > 1 && (
        <div className="flex flex-wrap gap-2 mb-4">
          {categories.map((category) => (
            <button
              key={category}
              onClick={() => setSelectedCategory(category)}
              className={`px-3 py-1 text-xs rounded-full border transition-colors ${
                selectedCategory === category
                  ? 'bg-blue-100 border-blue-300 text-blue-700'
                  : 'bg-gray-100 border-gray-300 text-gray-600 hover:bg-gray-200'
              }`}
            >
              {category === 'all' ? 'All' : category}
            </button>
          ))}
        </div>
      )}

      {/* Documents Grid */}
      <div className="space-y-3">
        {filteredDocuments.map((document) => (
          <div
            key={document.id}
            className="bg-white border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow cursor-pointer"
            onClick={() => onDocumentSelect?.(document)}
          >
            <div className="flex items-start justify-between">
              <div className="flex-1 min-w-0">
                <div className="flex items-center mb-2">
                  <FileText className="w-4 h-4 text-green-500 mr-2 flex-shrink-0" />
                  <h3 className="text-sm font-medium text-gray-900 truncate">
                    {document.title}
                  </h3>
                  {document.is_template && (
                    <span className="ml-2 px-2 py-0.5 bg-purple-100 text-purple-700 text-xs rounded-full">
                      Template
                    </span>
                  )}
                </div>
                
                {document.description && (
                  <p className="text-xs text-gray-600 mb-2 line-clamp-2">
                    {document.description}
                  </p>
                )}
                
                <div className="space-y-1">
                  <div className="flex items-center text-xs text-gray-500">
                    <Users className="w-3 h-3 mr-1" />
                    {document.owner_name}
                  </div>
                  
                  <div className="flex items-center text-xs text-gray-500">
                    <Clock className="w-3 h-3 mr-1" />
                    {formatDate(document.created_at)}
                  </div>
                  
                  <div className="flex items-center text-xs text-gray-500">
                    <FileText className="w-3 h-3 mr-1" />
                    {document.total_pages} pages
                  </div>
                </div>
                
                {/* Tags */}
                {document.tags.length > 0 && (
                  <div className="flex flex-wrap gap-1 mt-2">
                    {document.tags.slice(0, 3).map((tag, index) => (
                      <span
                        key={index}
                        className="inline-flex items-center px-2 py-0.5 bg-gray-100 text-gray-600 text-xs rounded"
                      >
                        <Tag className="w-2 h-2 mr-1" />
                        {tag}
                      </span>
                    ))}
                    {document.tags.length > 3 && (
                      <span className="text-xs text-gray-500">
                        +{document.tags.length - 3} more
                      </span>
                    )}
                  </div>
                )}
              </div>
            </div>

            <div className="mt-3 pt-3 border-t border-gray-100">
              <button
                onClick={(e) => {
                  e.stopPropagation();
                  sendTrackableDocument(document);
                }}
                disabled={sendingDocument === document.id}
                className="w-full bg-green-600 hover:bg-green-700 disabled:bg-green-400 text-white text-sm font-medium py-2 px-3 rounded-md flex items-center justify-center transition-colors"
              >
                {sendingDocument === document.id ? (
                  <>
                    <Loader2 className="w-4 h-4 animate-spin mr-2" />
                    Adding to Email...
                  </>
                ) : (
                  <>
                    <Send className="w-4 h-4 mr-2" />
                    Share Trackable
                  </>
                )}
              </button>
            </div>
          </div>
        ))}
      </div>
      
      <div className="mt-4 p-3 bg-green-50 border border-green-200 rounded-lg">
        <div className="flex items-center text-sm text-green-700">
          <Building className="w-4 h-4 mr-2" />
          <span className="font-medium">Organization Content</span>
        </div>
        <p className="text-xs text-green-600 mt-1">
          Shared documents and templates from your organization's admin.
        </p>
      </div>
    </div>
  );
};

export default ContentHub;