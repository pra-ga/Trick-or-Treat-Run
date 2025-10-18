using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource musicSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicate music
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = GetComponent<AudioSource>();
    }

    public void SetVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = volume;
    }

    public float GetVolume()
    {
        return musicSource != null ? musicSource.volume : 0f;
    }
}
