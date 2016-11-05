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

namespace RoomCheck
{
    [Activity(Label = "RoomDetailsActivity")]
    public class RoomDetailsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.RoomDetails);

            // Create your application here
            //might be better to store the current room object in the sqllite db from the previous view?
            //would still have to query a database here but it wouldn't be the cloud database...
            int roomID = Intent.GetIntExtra("RoomID", 0);
            Toast.MakeText(this, roomID.ToString(), ToastLength.Short).Show();
        }
    }
}