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
    public class DBRepository
    {
        public List<Room> GetAllRooms()
        {
            List<Room> rooms = new List<Room>();

            MySqlConnection con =
                   new MySqlConnection(
                       "Server=s00142227db.cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=kmorris;Password=s00142227;charset=utf8");

            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    //TODO: Where date = today and user = the user that is currently signed in
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM RoomTbl", con);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var ID = (int)reader["ID"];
                        var RoomNo = (string)reader["RoomNo"];
                        var roomOcc = (int)reader["RoomOccupiedStatusID"];
                        var roomClean = (int)reader["RoomCleanStatusID"];
                        var roomType = (int)reader["RoomTypeID"];
                        //need to handle null notes
                        var Note = "";
                        if (reader["Note"] is string)
                            Note = (string) reader["Note"];
                        rooms.Add(new Room(ID, RoomNo, roomOcc, roomClean, roomType, Note));

                    }

                }
            }
            catch (MySqlException ex)
            {
                //Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

            return rooms;
        }

        public Room GetRoomById(int id)
        {
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
                        cmd.Parameters.AddWithValue("@id", id);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //need to handle null note
                                var note = "";
                                if (reader["Note"] is string)
                                    note = (string) reader["Note"];
                                room = new Room((int)reader["ID"], (string)reader["RoomNo"],
                                    (int)reader["RoomOccupiedStatusID"], (int)reader["RoomCleanStatusID"], (int)reader["RoomTypeID"], note);
                            }
                        }
                    }
                }

            }
            catch (MySqlException ex)
            {
                //Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

            return room;
        }

        public RoomOccupiedStatus GetOccupiedStatusById(int id)
        {
            MySqlConnection con =
                   new MySqlConnection(
                       "Server=s00142227db.cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=kmorris;Password=s00142227;charset=utf8");
            RoomOccupiedStatus occStatus = new RoomOccupiedStatus();
            try
            {
                
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM RoomOccupiedStatusTbl where ID = @id;", con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                occStatus.Description = (string) reader["Description"];
                                occStatus.IconPath = (string)reader["IconPath"];
                            }
                        }
                    }
                }

            }
            catch (MySqlException ex)
            {
                //Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

            return occStatus;
        }

        public RoomCleanStatus GetCleanStatusById(int id)
        {
            MySqlConnection con =
                   new MySqlConnection(
                       "Server=s00142227db.cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=kmorris;Password=s00142227;charset=utf8");
            RoomCleanStatus cleanStatus = new RoomCleanStatus();
            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM RoomCleanStatusTbl where ID = @id;", con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cleanStatus.Description = (string)reader["Description"];
                                cleanStatus.IconPath = (string)reader["IconPath"];
                                cleanStatus.BorderImage = (string)reader["BorderImage"];
                            }
                        }
                    }
                }

            }
            catch (MySqlException ex)
            {
                //Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

            return cleanStatus;
        }
        public JavaList<RoomCleanStatus> GetAllCleaningStatuses()
        {
            MySqlConnection con =
                   new MySqlConnection(
                       "Server=s00142227db.cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=kmorris;Password=s00142227;charset=utf8");
            JavaList<RoomCleanStatus> cleanStatuses = new JavaList<RoomCleanStatus>();
            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM RoomCleanStatusTbl;", con))
                    {
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RoomCleanStatus cleanStatus = new RoomCleanStatus();
                                cleanStatus.ID = (int) reader["ID"];
                                cleanStatus.Description = (string)reader["Description"];
                                cleanStatus.IconPath = (string)reader["IconPath"];
                                cleanStatus.BorderImage = (string)reader["BorderImage"];
                                cleanStatuses.Add(cleanStatus);
                            }
                        }
                    }
                }

            }
            catch (MySqlException ex)
            {
                //Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

            return cleanStatuses;
        }

        public RoomType GetRoomTypeById(int id)
        {
            MySqlConnection con =
                   new MySqlConnection(
                       "Server=s00142227db.cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=kmorris;Password=s00142227;charset=utf8");
            RoomType roomType = new RoomType();
            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM RoomTypeTbl where ID = @id;", con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                roomType.Description = (string)reader["Description"];
                                roomType.IconPath = (string)reader["IconPath"];
                            }
                        }
                    }
                }

            }
            catch (MySqlException ex)
            {
                //Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

            return roomType;
        }

        public void UpdateRoom(int id, string cleanStat, string note)
        {
            MySqlConnection con =
                   new MySqlConnection(
                       "Server=s00142227db.cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=kmorris;Password=s00142227;charset=utf8");

            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand("UPDATE RoomTbl SET RoomCleanStatusID = (SELECT ID from RoomCleanStatusTbl WHERE Description LIKE @roomclean), Note = @note WHERE ID = @id;", con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@roomclean", cleanStat);
                        cmd.Parameters.AddWithValue("@note", note);
                        cmd.ExecuteNonQuery();
                    }
                }


            }
            catch (MySqlException ex)
            {
                //Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

        }

        public EventType GetEventTypeById(int id)
        {
            MySqlConnection con =
                   new MySqlConnection(
                       "Server=s00142227db.cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=kmorris;Password=s00142227;charset=utf8");
            EventType eventType = new EventType();
            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM EventTypeTbl where ID = @id;", con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                eventType.ID = (int) reader["ID"];
                                eventType.Description = (string)reader["Description"];
                                eventType.IconPath = (string)reader["IconPath"];
                            }
                        }
                    }
                }

            }
            catch (MySqlException ex)
            {
                //Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

            return eventType;
        }

        public Event GetEventById(int id)
        {
            MySqlConnection con =
                   new MySqlConnection(
                       "Server=s00142227db.cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=kmorris;Password=s00142227;charset=utf8");
            Event eventE = new Event();
            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM EventTbl where ID = @id;", con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                eventE.ID = (int)reader["ID"];
                                eventE.Description = (string)reader["Description"];
                                eventE.StartTime = (DateTime)reader["StartTime"];
                                eventE.EndTime = (DateTime)reader["EndTime"];
                            }
                        }
                    }
                }

            }
            catch (MySqlException ex)
            {
                //Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

            return eventE;
        }

        public List<Room> GetRoomsForEvent(int id)
        {
            MySqlConnection con =
                   new MySqlConnection(
                       "Server=s00142227db.cshbhaowu4cu.eu-west-1.rds.amazonaws.com;Port=3306;database=RoomCheckDB;User Id=kmorris;Password=s00142227;charset=utf8");
            List<Room> rooms = new List<Room>();
            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();

                   
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM RoomEvent where EventID = @id;", con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int roomID = (int) reader["RoomID"];
                                Room room = GetRoomById(roomID);
                                //todo: only add to list if the date is today's date
                                rooms.Add(room);
                            }
                        }
                    }
                }

            }
            catch (MySqlException ex)
            {
                //Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

            return rooms;
        }
    }

    
}
