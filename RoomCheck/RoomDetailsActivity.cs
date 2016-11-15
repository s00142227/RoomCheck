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
using Object = Java.Lang.Object;

namespace RoomCheck
{
    [Activity(Label = "Room Details")]
    public class RoomDetailsActivity : Activity
    {
        public static DBRepository dbr = new DBRepository();
        public static JavaList<RoomCleanStatus> cleanstatuses;
        public static int roomID;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.RoomDetails);

            // Create your application here
           

            //might be better to store the current room object in the sqllite db from the previous view?
            //would still have to query a database here but it wouldn't be the cloud database...
            roomID = Intent.GetIntExtra("RoomID", 0);

            Room room = dbr.GetRoomById(roomID);

            //Get all fields on screen
            TextView txtRoomNo = FindViewById<TextView>(Resource.Id.txtRoomNo);
            TextView txtRoomType = FindViewById<TextView>(Resource.Id.txtRoomType);
            TextView txtRoomOccStatus = FindViewById<TextView>(Resource.Id.txtRoomOccupiedStatus);
            EditText txtNote = FindViewById<EditText>(Resource.Id.txtNote);
            ImageView imgRoomType = FindViewById<ImageView>(Resource.Id.imgRoomType);
            ImageView imgRoomOccStatus = FindViewById<ImageView>(Resource.Id.imgRoomOccupiedStatus);
            TextView txtRoomClean = FindViewById<TextView>(Resource.Id.txtCleanStatus);
            

            //populate the spinner 
            Spinner sprRoomClean = FindViewById<Spinner>(Resource.Id.sprRoomClean);
            cleanstatuses = dbr.GetAllCleaningStatuses();
            RoomCleanSpinnerAdapter adapter = new RoomCleanSpinnerAdapter(this,cleanstatuses);
            sprRoomClean.Adapter = adapter;
            sprRoomClean.ItemSelected += SprRoomCleanOnItemSelected;

          

            //Get statuses
            RoomType roomType = dbr.GetRoomTypeById(room.RoomTypeID);
            RoomOccupiedStatus roomOccStatus = dbr.GetOccupiedStatusById(room.OccupiedStatusID);
            RoomCleanStatus roomCleanStatus = dbr.GetCleanStatusById(room.CleanStatusID);

            //Populate fields
            txtRoomType.Text = roomType.Description;
            txtRoomOccStatus.Text = roomOccStatus.Description;
            txtRoomNo.Text = "Room " + room.RoomNo;
            txtNote.Text = room.Note;
            txtRoomClean.Text = roomCleanStatus.Description;


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

            int roomOccupiedResourceId = (int)typeof(Resource.Drawable).GetField(roomOccStatus.IconPath).GetValue(null);
            imgRoomOccStatus.SetImageResource(roomOccupiedResourceId);

            Button btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnSave.Click += BtnSaveOnClick;

        }

        private void SprRoomCleanOnItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            TextView txtRoomClean = FindViewById<TextView>(Resource.Id.txtCleanStatus);
            txtRoomClean.Text = cleanstatuses[e.Position].Description;
        }

        private void BtnSaveOnClick(object sender, EventArgs eventArgs)
        {
            //TODO: get the room id, cleaning status and note and then update database and close the view (and/or show a popup)
            
        }

        public class RoomCleanSpinnerAdapter : BaseAdapter<RoomCleanStatus>
        {
            private Context c;
            private JavaList<RoomCleanStatus> cleanstatuses;
            private LayoutInflater inflater;

            public RoomCleanSpinnerAdapter(Context c, JavaList<RoomCleanStatus> statuses )
            {
                this.c = c;
                this.cleanstatuses = statuses;

            }

            public override RoomCleanStatus this[int position]
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override int Count
            {
                get { return cleanstatuses.Size(); }
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override Object GetItem(int position)
            {
                return cleanstatuses.Get(position);
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                if (inflater == null)
                {
                    inflater = (LayoutInflater) c.GetSystemService(LayoutInflaterService);
                }

                if (convertView == null)
                {
                    convertView = inflater.Inflate(Resource.Layout.RoomCleanSpinnerLayout, parent, false);
                }

                ImageView imgCleanStatus = convertView.FindViewById<ImageView>(Resource.Id.imgCleanSPR);
                int roomCleanResourceId = (int)typeof(Resource.Drawable).GetField(cleanstatuses[position].IconPath).GetValue(null);
                imgCleanStatus.SetImageResource(roomCleanResourceId);

                return convertView;
            }
        }
    }
}