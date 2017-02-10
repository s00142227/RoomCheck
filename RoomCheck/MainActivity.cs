using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Threading;
using SQLite;

namespace RoomCheck
{
    [Activity(Label = "RoomCheck", Icon = "@drawable/icon")]
    // MainLauncher = true, -- this was removed to allow for splash screen
    public class MainActivity : Activity
    {
        DBRepository dbr = new DBRepository();
        public List<User> users = new List<User>();

        private Button btnSignUp;
        private Button btnLogin;
        private ProgressBar prgBar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //NEW CODE: ANDROID LOGIN/SIGN UP TUTORIAL 
            btnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);
            btnSignUp.Click += BtnSignUpOnClick;

            prgBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            btnLogin = FindViewById<Button>(Resource.Id.btnSignIn);
            btnLogin.Click += BtnLoginOnClick;

        }

        private void BtnLoginOnClick(object sender, EventArgs eventArgs)
        {
            //Pull up dialog
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            dialog_SignIn signInDialog = new dialog_SignIn();
            signInDialog.Show(transaction, "dialog fragment");

            signInDialog.onSignInComplete += SignInDialogOnOnSignInComplete;
        }

        private void SignInDialogOnOnSignInComplete(object sender, OnSignInEventArgs e)
        {
            prgBar.Visibility = ViewStates.Visible;

            LogIn(e.Email, e.Password);
        }

        private void LogIn(string email, string password)
        {
            //todo: log user in and start the next activity
            users = dbr.GetAllUsersInfo();

            if (CheckUserCredentials(email, password))
            {

                User user = users.FirstOrDefault(u => u.Email == email);
                SaveToSQLite(user);
                
                
                //start new activity
                StartActivity(typeof(MyRoomsActivity));

            }
            else
            {
                Toast.MakeText(this, "Invalid email or password", ToastLength.Long).Show();
            }


            prgBar.Visibility = ViewStates.Invisible;

        }

        private void SaveToSQLite(User user)
        {
            //Save the logged in user to the sqlite database to be fetched on the next screen or next time
            //the app is opened
            var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");

            var result = createDatabase(pathToDatabase);
            var db = new SQLiteConnection(pathToDatabase, true);

            if (db.Insert(user) != 0)
                db.Update(user);

        }

        private string createDatabase(string path)
        {
            try
            {
                var connection = new SQLiteAsyncConnection(path);
                {
                    connection.CreateTableAsync<User>();
                    return "Database created";
                }
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
        }

        private bool CheckUserCredentials(string email, string password)
        {

            if (users.Any(u => u.Email == email))
            {
                User user = users.FirstOrDefault(u => u.Email == email);
                var salt = user.Salt;
                var passAttempt = Crypto.EncryptAes(password, "roomcheckpassword", salt);
                //these are the same when i look when debugging, do i need to convert to strings to compare?
                if (passAttempt.ToString() == user.Password.ToString())
                {
                    return true;
                }
                //passwords don't match
                return false;
            }
            //email doesnt exist
            return false;
        }

        private void BtnSignUpOnClick(object sender, EventArgs eventArgs)
        {
            //Pull up dialog
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            dialog_SignUp signUpDialog = new dialog_SignUp();
            signUpDialog.Show(transaction, "dialog fragment");

            signUpDialog.onSignUpComplete += SignUpDialogOnOnSignUpComplete;

        }

        private void SignUpDialogOnOnSignUpComplete(object sender, OnSignUpEventArgs e)
        {
            prgBar.Visibility = ViewStates.Visible;
            
            //here we can use e.HotelID etc fromt he dialog
            if (!dbr.CheckEmailExists(e.Email))
            {
                if (dbr.CheckHotelID(e.HotelID))
                {
                    //create new user in the database
                    dbr.CreateUser(e.Email, e.FirstName, e.Password, e.HotelID);
                }
                else
                {
                    Toast.MakeText(this, "Invalid Hotel ID", ToastLength.Long).Show();
                }
            }
            else
            {
                Toast.MakeText(this, "Email already exists", ToastLength.Long).Show();
            }

            prgBar.Visibility = ViewStates.Invisible;
        }

        

    }
}

