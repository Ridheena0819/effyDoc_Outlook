import { useState, useEffect } from 'react';
import { websocketService } from '../services/api';

interface TrackingUpdate {
  type: string;
  document_id: string;
  event: {
    id: string;
    event_type: string;
    timestamp: string;
    user_email?: string;
    recipient_email?: string;
    page_number?: number;
    duration?: number;
    metadata?: any;
  };
}

interface LiveMetrics {
  document_id: string;
  metrics: {
    current_readers: Array<{
      email: string;
      page: number;
      since: string;
      duration: number;
    }>;
    recent_activity: Array<{
      event_type: string;
      timestamp: string;
      user_email?: string;
      recipient_email?: string;
      page_number?: number;
    }>;
    today_stats: {
      emails_opened: number;
      links_clicked: number;
      page_views: number;
      unique_viewers: number;
    };
    generated_at: string;
  };
}

export const useWebSocket = (userEmail?: string) => {
  const [isConnected, setIsConnected] = useState(false);
  const [trackingUpdates, setTrackingUpdates] = useState<TrackingUpdate[]>([]);
  const [liveMetrics, setLiveMetrics] = useState<{ [documentId: string]: LiveMetrics }>({});
  const [connectionError, setConnectionError] = useState<string | null>(null);

  useEffect(() => {
    if (!userEmail) return;

    const token = localStorage.getItem('effydoc_token');
    if (!token) return;

    // Set up WebSocket event listeners
    const handleConnected = () => {
      setIsConnected(true);
      setConnectionError(null);
      console.log('WebSocket connected successfully');
    };

    const handleDisconnected = () => {
      setIsConnected(false);
      console.log('WebSocket disconnected');
    };

    const handleError = (data: { error: any }) => {
      setConnectionError('Connection error occurred');
      console.error('WebSocket error:', data.error);
    };

    const handleTrackingUpdate = (data: TrackingUpdate) => {
      setTrackingUpdates(prev => {
        const newUpdates = [data, ...prev].slice(0, 50); // Keep last 50 updates
        return newUpdates;
      });
      console.log('Received tracking update:', data);
    };

    const handleLiveMetrics = (data: LiveMetrics) => {
      setLiveMetrics(prev => ({
        ...prev,
        [data.document_id]: data
      }));
      console.log('Received live metrics:', data);
    };

    // Subscribe to events
    websocketService.subscribe('connected', handleConnected);
    websocketService.subscribe('disconnected', handleDisconnected);
    websocketService.subscribe('error', handleError);
    websocketService.subscribe('tracking_update', handleTrackingUpdate);
    websocketService.subscribe('live_metrics', handleLiveMetrics);

    // Connect WebSocket
    websocketService.connect(userEmail, token);

    // Set up heartbeat
    const heartbeatInterval = setInterval(() => {
      if (websocketService.isConnected()) {
        websocketService.sendHeartbeat();
      }
    }, 30000); // Every 30 seconds

    // Cleanup on unmount
    return () => {
      clearInterval(heartbeatInterval);
      websocketService.unsubscribe('connected', handleConnected);
      websocketService.unsubscribe('disconnected', handleDisconnected);
      websocketService.unsubscribe('error', handleError);
      websocketService.unsubscribe('tracking_update', handleTrackingUpdate);
      websocketService.unsubscribe('live_metrics', handleLiveMetrics);
      websocketService.disconnect();
    };
  }, [userEmail]);

  const subscribeToDocument = (documentId: string) => {
    websocketService.subscribeToDocument(documentId);
  };

  const unsubscribeFromDocument = (documentId: string) => {
    websocketService.unsubscribeFromDocument(documentId);
  };

  const clearTrackingUpdates = () => {
    setTrackingUpdates([]);
  };

  const getDocumentMetrics = (documentId: string): LiveMetrics | null => {
    return liveMetrics[documentId] || null;
  };

  return {
    isConnected,
    trackingUpdates,
    liveMetrics,
    connectionError,
    subscribeToDocument,
    unsubscribeFromDocument,
    clearTrackingUpdates,
    getDocumentMetrics
  };
};