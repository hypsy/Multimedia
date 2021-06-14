using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChooseGalleryTheme : MonoBehaviour
{
    public static string topic = "nature";

    void Start()
    {
        var dropdown = transform.GetComponent<Dropdown>();
        dropdown.options.Clear();
        List<string> test = new List<string>(){"", "Katze", "Hund"};
        
        foreach(var item in test)
        {
            dropdown.options.Add(new Dropdown.OptionData(){text = item});
        }

        dropdown.onValueChanged.AddListener(delegate { DropdownSelected(dropdown);});

    }
    /*public void ChooseTheme()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }*/



    void DropdownSelected(Dropdown dropdown)
    {
        int index = dropdown.value;
        topic = dropdown.options[index].text;
        Debug.Log(topic);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    
    }
}
