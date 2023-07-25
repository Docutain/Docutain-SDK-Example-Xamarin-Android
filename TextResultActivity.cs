using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Docutain.SDK.Xamarin.Android;
using DocumentDataReader = Docutain.SDK.Xamarin.Android.DocumentDataReader;
using AndroidX.AppCompat.App;

namespace Docutain_SDK_Example_Xamarin_Android
{
    [Activity(Label = "TextResultActivity")]
    public class TextResultActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_text_result);

            LoadText();
        }

        private void LoadText()
        {
            Task.Run(() =>
            {
                var filePath = Intent.GetStringExtra("filePath");
                if (filePath != null)
                {
                    // If an URI is available, it means we have imported a file. If so, we need to load it into the SDK first.
                    if (!DocumentDataReader.LoadFile(filePath))
                    {
                        // An error occurred, get the latest error message.
                        System.Console.WriteLine($"DocumentDataReader.LoadFile failed, last error: {DocutainSDK.LastError}");
                        return;
                    }
                }

                // Get the text of all currently loaded pages.
                // If you want text of just one specific page, define the page number.
                // See https://docs.docutain.com/docs/Xamarin/textDetection for more details.
                string text = DocumentDataReader.GetText();

                RunOnUiThread(() =>
                {
                    FindViewById<ProgressBar>(Resource.Id.activity_indicator).Visibility = ViewStates.Gone;
                    FindViewById<TextView>(Resource.Id.textView).Text = text;
                });
            });
        }
    }
}
