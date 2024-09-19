using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{

    // Function to start/Restart the game
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Minigame");
    }
}
