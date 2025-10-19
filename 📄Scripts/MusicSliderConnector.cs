using UnityEngine;
using UnityEngine.UI;

public class MusicSliderConnector : MonoBehaviour
{
    public Slider musicSlider;
    [SerializeField] private AudioSource musicSource;

    void Start()
    {
        musicSource = GetComponent<AudioSource>();

        if (musicSlider == null)
        {
            //Must call the slider as "MusicSlider" for this script to work! Lol!! 18Oct25
            musicSlider = GameObject.Find("MusicSlider")?.GetComponent<Slider>();
        }

        if (musicSlider != null)
        {
            //musicSlider.value = MusicManager.Instance.GetVolume();
            //musicSlider.onValueChanged.AddListener(MusicManager.Instance.SetVolume);
            musicSlider.value =GetVolume();
            //SetVolume(musicSlider.value);
            musicSlider.onValueChanged.AddListener(SetVolume);

        }
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
