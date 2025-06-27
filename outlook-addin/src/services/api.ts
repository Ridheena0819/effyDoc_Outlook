import axios from 'axios';

// Get backend URL from environment or use default
const BACKEND_URL = process.env.REACT_APP_BACKEND_URL || 'https://d151863e-ee5d-469c-b381-52e4cf994367.preview.emergentagent.com';

// Create axios instance with base configuration
export const apiClient = axios.create({
  baseURL: BACKEND_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add request interceptor to include auth token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('effydoc_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Add response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token expired or invalid
      localStorage.removeItem('effydoc_token');
      localStorage.removeItem('effydoc_user');
      window.location.reload();
    }
    return Promise.reject(error);
  }
);

// Auth API
export const authAPI = {
  login: async (credentials: { email: string; password: string }) => {
    const response = await apiClient.post('/api/auth/login', credentials);
    return response.data;
  },
  
  getCurrentUser: async () => {
    const response = await apiClient.get('/api/users/me');
    return response.data;
  },
  
  getSessionInfo: async () => {
    const response = await apiClient.get('/api/outlook/user/session-info');
    return response.data;
  }
};

// Document Library API
export const documentsAPI = {
  getMyLibrary: async () => {
    const response = await apiClient.get('/api/outlook/documents/my-library');
    return response.data;
  },
  
  getContentHub: async () => {
    const response = await apiClient.get('/api/outlook/documents/content-hub');
    return response.data;
  },
  
  generateTrackableLink: async (documentId: string) => {
    const response = await apiClient.get(`/api/outlook/documents/${documentId}/share-link`);
    return response.data;
  },

  getDocumentContent: async (documentId: string) => {
    const response = await apiClient.get(`/api/outlook/documents/${documentId}/content`);
    return response.data;
  },

  updateDocumentContent: async (documentId: string, contentData: any) => {
    const response = await apiClient.put(`/api/outlook/documents/${documentId}/content`, contentData);
    return response.data;
  },

  generateAttachmentData: async (documentId: string, options: any) => {
    const response = await apiClient.post(`/api/outlook/documents/${documentId}/attachment-data`, options);
    return response.data;
  }
};

// Tracking API
export const trackingAPI = {
  trackEmailSent: async (trackingData: {
    document_id: string;
    recipients: string[];
    subject: string;
    email_body?: string;
  }) => {
    const response = await apiClient.post('/api/outlook/tracking/email-sent', trackingData);
    return response.data;
  },
  
  getLiveMetrics: async (documentId: string) => {
    const response = await apiClient.get(`/api/outlook/tracking/live-metrics/${documentId}`);
    return response.data;
  },
  
  getDocumentAnalytics: async (documentId: string) => {
    const response = await apiClient.get(`/api/outlook/tracking/document-analytics/${documentId}`);
    return response.data;
  }
};

// WebSocket connection
export class WebSocketService {
  private ws: WebSocket | null = null;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;
  private reconnectDelay = 1000;
  private callbacks: { [key: string]: Function[] } = {};

  connect(userEmail: string, token: string) {
    const wsUrl = `${BACKEND_URL.replace('https:', 'wss:').replace('http:', 'ws:')}/api/outlook/ws?user_email=${encodeURIComponent(userEmail)}&token=${encodeURIComponent(token)}`;
    
    this.ws = new WebSocket(wsUrl);
    
    this.ws.onopen = () => {
      console.log('WebSocket connected');
      this.reconnectAttempts = 0;
      this.emit('connected', {});
    };
    
    this.ws.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data);
        this.emit(data.type, data);
      } catch (error) {
        console.error('Error parsing WebSocket message:', error);
      }
    };
    
    this.ws.onclose = () => {
      console.log('WebSocket disconnected');
      this.emit('disconnected', {});
      this.attemptReconnect(userEmail, token);
    };
    
    this.ws.onerror = (error) => {
      console.error('WebSocket error:', error);
      this.emit('error', { error });
    };
  }

  private attemptReconnect(userEmail: string, token: string) {
    if (this.reconnectAttempts < this.maxReconnectAttempts) {
      this.reconnectAttempts++;
      setTimeout(() => {
        console.log(`Attempting to reconnect (${this.reconnectAttempts}/${this.maxReconnectAttempts})`);
        this.connect(userEmail, token);
      }, this.reconnectDelay * this.reconnectAttempts);
    }
  }

  subscribe(eventType: string, callback: Function) {
    if (!this.callbacks[eventType]) {
      this.callbacks[eventType] = [];
    }
    this.callbacks[eventType].push(callback);
  }

  unsubscribe(eventType: string, callback: Function) {
    if (this.callbacks[eventType]) {
      this.callbacks[eventType] = this.callbacks[eventType].filter(cb => cb !== callback);
    }
  }

  private emit(eventType: string, data: any) {
    if (this.callbacks[eventType]) {
      this.callbacks[eventType].forEach(callback => callback(data));
    }
  }

  subscribeToDocument(documentId: string) {
    if (this.ws && this.ws.readyState === WebSocket.OPEN) {
      this.ws.send(JSON.stringify({
        type: 'subscribe_document',
        document_id: documentId
      }));
    }
  }

  unsubscribeFromDocument(documentId: string) {
    if (this.ws && this.ws.readyState === WebSocket.OPEN) {
      this.ws.send(JSON.stringify({
        type: 'unsubscribe_document',
        document_id: documentId
      }));
    }
  }

  sendHeartbeat() {
    if (this.ws && this.ws.readyState === WebSocket.OPEN) {
      this.ws.send(JSON.stringify({
        type: 'heartbeat',
        timestamp: new Date().toISOString()
      }));
    }
  }

  disconnect() {
    if (this.ws) {
      this.ws.close();
      this.ws = null;
    }
  }

  isConnected(): boolean {
    return this.ws?.readyState === WebSocket.OPEN;
  }
}

// Global WebSocket instance
export const websocketService = new WebSocketService();

export default apiClient;