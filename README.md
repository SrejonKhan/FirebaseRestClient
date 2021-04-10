# Firebase Rest Client

Lightweight Firebase Library, made on top of REST API.

Implement Firebase to any Project without importing any Firebase SDK. This Library comes with all major features of Firebase SDK, including Realtime Database, Authentication and others are coming soon.

# Installation

Open Package Manager in Unity and Click on Plus Icon -> Add package from git URL, paste following link `https://github.com/SrejonKhan/FirebaseRestClient.git` and click Add.

Other methods (Asset Store, UPM, Release Page) will be added later after a stable release.

After importing to your project, **Open Settings from (Edit -> Project Settings -> Firebase Rest Client) and set all required credentials.**

<p align="center">
  <img width="100%" src="Documentation/configuration_snap.png">
</p>

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

```csharp
//Initialize
var firebase = new RealtimeDatabase();
```

```csharp
/* -- Read -- */

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

```csharp
/* -- Push -- */
/*
 * Push functionality in this library is a bit different from actual Firebase SDK
 *
 * In this library, when Push() function called, it implicitly complete followings task -
 * 1. Generate a Push ID locally
 * 2. Write passed body to following path (childs + push id)
 * 3. Returns generated Push ID as string
 *
 * If you just want a Push ID, not directly writing,
 * you can call firebase.GeneratePushID(), which returns push id in string format.
*/

firebase.Child("users").Push(anyObject).OnSuccess(uid =>
{
    //Returns Push ID in string
    Debug.Log(uid);
});

firebase.Child("users").Push(jsonString).OnSuccess(uid =>
{
    //Returns Push ID in string
    Debug.Log(uid);
});
```

```csharp
/* -- Write -- */

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

```csharp
/* -- Update -- */

// Object as payload
firebase.Child("users").Child("123").Update(user).OnSuccess( () => { /*...Codes...*/ });

// Json string as payload
firebase.Child("users").Child("123").Update(jsonString);
```

```csharp
/* -- Remove -- */

firebase.Child("product").Child("orange").Remove().OnSuccess( () => { /*...Codes...*/ });

firebase.Child("product").Child("orange").Remove();
```

```csharp
/* -- Order -- */
/*
 * Order functions return a Json String.
 *
 * Remember, returned JSON isn't in order, as JSON interpreters don't enforce any ordering.
 *
 * Same applies to any filtered result.
 *
 * If you want to order, you have to order itself in application.
 * I'm planning to write a helper function to order json in future updates.
*/


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

```csharp
/* -- Order with Filters -- */
/*
 * Things to remember while filtering -
 *  1. Filter functions come before Order function. E.g StartAt("5").OrderByKey()
 *  2. Returned JSON isn't in order, as JSON interpreters don't enforce any ordering.
 *  3. Filter functions params are in string, if you pass 5 as string, it will count
       it as number. If you want to filter by string, pass string inside quoutes.
       E.g EqualTo(""\Orange"\")
 *
 * If you want to order, you have to order itself in application.
 * I'm planning to write a helper function to order json in future updates.
*/

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

```csharp
/* -- Generate Push ID Locally -- */

string pushId = firebase.GeneratePushID();
```

### Authentication

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

```csharp
//initialize
FirebaseAuthentication firebaseAuth = new FirebaseAuthentication();

firebaseAuth.CreateUserWithEmailAndPassword(email, password); //Password Signup

firebaseAuth.SignInWithEmailAndPassword(email, password); //Password Signin

firebaseAuth.SignInAnonymously(); //Anonymous SignIn

firebaseAuth.SignInWithOAuth("ACCESS_TOKEN_FROM_PROVIDER", "YOUR_PROVIDER_ID"); //OAuth SignIn

firebaseAuth.GoogleSignIn("AUTH_CODE_FROM_GOOGLE_TO_EXCHANGE_ACCESS_TOKEN"); //Google SignIn
firebaseAuth.FacebookSignIn("AUTH_CODE_FROM_FACEBOOK_TO_EXCHANGE_ACCESS_TOKEN"); //Facebook SignIn

firebaseAuth.SendPasswordResetEmail(email); //Password Reset Mail
firebaseAuth.SendEmailVerification(); //Email Verification (user must be signed in)
```

```csharp
/*
* Firebase User Actions
*/

FirebaseUser user = firebaseAuth.CurrentUser; //Current User automatically updates after any sign-in

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


/*
* Auth State Change Listening
*/

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

### Firebase Storage

- Upload File

```csharp
//initialize
var firebaseStorage = new FirebaseStorage();

//Upload from direct filepath
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

//Upload from byte array
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

//Progress are between 0~1

//Progress action can be passed as null argument, or simply without passing any.

firebaseStorage.Upload(filePath, "File_From_File_Path_Array", null);
firebaseStorage.Upload(filePath, "File_From_File_Path_Array");

```

More Features are being added on regular basis.

# Documentation

There will be a well written Documentation on a Stable Release and web-based Wiki in future.

# Known Limitations

This library is still under development. All limitations are being resolved one-by-one. Check [CHANGELOG](https://github.com/SrejonKhan/FirebaseRestClient/blob/main/CHANGELOG.md) for more details.
