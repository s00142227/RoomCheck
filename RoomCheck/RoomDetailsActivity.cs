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
using System.Net.Mime;
using Android.Content.Res;
using SQLite;
using Object = Java.Lang.Object;

namespace RoomCheck
{
    [Activity(Label = "Room Details")]
    public class RoomDetailsActivity : Activity
    {
        public static DBRepository dbr = new DBRepository();
        public static JavaList<RoomCleanStatus> cleanstatuses;
        public static int roomID;
        private SQLiteConnection db;
        private Room room;
        private EditText txtGuestRequest;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.RoomDetails);

            // Create your application here
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Room Details";

            roomID = Intent.GetIntExtra("RoomID", 0);

            room = dbr.GetRoomById(roomID);

            //Get all fields on screen
            TextView txtRoomNo = FindViewById<TextView>(Resource.Id.txtRoomNo);
            TextView txtRoomType = FindViewById<TextView>(Resource.Id.txtRoomType);
            TextView txtRoomOccStatus = FindViewById<TextView>(Resource.Id.txtRoomOccupiedStatus);
            EditText txtNote = FindViewById<EditText>(Resource.Id.txtNote);
            txtGuestRequest = FindViewById<EditText>(Resource.Id.txtGuestRequest);
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
            txtGuestRequest.Text = room.GuestRequest;
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

            ImageButton btnCompleteRequest = FindViewById<ImageButton>(Resource.Id.btnCompleteRequest);
            btnCompleteRequest.Click += BtnCompleteRequestOnClick;

            //TODO: set room clean status spinner to have the rooms status selected rather than just the first value
            //sprCleanStat.SelectedItem = sprCleanStat
            RoomCleanStatus rc = dbr.GetCleanStatusById(room.CleanStatusID);
            
            sprRoomClean.SetSelection(room.CleanStatusID - 1);

            //TODO: move all this to a separate method with the room id
            //TODO: check if there is an event on the room today and display here with time
            //TODO: move database operations to main rooms page
            LinearLayout llEvent = FindViewById<LinearLayout>(Resource.Id.llEventLayout);
            TextView txtEvent = FindViewById<TextView>(Resource.Id.txtEventType);
            ImageView imgEventType = FindViewById<ImageView>(Resource.Id.imgEventType);
            TextView txtEventTime = FindViewById<TextView>(Resource.Id.txtEventTime);

            if(dbr.CheckEvents(room.ID))
            {
                List<Event> events = dbr.EventsForRoom(room.ID);

                //TODO: check which event is happening right now or just use first in the list
                //TODO: maybe change the layout to an expandable list of events instead of just displaying one?
                Event currentEvent = events[0];
                EventType eventType = dbr.GetEventTypeByID(currentEvent.EventTypeID);

                //set image, text for event
                int eventTypeResourceId = (int)typeof(Resource.Drawable).GetField(eventType.IconPath).GetValue(null);
                imgEventType.SetImageResource(eventTypeResourceId);
                txtEvent.Text = currentEvent.Description;
                txtEventTime.Text = string.Format("{0} - {1}", currentEvent.StartTime.TimeOfDay, currentEvent.EndTime.TimeOfDay);


                llEvent.Visibility = ViewStates.Visible; 
            }
            


        }

        private void BtnCompleteRequestOnClick(object sender, EventArgs eventArgs)
        {
            if (txtGuestRequest.Text != "" && txtGuestRequest.Text != "")
            {
                dbr.CompleteGuestRequest(room.ID);
                txtGuestRequest.Text = "";
                Toast.MakeText(this, "Guest request completed!", ToastLength.Short).Show();
                //TODO: find out how to send notification to the user with the app on here
            }
            else
            {
                Toast.MakeText(this, "No request to complete", ToastLength.Short).Show();
            }
        }

        //tried to use this method when setting the selection for the spinner but was havign problems
        //getting the id of an item at a certain position in the spinner. So to compromise above
        //i am just assuming that the index will be the item id - 1
        //private int getIndex(Spinner spinner, int id)
        //{
        //    int index = 0;
        //    for (int i = 0; i < spinner.Count; i++)
        //    {
        //        int rcid = Convert.ToInt32(spinner.Adapter.GetItemId(i));
        //        if (rcid == id)
        //        {
        //            index = i;
        //    }
        //            break;
        //        }
        //    return index;
        //}


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();

            var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");

            var result = createDatabase(pathToDatabase);
            db = new SQLiteConnection(pathToDatabase, true);

            var count = db.ExecuteScalar<int>("SELECT Count(*) FROM User");
            if (count > 0)
            {
                db.DeleteAll<User>();
            }

            StartActivity(typeof(SplashActivity));

            return base.OnOptionsItemSelected(item);
        }

        private void SprRoomCleanOnItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            TextView txtRoomClean = FindViewById<TextView>(Resource.Id.txtCleanStatus);
            txtRoomClean.Text = cleanstatuses[e.Position].Description;
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

        private void BtnSaveOnClick(object sender, EventArgs eventArgs)
        {
            int id = Intent.GetIntExtra("RoomID", 0);
            TextView txtCleanStat = FindViewById<TextView>(Resource.Id.txtCleanStatus);
            EditText txtNote = FindViewById<EditText>(Resource.Id.txtNote);
            EditText txtGuestRequest = FindViewById<EditText>(Resource.Id.txtGuestRequest);

            dbr.UpdateRoom(id, txtCleanStat.Text, txtNote.Text, txtGuestRequest.Text);

            //Toast.MakeText(this, "Room Updated", ToastLength.Short).Show();
            //might need more feedback for user here i.e. a popup message when they go back to previous screen
            //TODO: is there a better approach than starting the activity again? might be doable once i have pull-to-refresh implemented
            StartActivity(typeof(MyRoomsActivity));

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
                get { throw new NotImplementedException(); }
            }

            public long GetItemIdAtPosition(int position)
            {
                return cleanstatuses[position].ID;
            }

            public override int Count
            {
                get { return cleanstatuses.Size(); }
            }

            public override long GetItemId(int position)
            {
                return cleanstatuses[position].ID;
            }

            public RoomCleanStatus GetItemCleanStatus(int position)
            {
                return cleanstatuses[position];
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