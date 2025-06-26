using System;
using System.Collections.Generic;

namespace EffyDocOutlookAddin.Models
{
    public class LiveTrackingMetrics
    {
        public string document_id { get; set; }
        public List<CurrentReader> current_readers { get; set; }
        public List<TrackingEvent> recent_activity { get; set; }
        public TodayStats today_stats { get; set; }
    }

    public class CurrentReader
    {
        public string email { get; set; }
        public int page { get; set; }
        public string since { get; set; }
        public int duration { get; set; }
    }

    public class TrackingEvent
    {
        public string id { get; set; }
        public string event_type { get; set; }
        public string document_id { get; set; }
        public string user_email { get; set; }
        public string recipient_email { get; set; }
        public int? page_number { get; set; }
        public int? duration { get; set; }
        public string user_agent { get; set; }
        public string ip_address { get; set; }
        public string session_id { get; set; }
        public DateTime timestamp { get; set; }
        public object metadata { get; set; }
    }

    public class TodayStats
    {
        public int emails_opened { get; set; }
        public int links_clicked { get; set; }
        public int page_views { get; set; }
        public int unique_viewers { get; set; }
    }

    public class DocumentAnalytics
    {
        public string document_id { get; set; }
        public string document_title { get; set; }
        public AnalyticsSummary summary { get; set; }
        public Dictionary<string, PageAnalytics> page_analytics { get; set; }
        public int recent_activity_count { get; set; }
        public int total_events { get; set; }
        public DateTime? last_activity { get; set; }
        public string generated_at { get; set; }
    }

    public class AnalyticsSummary
    {
        public int total_emails_sent { get; set; }
        public int total_opens { get; set; }
        public int total_clicks { get; set; }
        public int unique_viewers { get; set; }
        public double open_rate { get; set; }
        public double click_rate { get; set; }
    }

    public class PageAnalytics
    {
        public int views { get; set; }
        public int unique_viewers { get; set; }
        public double total_time { get; set; }
        public double avg_time { get; set; }
    }

    public class WebSocketMessage
    {
        public string type { get; set; }
        public string document_id { get; set; }
        public object data { get; set; }
        public string timestamp { get; set; }
    }

    public class EmailTrackingData
    {
        public string document_id { get; set; }
        public List<string> recipients { get; set; }
        public string subject { get; set; }
        public string email_body { get; set; }
        public List<string> attachments { get; set; }
    }

    public class DocumentEventData
    {
        public string event_type { get; set; }
        public string document_id { get; set; }
        public string user_email { get; set; }
        public string recipient_email { get; set; }
        public int? page_number { get; set; }
        public int? duration { get; set; }
        public string user_agent { get; set; }
        public string ip_address { get; set; }
        public string session_id { get; set; }
        public object metadata { get; set; }
    }
}