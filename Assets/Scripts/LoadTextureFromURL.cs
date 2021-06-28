using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LoadTextureFromURL : MonoBehaviour
{

    
    public GameObject paintings;
    public List<string> urls = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DownloadImage());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DownloadImage()
    {
        //string topic_theme= ChooseGalleryTheme.topic;
        string topic_theme = ChooseGalleryTheme.topic;
        string auth = "Jy1XPTnH-UZo3OuFCcSJSZ5yxz7SlekrhDnBowAdNfk";
        string search_url="";
        UnityWebRequest request_urls = UnityWebRequest.Get("https://api.unsplash.com/search/photos?query="+topic_theme+"&per_page=30&client_id="+auth);
        yield return request_urls.SendWebRequest();
        if (request_urls.isNetworkError || request_urls.isHttpError)
            Debug.Log(request_urls.error);
        else{
            string response= request_urls.downloadHandler.text;
            string altered = response.Replace("'", " ").Replace("\"", "\'");
            Debug.Log(altered); 
            JObject json_response = JObject.Parse(altered);
            
            for(int i=0; i<paintings.transform.childCount; i++)
            {
                var results = json_response["results"][i]["urls"]["small"];

                // serialize JSON results into .NET objects
                Debug.Log(results);
                search_url=results.ToString();
                urls.Add(search_url);
            }
            
        }
        for(int i=0; i< paintings.transform.childCount; i++)
        {
            search_url = urls[i];
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(search_url);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
                paintings.transform.GetChild(i).gameObject.GetComponent<Renderer>().material.mainTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
        
    }





}
