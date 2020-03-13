using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private GameObject m_creditsPanel, m_mainMenuPanel;

    private void Start()
    {
        // Turn off the credits panel on play
        m_creditsPanel = GameObject.Find("Credits_Panel");
        m_mainMenuPanel = GameObject.Find("MainMenu_Panel");

        m_creditsPanel.SetActive(false);
    }

    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene(1);
    }

    public void OnCreditsClick()
    {
        // Turn on, and possibly off, the credits panel

        if (m_creditsPanel.activeSelf == true)
        {
            m_creditsPanel.SetActive(false);
            m_mainMenuPanel.SetActive(true);
        }
        else if (m_creditsPanel.activeSelf == false)
        {
            m_creditsPanel.SetActive(true);
            m_mainMenuPanel.SetActive(false);
        }
    }

    public void OnExitToDesktopClicked()
    {
        Application.Quit();
    }
}
