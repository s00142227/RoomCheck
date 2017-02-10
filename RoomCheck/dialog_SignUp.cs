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
    public class OnSignUpEventArgs : EventArgs
    {
        private string mHotelID;
        private string mFirstName;
        private string mEmail;
        private string mPassword;

        public string FirstName
        {
            get { return mFirstName;}
            set { mFirstName = value; }
        }
        public string HotelID
        {
            get { return mHotelID; }
            set { mHotelID = value; }
        }
        public string Email
        {
            get { return mEmail; }
            set { mEmail = value; }
        }
        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }

        public OnSignUpEventArgs(string hotelId, string firstName, string email, string password) : base()
        {
            HotelID = hotelId;
            FirstName = firstName;
            Email = email;
            Password = password;
        }
    }
    class dialog_SignUp : DialogFragment
    {
        private EditText txtHotelID;
        private EditText txtFirstName;
        private EditText txtEmail;
        private EditText txtPassword;
        private Button btnSignUp;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.dialog_sign_up, container, false);

            txtHotelID = view.FindViewById<EditText>(Resource.Id.txtHotelId);
            txtFirstName = view.FindViewById<EditText>(Resource.Id.txtFirstName);
            txtEmail = view.FindViewById<EditText>(Resource.Id.txtEmail);
            txtPassword = view.FindViewById<EditText>(Resource.Id.txtPassword);
            btnSignUp = view.FindViewById<Button>(Resource.Id.btnDialogEmail);
            btnSignUp.Click += BtnSignUpOnClick;

            return view;
        }

        public event EventHandler<OnSignUpEventArgs> onSignUpComplete;

        private void BtnSignUpOnClick(object sender, EventArgs eventArgs)
        {
            onSignUpComplete.Invoke(this, new OnSignUpEventArgs(txtHotelID.Text, txtFirstName.Text, txtEmail.Text, txtPassword.Text));
            this.Dismiss();
        }

        
         

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //set the title bar to invisible
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //set the animation
        }
    }
}