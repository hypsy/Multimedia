using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioClip Collection", menuName = "ObjectCollections/AudioClip Collection", order = 1)]
public class AudioClipCollectionObject : ScriptableObject{
    public AudioClipCollection collection;
    public AudioClip Get(){
        return collection.Get();
    }
}