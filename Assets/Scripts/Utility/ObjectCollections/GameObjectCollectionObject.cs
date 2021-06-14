using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameObjectCollection : ObjectCollection<GameObject> { }
[CreateAssetMenu(fileName = "GameObject Collection", menuName = "ObjectCollections/GameObject Collection", order = 1)]
public class GameObjectCollectionObject : ScriptableObject{
    public GameObjectCollection collection;
    public GameObject Get(){
        return collection.Get();
    }
}