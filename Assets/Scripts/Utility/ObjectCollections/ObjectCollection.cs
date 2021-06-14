using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectCollection<T>
{
    public List<T> objects = new List<T>();

    private BetterRandom betterRandom = new BetterRandom();

    public void Add(T newItem)
    {
        objects.Add(newItem);
    }

    public T Get()
    {
        return objects[betterRandom.RandomNoRepeat(0, objects.Count)];
    }
}

[System.Serializable]
public class AudioClipCollection : ObjectCollection<AudioClip> { }

[System.Serializable]
public class StringCollection : ObjectCollection<string> { }
[CreateAssetMenu(fileName = "String Collection", menuName = "ObjectCollections/String Collection", order = 1)]
public class StringCollectionObject : ScriptableObject{
    public StringCollection collection;
    public string Get(){
        return collection.Get();
    }
}

[System.Serializable]
public class MaterialCollection : ObjectCollection<Material> { }
[CreateAssetMenu(fileName = "Material Collection", menuName = "ObjectCollections/Material Collection", order = 1)]
public class MaterialCollectionObject : ScriptableObject{
    public MaterialCollection collection;
    public Material Get(){
        return collection.Get();
    }
}