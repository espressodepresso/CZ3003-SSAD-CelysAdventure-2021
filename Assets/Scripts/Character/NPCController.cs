using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//!  A NPC Control class. 
/*!
  A more elaborate class description.
*/
public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;

    //! A method.
    /*! Starts the NPC dialog upon collision */

    public void Interact()
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));

    }
}