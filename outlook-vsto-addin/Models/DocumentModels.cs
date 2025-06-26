using System;
using System.Collections.Generic;

namespace EffyDocOutlookAddin.Models
{
    public class LoginResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public User user { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public string email { get; set; }
        public string full_name { get; set; }
        public string role { get; set; }
        public string organization { get; set; }
    }

    public class UserSessionInfo
    {
        public string user_email { get; set; }
        public string full_name { get; set; }
        public string organization { get; set; }
        public string role { get; set; }
        public string connected_at { get; set; }
        public string websocket_endpoint { get; set; }
        public Permissions permissions { get; set; }
    }

    public class Permissions
    {
        public bool can_send_documents { get; set; }
        public bool can_view_analytics { get; set; }
        public bool can_access_content_hub { get; set; }
        public bool can_create_documents { get; set; }
    }

    public class DocumentLibraryResponse
    {
        public List<DocumentInfo> documents { get; set; }
        public int total_count { get; set; }
        public string user_email { get; set; }
        public string organization { get; set; }
    }

    public class DocumentInfo
    {
        public string id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int total_pages { get; set; }
        public long file_size { get; set; }
        public TrackingStats tracking_stats { get; set; }
        public string share_link { get; set; }
        public bool is_trackable { get; set; }
        public bool is_template { get; set; }
        public string owner_name { get; set; }
        public string description { get; set; }
        public List<string> tags { get; set; }
    }

    public class TrackingStats
    {
        public int total_views { get; set; }
        public int total_shares { get; set; }
        public string last_viewed { get; set; }
    }

    public class DocumentContent
    {
        public string id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public int total_pages { get; set; }
        public List<DocumentPage> pages { get; set; }
        public List<DocumentSection> sections { get; set; }
        public bool can_edit { get; set; }
    }

    public class DocumentPage
    {
        public int page_number { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public List<MultimediaElement> multimedia_elements { get; set; }
        public List<InteractiveElement> interactive_elements { get; set; }
        public object metadata { get; set; }
    }

    public class DocumentSection
    {
        public string id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public int order { get; set; }
        public List<MultimediaElement> multimedia_elements { get; set; }
        public List<InteractiveElement> interactive_elements { get; set; }
    }

    public class MultimediaElement
    {
        public string id { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int? duration { get; set; }
        public object position { get; set; }
        public object size { get; set; }
    }

    public class InteractiveElement
    {
        public string id { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        public string action { get; set; }
        public bool required { get; set; }
        public object position { get; set; }
        public object size { get; set; }
    }

    public class TrackableLinkResponse
    {
        public string document_id { get; set; }
        public string document_title { get; set; }
        public string trackable_link { get; set; }
        public string full_url { get; set; }
        public string generated_by { get; set; }
        public string generated_at { get; set; }
        public bool tracking_enabled { get; set; }
    }

    public class AttachmentDataResponse
    {
        public string document_id { get; set; }
        public string filename { get; set; }
        public string content { get; set; }
        public string tracking_link { get; set; }
    }
}