using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: Component
{
    private static T instance;

    //Property
    public static T Instance
    {
        //Getter
        get
        {
            //Check if there is no instance yet
            if (instance == null)
            {
                //Check if there is an existing GameObject with the script attached in the active scene
                instance = FindObjectOfType<T>();
                //Check if there is still no script found in the active scene
                if (instance == null)
                {
                    //Create a new GameObjet
                    GameObject go = new GameObject();
                    //Rename the newly created GameObject
                    go.name = typeof(T).Name;
                    //DontDestroyOnLoad(go);
                    //Assign the script to the gameObject and set it as the instance
                    instance = go.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public float globalVariable;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            //DontDestroyOnLoad(instance.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

}
