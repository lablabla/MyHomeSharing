using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using Android.Widget;
using System;

namespace MyHomeSharing
{
    [Activity(Label = "SplashActivity", Theme = "@style/AppTheme", MainLauncher = true)]
    public class SplashActivity : AppCompatActivity, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, FirebaseAuth.IAuthStateListener
    {
        public static FirebaseApp app;
        static GoogleApiClient mGoogleApiClient;

        static readonly string TAG = "X:" + typeof(SplashActivity).Name;
        private const int SIGN_IN_CODE = 9000;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Log.Debug(TAG, "SplashActivity.OnCreate");
            SetContentView(Resource.Layout.activity_main);

            var signinButton = FindViewById<Button>(Resource.Id.signin_button);
            signinButton.Click += SigninButton_Click;
            InitFirebaseAuth();
            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestEmail()
                .RequestIdToken(""/*TODO - remove hardcoded*/)
                .Build();

            mGoogleApiClient = new GoogleApiClient.Builder(this)
                    .EnableAutoManage(this, this)
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                    .AddConnectionCallbacks(this)
                    .Build();



        }

        private async void SigninButton_Click(object sender, System.EventArgs e)
        {

            //await Auth.GoogleSignInApi.SignOut(mGoogleApiClient);
            GoogleSignInAccount account = GoogleSignIn.GetLastSignedInAccount(this);
            if (account == null)
            {
                var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
                StartActivityForResult(signInIntent, SIGN_IN_CODE);
            }
            else
            {
                FirebaseLogin(account);
            }
        }

        private async void FirebaseLogin(GoogleSignInAccount account)
        {
            var credentials = GoogleAuthProvider.GetCredential(account.IdToken, null);
            var instance = FirebaseAuth.GetInstance(app);
            if (instance == null)
            {
                instance = new FirebaseAuth(app);
            }
            var result = await instance.SignInWithCredentialAsync(credentials);
            if (result.User != null)
            {

            }
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { LoadApplicationData(); });
            startupWork.Start();
        }

        private async void LoadApplicationData()
        {
            //StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
        private void InitFirebaseAuth()
        {
            if (app == null)
            {
                app = FirebaseApp.InitializeApp(this);
            }
            
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == SIGN_IN_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                    if (result.IsSuccess)
                    {
                        FirebaseLogin(result.SignInAccount);
                    }
                    Log.Debug(TAG, $"result.StatusCode: {result.Status.StatusCode}\nMeessage: {result.Status.StatusMessage}");
                    //GoogleAuthProvider.GetCredential()
                    //// create event listener so we know when the user has sucessfully signed in
                    //await FirebaseAuth.Instance.SignInWithCredentialAsync(email, password);
                }
            }
        }

        public void OnConnected(Bundle connectionHint)
        {
        }

        public void OnConnectionSuspended(int cause)
        {
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
        }


        private void AuthStateChanged(object sender, FirebaseAuth.AuthStateEventArgs e)
        {
        }

        public void OnAuthStateChanged(FirebaseAuth auth)
        {
            FirebaseUser user = FirebaseAuth.GetInstance(app).CurrentUser;
            if (user != null)
            {

            }
        }
    }
}