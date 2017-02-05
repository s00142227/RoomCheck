using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MySql.Data.MySqlClient;
using System.Data;

namespace RoomCheck
{
    [Activity(Label = "RoomCheck", Icon = "@drawable/icon")]
    // MainLauncher = true, -- this was removed to allow for splash screen
    public class MainActivity : Activity
    {

        private EditText txtUsername, txtPassword;
        private Button btnInsert;
        private TextView txtSysLog;

        private Button btnSignUp;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //NEW CODE: ANDROID LOGIN/SIGN UP TUTORIAL 
            btnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);
            btnSignUp.Click += BtnSignUpOnClick;


            //OLD CODE FROM MYSQL TUTORIAL
            //txtUsername = FindViewById<EditText>(Resource.Id.txtUsername);
            //txtUsername.Text = "janedoe";
            //txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
            //txtPassword.Text = "password";
            //btnInsert = FindViewById<Button>(Resource.Id.btnInsert);
            //txtSysLog = FindViewById<TextView>(Resource.Id.txtSysLog);

            //btnInsert.Click += BtnInsert_Click;

            //Button btnShowALL = FindViewById<Button>(Resource.Id.btnShowRecords);
            //btnShowALL.Click += BtnShowALL_Click;

            //Button btnRoomList = FindViewById<Button>(Resource.Id.btnRoomList);
            //btnRoomList.Click += BtnRoomListOnClick;
        }

        private void BtnSignUpOnClick(object sender, EventArgs eventArgs)
        {
            //Pull up dialog
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            dialog_SignUp signUpDialog = new dialog_SignUp();
            signUpDialog.Show(transaction, "dialog fragment");

        }

        //private void BtnRoomListOnClick(object sender, EventArgs eventArgs)
        //{
        //    //StartActivity(typeof(MyRoomsActivity));
        //    var splashActivity = new Intent(this, typeof(SplashActivity));
        //    splashActivity.PutExtra("Activity", "MyRoomsActivity");
        //    StartActivity(splashActivity);
        //}

        private void BtnShowALL_Click(object sender, EventArgs e)
        {
            MySqlConnection con =
                   new MySqlConnection(
                       "Server=roomcheckaurora.cluster-cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=s00142227;Password=Lollipop12;charset=utf8");

            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    var result = "";
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM tblTest", con);
                    var reader = cmd.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        var someValue = reader["user"];

                        result += (someValue + "\n");
                    }

                    Toast.MakeText(this, result, ToastLength.Short).Show();

                }
            }
            catch (MySqlException ex)
            {
                txtSysLog.Text = ex.ToString();
            }
            finally
            {
                con.Close();
            }

        }

        private void BtnInsert_Click(object sender, EventArgs e)
        {
            MySqlConnection con =
                    new MySqlConnection(
"Server=roomcheckaurora.cluster-cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=s00142227;Password=Lollipop12;charset=utf8");
            try
            {
                
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO tblTest(user,pass) VALUES(@user, @pass)", con);
                    //txtSysLog.Text = "Successfully Connected";
                    cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@pass", txtPassword.Text);

                    cmd.ExecuteNonQuery();
                    txtSysLog.Text = "Data successfully inserted";

                }
            }
            catch (MySqlException ex)
            {
                txtSysLog.Text = ex.ToString();
            }
            finally
            {
                con.Close();
            }

           
        }
    }
}

