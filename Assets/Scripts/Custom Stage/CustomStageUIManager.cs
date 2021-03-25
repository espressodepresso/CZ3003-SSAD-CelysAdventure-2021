using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CustomStageUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    List<string> pokemonSelected;
    string background;
    public GameObject SelectPokemonUI;
    public GameObject SelectQuestionUI;
    public GameObject AddYourOwnQuestionsUI;
    void Awake()
    {
        pokemonSelected = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectPokemon(string name)
    {
        if (pokemonSelected.Contains(name))
        {
            pokemonSelected.RemoveAll(pokemon => pokemon == name);
        } else
        {
            pokemonSelected.Add(name);
        }
        
    }
    
    public void SelectBackground(string name)
    {
        background = name;
       
    }
    public void clickOKInAddYourOwnQuestionsUI()
    {
        AddYourOwnQuestionsUI.SetActive(false);
        SelectQuestionUI.SetActive(true);
        GameObject.Find("QuestionsManager").GetComponent<CustomStageChooseQuestions>().Dropdown_IndexChanged(0);
       
    }

    public void clickAddYourOwnQuestionsButton()
    {
        SelectQuestionUI.SetActive(false);
        AddYourOwnQuestionsUI.SetActive(true);
    }
    
    public void clickNextButton()
    {
        SelectPokemonUI.SetActive(false);
        SelectQuestionUI.SetActive(true);
    }
    public void clickFinishButton()
    {
        List<string> chosenQuestionsList = GameObject.Find("QuestionsManager").GetComponent<CustomStageChooseQuestions>().getChosenQuestionsList();
        foreach(string chosenQuestion in chosenQuestionsList)
        {
            Debug.Log(chosenQuestion);
        }
        foreach(string pokemon in pokemonSelected)
        {
            Debug.Log(pokemon);
        }
        Debug.Log(background);
    }



}
