using UnityEngine;
using System;
using System.Collections.Generic;

public class ManagersManager : MonoBehaviour
{
    [SerializeField] protected List<SingletonManager> managers = new List<SingletonManager>();

    public static ManagersManager Instance;

    private Dictionary<Type, object> managerDictionary = new Dictionary<Type, object>();
    
    #region Singleton Setup
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (SingletonManager manager in Instance.managers)
        {
            manager.InitializeManager();
            Instance.managerDictionary.Add(manager.GetType(), manager);
        }
    }
    
    #endregion

    public static T Get<T>()
    {
        if (!Instance.managerDictionary.ContainsKey(typeof(T)))
        {
            Debug.LogError($"Type: {typeof(T)} could not be found inside the manager dictionary");
            return default(T);
        }

        return (T)Instance.managerDictionary[typeof(T)];
    }
}