using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door_Placeholder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Load Title Scene
        SceneManager.LoadScene(0);
    }
}
