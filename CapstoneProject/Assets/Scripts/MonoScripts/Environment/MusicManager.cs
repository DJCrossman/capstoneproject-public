using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] Tracks;

	// Use this for initialization
	void Start ()
	{
        PlayNextSong();
    }

    private void PlayNextSong()
    {
        audio.clip = Tracks[Random.Range(0, Tracks.Length)];
        audio.Play();
        Invoke("PlayNextSong", audio.clip.length);
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static MusicManager _instance;

    public static MusicManager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<MusicManager>()); }
    }
    public static void Reset()
    {
        _instance = null;
    }
	
}
