using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;

/**
 *  A class that manages firebase initialisation.
 */
public class FirebaseManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus; /** A firebase class used in initialisation */
    public FirebaseUser User; /** A firebase class that contains data of current user */
    public DatabaseReference DBreference; /** A firebase class that contains the reference to the database */
    public FirebaseAuth auth; /** A firebase class that contains the auth token */

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField; /** A TMPro class for email input */
    public TMP_InputField passwordLoginField; /** A TMPro class for password input */
    public TMP_Text warningLoginText; /** A TMPro class for error message text for login */
    public TMP_Text confirmLoginText; /** A TMPro class for success message text for login */

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField; /** A TMPro class for username input */
    public TMP_InputField emailRegisterField; /** A TMPro class for email input */
    public TMP_InputField passwordRegisterField; /** A TMPro class for password input */
    public TMP_InputField passwordRegisterVerifyField; /** A TMPro class for password confirmation input */
    public TMP_Text warningRegisterText; /** A TMPro class for error message text for register */

    //User Data variables
    [Header("UserData")]
    public TMP_InputField usernameField; /** A TMPro class for username input */

    public string userID; /** userID of currently logged in user */

    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();

            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
        DontDestroyOnLoad(this.gameObject);
    }

    /**
     * this method loads user data from firebase database based on the current logged in user.
     */
    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        //StartCoroutine(Login("user@ssad.com","123456"));
        //Debug.Log("doneeee");

    }
    /**
     * this method handles auth state change during sign out 
     */
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        var user = auth.CurrentUser;
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }
    /**
     * this method handles clearing login fields after login 
     */
    public void ClearLoginFeilds()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }
    /**
     * this method handles clearing register fields after register
     */
    public void ClearRegisterFeilds()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    /**
     * this method handles OnClick for login button 
     */
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    /**
     * this method handles OnClick for register button
     */
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }
    /**
     * this method handles OnClick for sign out button
     */
    public void SignOutButton()
    {
        auth.SignOut();
        SceneManager.LoadScene("LoginScene");
    }


    /**
     * this method handles login authentication with firebase methods
     * @param _email - email for the game 
     * @param _password - password for the game
     */
    private IEnumerator Login(string _email, string _password)
    {
        Debug.Log("In login");
        Debug.Log(_email);
        //Call the Firebase auth signin function passing the email and password
        Debug.Log("Starting calculation");
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes

        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        watch.Stop();
        Debug.Log("Calculation completed");
        Debug.Log("Time taken for login: " +watch.ElapsedMilliseconds +  "ms");

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            userID = auth.CurrentUser.UserId;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            Debug.Log("userID = " + userID);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
           

            yield return new WaitForSeconds(0.5f);

            if (_email == "rachel@ssad.com")
            {
                SceneManager.LoadScene("TeacherScene");
                usernameField.text = User.DisplayName;
                confirmLoginText.text = "";
                ClearLoginFeilds();
            }
            else{
            usernameField.text = User.DisplayName;
            UIManager.instance.MenuScreen();
            confirmLoginText.text = "";
            ClearLoginFeilds();
            //ClearRegisterFeilds();
            }
        }
    }
    /**
     * this method handles registering a new user with firebase methods
     * @param _email - email for the game
     * @param _password - password for the game
     * @param _username - username for the game
     */
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
                        //Now return to login screen
                        UIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                        ClearRegisterFeilds();
                        ClearLoginFeilds();
                    }
                }
            }
        }
    }
    /** this method handles updating the username in firebase authentication
     * @param _username - username for the game
     */
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
    /** this method handles updating the username in firebase database
     * @param _username - username for the game
     */
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
    /**
     * this method handles OnClick for play button
     */
    public void PlayButton()
    {
        SceneManager.LoadScene("StageSelect");
    }
    /**
     * this method handles OnClick for custom stage button
     */
    public void CustomStageButton()
    {
        SceneManager.LoadScene("CustomStage");
    }
    /**
     * this method handles OnClick for mastery report button
     */
    public void MasteryReportButton()
    {
        SceneManager.LoadScene("MasteryReport");
    }
    /**
     * this method handles OnClick for leaderboard button
     */
    public void LeaderboardButton()
    {
        SceneManager.LoadScene("Leaderboard");
    }
    /**
     * this method handles OnClick for teacher button
     */
    public void TeacherButton()
    {
        SceneManager.LoadScene("TeacherScene");
    }
  
}