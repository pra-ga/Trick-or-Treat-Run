using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;
    void Start()
    {
        creditsPanel.SetActive(false);
    }

    public void LoadFirstScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex+1);
    }

    public void ShowCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void HideCredits()
        {
            if (creditsPanel.activeSelf)
            {
                creditsPanel.SetActive(false);
            }
        }
    
}
