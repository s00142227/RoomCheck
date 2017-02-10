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
    class dialog_SignIn : DialogFragment
    {
        private EditText txtEmail;
        private EditText txtPassword;
        private Button btnSignIn;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.dialog_sign_in, container, false);

            txtEmail = view.FindViewById<EditText>(Resource.Id.txtSignInEmail);
            txtPassword = view.FindViewById<EditText>(Resource.Id.txtSignInPassword);
            btnSignIn = view.FindViewById<Button>(Resource.Id.btnSignIn);
            btnSignIn.Click += BtnSignInOnClick;

            return view;
        }

        public event EventHandler<OnSignInEventArgs> onSignInComplete;

        private void BtnSignInOnClick(object sender, EventArgs eventArgs)
        {
            onSignInComplete.Invoke(this, new OnSignInEventArgs(txtEmail.Text, txtPassword.Text));
            this.Dismiss();
        }

        public event EventHandler<OnSignUpEventArgs> onSignUpComplete;


        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //set the title bar to invisible
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //set the animation
        }
    }

    public class OnSignInEventArgs : EventArgs
    {

        private string mEmail;
        private string mPassword;

       
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

        public OnSignInEventArgs(string email, string password) : base()
        {
            Email = email;
            Password = password;
        }
    }
}