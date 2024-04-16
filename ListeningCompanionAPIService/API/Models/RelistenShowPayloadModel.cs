using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListeningCompanionAPIService.API.Models
{
    // Root myDeserializedClass = JsonConvert.Deserializestring<Root>(myJsonResponse);
    public class Link
    {
        public int source_id { get; set; }
        public int upstream_source_id { get; set; }
        public bool for_reviews { get; set; }
        public bool for_ratings { get; set; }
        public bool for_source { get; set; }
        public string url { get; set; }
        public string label { get; set; }
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class Root
    {
        public List<Source> sources { get; set; }
        public int artist_id { get; set; }
        public string artist_uuid { get; set; }
        public int venue_id { get; set; }
        public Venue venue { get; set; }
        public string venue_uuid { get; set; }
        public int? tour_id { get; set; }
        public string tour_uuid { get; set; }
        public Tour tour { get; set; }
        public int year_id { get; set; }
        public string year_uuid { get; set; }
        public Year year { get; set; }
        public string era_id { get; set; }
        public string era { get; set; }
        public DateTime date { get; set; }
        public double avg_rating { get; set; }
        public double avg_duration { get; set; }
        public string display_date { get; set; }
        public DateTime most_recent_source_updated_at { get; set; }
        public bool has_soundboard_source { get; set; }
        public bool has_streamable_flac_source { get; set; }
        public int source_count { get; set; }
        public string uuid { get; set; }
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class Set
    {
        public int source_id { get; set; }
        public string source_uuid { get; set; }
        public int artist_id { get; set; }
        public string artist_uuid { get; set; }
        public string uuid { get; set; }
        public int index { get; set; }
        public bool is_encore { get; set; }
        public string name { get; set; }
        public List<Track> tracks { get; set; }
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class Source
    {
        public int review_count { get; set; }
        public List<Set> sets { get; set; }
        public List<Link> links { get; set; }
        public int show_id { get; set; }
        public string show_uuid { get; set; }
        public string show { get; set; }
        public string description { get; set; }
        public string taper_notes { get; set; }
        public string source { get; set; }
        public string taper { get; set; }
        public string transferrer { get; set; }
        public string lineage { get; set; }
        public string flac_type { get; set; }
        public int artist_id { get; set; }
        public string artist_uuid { get; set; }
        public int? venue_id { get; set; }
        public string venue_uuid { get; set; }
        public string venue { get; set; }
        public string display_date { get; set; }
        public bool is_soundboard { get; set; }
        public bool is_remaster { get; set; }
        public bool has_jamcharts { get; set; }
        public double avg_rating { get; set; }
        public int num_reviews { get; set; }
        public string num_ratings { get; set; }
        public double avg_rating_weighted { get; set; }
        public double duration { get; set; }
        public string upstream_identifier { get; set; }
        public string uuid { get; set; }
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class Tour
    {
        public int artist_id { get; set; }
        public string artist_uuid { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string upstream_identifier { get; set; }
        public string uuid { get; set; }
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class Track
    {
        public int source_id { get; set; }
        public string source_uuid { get; set; }
        public int source_set_id { get; set; }
        public string source_set_uuid { get; set; }
        public int artist_id { get; set; }
        public string artist_uuid { get; set; }
        public string show_uuid { get; set; }
        public int track_position { get; set; }
        public int duration { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string mp3_url { get; set; }
        public string mp3_md5 { get; set; }
        public string flac_url { get; set; }
        public string flac_md5 { get; set; }
        public string uuid { get; set; }
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class Venue
    {
        public int shows_at_venue { get; set; }
        public int artist_id { get; set; }
        public string artist_uuid { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string upstream_identifier { get; set; }
        public string slug { get; set; }
        public string past_names { get; set; }
        public string sortName { get; set; }
        public string uuid { get; set; }
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class Year
    {
        public int show_count { get; set; }
        public int source_count { get; set; }
        public int duration { get; set; }
        public double avg_duration { get; set; }
        public double avg_rating { get; set; }
        public string year { get; set; }
        public int artist_id { get; set; }
        public string artist_uuid { get; set; }
        public string uuid { get; set; }
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }




}
