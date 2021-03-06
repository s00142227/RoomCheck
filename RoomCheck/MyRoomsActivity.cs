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
using SQLite;

namespace RoomCheck
{
    [Activity(Label = "My Rooms")]
    public class MyRoomsActivity : Activity
    {
        List<Room> rooms = new List<Room>();
        GridView gridview;
        DBRepository dbr = new DBRepository();
        private SQLiteConnection db;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.MyRoomsMain);

            gridview = FindViewById<GridView>(Resource.Id.lstRooms);

            //get user from database
            var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");

            var result = createDatabase(pathToDatabase);
            var db = new SQLiteConnection(pathToDatabase, true);
            int userId = 0;

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "My Rooms";
            

            //fix this 
            var firstOrDefault = db.Query<User>("Select * from User").FirstOrDefault();
            if (firstOrDefault != null)
            {
                userId = firstOrDefault.ID;
            }

            rooms = dbr.GetAllRooms(userId);

            dbr.GetAllRoomInfo(ref rooms);

            gridview.Adapter = new RoomAdapter(this, rooms);

            gridview.ItemClick += ListViewOnItemClick;


        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();

            //delete the user from sqlite and go back to the main activity - should provide the expected result
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
        //DBRepository dbr = new DBRepository();

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

            RoomOccupiedStatus roomOccStatus = item.OccupiedStatus;
            RoomCleanStatus roomCleanStatus = item.CleanStatus;
            RoomType roomType = item.RoomType;

            ImageView imgRoomIcon = view.FindViewById<ImageView>(Resource.Id.imgRoomIcon);

            //populate room number labels
            view.FindViewById<TextView>(Resource.Id.lblRoomNo).Text = item.RoomNo;

            //change icon to reflect occupied status
            var roomOccResourceId = (int)typeof(Resource.Drawable).GetField(roomOccStatus.IconPath).GetValue(null);
            imgRoomIcon.SetImageResource(roomOccResourceId);

            //change border colour to reflect cleaning status
            //var background = (int)typeof(Resource.Drawable).GetField(roomCleanStatus.BorderImage).GetValue(null);
            //view.FindViewById<ImageView>(Resource.Id.imgRoomIcon).SetBackgroundResource(background);

            //change background color to reflect cleaning status
            LinearLayout listItem = view.FindViewById<LinearLayout>(Resource.Id.llMyRoomsListItem);
            listItem.SetBackgroundColor(Android.Graphics.Color.ParseColor(roomCleanStatus.BorderImage));

            //TODO: if room type is empty grey out the icon and border (room occupied status shoud alw-ays be unoccupied on empty rooms
            if (roomType.Description == "Empty")
            {
                roomOccResourceId = (int)typeof(Resource.Drawable).GetField("Unoccupied").GetValue(null);
                imgRoomIcon.SetImageResource(roomOccResourceId);

                view.FindViewById<TextView>(Resource.Id.lblRoomNo).SetTextColor(Android.Graphics.Color.ParseColor("#808080"));
                listItem.SetBackgroundColor(Android.Graphics.Color.ParseColor("#e6e6e6"));
            }

            //If there is a note on the room - show the note icon
            ImageView imgNote = view.FindViewById<ImageView>(Resource.Id.imgNote);
            

            //guest request overrides note - if there are both, the cleaner will have to 
            //view details anyways and will see both
            if (!string.IsNullOrEmpty(item.GuestRequest))
            {
                var resourceId = (int) typeof(Resource.Drawable).GetField("GuestRequest").GetValue(null);
                imgNote.SetImageResource(resourceId);
                imgNote.Visibility = ViewStates.Visible;
            }
            else
            {
                imgNote.Visibility = ViewStates.Invisible;
                if (!string.IsNullOrEmpty(item.Note))
                {
                    var resourceId = (int)typeof(Resource.Drawable).GetField("Info").GetValue(null);
                    imgNote.SetImageResource(resourceId);
                    imgNote.Visibility = ViewStates.Visible;
                }
                else
                    imgNote.Visibility = ViewStates.Invisible;
            }
                


            //Check if there is currently an event for the room and show icon
            ImageView imgEventNotification = view.FindViewById<ImageView>(Resource.Id.imgEventNotification);

            if (item.Events.Count > 0)
            {
                List<Event> roomEvents = item.Events;
                foreach (Event e in roomEvents)
                {
                    //TODO: remove .TimeOfDay (just there for test purposes)
                    if (DateTime.Now.TimeOfDay >= e.StartTime.TimeOfDay && DateTime.Now.TimeOfDay < e.EndTime.TimeOfDay)
                    {
                        EventType et = e.EventType;
                        //show the notification icon
                        var eventNotifResourceId = (int)typeof(Resource.Drawable).GetField(et.IconPath + "Small").GetValue(null);
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