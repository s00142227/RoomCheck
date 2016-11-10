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
using Android.Content.Res;

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

            Room room = dbr.GetRoomById(roomID);

            //Get all fields on screen
            TextView txtRoomNo = FindViewById<TextView>(Resource.Id.txtRoomNo);
            TextView txtRoomType = FindViewById<TextView>(Resource.Id.txtRoomType);
            TextView txtRoomOccStatus = FindViewById<TextView>(Resource.Id.txtRoomOccupiedStatus);
            TextView txtRoomCleanStatus = FindViewById<TextView>(Resource.Id.txtRoomCleanStatus);
            EditText txtNote = FindViewById<EditText>(Resource.Id.txtNote);
            ImageView imgRoomType = FindViewById<ImageView>(Resource.Id.imgRoomType);
            ImageView imgRoomOccStatus = FindViewById<ImageView>(Resource.Id.imgRoomOccupiedStatus);
            ImageView imgRoomCleanStatus = FindViewById<ImageView>(Resource.Id.imgRoomCleanStatus);

            //Get statuses
            RoomType roomType = dbr.GetRoomTypeById(room.RoomTypeID);
            RoomOccupiedStatus roomOccStatus = dbr.GetOccupiedStatusById(room.OccupiedStatusID);
            RoomCleanStatus roomCleanStatus = dbr.GetCleanStatusById(room.CleanStatusID);

            //Populate fields
            txtRoomType.Text = roomType.Description;
            txtRoomCleanStatus.Text = roomCleanStatus.Description;
            txtRoomOccStatus.Text = roomOccStatus.Description;
            txtRoomNo.Text = "Room " + room.RoomNo;
            txtNote.Text = room.Note;

            //Attempts at getting the resource id from the string stored in the database:

            //Attempt 1:
            //int resourceId = Resources.GetIdentifier(
            //   roomType.IconPath, "drawable", GetPackageName());
            //imgRoomType.SetImageResource(resourceId);

            //Attempt 2
            //int resImage = Resources.GetIdentifier(roomType.IconPath, "drawable", PackageName);

            //Attempt 3
            //int id = Resources.GetIdentifier("drawable/" + roomType.IconPath, null, null);

            //Attempt 4
            int roomTypeResourceId = (int)typeof(Resource.Drawable).GetField(roomType.IconPath).GetValue(null);
            imgRoomType.SetImageResource(roomTypeResourceId);

            int roomCleanResourceId = (int)typeof(Resource.Drawable).GetField(roomCleanStatus.IconPath).GetValue(null);
            imgRoomCleanStatus.SetImageResource(roomCleanResourceId);

            int roomOccupiedResourceId = (int)typeof(Resource.Drawable).GetField(roomOccStatus.IconPath).GetValue(null);
            imgRoomOccStatus.SetImageResource(roomOccupiedResourceId);

            Button btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnSave.Click += BtnSaveOnClick;

        }

        private void BtnSaveOnClick(object sender, EventArgs eventArgs)
        {
            //TODO: implement saving of the room object
        }
    }
}