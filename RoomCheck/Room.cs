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
    public class Room
    {
        public int ID { get; set; }
        public string RoomNo { get; set; }
        public int OccupiedStatusID { get; set; }
        public int CleanStatusID { get; set; }
        public int RoomTypeID { get; set; }
        public string Note { get; set; }
        public int NoGuests { get; set; }
        public int UserID { get; set; }
        public DateTime Date { get; set; }

        public Room(int id,string roomNo, int occupiedStatus, int cleanStatus, int roomType)
        {
            ID = id;
            RoomNo = roomNo;
            OccupiedStatusID = occupiedStatus;
            CleanStatusID = cleanStatus;
            RoomTypeID = roomType;
        }

        public Room()
        {
                
        }
    }

    public class RoomOccupiedStatus
    {
        public int ID { get; set; }
        public string Description { get; set; }
    }

    public class RoomCleanStatus
    {
        public int ID { get; set; }
        public string Description { get; set; }
    }

    public class RoomType
    {
        public int ID { get; set; }
        public string Description { get; set; }
    }

    public class User
    {
        //fill in later
    }

}