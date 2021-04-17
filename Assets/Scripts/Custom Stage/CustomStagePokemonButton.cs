using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//! A class that handles pokemon buttons functions for custom stage

public class CustomStagePokemonButton : MonoBehaviour
{
    public GameObject customStageUIManager; /** A unity gameObject that contains the script to customStageUIManager */
    public GameObject selectedGlow; /** A unity gameObject that contains the sprite of a selected button */
    private bool selectedState; /** The selection state of this pokemon */
    private GameObject[] backgrounds; /** An array of gameObject that contains all the backgrounds */
    
    /** 
     * this method handles OnClick for pokemon button
     */
    public void choosePokemon()
    {
        customStageUIManager.GetComponent<CustomStageUIManager>().SelectPokemon(this.name);
        selectedState = !selectedState;
        selectedGlow.SetActive(selectedState);
       
        
    }
    /**
     * this method handles OnClick for background button
     */
    public void chooseBackground()
    {
        customStageUIManager.GetComponent<CustomStageUIManager>().SelectBackground(this.name);

        selectedState = !selectedState;
        backgrounds = GameObject.FindGameObjectsWithTag("background");
        foreach(GameObject background in backgrounds)
        {
            Outline o = background.GetComponent<Outline>();
            o.enabled = false;
        }
        this.GetComponent<Outline>().enabled = true;
    }
}
