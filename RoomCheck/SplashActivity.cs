using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;

namespace RoomCheck
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;
        private string activityName;
        private User loggedInUser;
        private SQLiteConnection db;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            Log.Debug(TAG, "SplashActivity.OnCreate");
            
        }

        protected override void OnResume()
        {
            base.OnResume();

            Task startupWork = new Task(() =>
            {
                Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
                Task.Delay(5000);  // Simulate a bit of startup work.
                //todo: use this area to bring all rooms down to sqlite db
                Log.Debug(TAG, "Working in the background - important stuff.");

                //TODO: CHECK IN SQLITE IF USER IS LOGGED IN AND LOAD THEIR ROOMS
                // create DB path
                var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");

                var result = createDatabase(pathToDatabase);
                db = new SQLiteConnection(pathToDatabase, true);
                
                //TESTING PURPOSES: delete user so i can test recreating the user
                //db.DeleteAll<User>();


                if (result != null)
                {
                    
                    var count = db.ExecuteScalar<int>("SELECT Count(*) FROM User");
                    if (count > 0)
                    {
                        loggedInUser = db.Query<User>("Select * from User").FirstOrDefault();
                        //may be able to get users rooms from mysql down to sqlite here
                    }
                }

            });

            startupWork.ContinueWith(t =>
            {
                Log.Debug(TAG, "Work is finished - start Activity.");
                activityName = Intent.GetStringExtra("Activity") ?? "";
                if (activityName == null || activityName == "")
                {
                    if(loggedInUser == null)
                    StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                    else
                        //todo: check sqlite db for user id at start of myroomsactivity
                        //todo: load rooms for that user into memory here
                        StartActivity(typeof(MyRoomsActivity));
                }
                else
                {
                    var actType = Type.GetType("RoomCheck." + activityName);
                    var intentPlace = new Intent(this, actType);
                    StartActivity(intentPlace);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
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
    }
}