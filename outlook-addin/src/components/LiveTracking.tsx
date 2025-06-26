import React, { useState, useEffect } from 'react';
import { useWebSocket } from '../hooks/useWebSocket.ts';
import { trackingAPI } from '../services/api';
import { 
  Eye, 
  MousePointer, 
  FileText, 
  Users, 
  Activity,
  Clock,
  TrendingUp,
  AlertCircle,
  CheckCircle2,
  Loader2
} from 'lucide-react';
import { formatDistanceToNow } from 'date-fns';

interface LiveTrackingProps {
  userEmail: string;
  selectedDocumentId?: string;
}

interface RecentActivity {
  event_type: string;
  timestamp: string;
  user_email?: string;
  recipient_email?: string;
  page_number?: number;
}

interface CurrentReader {
  email: string;
  page: number;
  since: string;
  duration: number;
}

interface TodayStats {
  emails_opened: number;
  links_clicked: number;
  page_views: number;
  unique_viewers: number;
}

const LiveTracking: React.FC<LiveTrackingProps> = ({ userEmail, selectedDocumentId }) => {
  const { isConnected, trackingUpdates, getDocumentMetrics, subscribeToDocument } = useWebSocket(userEmail);
  const [liveMetrics, setLiveMetrics] = useState<any>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [subscribedDocuments, setSubscribedDocuments] = useState<Set<string>>(new Set());

  useEffect(() => {
    if (selectedDocumentId && !subscribedDocuments.has(selectedDocumentId)) {
      subscribeToDocument(selectedDocumentId);
      setSubscribedDocuments(prev => new Set([...prev, selectedDocumentId]));
      loadMetrics(selectedDocumentId);
    }
  }, [selectedDocumentId, subscribeToDocument, subscribedDocuments]);

  useEffect(() => {
    // Update metrics when new WebSocket data arrives
    if (selectedDocumentId) {
      const metrics = getDocumentMetrics(selectedDocumentId);
      if (metrics) {
        setLiveMetrics(metrics.metrics);
      }
    }
  }, [trackingUpdates, selectedDocumentId, getDocumentMetrics]);

  const loadMetrics = async (documentId: string) => {
    try {
      setLoading(true);
      setError(null);
      const response = await trackingAPI.getLiveMetrics(documentId);
      setLiveMetrics(response);
    } catch (error: any) {
      console.error('Failed to load metrics:', error);
      setError('Failed to load tracking data');
    } finally {
      setLoading(false);
    }
  };

  const getEventIcon = (eventType: string) => {
    switch (eventType) {
      case 'email_opened':
        return <Eye className="w-4 h-4 text-blue-500" />;
      case 'link_clicked':
        return <MousePointer className="w-4 h-4 text-green-500" />;
      case 'page_viewed':
        return <FileText className="w-4 h-4 text-purple-500" />;
      case 'currently_reading':
        return <Activity className="w-4 h-4 text-orange-500" />;
      default:
        return <AlertCircle className="w-4 h-4 text-gray-500" />;
    }
  };

  const getEventDescription = (activity: RecentActivity) => {
    const email = activity.recipient_email || activity.user_email || 'Someone';
    const shortEmail = email.length > 20 ? email.substring(0, 17) + '...' : email;
    
    switch (activity.event_type) {
      case 'email_opened':
        return `${shortEmail} opened the email`;
      case 'link_clicked':
        return `${shortEmail} clicked the document link`;
      case 'page_viewed':
        return `${shortEmail} viewed page ${activity.page_number || 1}`;
      case 'currently_reading':
        return `${shortEmail} is reading page ${activity.page_number || 1}`;
      default:
        return `${shortEmail} performed an action`;
    }
  };

  const formatTimeAgo = (timestamp: string) => {
    try {
      return formatDistanceToNow(new Date(timestamp), { addSuffix: true });
    } catch {
      return 'Unknown time';
    }
  };

  if (!selectedDocumentId) {
    return (
      <div className="text-center py-8">
        <Activity className="w-12 h-12 text-gray-400 mx-auto mb-3" />
        <h3 className="text-lg font-medium text-gray-900 mb-2">Live Tracking</h3>
        <p className="text-gray-500 text-sm">
          Select a document from your library to see real-time tracking data.
        </p>
      </div>
    );
  }

  if (loading && !liveMetrics) {
    return (
      <div className="flex items-center justify-center py-8">
        <Loader2 className="w-6 h-6 animate-spin text-blue-500" />
        <span className="ml-2 text-gray-600">Loading tracking data...</span>
      </div>
    );
  }

  if (error && !liveMetrics) {
    return (
      <div className="p-4 bg-red-50 border border-red-200 rounded-lg">
        <p className="text-red-600 text-sm">{error}</p>
        <button 
          onClick={() => loadMetrics(selectedDocumentId)}
          className="mt-2 px-3 py-1 bg-red-600 text-white text-xs rounded hover:bg-red-700"
        >
          Retry
        </button>
      </div>
    );
  }

  const currentReaders: CurrentReader[] = liveMetrics?.current_readers || [];
  const recentActivity: RecentActivity[] = liveMetrics?.recent_activity || [];
  const todayStats: TodayStats = liveMetrics?.today_stats || {
    emails_opened: 0,
    links_clicked: 0,
    page_views: 0,
    unique_viewers: 0
  };

  return (
    <div className="space-y-4">
      {/* Connection Status */}
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-lg font-semibold text-gray-900">Live Tracking</h2>
        <div className={`flex items-center text-xs px-2 py-1 rounded-full ${
          isConnected ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'
        }`}>
          <div className={`w-2 h-2 rounded-full mr-1 ${
            isConnected ? 'bg-green-500' : 'bg-red-500'
          }`} />
          {isConnected ? 'Connected' : 'Disconnected'}
        </div>
      </div>

      {/* Currently Reading */}
      {currentReaders.length > 0 && (
        <div className="bg-green-50 border border-green-200 rounded-lg p-4">
          <div className="flex items-center mb-2">
            <div className="w-2 h-2 bg-green-500 rounded-full animate-pulse mr-2" />
            <h3 className="text-sm font-medium text-green-800">Currently Reading</h3>
          </div>
          <div className="space-y-2">
            {currentReaders.map((reader, index) => (
              <div key={index} className="flex items-center justify-between text-sm">
                <div className="flex items-center">
                  <Users className="w-3 h-3 text-green-600 mr-1" />
                  <span className="text-green-700 font-medium">
                    {reader.email.length > 25 ? reader.email.substring(0, 22) + '...' : reader.email}
                  </span>
                </div>
                <div className="text-green-600">
                  Page {reader.page} • {formatTimeAgo(reader.since)}
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Today's Stats */}
      <div className="grid grid-cols-2 gap-3">
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-3">
          <div className="flex items-center">
            <Eye className="w-4 h-4 text-blue-600 mr-2" />
            <div>
              <div className="text-lg font-semibold text-blue-900">{todayStats.emails_opened}</div>
              <div className="text-xs text-blue-600">Emails Opened</div>
            </div>
          </div>
        </div>
        
        <div className="bg-green-50 border border-green-200 rounded-lg p-3">
          <div className="flex items-center">
            <MousePointer className="w-4 h-4 text-green-600 mr-2" />
            <div>
              <div className="text-lg font-semibold text-green-900">{todayStats.links_clicked}</div>
              <div className="text-xs text-green-600">Links Clicked</div>
            </div>
          </div>
        </div>
        
        <div className="bg-purple-50 border border-purple-200 rounded-lg p-3">
          <div className="flex items-center">
            <FileText className="w-4 h-4 text-purple-600 mr-2" />
            <div>
              <div className="text-lg font-semibold text-purple-900">{todayStats.page_views}</div>
              <div className="text-xs text-purple-600">Page Views</div>
            </div>
          </div>
        </div>
        
        <div className="bg-orange-50 border border-orange-200 rounded-lg p-3">
          <div className="flex items-center">
            <Users className="w-4 h-4 text-orange-600 mr-2" />
            <div>
              <div className="text-lg font-semibold text-orange-900">{todayStats.unique_viewers}</div>
              <div className="text-xs text-orange-600">Unique Viewers</div>
            </div>
          </div>
        </div>
      </div>

      {/* Recent Activity */}
      <div className="bg-white border border-gray-200 rounded-lg">
        <div className="px-4 py-3 border-b border-gray-200">
          <h3 className="text-sm font-medium text-gray-900 flex items-center">
            <TrendingUp className="w-4 h-4 mr-2" />
            Recent Activity
          </h3>
        </div>
        
        <div className="max-h-64 overflow-y-auto">
          {recentActivity.length > 0 ? (
            <div className="divide-y divide-gray-100">
              {recentActivity.slice(0, 10).map((activity, index) => (
                <div key={index} className="px-4 py-3 hover:bg-gray-50">
                  <div className="flex items-start">
                    <div className="mr-3 mt-0.5">
                      {getEventIcon(activity.event_type)}
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm text-gray-900">
                        {getEventDescription(activity)}
                      </p>
                      <p className="text-xs text-gray-500 mt-1">
                        {formatTimeAgo(activity.timestamp)}
                      </p>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="px-4 py-8 text-center">
              <Clock className="w-8 h-8 text-gray-400 mx-auto mb-2" />
              <p className="text-sm text-gray-500">No recent activity</p>
              <p className="text-xs text-gray-400 mt-1">
                Activity will appear here when recipients interact with your document
              </p>
            </div>
          )}
        </div>
      </div>

      {/* Real-time Updates Indicator */}
      {trackingUpdates.length > 0 && (
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-3">
          <div className="flex items-center text-sm text-blue-700">
            <CheckCircle2 className="w-4 h-4 mr-2" />
            <span>Real-time updates active</span>
          </div>
          <p className="text-xs text-blue-600 mt-1">
            {trackingUpdates.length} update(s) received in this session
          </p>
        </div>
      )}
    </div>
  );
};

export default LiveTracking;