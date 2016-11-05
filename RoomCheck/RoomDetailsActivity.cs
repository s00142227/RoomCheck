using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MySql.Data.MySqlClient;
using System.Data;

namespace RoomCheck
{
    [Activity(Label = "Room Details")]
    public class RoomDetailsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.RoomDetails);

            // Create your application here
            DBRepository dbr = new DBRepository();
            
            //might be better to store the current room object in the sqllite db from the previous view?
            //would still have to query a database here but it wouldn't be the cloud database...
            int roomID = Intent.GetIntExtra("RoomID", 0);
            //Toast.MakeText(this, roomID.ToString(), ToastLength.Short).Show();

            Room room = new Room();
            MySqlConnection con =
                   new MySqlConnection(
                       "Server=s00142227db.cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=kmorris;Password=s00142227;charset=utf8");

            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM RoomTbl where ID = @id;", con))
                    {
                        cmd.Parameters.AddWithValue("@id", roomID);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                room = new Room((int) reader["ID"], (string) reader["RoomNo"],
                                    (int) reader["RoomOccupiedStatusID"], (int) reader["RoomCleanStatusID"], (int)reader["RoomTypeID"]);
                            }
                        }
                    }
                }

                TextView txtRoomNo = FindViewById<TextView>(Resource.Id.txtRoomNo);
                TextView txtRoomType = FindViewById<TextView>(Resource.Id.txtRoomType);
                TextView txtRoomOccStatus = FindViewById<TextView>(Resource.Id.txtRoomOccupiedStatus);
                TextView txtRoomCleanStatus = FindViewById<TextView>(Resource.Id.txtRoomCleanStatus);
                EditText txtNote = FindViewById<EditText>(Resource.Id.txtNote);

                txtRoomType.Text = dbr.GetRoomTypeById(room.RoomTypeID);
                txtRoomCleanStatus.Text = dbr.GetCleanStatusById(room.CleanStatusID);
                txtRoomOccStatus.Text = dbr.GetOccupiedStatusById(room.OccupiedStatusID);
                txtRoomNo.Text = "Room " + room.RoomNo;

            }
            catch (MySqlException ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

        }
    }
}