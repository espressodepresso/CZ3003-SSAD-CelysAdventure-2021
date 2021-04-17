using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Auth;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;
using System.Xml.Linq;
using System.IO;
/** 
 * A class for handling logic of the leaderboard 
 */
public class LeaderboardManager : MonoBehaviour
{
    public GameObject rowPrefab; /** A unity game object container that will contain one row of the leaderboard */
    public Transform rowsParent; /** A unity game object transform container that will contain all rows of the leaderboard */

    private GameObject firebasesManager; /** A firebase class that contains the script to FirebaseManager */
    public DatabaseReference DBreference; /** A firebase class that contains the reference to the database */
    public FirebaseAuth auth; /** A firebase class that contains the auth token */

    private string currentUserId; /** Current User id */
    private string currentPlayerName="default"; /** Current player name, initialised to default */

    public string filename; /** name of the image on local storage */
    public string path; /** path of the image on local storage */
    public string imageLink; /** online link to screenshot */

    public bool clickTwitter = false; /** A check for whether twitter or whatsapp was clicked */

    private Dictionary<string, int> leaderBoardData = new Dictionary<string, int>();  /** leaderboard data containing player name and score */

    public Dictionary<string, string> userIDList = new Dictionary<string,string>();  /** data mapping user id and player name */

    //twitter share link
    string twitterAddress = "http://twitter.com/intent/tweet"; /** link to twitter share */

    //language
    string tweetLanguage = "en";

    /*Whatsapp*/
    string whatsappAddress = "https://wa.me/";/** link to whatsapp share */


    /*message details*/

    //text to show session id
    string textToDisplay = string.Format("Hi! Check out my score on the leaderboard."); /** Text to display on social media share */

    private void Awake()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
        auth= firebasesManager.GetComponent<FirebaseManager>().auth;
        currentUserId = auth.CurrentUser.UserId.ToString();
        StartCoroutine(LoadLeaderboardData());

    }
    /** 
     * this method handles OnClick for twitter share button
     */
    public void ShareOnTwitter()
    {
        clickTwitter = true;
        StartCoroutine(captureScreenshot());
        
    }
    /** 
     * this method handles OnClick for whatsapp share button
     */
    public void ShareOnWhatsapp()
    {
        clickTwitter = false;
        StartCoroutine(captureScreenshot());

    }
    /** 
     * this method handles sharing the leaderboard data through the twitter link called by AppScreenshotUpload()
     */
    public void shareOnTwitterLeaderboard()
    {
        Application.OpenURL(twitterAddress + "?text=" + WWW.EscapeURL(textToDisplay + " " +imageLink)+ "&amp;lang" + WWW.EscapeURL(tweetLanguage));
    }
    /** 
     * this method handles sharing the leaderboard data through the whatsapp link called by AppScreenshotUpload()
     */
    public void shareOnWhatsAppLeaderboard()
    { 
        Application.OpenURL(whatsappAddress + "?text=" + WWW.EscapeURL(textToDisplay + " " + imageLink));
    }
    /** 
     * this method handles taking a screenshot of the current leaderboard and calls AppScreeshotUpload
     */
    IEnumerator captureScreenshot()
    {
        yield return new WaitForEndOfFrame();

        path = Application.persistentDataPath + "_0" + "_" + Screen.width + "X" + Screen.height + "" + ".png";

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
        //Get Image from screen
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();
        //Convert to png
        byte[] imageBytes = screenImage.EncodeToPNG();

        //Save image to file
        System.IO.File.WriteAllBytes(path, imageBytes);
        StartCoroutine(AppScreenshotUpload());
        Debug.Log("screen shot taken");
    }
    /** 
     * this method handles uploading the screenshot to imgur online storage and calls shareOnTwitterLeaderboard/shareOnWhatsAppLeaderboard
     */
    IEnumerator AppScreenshotUpload()
    {
        yield return new WaitForEndOfFrame();

        //Make sure that the file save properly
        float startTime = Time.time;
        while (false == File.Exists(path))
        {
            if (Time.time - startTime > 5.0f)
            {
                yield break;
            }
            yield return null;
        }

        //Read the saved file back into bytes
        byte[] rawImage = File.ReadAllBytes(path);

        //Before we try uploading it to Imgur we need a Server Certificate Validation Callback
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        //Attempt to upload the image
        using (var w = new WebClient())
        {
            string clientID = "fd24e2bcc4736ac";
            w.Headers.Add("Authorization", "Client-ID " + clientID);
            var values = new NameValueCollection
             {
                 { "image", Convert.ToBase64String(rawImage) },
                 { "type", "base64" },
             };

            byte[] response = w.UploadValues("https://api.imgur.com/3/image.xml", values);

            XDocument xml = XDocument.Load(new MemoryStream(response));
            IEnumerable<XElement> innerData = xml.Descendants("data").Elements();
          
      
          
            foreach (var xmlData in innerData)
            {
                
                if (xmlData.ToString().Contains("<link>"))
                {
          ;
                    string str = xmlData.ToString();
                    int startIndex = 6;
                    int endIndex = str.Length - 13;
                    string link = str.Substring(startIndex, endIndex);
                    imageLink = link;
                    if (clickTwitter)
                    {
                        shareOnTwitterLeaderboard();
                        clickTwitter = false;
                    } else
                    {
                        shareOnWhatsAppLeaderboard();
                    }
                    
                }
                
            }


        }
    }
    /** 
     * this method handles certificate validation process when uploading screenshot to imgur online storage
     */
    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }
    /**
     * this method loads the user's playername and their total scores from database
     */
    private IEnumerator LoadLeaderboardData()
    {
        var DBTask = DBreference.Child("users").GetValueAsync();
        

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
             
                string userName = childSnapshot.Child("playerName").Value.ToString();
                if (childSnapshot.Key.ToString() == currentUserId)
                {
                    currentPlayerName = userName;
                }
                int totalNoCorrect = int.Parse(childSnapshot.Child("totalNoCorrect").Value.ToString());
                leaderBoardData.Add(userName, totalNoCorrect);
                userIDList.Add(userName,childSnapshot.Key);
            }
            // sort the leaderboard
            // may need to consider what to do if data does not fit on the leaderboard, scroll view?
            var sortedDict = leaderBoardData.OrderByDescending(x => x.Value);
            int rank = 1;

            // display the rows on leaderboard
            foreach(KeyValuePair<string,int> pair in sortedDict)
            {
                Debug.Log(pair.Key + "-" + pair.Value);
                GameObject newEntry = Instantiate(rowPrefab, rowsParent);
                TextMeshProUGUI[] texts = newEntry.GetComponentsInChildren<TextMeshProUGUI>();
                if (SceneManager.GetActiveScene().name =="TeacherScene")
                {
                    newEntry.GetComponentInChildren<ButtonUsername>().setUsername(pair.Key);
                }
                newEntry.transform.tag = "Leaderboard";
                texts[0].text = rank.ToString();
                rank++;
                texts[1].text = pair.Key;
                if(pair.Key == currentPlayerName)
                {
                    texts[1].color = Color.red;
                    texts[1].fontStyle = FontStyles.Underline;
                }
                texts[2].text = pair.Value.ToString();
                
            }

        }
    }
    


}
