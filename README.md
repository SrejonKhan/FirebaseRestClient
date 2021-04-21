# Firebase Rest Client

Lightweight Firebase Library, made on top of REST API.

Implement Firebase to any Project without importing any Firebase SDK. This Library comes with all major features of Firebase SDK, including Realtime Database, Authentication, Storage and others are coming soon.

# Installation

Open Package Manager in Unity and Click on Plus Icon -> Add package from git URL, paste following link `https://github.com/SrejonKhan/FirebaseRestClient.git` and click Add.

Other methods (Asset Store, UPM, Release Page) will be added later after a stable release.

# Configuring Project

After importing to your project, **Open Settings from (Edit -> Project Settings -> Firebase Rest Client).** This is where we will set all api configuration.

<p align="center">
  <img width="100%" src="Documentation/Media/configuration_snap.png">
</p>

### Setup Firebase Project (Recommended)

1. Go to Google Cloud Platform and [Create a Project](https://console.cloud.google.com/projectcreate)
   ![step_zero](Documentation/Media/create_proj_auth_0.png)
2. Go to [Firebase Console](https://console.firebase.google.com/) and Create New Project. While creating project, **make sure to connect GCP Project** rather than creating new one.
   ![step_one](Documentation/Media/create_proj_auth_2.png)
3. After creating Firebase Project, enable your desired service(s) e.g Authentication / Realtime Database.
4. Remember, for **Web API Key**, you need to enable Authentication. Else, it will be empty.
   ![step_two](Documentation/Media/create_proj_auth_3.png)

This is recommended way to setup project if you're planning add authentication. If you're only planning to use Realtime Database with no authentication, try below procedures. Please keep in mind, if you connect your **Firebase Project with GCP Project**, there are somethings to remember. For instance, if you delete Firebase Project, it will delete your GCP project too.

### Setup Firebase Project (Shortcut)

1. Go to [Firebase Console](https://console.firebase.google.com/) and Create New Project. This time, no need to connect any GCP Project.
2. After creating Firebase Project, enable your desired service(s) e.g Authentication / Realtime Database.
3. Remember, for **Web API Key**, you need to enable Authentication. Else, it will be empty.

If you follow this procedure, and planning to add Google Sign-In SDK, you may face some problem with ClientId and Secret. It's better to follow recommended procedure to avoid any unwanted problem.

# Features

This library is so far supporting Realtime Database, Authentication and Storage from Firebase. Planning to add more support in future.

### Realtime Database

- Read
- RawRead (Json Format)
- Push
- Write
- Update
- Remove
- Child Events
  - OnValueChanged
  - OnChildAdded
  - OnChildRemoved
  - OnChildChanged
- Ordering
  - Order by Key
  - Order by Value
  - Order by Child value
- Filtering
  - StartAt
  - EndAt
  - EqualTo
  - LimitTo
- Locally generate Push ID

### Authentication (Experimental)

- Email-Password Authentication
  - Login and Registration
  - Forget Password Email
  - Email Verification Email
  - Change Email/Password
- Anonymous Authentication
- OAuth Login
  - Login with oauth token from any provider
- Google OAuth Desktop Flow (Loopback)
- User Profile Actions (Display Name, Photo URL)
  - Change/Add Display Name
  - Change/Add Photo URL
  - Get full profile

### Firebase Storage

- Upload File

More Features are being added on regular basis.

# Example

## Realtime Database

### Initialize

```csharp
var firebase = new RealtimeDatabase();
```

### Read

```csharp
firebase.Child("users").Read<User>().OnSuccess(result =>
{
    //Returns result in Dictionary<string,User> (Dictionary<string,T>)
    //item.value.id is a property of user class
    foreach (var item in result)
        Debug.Log($"Key: {item.Key} - Value: {item.Value.id}\n");
}).
OnError(error =>
{
    Debug.LogError(error.Message);
});

firebase.Child("scores").Read().OnSuccess(result =>
{
    //Returns result in Dictionary<string,string>
    foreach (var item in result)
        Debug.Log($"Name: {item.Key} - score: {item.Value}\n");
});

firebase.Child("product").Child("orange").RawRead().OnSuccess(result =>
{
    //Returns result in Json string
    Debug.Log(result);
});
```

### Push

Push functionality in this library is a bit different from actual Firebase SDK. In this library, when Push() function called, it implicitly complete followings task -

1. Generate a Push ID locally
2. Write passed body to following path (childs + push id)
3. Returns generated Push ID as string

If you just want a Push ID, not directly writing, you can call `firebase.GeneratePushID()`, which returns push id in string format.

```csharp
var userObject = new User("DefaultName");
firebase.Child("users").Push(userObject).OnSuccess(uid =>
{
    //Returns Push ID in string
    Debug.Log(uid);
});

string jsonString = JsonUtility.ToJson(userObject);
firebase.Child("users").Push(jsonString).OnSuccess(uid =>
{
    //Returns Push ID in string
    Debug.Log(uid);
});
```

### Write

```csharp
// Object as payload
firebase.Child("product").Child("orange").Write(anyObject).OnSuccess( () =>
{
    Debug.Log("Successfully written.");
});

// Key and Value Pair, suitable for key-value pair leaderboard or similar.
firebase.Child("leaderboard").Write("player123", "123");

// Json string as Payload
firebase.Child("product").Child("orange").Write(jsonString);
```

### Update

```csharp
// Object as payload
firebase.Child("users").Child("123").Update(user).OnSuccess( () => { /*...Codes...*/ });

// Json string as payload
firebase.Child("users").Child("123").Update(jsonString);
```

### Remove

```csharp
firebase.Child("product").Child("orange").Remove().OnSuccess( () => { /*...Codes...*/ });

firebase.Child("product").Child("orange").Remove();
```

### Order

Order functions return a Json String. **Remember, returned JSON isn't in order, as JSON interpreters don't enforce any ordering.** Same applies to any filtered result.

If you want to order, you have to order itself in application. I'm planning to write a helper function to order json in future updates.

```csharp
firebase.Child("users").OrderByKey().OnSuccess(json =>
{
    //returns json string
});

firebase.Child("leaderboard").OrderByValue().OnSuccess(json =>
{
    //returns json string
});

firebase.Child("users").OrderByChild("id").OnSuccess(json =>
{
    //returns json string
});
```

### Order with Filters

Things to remember while filtering -

1. Filter functions come before Order function. E.g `StartAt("5").OrderByKey()`
2. Returned JSON isn't in order, as JSON interpreters don't enforce any ordering.
3. Filter functions params are in string, if you pass `5` as string, it will count
   it as number. If you want to filter by string, pass string inside quoutes.
   E.g `EqualTo(""\Orange"\")`

If you want to order, you have to order itself in application. I'm planning to write a helper function to order json in future updates.

```csharp
//StartAt
firebase.Child("users").StartAt("5").OrderByKey().OnSuccess(json =>
{
    //returns json string
});

//EndAt
firebase.Child("users").EndAt("125").OrderByKey().OnSuccess(json =>
{
    //returns json string
});

//LimitToFirst
firebase.Child("users").LimitToFirst("25").OrderByChild("id").OnSuccess(json =>
{
    //returns json string
});


//LimitToLast
firebase.Child("users").LimitToLast("25").OrderByChild("id").OnSuccess(json =>
{
    //returns json string
});

//EqualTo
firebase.Child("users").EqualTo("\"John Doe\"").OrderByChild("name").OnSuccess(json =>
{
    //returns json string
});
```

### Generate Push ID Locally

```csharp
string pushId = firebase.GeneratePushID();
```

## Authentication

### Initialize

```csharp
FirebaseAuthentication firebaseAuth = new FirebaseAuthentication();
```

### Account Actions

```csharp
firebaseAuth.CreateUserWithEmailAndPassword(email, password); //Password Signup

firebaseAuth.SignInWithEmailAndPassword(email, password); //Password Signin

firebaseAuth.SignInAnonymously(); //Anonymous SignIn

firebaseAuth.SignInWithOAuth("ACCESS_TOKEN_FROM_PROVIDER", "YOUR_PROVIDER_ID"); //OAuth SignIn

firebaseAuth.GoogleSignIn("AUTH_CODE_FROM_GOOGLE_TO_EXCHANGE_ACCESS_TOKEN"); //Google SignIn
firebaseAuth.FacebookSignIn("AUTH_CODE_FROM_FACEBOOK_TO_EXCHANGE_ACCESS_TOKEN"); //Facebook SignIn

firebaseAuth.SendPasswordResetEmail(email); //Password Reset Mail
firebaseAuth.SendEmailVerification(); //Email Verification (user must be signed in)
```

### User Actions

```csharp
FirebaseUser user = firebaseAuth.CurrentUser; //Current User automatically updates after any sign-in actions

user.ChangeEmail(newEmail); //Change Email
user.ChangePassword(newPassword); //Change Password

user.UpdateProfile("NEW_DISPLAY_NAME", "PHOTO_URL"); //Update Profile

user.Reload(); //Refresh User
user.RefreshAccessToken(); //Force to refresh token
user.Delete(); //Delete user

//User props
string displayName = user.DisplayName;
string userEmail = user.Email;
bool isEmailVerified = user.IsEmailVerified;
bool isAnonymous = user.IsAnonymous;
string photoUrl = user.PhotoUrl;
string provider = user.Provider;

firebaseAuth.SignOut(); //Sign out any signed-in user
```

### Auth State Change Listening

```csharp
firebaseAuth.StateChanged += AuthStateChanged;

// Track state changes of the auth object.
void AuthStateChanged(object sender, System.EventArgs eventArgs)
{
    if (firebaseAuth.CurrentUser != user)
    {
        bool signedIn = user != firebaseAuth.CurrentUser && firebaseAuth.CurrentUser != null;
        if (!signedIn && user != null)
        {
            Debug.Log("Signed out " + user.LocalId);
        }
        user = firebaseAuth.CurrentUser;
        if (signedIn)
        {
            Debug.Log("Signed in " + user.LocalId);
        }
    }
}
```

## Storage

### Initialize

```csharp
var firebaseStorage = new FirebaseStorage();
```

### Upload from Direct Filepath

```csharp
string filePath = @"D:\Download\audacity-win-2.4.2.exe";

firebaseStorage.Upload(filePath, "File_From_File_Path_Array", progress =>
{
    Debug.Log(progress);
}).
OnSuccess(res =>
{
    //res = UploadResponse
    Debug.Log(res.downloadUrl);
}).
OnError(err => Debug.LogError(err.Message));
```

### Upload from Byte Array

```csharp
byte[] data = File.ReadAllBytes(filePath);

firebaseStorage.Upload(data, "File_From_Byte_Array", progress =>
{
    Debug.Log(progress);
}).
OnSuccess(res =>
{
    //res = UploadResponse
    Debug.Log(res.downloadUrl);
});
```

### Progress

Progress are between 0~1. Progress action can be passed as null argument, or simply without passing any.

```csharp
firebaseStorage.Upload(filePath, "File_From_File_Path_Array", null);
firebaseStorage.Upload(filePath, "File_From_File_Path_Array");
```

# Implementing Auth SDK

## Google Sign-In SDK (Android/IOS)

1. Download [Google SignIn SDK](https://github.com/googlesamples/google-signin-unity/releases/tag/v1.0.4) and [Unity Jar Resolver](https://github.com/googlesamples/unity-jar-resolver/releases). When importing SignIn SDK, make sure to exclude **Parse** & **PlayServicesResolver** folder. After importing SignIn Sdk, Import `external-dependency-manager-latest.unitypackage` from Unity Jar Resolver.
   ![configure_image](Documentation/Media/google_configure_project_0.png)

2. Go to [Configure Project](https://developers.google.com/identity/sign-in/android/start#configure-a-google-api-project) and configure your desired project.
   ![configure_image](Documentation/Media/google_configure_project.png)

3. Make sure Android is selected and exact Package Name and SHA-1 cert provided same as your project Package Name. For creating Keystore, use Unity's default [Keystore Manager](https://docs.unity3d.com/Manual/android-keystore-manager.html) and for generating SHA-1 Certificate, use [Keystore Explorer](https://keystore-explorer.org/) or command.  
   ![configure_image_1](Documentation/Media/google_configure_project_1.png)

4. Go to [Credential Tab](https://console.cloud.google.com/apis/credentials) in GCP and click on **Web client (Auto-created for Google Sign-in)** in OAuth 2.0 Client IDs Section. ![configure_image_1](Documentation/Media/google_configure_project_3.png)

5. Add **http://127.0.0.1:5050** in Authorized Redirect URIs.
   ![configure_image_1](Documentation/Media/google_configure_project_4.png)

6. Copy **Client ID** and **Client Secret**, paste them in **Firebase Configuration (Edit -> Project Settings -> Firebase Rest Client).** ![configure_image_1](Documentation/Media/google_configure_project_5.png)

7. Enable Google SignIn Method in Firebase Authentication. And copy **Client ID** and **Client Secret** from GCP (Step 4) and Paste them in **Web SDK Configuration**. ![configure_image_1](Documentation/Media/google_configure_project_6.png)

8. Invoke this code when necessery.

   ```csharp
   GoogleSignIn.Configuration = new GoogleSignInConfiguration
   {
       RequestIdToken = true,
       // Copy this ClientID from GCP OAuth Client IDs(Step 4).
       WebClientId = "723306904970-2kdkej7jcucl2b8kktpvivucov6t0r76.apps.googleusercontent.com",
       RequestAuthCode = true,
       AdditionalScopes = new List<string>() { "https://www.googleapis.com/auth/userinfo.email" } //Scope for Email
   };

   //Google Sign-In SDK
   Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

   signIn.ContinueWith(task =>
   {
       if (task.IsCanceled)
       {
           Debug.LogError("Cancelled");
       }
       else if (task.IsFaulted)
       {
           Debug.LogError(task.Exception.Message);

           using (IEnumerator<System.Exception> enumerator =
               task.Exception.InnerExceptions.GetEnumerator())
           {
               if (enumerator.MoveNext())
               {
                   GoogleSignIn.SignInException error =
                           (GoogleSignIn.SignInException)enumerator.Current;
                   Debug.LogError("Got Error: " + error.Status + " " + error.Message);
               }
               else
               {
                   Debug.LogError("Got Unexpected Exception?!?" + task.Exception);
               }
           }
       }

       //No Error
       else
       {
           string authCode = task.Result.AuthCode; //Auth Code

           //Signing-in to Firebase Auth
           firebaseAuth.GoogleSignIn(authCode).OnSuccess(user =>
           {
               string resulText = $"\n " +
               $"Email: {user.Email}, \n " +
               $"Local ID : {user.LocalId}, \n " +
               $"Provider : {user.Provider}, \n " +
               $"Is Anon : {user.IsAnonymous}, \n " +
               $"Display Name : {user.DisplayName}";

               Debug.Log(resulText);
           }).
           OnError(error =>
           {
               Debug.Log(error.Message);
           });
       }
   });
   ```

9. DONE!

## Google Sign-In (Standalone)

1. Skip if you have already configured GCP Project. If you're planning to implement Google Sign-In **both for Android/IOS and Standalone**, follow previous segment to configure project. If you're only planning for Desktop Sign-In, follow all steps below-
   - Go to [Credential Tab](https://console.cloud.google.com/apis/credentials) in GCP and click on **Create Credentials -> OAuth Client ID**
   - Select **Web Application** as Application Type.
   - Give meaningful name
   - Add `http://127.0.0.1:5050` to Authorized redirect URIs.
   - Click Create
2. Go back to [Credential Tab](https://console.cloud.google.com/apis/credentials) in GCP and click on previously created Client ID in OAuth 2.0 Client IDs Section. ![configure_image_1](Documentation/Media/google_configure_project_3.png)
3. Copy **Client ID** and **Client Secret**, paste them in **Firebase Configuration (Edit -> Project Settings -> Firebase Rest Client).** You can either get ID and Secret from Firebase or GCP. ![configure_image_1](Documentation/Media/google_configure_project_5.png)
4. Enable Google SignIn Method in Firebase Authentication. And copy **Client ID** and **Client Secret** from GCP (Step 3) and Paste them in **Web SDK Configuration**. ![configure_image_1](Documentation/Media/google_configure_project_6.png)
5. Invoke this code when necessary.

   ```csharp
   firebaseAuth.GoogleSignInLoopback().OnSuccess(user =>
   {
       string resulText = $"\n " +
       $"Email: {user.Email}, \n " +
       $"Local ID : {user.LocalId}, \n " +
       $"Provider : {user.Provider}, \n " +
       $"Is Anon : {user.IsAnonymous}, \n " +
       $"Display Name : {user.DisplayName}";

       Debug.Log(resulText);
   }).
   OnError(error =>
   {
       Debug.Log(error.Message);
   });
   ```

6. Done!

# Documentation

There will be a well written Documentation on a Stable Release and web-based Wiki in future.

# Known Limitations

This library is still under development. All limitations are being resolved one-by-one. Check [CHANGELOG](https://github.com/SrejonKhan/FirebaseRestClient/blob/main/CHANGELOG.md) for more details.
