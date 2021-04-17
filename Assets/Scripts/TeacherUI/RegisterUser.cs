using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
public class RegisterUser : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    private GameObject firebasesManager;
    public DatabaseReference DBreference;
    public FirebaseAuth auth;
    public FirebaseUser User;

    private void Awake()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        //Debug.Log("Heredeath");
        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
        auth = firebasesManager.GetComponent<FirebaseManager>().auth;
    }
    public void RegisterUserToFirebase()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }
    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        warningRegisterText.text = "User has been registered successfully";
                        ClearRegisterFeilds();
                        Debug.Log(User.UserId);
                        //write default paramaters for new users
                        StartCoroutine(writeUserData(User.UserId, _username, _email));
                     
                        
                    }
                }
            }
        }
    }
    private IEnumerator writeUserData(string userId, string playerName, string email)
    {
        PlayerInfo defaultPlayerInfo = new PlayerInfo(playerName, "Background_1", "Background_Gym", "1_easy", "Cely", 0, 0, 0);
        string player_json = JsonConvert.SerializeObject(defaultPlayerInfo);
        var DBTask = DBreference.Child("users").Child(userId).SetRawJsonValueAsync(player_json);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            StartCoroutine(writeEmailData(userId, email));
           
        }

    }
    private IEnumerator writeEmailData(string userId, string email)
    {
        var EmailTask = DBreference.Child("emails").Child(userId).Child("email").SetValueAsync(email);

        yield return new WaitUntil(predicate: () => EmailTask.IsCompleted);

        if (EmailTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {EmailTask.Exception}");
        }
        else
        {
            //Database username is now updated
            Debug.Log("new user is registered correctly");
        }
    }

    public void ClearRegisterFeilds()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = User.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

}
