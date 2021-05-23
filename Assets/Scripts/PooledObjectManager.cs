using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PooledObject
{
    public string id;
    public GameObject prefab;
    public Transform parent;
    public int amountToPool;
    public bool shouldExpand;
}

public class PooledObjectData : MonoBehaviour
{
    private string _id;
    public string id
    {
        get { return _id; }
        set { _id = value; }
    }
}

public class PooledObjectManager : Singleton<PooledObjectManager>
{
    public List<PooledObject> objectsToPool;
    public List<GameObject> pooledObjects;

    protected override void Awake()
    {
        base.Awake();
        InitializePooledObjects();
    }

    private void InitializePooledObjects()
    {
        foreach (PooledObject item in objectsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                AddObjectToList(item);
            }
        }
    }

    private GameObject AddObjectToList(PooledObject item)
    {
        GameObject obj = (GameObject)Instantiate(item.prefab);
        obj.AddComponent<PooledObjectData>();
        obj.GetComponent<PooledObjectData>().id = item.id;
        if(item.parent != null)
            obj.transform.SetParent(item.parent);
        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }

    public GameObject GetPooledObject(string id)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].GetComponent<PooledObjectData>().id == id)
            {
                return pooledObjects[i];
            }
        }

        foreach (PooledObject item in objectsToPool)
        {
            if (item.id == id)
            {
                if (item.shouldExpand)
                {
                    return AddObjectToList(item);
                }
            }
        }
        return null;
    }

}
