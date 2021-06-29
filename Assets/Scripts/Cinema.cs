using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Configuration;
using System.Text.RegularExpressions;
using System;
using System.Globalization;
using UnityEngine.Networking;

public class Cinema : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public TextMeshPro titleDisplay;
    private string videoUrl = string.Empty;
    private string title = string.Empty;
    private PlayerController player;
    private List<Tuple<string,string>> videoUrls = new List<Tuple<string,string>>();
    private static HttpWebRequest request;
    public string currentUrl = string.Empty;
    private bool directUrlLoaded = false;
    private bool hasLoadedVideoUrls = false;
    private bool isLoadingUrl = false;

    private void Start() {
        player = FindObjectOfType<PlayerController>();
        LoadUrls(ChooseGalleryTheme.topic);
    }

    private void Update() {
        if(hasLoadedVideoUrls && !isLoadingUrl && videoUrls.Count > 0){
            var entry = videoUrls[UnityEngine.Random.Range(0, videoUrls.Count)];
            videoUrls.Clear();
            DirectUrlFromUrl(entry.Item1);
            isLoadingUrl = true;
        }
        if(directUrlLoaded){
            try{
                videoPlayer.url = currentUrl;
                videoPlayer.Play();
            }
            catch(Exception e){
                UnityEngine.Debug.LogWarning("Video konnte nicht abgespielt werden!");
            }
            directUrlLoaded = false;
        }
        // if(videoPlayer.isPlaying && Vector3.Distance(transform.position, player.transform.position) >= 55.0f){
        //     videoPlayer.Stop();
        //     titleDisplay.text = title;
        // }
    }

    public void LoadUrls(string query){
        videoUrls.Clear();
        string formattedQuery = query.Replace(" ", "+").ToLowerInvariant();
        string requestUri = string.Format("https://www.youtube.com/results?search_query={0}", formattedQuery);
        request = (HttpWebRequest)WebRequest.Create(requestUri);
        request.Accept = "text/html, application/xhtml+xml, */*";
        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
        request.BeginGetResponse(new AsyncCallback(LoadUrls_FinishedResponse), null);
    }

    public void LoadUrls_FinishedResponse(IAsyncResult result){
        string resultPage = string.Empty;
        using(HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result)){
            using(Stream responseStream = response.GetResponseStream()){
                using(StreamReader reader = new StreamReader(responseStream)){
                    resultPage = reader.ReadToEnd();
                    var matches = Regex.Matches(resultPage, @"""title"":{""runs"":\[{""text"":(.*?)""watchEndpoint""");
                    foreach(Match p in matches){
                        Match urlMatch = Regex.Match(p.Value, @"""url"":""/watch\?v=(.*?)""");
                        string videoUrl = urlMatch.Value;
                        videoUrl = videoUrl.Replace("\"url\":\"", string.Empty);
                        videoUrl = videoUrl.Replace("\"", string.Empty);
                        Match titleMatch = Regex.Match(p.Value, @"""title"":{""runs"":\[{""text"":""(.*?)""}\]");
                        string title = titleMatch.Value;
                        title = title.Replace("\"title\":{\"runs\":[{\"text\":\"", string.Empty);
                        title = title.Replace("\"}]", string.Empty);
                        videoUrls.Add(new Tuple<string, string>("http://www.youtube.com" + videoUrl, title));
                    }
                }
            }
        }
        hasLoadedVideoUrls = true;
    }
    
    public void DirectUrlFromUrl(string url){
        request = (HttpWebRequest)WebRequest.Create(url);
        request.Accept = "text/html, application/xhtml+xml, */*";
        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
        request.BeginGetResponse(new AsyncCallback(DirectUrlFromUrl_FinishedResponse), null);
    }

    public void DirectUrlFromUrl_FinishedResponse(IAsyncResult result){
        string resultPage = string.Empty;
        using(HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result)){
            using(Stream responseStream = response.GetResponseStream()){
                using(StreamReader reader = new StreamReader(responseStream)){
                    resultPage = reader.ReadToEnd();
                    var matches = Regex.Matches(resultPage, @"""url"":""https://(.*?).googlevideo.com/videoplayback(.*?)"",""mimeType""");
                    if(matches.Count > 0){
                        string sourceURL = matches[0].Value;
                        sourceURL = sourceURL.Replace("\"url\":\"", string.Empty);
                        sourceURL = sourceURL.Replace("\",\"mimeType\"", string.Empty);
                        sourceURL = System.Web.HttpUtility.UrlDecode(sourceURL);
                        sourceURL = sourceURL.Replace(@"\u003d", "=");
                        sourceURL = sourceURL.Replace(@"\u0026", "&");
                        currentUrl = sourceURL;
                        directUrlLoaded = true;
                    }
                }
            }
        }
        isLoadingUrl = false;
    }
}
