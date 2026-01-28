using UnityEngine;

public enum MusicContext
{
    RegularPages,
    Chapter1,
    Chapter2
}

[CreateAssetMenu(fileName = "NewMusicTrack", menuName = "ScriptableObjects/MusicTrack")]
public class MusicTrack : ScriptableObject
{
    public AudioClip audioClip;
    public MusicContext context;

    [Range(0f, 1f)] 
    public float defaultVolume = 1f;
}