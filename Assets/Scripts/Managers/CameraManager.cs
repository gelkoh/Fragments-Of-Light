using UnityEngine;
using System.Collections.Generic;

public class CameraManager : SingletonManager 
{
  	public static CameraManager Instance;

  	private Dictionary<PageID, Camera> m_map = new Dictionary<PageID, Camera>();

  	public override void InitializeManager() 
	{
    	if (Instance != null && Instance != this) {
      		Destroy(gameObject);
      		return;
    	}

    	Instance = this;
  	}

  	public void Register(PageID pageID, Camera cam)
    {
    	m_map[pageID] = cam;
  	}

  	public void Unregister(PageID pageID, Camera cam) 
	{
    	if (m_map.TryGetValue(pageID, out Camera existing) && existing == cam) 
		{
     		m_map.Remove(pageID);
    	}
  	}

  	public Camera GetCamera(PageID pageID) 
	{
    	m_map.TryGetValue(pageID, out Camera cam);
    	return cam;
  	}

  	public void EnableCamera() {}
  	public void DisableCamera() {}
}