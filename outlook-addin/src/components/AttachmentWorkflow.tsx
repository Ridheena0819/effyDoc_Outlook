import React, { useState, useEffect } from 'react';
import { documentsAPI } from '../services/api';
import { 
  Paperclip, 
  Computer, 
  FileText, 
  Eye, 
  Edit3, 
  Send,
  X,
  Save,
  Loader2
} from 'lucide-react';
import toast from 'react-hot-toast';

interface AttachmentWorkflowProps {
  isOpen: boolean;
  onClose: () => void;
}

interface Document {
  id: string;
  title: string;
  type: string;
  total_pages: number;
  pages?: any[];
  sections?: any[];
  can_edit: boolean;
}

const AttachmentWorkflow: React.FC<AttachmentWorkflowProps> = ({ isOpen, onClose }) => {
  const [step, setStep] = useState<'choose' | 'browse' | 'preview' | 'edit'>('choose');
  const [selectedDocument, setSelectedDocument] = useState<Document | null>(null);
  const [documentContent, setDocumentContent] = useState<any>(null);
  const [isEditing, setIsEditing] = useState(false);
  const [editedContent, setEditedContent] = useState<any>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!isOpen) {
      // Reset state when modal closes
      setStep('choose');
      setSelectedDocument(null);
      setDocumentContent(null);
      setIsEditing(false);
      setEditedContent(null);
    }
  }, [isOpen]);

  const handleComputerAttachment = async () => {
    try {
      // Use Office.js to trigger standard file attachment
      if (Office.context.mailbox.item) {
        // For computer files, we use standard Outlook attachment
        toast.success('Please use Outlook\'s standard attachment button for computer files');
        onClose();
      }
    } catch (error) {
      toast.error('Failed to access Outlook attachment functionality');
    }
  };

  const handleEffyDocAttachment = () => {
    setStep('browse');
  };

  const handleDocumentSelect = async (document: Document) => {
    try {
      setLoading(true);
      setSelectedDocument(document);
      
      // Load document content for preview
      const content = await documentsAPI.getDocumentContent(document.id);
      setDocumentContent(content);
      setEditedContent(content);
      setStep('preview');
      
    } catch (error: any) {
      console.error('Error loading document:', error);
      toast.error('Failed to load document content');
    } finally {
      setLoading(false);
    }
  };

  const handleEditDocument = () => {
    setIsEditing(true);
    setStep('edit');
  };

  const handleSaveChanges = async () => {
    try {
      setLoading(true);
      
      if (!selectedDocument || !editedContent) return;

      await documentsAPI.updateDocumentContent(selectedDocument.id, {
        title: editedContent.title,
        pages: editedContent.pages
      });
      
      setDocumentContent(editedContent);
      setIsEditing(false);
      setStep('preview');
      toast.success('Document updated successfully!');
      
    } catch (error: any) {
      console.error('Error saving changes:', error);
      toast.error('Failed to save changes');
    } finally {
      setLoading(false);
    }
  };

  const handleAttachDocument = async () => {
    try {
      setLoading(true);
      
      if (!selectedDocument) return;

      // Generate trackable attachment
      const attachmentData = await documentsAPI.generateAttachmentData(selectedDocument.id, {
        format: 'html',
        include_tracking: true
      });

      // Create HTML file content
      const htmlContent = attachmentData.content;
      const filename = attachmentData.filename;

      // Create blob and attach to email
      const blob = new Blob([htmlContent], { type: 'text/html' });
      
      // Use Office.js to attach the file
      if (Office.context.mailbox.item && Office.context.mailbox.item.addFileAttachmentFromBase64) {
        // Convert blob to base64
        const reader = new FileReader();
        reader.onload = () => {
          const base64 = (reader.result as string).split(',')[1];
          
          Office.context.mailbox.item!.addFileAttachmentFromBase64(
            base64,
            filename,
            (result) => {
              if (result.status === Office.AsyncResultStatus.Succeeded) {
                toast.success(`Trackable document attached: ${filename}`);
                
                // Track the attachment
                documentsAPI.trackEmailSent({
                  document_id: selectedDocument.id,
                  recipients: [], // Will be filled when email is sent
                  subject: 'Document Attached',
                  email_body: `Trackable document attached: ${selectedDocument.title}`
                });
                
                onClose();
              } else {
                toast.error('Failed to attach document to email');
              }
            }
          );
        };
        reader.readAsDataURL(blob);
      } else {
        // Fallback: Insert link in email body
        const trackingLink = attachmentData.tracking_link;
        const linkText = `📎 Trackable Document: ${selectedDocument.title}\n\nView online: ${trackingLink}\n\n`;
        
        Office.context.mailbox.item!.body.getAsync(
          Office.CoercionType.Text,
          (result) => {
            if (result.status === Office.AsyncResultStatus.Succeeded) {
              const currentBody = result.value || '';
              const newBody = linkText + currentBody;
              
              Office.context.mailbox.item!.body.setAsync(
                newBody,
                { coercionType: Office.CoercionType.Text },
                (setResult) => {
                  if (setResult.status === Office.AsyncResultStatus.Succeeded) {
                    toast.success('Trackable document link added to email!');
                    onClose();
                  }
                }
              );
            }
          }
        );
      }
      
    } catch (error: any) {
      console.error('Error attaching document:', error);
      toast.error('Failed to attach document');
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-xl max-w-4xl w-full max-h-[90vh] overflow-hidden">
        
        {/* Header */}
        <div className="bg-blue-600 text-white px-6 py-4 flex items-center justify-between">
          <div className="flex items-center">
            <Paperclip className="w-5 h-5 mr-2" />
            <h2 className="text-lg font-semibold">
              {step === 'choose' && 'Attach Document'}
              {step === 'browse' && 'Choose effyDOC Document'}
              {step === 'preview' && 'Preview Document'}
              {step === 'edit' && 'Edit Document'}
            </h2>
          </div>
          <button onClick={onClose} className="text-white hover:text-gray-200">
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* Content */}
        <div className="p-6 overflow-y-auto max-h-[calc(90vh-200px)]">
          
          {/* Step 1: Choose Attachment Type */}
          {step === 'choose' && (
            <div className="space-y-4">
              <p className="text-gray-600 mb-6">Choose how you want to attach your document:</p>
              
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {/* Computer Attachment */}
                <div 
                  onClick={handleComputerAttachment}
                  className="border-2 border-gray-200 rounded-lg p-6 hover:border-blue-300 hover:bg-blue-50 cursor-pointer transition-all"
                >
                  <div className="text-center">
                    <Computer className="w-12 h-12 text-gray-500 mx-auto mb-3" />
                    <h3 className="text-lg font-medium text-gray-900 mb-2">From Computer</h3>
                    <p className="text-sm text-gray-600 mb-4">
                      Attach files from your computer (standard attachment, non-trackable)
                    </p>
                    <div className="text-xs text-gray-500">
                      • Standard Outlook attachment
                      • No tracking capabilities
                      • Files remain on your computer
                    </div>
                  </div>
                </div>

                {/* effyDOC Attachment */}
                <div 
                  onClick={handleEffyDocAttachment}
                  className="border-2 border-blue-200 bg-blue-50 rounded-lg p-6 hover:border-blue-400 hover:bg-blue-100 cursor-pointer transition-all"
                >
                  <div className="text-center">
                    <FileText className="w-12 h-12 text-blue-600 mx-auto mb-3" />
                    <h3 className="text-lg font-medium text-gray-900 mb-2">From effyDOC</h3>
                    <p className="text-sm text-gray-600 mb-4">
                      Attach trackable documents from your effyDOC library
                    </p>
                    <div className="text-xs text-blue-600 space-y-1">
                      <div>• Real-time tracking</div>
                      <div>• View analytics</div>
                      <div>• Preview & edit before sending</div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Step 2: Browse effyDOC Documents */}
          {step === 'browse' && (
            <div>
              <div className="mb-4">
                <button
                  onClick={() => setStep('choose')}
                  className="text-blue-600 hover:text-blue-800 text-sm flex items-center"
                >
                  ← Back to attachment options
                </button>
              </div>
              
              {/* This would show the document library */}
              <div className="text-center py-8">
                <FileText className="w-12 h-12 text-gray-400 mx-auto mb-3" />
                <p className="text-gray-600">Document library would be shown here</p>
                <p className="text-sm text-gray-500 mt-2">
                  Integration with DocumentLibrary component needed
                </p>
              </div>
            </div>
          )}

          {/* Step 3: Preview Document */}
          {step === 'preview' && documentContent && (
            <div className="space-y-4">
              <div className="flex items-center justify-between mb-4">
                <button
                  onClick={() => setStep('browse')}
                  className="text-blue-600 hover:text-blue-800 text-sm flex items-center"
                >
                  ← Back to documents
                </button>
                
                <div className="flex space-x-2">
                  {documentContent.can_edit && (
                    <button
                      onClick={handleEditDocument}
                      className="flex items-center px-3 py-1 bg-yellow-100 text-yellow-700 rounded-md hover:bg-yellow-200 text-sm"
                    >
                      <Edit3 className="w-4 h-4 mr-1" />
                      Edit
                    </button>
                  )}
                  
                  <button
                    onClick={handleAttachDocument}
                    disabled={loading}
                    className="flex items-center px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:bg-blue-400"
                  >
                    {loading ? (
                      <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                    ) : (
                      <Paperclip className="w-4 h-4 mr-2" />
                    )}
                    Attach as Trackable
                  </button>
                </div>
              </div>

              {/* Document Preview */}
              <div className="border rounded-lg p-4 bg-gray-50">
                <h3 className="text-lg font-semibold mb-3">{documentContent.title}</h3>
                
                <div className="space-y-3">
                  {documentContent.pages && documentContent.pages.length > 0 ? (
                    documentContent.pages.map((page: any, index: number) => (
                      <div key={index} className="bg-white p-4 rounded border">
                        <h4 className="font-medium text-gray-900 mb-2">{page.title}</h4>
                        <div 
                          className="text-sm text-gray-700"
                          dangerouslySetInnerHTML={{ __html: page.content }}
                        />
                      </div>
                    ))
                  ) : (
                    <div className="text-center py-8 text-gray-500">
                      <Eye className="w-8 h-8 mx-auto mb-2" />
                      <p>No content available for preview</p>
                    </div>
                  )}
                </div>
              </div>
            </div>
          )}

          {/* Step 4: Edit Document */}
          {step === 'edit' && editedContent && (
            <div className="space-y-4">
              <div className="flex items-center justify-between mb-4">
                <button
                  onClick={() => {
                    setIsEditing(false);
                    setStep('preview');
                  }}
                  className="text-blue-600 hover:text-blue-800 text-sm flex items-center"
                >
                  ← Cancel editing
                </button>
                
                <button
                  onClick={handleSaveChanges}
                  disabled={loading}
                  className="flex items-center px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 disabled:bg-green-400"
                >
                  {loading ? (
                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                  ) : (
                    <Save className="w-4 h-4 mr-2" />
                  )}
                  Save Changes
                </button>
              </div>

              {/* Document Editor */}
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Document Title
                  </label>
                  <input
                    type="text"
                    value={editedContent.title}
                    onChange={(e) => setEditedContent({
                      ...editedContent,
                      title: e.target.value
                    })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                {/* Simple content editor */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Content
                  </label>
                  <div className="border rounded-lg">
                    {editedContent.pages && editedContent.pages.map((page: any, index: number) => (
                      <div key={index} className="border-b p-4 last:border-b-0">
                        <input
                          type="text"
                          value={page.title}
                          onChange={(e) => {
                            const newPages = [...editedContent.pages];
                            newPages[index] = { ...page, title: e.target.value };
                            setEditedContent({ ...editedContent, pages: newPages });
                          }}
                          className="w-full px-2 py-1 border-b border-gray-200 font-medium mb-2 focus:outline-none focus:border-blue-500"
                          placeholder="Page title"
                        />
                        <textarea
                          value={page.content?.replace(/<[^>]*>/g, '') || ''} // Strip HTML for editing
                          onChange={(e) => {
                            const newPages = [...editedContent.pages];
                            newPages[index] = { ...page, content: e.target.value };
                            setEditedContent({ ...editedContent, pages: newPages });
                          }}
                          rows={6}
                          className="w-full px-2 py-1 border-none resize-none focus:outline-none"
                          placeholder="Page content"
                        />
                      </div>
                    ))}
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>

        {/* Footer */}
        <div className="bg-gray-50 px-6 py-3 flex justify-end space-x-2">
          <button
            onClick={onClose}
            className="px-4 py-2 text-gray-600 hover:text-gray-800"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
};

export default AttachmentWorkflow;