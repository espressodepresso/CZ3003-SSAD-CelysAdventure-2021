using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Auth;
using Firebase;
using UnityEngine;
using TMPro;
/** 
 * A class for sharing custom stage to social media
 */
public class ShareCustomStage : MonoBehaviour
{
    private GameObject firebasesManager; /** A firebase class that contains the script to FirebaseManager */
    public DatabaseReference DBreference; /** A firebase class that contains the reference to the database */
    public TMP_Dropdown dropdown;

    public TMP_Text quizString; /** A TMPro class displaying Custom Stage ID */
    //twitter share link
    string twitterAddress = "http://twitter.com/intent/tweet"; /** A twitter share link */

    //language
    string tweetLanguage = "en"; /** Language used */

    /*Whatsapp*/
    string whatsappAddress = "https://wa.me/"; /** A whatsapp share link */

    /*message details*/

    //text to show session id
    string textToDisplay = string.Format("Hi! I have created a custom world. To enter, use this session ID "); /** Text to show custom stage session id */

    public List<string> emailist = new List<string>(); /** a list of emails of users in the game */
    string chosenEmail = "default"; /** the chosen email, initialised to default */

    public string quizId; /**  Custom Stage ID */

    private void Awake()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
        StartCoroutine(LoadEmailData());
    }
   
    void Start()
    {
        quizId = GameObject.Find("UIManager").GetComponent<CustomStageUIManager>().quizId;
        quizString.text = "Successful creation of custom stage at id " + quizId;


    }
    /**
     * this method handles sharing custom stage id on twitter 
     */
    public void shareOnTwitter()
    {
        Application.OpenURL(twitterAddress + "?text=" + WWW.EscapeURL(textToDisplay) + quizId + "&amp;lang" + WWW.EscapeURL(tweetLanguage));
    }
    /** 
     * this method handles sharing custom stage id on whatsapp
     */
    public void shareOnWhatsApp()
    {
        Application.OpenURL(whatsappAddress + "?text=" + WWW.EscapeURL(textToDisplay) + quizId);
    }
    /**
     * this method handles loading email data of users from database
     */
    private IEnumerator LoadEmailData()
    {
        var DBTask = DBreference.Child("emails").GetValueAsync();


        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshots = DBTask.Result;


            foreach (var childSnapshot in snapshots.Children)
            {

                string email = childSnapshot.Child("email").Value.ToString();
                emailist.Add(email);

            }
            dropdown.AddOptions(emailist);

        }
    }

    /**
     * this method handles sharing custom stage id on email
     */
    public void shareOnEmail()
    {

        chosenEmail = emailist[dropdown.value];
        string msg = "mailto:"+chosenEmail;
        msg += "?subject=" + "Try this custom stage!";
        msg += "&body=" +"Dear Student, Please do this Custom Stage ID:" + quizId;
        Application.OpenURL(msg);
    }


}
