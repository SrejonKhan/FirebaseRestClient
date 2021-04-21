## [0.6.9] - 21-04-2021

Google SignIn Standalone loopback enhanced and sdk implementation guide

### Added

- Google SignIn SDK implementation guide and loopback guide

### Changed

- Google SignIn loopback converted to a Function

## [0.6.8] - 12-04-2021

Fixed DisplayName and PhotoURL issue.

### Fixed

- Fixed DisplayName and PhotoURL not updating issue fixed.

## [0.6.7] - 12-04-2021

Update profile issue fixed.

### Fixed

- Update Profile refresh token and id token reassign issue fixed

### Changed

- ChangePassword() and ChangeEmail() function's callback changed to GeneralCallback.

## [0.6.6] - 11-04-2021

Small fixes.

### Fixed

- User UpdateProfile() function fixed

## [0.6.5] - 10-04-2021

Added authenticated request for both Firebase Realtime Database and Storage.

### Added

- Authenticated Request support for Realtime Database and Storage

### Fixed

- UWP UploadHandler null ref issue fixed

## [0.6.4] - 08-04-2021

Optimized HasChild function, LoadSettings enhanced.

### Changed

- HasChild function optimized. (RealtimeDatabase.cs)
- LoadSettings() funciton enhanced. (FirebaseSettings.cs)

## [0.6.3] - 08-04-2021

Core classes name changed to more meaningfull name.

### Changed

- Core class name changed
- Readme updated

### Added

- Upload Function overloaded with no progress params (Storage)

## [0.6.2] - 08-04-2021

All configuration is now directly editable from Project Settings and save all settings inside resource to load in runtime.

### Added

- Configuration migrated to ScriptableObject
- Configuration edit from Project Settings

## [0.6.1] - 07-04-2021

Storage upload respose download url fixed

### Fixed

- UploadRespose downloadUrl fixed

## [0.6.0] - 06-04-2021

Support for Firebase Storage added

### Added

- Upload file to Firebase Storage

## [0.5.9] - 05-04-2021

New Read Function to return object itself converted from Json response.

### Added

- New Read Function in Realtime Database

## [0.5.8] - 05-04-2021

Local PushID generation, Push function updated according to Push ID Generation.

### Added

- Local PushID Generation

## [0.5.7] - 05-04-2021

Raw Json Push function added, WriteAppend function added for key-value pair collection.

### Added

- Raw Json Push in Realtime Database
- WriteAppend function in Realtime Databe

## [0.5.6] - 04-04-2021

FetchProvidersForEmail added.

### Added

- FetchProvidersForEmail added in Authentication

## [0.5.5] - 04-04-2021

Send email verfication mail and Minor Fixes.

### Added

- Send email verificaiton email

### Fixed

- HasChild null reference issue fixed

## [0.5.4] - 02-04-2021

Independant HasChild functionality, read operation is nested and robustness in deserialization.

### Added

- MIT License added

### Changed

- Enhanced HasChild Function

## [0.5.3] - 02-04-2021

UpdateProfile issue fixed, HasChild function added.

### Added

- HasChild Function
- User object callback
- StateChanged event
- SignOut
- Responses to User Object (Internal Only)
- Key-Value Pair Writing to Realtime Database
- Efficient way to assign Filters

### Fixed

- UpdateProfile Json error

### Changed

- Namespace name changed

## [0.5.2] - 01-04-2021

User related Functions returns user object, namespace name changed to FirebaseRestClient, StateChanged event and minor changes.

### Added

- User object callback
- StateChanged event
- SignOut
- Responses to User Object (Internal Only)
- Key-Value Pair Writing to Realtime Database
- Efficient way to assign Filters

### Changed

- Namespace name changed

## [0.5.1] - 29-03-2021

Fixed Props names.

### Added

- CurrentUser and OnStateChanged props name change

## [0.5.0] - 29-03-2021

Initially added Realtime Database and Firebase Authentication (Experimental) support.

### Added

- Realtime Database CRUD Operations
- Realtime Database Ordering and Filtering
- Realtime Database Events
- Email-Password Authentication
- OAuth Authentication
- Google Authentication
- Facebook Authentication
- User Profile Email Change
- User Profile Password Change
- User Profile Link with Email or Providers
