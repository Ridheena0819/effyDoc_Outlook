using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffyDocOutlookPlugin.Models
{
    public class DocumentModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalPages { get; set; }
        public int TotalViews { get; set; }
        public string Description { get; set; }
        public string TrackingLink { get; set; }
        public bool IsTrackable { get; set; } = true;
        
        public override string ToString()
        {
            return Title;
        }
    }

    public class TrackingStats
    {
        public int TotalViews { get; set; }
        public int TotalShares { get; set; }
        public DateTime? LastViewed { get; set; }
        public double OpenRate { get; set; }
        public double ClickRate { get; set; }
    }

    public class DocumentAnalytics
    {
        public string DocumentId { get; set; }
        public string DocumentTitle { get; set; }
        public TrackingStats Summary { get; set; }
        public Dictionary<int, PageAnalytics> PageAnalytics { get; set; }
        public int RecentActivityCount { get; set; }
        public int TotalEvents { get; set; }
        public DateTime? LastActivity { get; set; }
    }

    public class PageAnalytics
    {
        public int Views { get; set; }
        public int UniqueViewers { get; set; }
        public double AverageTime { get; set; }
    }
}