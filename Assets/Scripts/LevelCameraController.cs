using UnityEngine;
using Unity.Cinemachine;

public class LevelCameraController : MonoBehaviour
{
    [Header("Camera Setup")]
    [SerializeField] private Camera m_levelCamera;
    [SerializeField] private CinemachineCamera m_cinemachineLevelCamera;

	[Header("Level Setup")]
	[SerializeField] private LevelInitializer m_initializer;

    public void Activate()
    {
        m_levelCamera.enabled = true;
        m_cinemachineLevelCamera.enabled = true;

		if (m_initializer != null)
		{
			m_initializer.SpawnPlayerHere();
		}

        m_cinemachineLevelCamera.Follow = Player.Instance.transform;
    }

    public void Deactivate()
    {
        m_levelCamera.enabled = false;
		m_levelCamera.targetTexture = null;
        m_cinemachineLevelCamera.enabled = false;
    }
}