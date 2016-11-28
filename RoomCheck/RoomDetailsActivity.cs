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
                //TODO: maybe change the layout to an expandibla list of events instead of just displaying one?
                Event currentEvent = events[0];
                EventType eventType = dbr.GetEventTypeByID(currentEvent.EventTypeID);

                //set image, text for event
                int eventTypeResourceId = (int)typeof(Resource.Drawable).GetField(eventType.IconPath).GetValue(null);
                imgEventType.SetImageResource(eventTypeResourceId);
                txtEvent.Text = currentEvent.Description;
                txtEventTime.Text = string.Format("{0} - {1}", currentEvent.StartTime.TimeOfDay.ToString(),
                    currentEvent.EndTime.TimeOfDay.ToString());


                llEvent.Visibility = ViewStates.Visible; 
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

        private void SprRoomCleanOnItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            TextView txtRoomClean = FindViewById<TextView>(Resource.Id.txtCleanStatus);
            txtRoomClean.Text = cleanstatuses[e.Position].Description;
        }

        private void BtnSaveOnClick(object sender, EventArgs eventArgs)
        {
            int id = Intent.GetIntExtra("RoomID", 0);
            TextView txtCleanStat = FindViewById<TextView>(Resource.Id.txtCleanStatus);
            EditText txtNote = FindViewById<EditText>(Resource.Id.txtNote);

            dbr.UpdateRoom(id, txtCleanStat.Text, txtNote.Text);

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