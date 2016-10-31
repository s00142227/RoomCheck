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
    [Activity(Label = "RoomCheck", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        private EditText txtUsername, txtPassword;
        private Button btnInsert;
        private TextView txtSysLog;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            txtUsername = FindViewById<EditText>(Resource.Id.txtUsername);
            txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
            btnInsert = FindViewById<Button>(Resource.Id.btnInsert);
            txtSysLog = FindViewById<TextView>(Resource.Id.txtSysLog);

            btnInsert.Click += BtnInsert_Click;

            Button btnShowALL = FindViewById<Button>(Resource.Id.btnShowRecords);
            btnShowALL.Click += BtnShowALL_Click;

            Button btnRoomList = FindViewById<Button>(Resource.Id.btnRoomList);
            btnRoomList.Click += BtnRoomListOnClick;
        }

        private void BtnRoomListOnClick(object sender, EventArgs eventArgs)
        {
            StartActivity(typeof(MyRoomsActivity));
        }

        private void BtnShowALL_Click(object sender, EventArgs e)
        {
            MySqlConnection con =
                   new MySqlConnection(
                       "Server=s00142227db.cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=kmorris;Password=s00142227;charset=utf8");

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
                        "Server=s00142227db.cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=kmorris;Password=s00142227;charset=utf8");

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

