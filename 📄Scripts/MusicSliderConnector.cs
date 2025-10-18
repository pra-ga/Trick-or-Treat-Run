using UnityEngine;
using UnityEngine.UI;

public class MusicSliderConnector : MonoBehaviour
{
    public Slider musicSlider;

    void Start()
    {
        if (musicSlider == null)
        {
            musicSlider = GameObject.Find("MusicSlider")?.GetComponent<Slider>();
        }

        if (musicSlider != null)
        {
            musicSlider.value = MusicManager.Instance.GetVolume();
            musicSlider.onValueChanged.AddListener(MusicManager.Instance.SetVolume);
        }
    }
}
