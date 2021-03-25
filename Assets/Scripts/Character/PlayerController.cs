using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float walkSpeed;
    public int encounterRate=10;
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask grassLayer;
    public SpriteMask grassMask;
    public event Action OnOpenInventory;
    public event Action OnEncounter;
    private bool isMoving = false;
    private Vector2 input;
    private Vector3 targetPos;
    private string curMap;
    private string nextMap;
    private float[] exitCoords;
    public  GameObject playerDataManager;
    private Animator animator;
    
    private void Awake(){
        animator=GetComponent<Animator>();
    }
    
    public void Start(){
        playerDataManager = GameObject.FindGameObjectWithTag("DataManager");
        curMap = playerDataManager.GetComponent<DataManager>().getCurMap();
        nextMap = playerDataManager.GetComponent<DataManager>().getNextMap();
        exitCoords = playerDataManager.GetComponent<DataManager>().getExitCoords(curMap);   
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
        if (Math.Round(transform.position.x) == exitCoords[0] && (Math.Round(transform.position.y) == exitCoords[1])){
            ChangeStage(curMap,nextMap);
        }

        }
        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Space)){
            Interact();
        }
        if (Input.GetKeyDown(KeyCode.I)){
            animator.SetBool("isMoving", false);
            OnOpenInventory();
        }
        if (Input.GetKeyDown(KeyCode.Q)){
            animator.SetBool("isMoving", false);
            OnEncounter();
        }
        
        

    }

    void Interact(){
        var playerDirection = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var facingTile = transform.position + playerDirection;

        //Debug.DrawLine(transform.position, facingTile, Color.red, 0.5f);

        var npcCollider = Physics2D.OverlapCircle(facingTile,0.3f,interactableLayer);
        if (npcCollider != null){
            npcCollider.GetComponent<Interactable>()?.Interact();
        }
    }

    void ChangeStage(string exitStage, string entryStage)
    {
        var exitCoords = playerDataManager.GetComponent<DataManager>().getExitCoords(exitStage);
        if (Math.Round(transform.position.x) == exitCoords[0] && (Math.Round(transform.position.y) == exitCoords[1]))
        { 
            var entryCoords = playerDataManager.GetComponent<DataManager>().getEntryCoords(entryStage);
            targetPos = new Vector3(entryCoords[0], entryCoords[1], 0);
            this.transform.position = targetPos;
            CameraFollow CFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
            CFollow.ChangeBounds(GameObject.Find(entryStage).GetComponent<BoxCollider2D>());
        }
    }

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


    private bool isWalkable(Vector3 targetPos){
        if(Physics2D.OverlapCircle(targetPos, 0.05f, solidObjectsLayer|interactableLayer) != null){
            return false;
        }
        return true;
    }

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