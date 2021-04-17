using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! A class that handles all behaviour regarding the player character.
public class PlayerController : MonoBehaviour
{

    /**Variable for the movement speed of the player */
    public float walkSpeed;
    /** Chance the player will encounter a pokemon in cgrass*/
    public int encounterRate=10;
    /** Reference to the SolidObjects tile layer in unity*/
    public LayerMask solidObjectsLayer;
    /** Reference to the Inteactable tile layer in unity*/
    public LayerMask interactableLayer;

    /** Reference to the Grass tile layer in unity*/
    public LayerMask grassLayer;
    /** Reference to the mask to cover the bottom half of the player while in grass */
    public SpriteMask grassMask;
    /**Event trigger for opening inventory in GameController */
    public event Action OnOpenInventory;
    /** Event trigger for encountering a pokemon in GameController */
    public event Action OnEncounter;
    private bool isMoving = false;
    private Vector2 input;
    private Vector3 targetPos;
    private string curMap;
    private string nextMap;
    private float[] exitCoords;
    public  GameObject playerDataManager;
    public GameObject quizManager;
    private Animator animator;
    public float previousCoords_x;
    public float previousCoords_y;
    public string previousLocation;
    
    private void Awake(){
        animator=GetComponent<Animator>();
        
    }
    /**
    * Initilizes all the data and game objects
    * Sets the default player information
    */
    public void Start(){
        playerDataManager = GameObject.FindGameObjectWithTag("DataManager");
        quizManager = GameObject.FindGameObjectWithTag("QuizManager");
        GameObject playerCustomManager = GameObject.FindGameObjectWithTag("CustomStage");
        
        playerDataManager.GetComponent<DataManager>().ResetCats();
        StartCoroutine(playerDataManager.GetComponent<DataManager>().SaveUserDataToFirebase());

        if (playerCustomManager.GetComponent<PlayCustomStage>().customStageCheck)
        {
            string background = playerCustomManager.GetComponent<PlayCustomStage>().getcustomBackground();
            TeleportToCustomStage(background);
            playerDataManager.GetComponent<DataManager>().SetCurStage("1_easy");    
        }
        //only for testing
        else
        {
            playerDataManager.GetComponent<DataManager>().SetCurMap("Background_1");
            playerDataManager.GetComponent<DataManager>().SetNextMap("Background_Gym");
        }
        //end of testing only parts
        curMap = playerDataManager.GetComponent<DataManager>().getCurMap();
        nextMap = playerDataManager.GetComponent<DataManager>().getNextMap();
        exitCoords = playerDataManager.GetComponent<DataManager>().getExitCoords(curMap);
        Debug.Log("curmap: " + curMap);
        Debug.Log("nextmap: " + nextMap);
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
    if(!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
        
            if (input.x !=0) input.y=0; 

            if(input != Vector2.zero)
            {
                animator.SetFloat("moveX",input.x);
                animator.SetFloat("moveY",input.y);

                targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;
                if (isWalkable(targetPos)){
                    StartCoroutine(Move(targetPos));
                }
            }
            if (Math.Round(transform.position.x) == exitCoords[0] && (Math.Round(transform.position.y) == exitCoords[1]))
            {
                ChangeStage(curMap,nextMap);
                UpdateMaps();
                if (this.curMap == "Background_Gym")
                {
                    animator.SetBool("isMoving", false);
                    OnEncounter();
                }
            }

        }
        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Space)){
            Interact();
        }
        if (Input.GetKeyDown(KeyCode.I)){
            animator.SetBool("isMoving", false);
            OpenInventory();
        }
        if (Input.GetKeyDown(KeyCode.Q)){
            animator.SetBool("isMoving", false);
            OnEncounter();
        }
        //Debug.Log(Math.Round(transform.position.x)+","+ Math.Round(transform.position.y));

    }

    /** 
    * Trigger for the OnOpenInventory Event
    */
    public void OpenInventory()
    {
        OnOpenInventory();
    }
    /** 
    * Checks if the player is facing an NPC and triggers dialog with the PC
    */
    void Interact(){
        var playerDirection = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var facingTile = transform.position + playerDirection;

        //Debug.DrawLine(transform.position, facingTile, Color.red, 0.5f);

        var npcCollider = Physics2D.OverlapCircle(facingTile,0.3f,interactableLayer);
        if (npcCollider != null){
            npcCollider.GetComponent<Interactable>()?.Interact();
        }
    }
    /**
     * Changes stage of player when player reaches exit coords of map
     * 
     * @param exitStage name of map to be left
     * @param entryStage name of new map
     */
    public void ChangeStage(string exitStage, string entryStage)
    {
        this.previousCoords_x = (float)Math.Round(transform.position.x, 2);
        this.previousCoords_y = (float)Math.Round(transform.position.y, 2);
        this.previousLocation = playerDataManager.GetComponent<DataManager>().getCurMap();
        var exitCoords = playerDataManager.GetComponent<DataManager>().getExitCoords(exitStage);
        if (Math.Round(transform.position.x) == exitCoords[0] && (Math.Round(transform.position.y) == exitCoords[1]))
        { 
            var entryCoords = playerDataManager.GetComponent<DataManager>().getEntryCoords(entryStage);
            targetPos = new Vector3(entryCoords[0], entryCoords[1], 0);
            TeleportToLocation(targetPos, entryStage);
            quizManager.GetComponent<QuizManager>().LoadQuestions();
        }
    }
    /**
     * Teleports player to new location.
     * 
     * @param targetlocation Vector3 of new location
     * @param entryStage name of new map
     */
    public void TeleportToLocation(Vector3 targetLocation, string entryStage)
    {
        this.transform.position = targetLocation;
        CameraFollow CFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        CFollow.ChangeBounds(GameObject.Find(entryStage).GetComponent<BoxCollider2D>());
    }
    /**
     * Teleports player back to entrance of gym when the quiz fail condition is triggered.
     * 
     * @param targetlocation Vector3 of new location
     * @param entryStage name of new map
     */
    public void ChangeStageQuizFail(Vector3 targetLocation, string entryStage)
    {
        targetLocation.y -= 1;
        TeleportToLocation(targetLocation, entryStage);
        string nextMap = playerDataManager.GetComponent<DataManager>().getCurMap();
        playerDataManager.GetComponent<DataManager>().SetCurMap(this.previousLocation);
        playerDataManager.GetComponent<DataManager>().SetNextMap(nextMap);

        StartCoroutine(UpdateMapsQuizFail());

        //Debug.Log("curmap" + this.curMap);
        //Debug.Log("nextmap" + this.nextMap);
        //Debug.Log("exit coords: " + this.exitCoords[0] + "," + this.exitCoords[1]);

        //Debug.Log("data manager current map " + playerDataManager.GetComponent<DataManager>().getCurMap());
    }
    /**
     * Teleports player to custom stage.
     * 
     * @param newStage name of new stage
     */
    void TeleportToCustomStage(string newStage)
    {
        var entryCoords = playerDataManager.GetComponent<DataManager>().getEntryCoords(newStage);
        targetPos = new Vector3(entryCoords[0], entryCoords[1], 0);
        TeleportToLocation(targetPos, newStage);

        playerDataManager.GetComponent<DataManager>().SetCurMap(newStage);
        playerDataManager.GetComponent<DataManager>().SetNextMap("Background_Gym");

        exitCoords = playerDataManager.GetComponent<DataManager>().getExitCoords(newStage);


        //quizManager.GetComponent<QuizManager>().LoadQuestions();

    }
    /**
     * Updates PlayerController with latest status of player curMap, nextMap, and exitCoords.
     */
    void UpdateMaps()
    {
        playerDataManager.GetComponent<DataManager>().UpdateMapStatus();
        this.curMap = playerDataManager.GetComponent<DataManager>().getCurMap();
        this.nextMap = playerDataManager.GetComponent<DataManager>().getNextMap();
        this.exitCoords = playerDataManager.GetComponent<DataManager>().getExitCoords(curMap);
    }
    /**
     * Updates PlayerController with latest status of player curMap, nextMap, and exitCoords in the situation where player has failed
     * the quiz.
     */
    public IEnumerator UpdateMapsQuizFail()
    {
        yield return new WaitForSeconds(1);
        this.curMap = playerDataManager.GetComponent<DataManager>().getCurMap();
        this.nextMap = playerDataManager.GetComponent<DataManager>().getNextMap();
        this.exitCoords = playerDataManager.GetComponent<DataManager>().getExitCoords(curMap);
        
    }

    /**
    * Moves the player to the target position
    *
    * @param targetPos the xyz position after movement
    */
    IEnumerator Move(Vector3 targetPos)
    {   
        isMoving=true;
        while ((targetPos-transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position,targetPos,walkSpeed*Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving=false;
        EncounterCheck();
    }

    /**
    * Checks if the players next position is valid
    *
    * @param targetPos the position the player should move to
    */
    private bool isWalkable(Vector3 targetPos){
        if(Physics2D.OverlapCircle(targetPos, 0.05f, solidObjectsLayer|interactableLayer) != null){
            return false;
        }
        return true;
    }

    /**
    * Checks if the player is on a grass tile
    * Tiggers encounter based on the encounterRate if player is on grass
    */
    private void EncounterCheck(){
        if(Physics2D.OverlapCircle(transform.position,0.2f,grassLayer) != null){
            
            grassMask.enabled = true;

            if (UnityEngine.Random.Range(1,101) <= encounterRate){
                animator.SetBool("isMoving", false);
                OnEncounter();
            }
        }
        else
        {
            grassMask.enabled = false;
        }
    }
    


}