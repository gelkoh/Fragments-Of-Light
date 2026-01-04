using UnityEngine;
using System.Collections;

public class MusicManager : SingletonManager
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource m_musicObject;

	[SerializeField] private MusicLibrary m_musicLibrary;
	private AudioSource m_audioSource;

    public override void InitializeManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

	private void Start()
	{
		Play(MusicContext.RegularPages);
	}

	public void Play(MusicContext context)
	{
		MusicTrack musicTrack = m_musicLibrary.GetMusicTrackForContext(context);

		if (m_audioSource != null)
		{
			StartCoroutine(FadeOutAndDestroy(m_audioSource, 3.0f));
		}

		m_audioSource = Instantiate(m_musicObject, transform);
		m_audioSource.clip = musicTrack.audioClip;
		m_audioSource.volume = musicTrack.defaultVolume;
		m_audioSource.Play();
	}

	private IEnumerator FadeOutAndDestroy(AudioSource source, float duration)
	{
		float startVolume = source.volume;

		while (source.volume > 0)
		{
			source.volume -= startVolume * Time.deltaTime / duration;
			yield return null;
		}

		source.Stop();
		Destroy(source.gameObject);
	}
}