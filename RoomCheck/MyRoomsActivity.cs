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
    [Activity(Label = "My Rooms")]
    public class MyRoomsActivity : Activity
    {
        List<Room> rooms = new List<Room>();
        GridView gridview;
        DBRepository dbr = new DBRepository();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.MyRoomsMain);

            //TODO: change to gridview
            gridview = FindViewById<GridView>(Resource.Id.lstRooms);

            rooms = dbr.GetAllRooms();
        
            gridview.Adapter = new RoomAdapter(this, rooms);

            gridview.ItemClick += ListViewOnItemClick;

        }

        private void ListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (gridview.GetItemAtPosition(e.Position) != null)
            {
                var room = gridview.GetItemAtPosition(e.Position).Cast<Room>();

                //pass the room id to the next activity
                var roomDetailsActivity = new Intent(this, typeof(RoomDetailsActivity));
                roomDetailsActivity.PutExtra("RoomID", room.ID);
                StartActivity(roomDetailsActivity);
            }
            else
            {
                string item = gridview.GetItemAtPosition(e.Position).ToString();
                Toast.MakeText(this, item, ToastLength.Short).Show();
            }
        }


    }

    public class RoomAdapter : BaseAdapter<Room>
    {
        DBRepository dbr = new DBRepository();

        List<Room> items;
        Activity context;
        public RoomAdapter(Activity context, List<Room> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Room this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];

            View view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.MyRooms, null);

            //TODO: see how often this method is getting called and if there are any changes i can make to optimise

            RoomOccupiedStatus roomOccStatus = dbr.GetOccupiedStatusById(item.OccupiedStatusID);
            RoomCleanStatus roomCleanStatus = dbr.GetCleanStatusById(item.CleanStatusID);
            RoomType roomType = dbr.GetRoomTypeById(item.RoomTypeID);

            //TODO: if room type is empty grey out the icon and border (room occupied status shoud always be unoccupied on empty rooms
            ImageView imgRoomIcon = view.FindViewById<ImageView>(Resource.Id.imgRoomIcon);

            //populate room number labels
            view.FindViewById<TextView>(Resource.Id.lblRoomNo).Text = item.RoomNo;

            //change icon to reflect occupied status
            var roomOccResourceId = (int)typeof(Resource.Drawable).GetField(roomOccStatus.IconPath).GetValue(null);
            imgRoomIcon.SetImageResource(roomOccResourceId);

            //change border colour to reflect cleaning status
            var background = (int)typeof(Resource.Drawable).GetField(roomCleanStatus.BorderImage).GetValue(null);
            view.FindViewById<ImageView>(Resource.Id.imgRoomIcon).SetBackgroundResource(background);


            //TODO: if there is currently an event on for the room get the event type and display
            ImageView imgEventNotification = view.FindViewById<ImageView>(Resource.Id.imgEventNotification);

            if (dbr.CheckEvents(item.ID))
            {
                List<Event> roomEvents = dbr.EventsForRoom(item.ID);
                foreach (Event e in roomEvents)
                {
                    //TODO: remove .TimeOfDay (just there for test purposes)
                    if (DateTime.Now.TimeOfDay >= e.StartTime.TimeOfDay && DateTime.Now.TimeOfDay < e.EndTime.TimeOfDay)
                    {
                        EventType et = dbr.GetEventTypeByID(e.EventTypeID);
                        //show the notification icon
                        var eventNotifResourceId = (int) typeof(Resource.Drawable).GetField(et.IconPath + "Small").GetValue(null);
                        imgEventNotification.SetImageResource(eventNotifResourceId);
                        imgEventNotification.Visibility = ViewStates.Visible;

                        //Guest is away at an event - we can change their occupied status to unoccupied
                        //TODO: implement properly (actually update database here or else have a sql job to handle this)
                        var roomOccResourceIdAway = (int)typeof(Resource.Drawable).GetField("Unoccupied").GetValue(null);
                        imgRoomIcon.SetImageResource(roomOccResourceIdAway);
                    }
                    else
                    {
                        imgEventNotification.Visibility = ViewStates.Invisible;
                    }
                }
            }
 

            return view;
        }

    }

    
}