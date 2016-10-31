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
    [Activity(Label = "MyRoomsActivity")]
    public class MyRoomsActivity : Activity
    {
        //List<ColorItem> colorItems = new List<ColorItem>();
        List<Room> rooms = new List<Room>();
        ListView listView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.MyRoomsMain);

            listView = FindViewById<ListView>(Resource.Id.lstRooms);

            MySqlConnection con =
                   new MySqlConnection(
                       "Server=s00142227db.cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=kmorris;Password=s00142227;charset=utf8");

            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    //var result = "";
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM RoomTbl", con);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        //var someValue = reader["user"];
                        var RoomNo = (string)reader["RoomNo"];
                        var roomOcc = (int)reader["RoomOccupiedStatusID"];
                        var roomClean = (int)reader["RoomCleanStatusID"];
                        //result += (someValue + "\n");
                        rooms.Add(new Room(RoomNo, roomOcc, roomClean));

                    }

                    //Toast.MakeText(this, result, ToastLength.Short).Show();
                }
            }
            catch (MySqlException ex)
            {
                //txtSysLog.Text = ex.ToString();
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

            //colorItems.Add(new ColorItem()
            //{
            //    Color = Android.Graphics.Color.DarkRed,
            //    ColorName = "Dark Red",
            //    Code = "8B0000"
            //});
            //colorItems.Add(new ColorItem()
            //{
            //    Color = Android.Graphics.Color.SlateBlue,
            //    ColorName = "Slate Blue",
            //    Code = "6A5ACD"
            //});
            //colorItems.Add(new ColorItem()
            //{
            //    Color = Android.Graphics.Color.ForestGreen,
            //    ColorName = "Forest Green",
            //    Code = "228B22"
            //});

            //listView.Adapter = new ColorAdapter(this, colorItems);
            listView.Adapter = new RoomAdapter(this, rooms);
        }
    }

    public class RoomAdapter : BaseAdapter<Room>
    {
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

            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.MyRooms, null);
            view.FindViewById<TextView>(Resource.Id.textView1).Text = item.RoomNo;
            switch (item.OccupiedStatusID)
            {
                case 1:
                    view.FindViewById<TextView>(Resource.Id.textView2).Text = "Unknown";
                    break;
                case 2:
                    view.FindViewById<TextView>(Resource.Id.textView2).Text = "Unoccupied";
                    break;
                case 3:
                    view.FindViewById<TextView>(Resource.Id.textView2).Text = "Occupied";
                    break;
                default:
                    view.FindViewById<TextView>(Resource.Id.textView2).Text = "Unknown";
                    break;

            }

            switch (item.CleanStatusID)
            {
                case 1:
                    view.FindViewById<ImageView>(Resource.Id.imageView1).SetBackgroundColor(Android.Graphics.Color.Red);
                    break;
                case 2:
                    view.FindViewById<ImageView>(Resource.Id.imageView1).SetBackgroundColor(Android.Graphics.Color.CadetBlue);
                    break;
                case 3:
                    view.FindViewById<ImageView>(Resource.Id.imageView1).SetBackgroundColor(Android.Graphics.Color.Green);
                    break;
                default:
                    view.FindViewById<ImageView>(Resource.Id.imageView1).SetBackgroundColor(Android.Graphics.Color.DimGray);
                    break;

            }

            return view;
        }
    }

    public class ColorAdapter : BaseAdapter<ColorItem>
    {
        List<ColorItem> items;
        Activity context;
        public ColorAdapter(Activity context, List<ColorItem> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override ColorItem this[int position]
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

            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.MyRooms, null);
            view.FindViewById<TextView>(Resource.Id.textView1).Text = item.ColorName;
            view.FindViewById<TextView>(Resource.Id.textView2).Text = item.Code;
            view.FindViewById<ImageView>(Resource.Id.imageView1).SetBackgroundColor(item.Color);

            return view;
        }
    }

    public class ColorItem
    {
        public string ColorName { get; set; }
        public string Code { get; set; }
        public Android.Graphics.Color Color { get; set; }
    }
}