using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * A class that handles stopping the running game
 */
public class QuitButton : MonoBehaviour
{
    /**
     * this method handles onClick for quit game
     */
    public void QuitGame()
    {
        Application.Quit();
    }
}
