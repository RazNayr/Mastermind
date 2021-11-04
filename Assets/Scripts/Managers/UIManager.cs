
using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] CanvasGroup menuPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] TextMeshProUGUI sliderText;

    private void Start()
    {
        // Start with menu panel as visible
        menuPanel.alpha = 1;

        // Deactivate options panel
        optionsPanel.SetActive(false);
    }

    public IEnumerator FadeOutMenu()
    {
        // loop over 1 second backwards
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // Fade Out Menu
            menuPanel.alpha = i;

            yield return null;
        }
        menuPanel.interactable = false;
    }

    public IEnumerator FadeInMenu()
    {
        // loop over 1 second
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            // Fade in Menu
            menuPanel.alpha = i;

            yield return null;
        }
        menuPanel.interactable = true;
    }

    public void OpenOptionsPanel()
    {
        optionsPanel.SetActive(true);
        menuPanel.interactable = false;
    }

    public void CloseOptionsPanel()
    {
        optionsPanel.SetActive(false);
        menuPanel.interactable = true;
    }

    public void UpdateSliderText(System.Single value)
    {
        sliderText.text = "Number of different Pegs = " + value.ToString();
    }
}
