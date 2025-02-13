using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
   public void QuitGame()
   {
        Application.Quit();
        Debug.Log("Uscito");
   }
}