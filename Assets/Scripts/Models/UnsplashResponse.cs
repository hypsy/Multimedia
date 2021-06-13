using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

    
    public class Urls
    {
        public string raw { get; set; }
        public string full { get; set; }
        public string regular { get; set; }
        public string small { get; set; }
        public string thumb { get; set; }
    }

    public class Links
    {
        public string self { get; set; }
        public string html { get; set; }
        public string download { get; set; }
        public string download_location { get; set; }
        public string photos { get; set; }
        public string likes { get; set; }
        public string portfolio { get; set; }
        public string following { get; set; }
        public string followers { get; set; }
        public string related { get; set; }
    }

    public class ProfileImage
    {
        public string small { get; set; }
        public string medium { get; set; }
        public string large { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public string updated_at { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string twitter_username { get; set; }
        public string portfolio_url { get; set; }
        public string bio { get; set; }
        public string location { get; set; }
        public Links links { get; set; }
        public ProfileImage profile_image { get; set; }
        public string instagram_username { get; set; }
        public int total_collections { get; set; }
        public int total_likes { get; set; }
        public int total_photos { get; set; }
        public bool accepted_tos { get; set; }
        public bool for_hire { get; set; }
    }

    public class Type
    {
        public string slug { get; set; }
        public string pretty_slug { get; set; }
    }

    public class Category
    {
        public string slug { get; set; }
        public string pretty_slug { get; set; }
    }

    public class Subcategory
    {
        public string slug { get; set; }
        public string pretty_slug { get; set; }
    }

    public class Ancestry
    {
        public Type type { get; set; }
        public Category category { get; set; }
        public Subcategory subcategory { get; set; }
    }

    public class CoverPhoto
    {
        public string id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string? promoted_at { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string color { get; set; }
        public string blur_hash { get; set; }
        public string description { get; set; }
        public string alt_description { get; set; }
        public Urls urls { get; set; }
        public Links links { get; set; }
        public object[] categories { get; set; }
        public int likes { get; set; }
        public bool liked_by_user { get; set; }
        public object[] current_user_collections { get; set; }
        public object sponsorship { get; set; }
        public User user { get; set; }
    }

    public class Source
    {
        public Ancestry ancestry { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public string description { get; set; }
        public string meta_title { get; set; }
        public string meta_description { get; set; }
        public CoverPhoto cover_photo { get; set; }
    }

    public class Tag
    {
        public string type { get; set; }
        public string title { get; set; }
        public Source source { get; set; }
    }
    
    public class Result
    {
        public string id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string? promoted_at { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string color { get; set; }
        public string blur_hash { get; set; }
        public string description { get; set; }
        public string alt_description { get; set; }
        public Urls urls { get; set; }
        public Links links { get; set; }
        public object[] categories { get; set; }
        public int likes { get; set; }
        public bool liked_by_user { get; set; }
        public List<object> current_user_collections { get; set; }
        public object sponsorship { get; set; }
        public User user { get; set; }
        public Tag[] tags { get; set; }
        public string title { get; set; }
        public string published_at { get; set; }
        public string last_collected_at { get; set; }
        public bool curated { get; set; }
        public bool featured { get; set; }
        public int total_photos { get; set; }
        public bool @private { get; set; }
        public string share_key { get; set; }
        public CoverPhoto cover_photo { get; set; }
        public PreviewPhoto[] preview_photos { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string twitter_username { get; set; }
        public string portfolio_url { get; set; }
        public string bio { get; set; }
        public string location { get; set; }
        public ProfileImage profile_image { get; set; }
        public string instagram_username { get; set; }
        public int total_collections { get; set; }
        public int total_likes { get; set; }
        public bool accepted_tos { get; set; }
        public bool for_hire { get; set; }
        public bool followed_by_user { get; set; }
        public Photos[] photos { get; set; }
    }

    public class Photos
    {
        public int total { get; set; }
        public int total_pages { get; set; }
        public Result[] results { get; set; }
        public string id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string blur_hash { get; set; }
        public Urls urls { get; set; }
    }

    public class PreviewPhoto
    {
        public string id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string blur_hash { get; set; }
        public Urls urls { get; set; }
    }

    public class Collections
    {
        public int total { get; set; }
        public int total_pages { get; set; }
        public Result[] results { get; set; }
    }

    public class Users
    {
        public int total { get; set; }
        public int total_pages { get; set; }
        public Result[] results { get; set; }
    }

    public class RelatedSearch
    {
        public string title { get; set; }
    }

    public class Meta
    {
        public string keyword { get; set; }
        public string title { get; set; }
        public object description { get; set; }
        public bool index { get; set; }
    }

    public class UnsplashResponse
    {
        public Photos photos { get; set; }
        public Collections collections { get; set; }
        public Users users { get; set; }
        public RelatedSearch[] related_searches { get; set; }
        public Meta meta { get; set; }
    }

