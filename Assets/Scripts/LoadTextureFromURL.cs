using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LoadTextureFromURL : MonoBehaviour
{

    public string TextureURL = "";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DownloadImage(TextureURL));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DownloadImage(string MediaUrl)
    {
        string topic="cat";
        string auth = "Jy1XPTnH-UZo3OuFCcSJSZ5yxz7SlekrhDnBowAdNfk";
        string search_url="";
        UnityWebRequest request_urls = UnityWebRequest.Get("https://api.unsplash.com/search/photos?query="+topic+"&client_id="+auth);
        yield return request_urls.SendWebRequest();
        if (request_urls.isNetworkError || request_urls.isHttpError)
            Debug.Log(request_urls.error);
        else{
            string response= request_urls.downloadHandler.text;
            string altered = response.Replace("'", " ").Replace("\"", "\'");
            Debug.Log(altered); 
            JObject json_response = JObject.Parse(altered);

            var results = json_response["results"][0]["urls"]["small"];

            // serialize JSON results into .NET objects
            Debug.Log(results);
            search_url=results.ToString();
        }

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(search_url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            this.gameObject.GetComponent<Renderer>().material.mainTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }





}
