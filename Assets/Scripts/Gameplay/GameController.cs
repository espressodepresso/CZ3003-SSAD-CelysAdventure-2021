using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Explore, Quiz , GymQuiz, Dialog , Inventory , Replace }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] Inventory inventorySystem; 
    [SerializeField] Replace replaceSystem; 
    [SerializeField] QuizManager quizSystem;
    [SerializeField] GameObject buttonCanvas;
    [SerializeField] Camera overworldCamera;
    [SerializeField] Camera inventoryCamera;
    [SerializeField] Camera replaceCamera;
    [SerializeField] GameObject quizCanvas;
    [SerializeField] Camera quizCamera;
    
     
    GameState state;

    private void Start()
    {
        //Subscribing to all the events
        playerController.OnOpenInventory += OpenInventory;
        inventorySystem.OnCloseInventory += CloseInventory;
        
        playerController.OnEncounter += OnEncounter;
        quizSystem.OnReplace += OnReplace;
        quizSystem.OnQuizEnd += OnQuizEnd;
        replaceSystem.OnReplaceComplete += OnReplaceComplete;

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };
        
        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.Explore;
        };
        
    }

    void OnEncounter()
    {
        state = GameState.Quiz;
        quizCanvas.SetActive(true);
        buttonCanvas.SetActive(false);
        overworldCamera.gameObject.SetActive(false);
        quizCamera.gameObject.SetActive(true);
    }
    void OnQuizEnd()
    {
        state = GameState.Explore;
        quizCanvas.SetActive(false);
        buttonCanvas.SetActive(true);
        quizCamera.gameObject.SetActive(false);
        overworldCamera.gameObject.SetActive(true);
    }
    void OnReplace()
    {
        state = GameState.Replace;
        buttonCanvas.SetActive(false);
        quizCanvas.SetActive(false);
        replaceSystem.gameObject.SetActive(true);
        quizCamera.gameObject.SetActive(false);
        replaceCamera.gameObject.SetActive(true);
    }
    void OnReplaceComplete()
    {
        state = GameState.Explore;
        buttonCanvas.SetActive(true);
        replaceCamera.gameObject.SetActive(false);
        overworldCamera.gameObject.SetActive(true);
        replaceSystem.gameObject.SetActive(false);
    }
    void OpenInventory()
    {
        state = GameState.Inventory;
        buttonCanvas.SetActive(false);
        overworldCamera.gameObject.SetActive(false);
        inventorySystem.gameObject.SetActive(true);
        quizCanvas.SetActive(false);
        inventoryCamera.gameObject.SetActive(true);
    }
    void CloseInventory()
    {
        state = GameState.Explore;
        buttonCanvas.SetActive(true);
        inventoryCamera.gameObject.SetActive(false);
        overworldCamera.gameObject.SetActive(true);
        
    }

    private void Update()
    {
        if (state == GameState.Explore)
        {
            playerController.HandleUpdate();
        }   
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Inventory)
        {
            inventorySystem.HandleUpdate();
        }
        else if (state == GameState.Quiz)
        {
            quizSystem.HandleUpdate();
        }
        else if (state == GameState.Replace)
        {
            replaceSystem.HandleUpdate();
        }
    }
}
