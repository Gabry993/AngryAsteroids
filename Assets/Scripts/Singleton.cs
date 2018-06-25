﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

/*
    Useful class to implement a singleton object in Unity
*/
public class Singleton<T>: MonoBehaviour where T: Component {


    private static T instance;

    public static T Instance {
        get {
            if (instance == null) { 
                instance = FindObjectOfType<T>();
                if (instance == null) {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }

	private void Awake() {
        if (instance == null) {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        } 
        else {
            Destroy(gameObject);
        }
	}
}
