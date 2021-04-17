using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionButton : MonoBehaviour
{
    public Canvas InstructionCanvas;

    public void onClick(){
        InstructionCanvas.gameObject.SetActive(false);
    }
}
