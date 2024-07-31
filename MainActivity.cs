using Android.App;
using Android.Content;
using Android.OS;
using Docutain.SDK.Xamarin.Android;
using Java.IO;
using System.Threading.Tasks;
using Uri = Android.Net.Uri;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Xamarin.Essentials;
using Google.Android.Material.Dialog;

namespace Docutain_SDK_Example_Xamarin_Android
{
    [Activity(Label = "Docutain SDK Example", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        //A valid license key is required, you can generate one on our website https://sdk.docutain.com/TrialLicense?Source=1311235
        private string licenseKey = "YOUR_LICENSE_KEY_HERE";
        private ItemType selectedOption = ItemType.NONE;
        private SettingsSharedPreferences _settingsSharedPreferences;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);            

            //the Docutain SDK needs to be initialized prior to using any functionality of it
            //a valid license key is required, you can generate one on our website https://sdk.docutain.com/TrialLicense?Source=1311235
            if (!DocutainSDK.InitSDK(Application, licenseKey))
            {
                //init of Docutain SDK failed, get the last error message
                System.Console.WriteLine("Initialization of the Docutain SDK failed: " + DocutainSDK.LastError);
                //your logic to deactivate access to SDK functionality
                if (licenseKey == "YOUR_LICENSE_KEY_HERE")
                    ShowLicenseEmptyInfo();
                else
                    ShowLicenseErrorInfo();
                return;
            }

            //If you want to use text detection (OCR) and/or data extraction features, you need to set the AnalyzeConfiguration
            //in order to start all the necessary processes
            var analyzeConfig = new AnalyzeConfiguration
            {
                ReadBIC = true,
                ReadPaymentState = true
            };
            if (!DocumentDataReader.SetAnalyzeConfiguration(analyzeConfig))
            {
                System.Console.WriteLine("Setting AnalyzeConfiguration failed: " + DocutainSDK.LastError);
            }

            //Depending on your needs, you can set the Logger's level
            Logger.SetLogLevel(Logger.Level.Verbose);


            //Depending on the log level that you have set, some temporary files get written on the filesystem
            //You can delete all temporary files by using the following method
            DocutainSDK.DeleteTempFiles(true);

            _settingsSharedPreferences = new SettingsSharedPreferences(this);

            if (_settingsSharedPreferences.IsEmpty())
                _settingsSharedPreferences.DefaultSettings();

            SetContentView(Resource.Layout.activity_main);

            SetupRecyclerView();         
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetupRecyclerView()
        {
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            LinearLayoutManager linear = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(linear);
            recyclerView.SetAdapter(new ListAdapter(item =>
            {
                switch (item.Type)
                {
                    case ItemType.DOCUMENT_SCAN:
                        selectedOption = ItemType.NONE;
                        StartScan(false);
                        break;
                    case ItemType.DATA_EXTRACTION:
                        selectedOption = ItemType.DATA_EXTRACTION;
                        StartDataExtraction();
                        break;
                    case ItemType.TEXT_RECOGNITION:
                        selectedOption = ItemType.TEXT_RECOGNITION;
                        StartTextRecognition();
                        break;
                    case ItemType.PDF_GENERATING:
                        selectedOption = ItemType.PDF_GENERATING;
                        StartPDFGenerating();
                        break;
                    case ItemType.SETTINGS:
                        StartActivity(new Intent(this, typeof(SettingsActivity)));
                        break;
                    default:
                        selectedOption = ItemType.NONE;
                        System.Console.WriteLine("Invalid item clicked");
                        break;
                }
            }));
            var dividerItemDecoration = new DividerItemDecoration(recyclerView.Context, LinearLayoutManager.Vertical);
            recyclerView.AddItemDecoration(dividerItemDecoration);
        }        

        private void HandleScanResult()
        {
            // Proceed depending on the previously selected option
            switch (selectedOption)
            {
                case ItemType.PDF_GENERATING:
                    GeneratePDF(null);
                    break;
                case ItemType.DATA_EXTRACTION:
                    OpenDataResultActivity(null);
                    break;
                case ItemType.TEXT_RECOGNITION:
                    OpenTextResultActivity(null);
                    break;
                default:
                    System.Console.WriteLine("Select an input option first");
                    break;
            }
        }

        private async void StartScan(bool imageImport)
        {
            //There are a lot of settings to configure the scanner to match your specific needs
            //Check out the documentation to learn more https://docs.docutain.com/docs/Xamarin/docScan#change-default-scan-behaviour
            var scanConfig = new DocumentScannerConfiguration();

            if (imageImport)
                scanConfig.Source = Source.GalleryMultiple;

            //In this sample app we provide a settings page which the user can use to alter the scan settings
            //The settings are stored in and read from SharedPreferences
            //This is supposed to be just an example, you do not need to implement it in that exact way
            //If you do not want to provide your users the possibility to alter the settings themselves at all
            //You can just set the settings according to the apps needs

            //scan settings
            scanConfig.AllowCaptureModeSetting = _settingsSharedPreferences.GetScanItem(ScanSettings.AllowCaptureModeSetting).CheckValue;
            scanConfig.AutoCapture = _settingsSharedPreferences.GetScanItem(ScanSettings.AutoCapture).CheckValue;
            scanConfig.AutoCrop = _settingsSharedPreferences.GetScanItem(ScanSettings.AutoCrop).CheckValue;
            scanConfig.MultiPage = _settingsSharedPreferences.GetScanItem(ScanSettings.MultiPage).CheckValue;
            scanConfig.PreCaptureFocus = _settingsSharedPreferences.GetScanItem(ScanSettings.PreCaptureFocus).CheckValue;
            scanConfig.DefaultScanFilter = _settingsSharedPreferences.GetScanFilterItem(ScanSettings.DefaultScanFilter).ScanValue;
           
            //edit settings
            scanConfig.PageEditConfig.AllowPageFilter = _settingsSharedPreferences.GetEditItem(EditSettings.AllowPageFilter).EditValue;
            scanConfig.PageEditConfig.AllowPageRotation = _settingsSharedPreferences.GetEditItem(EditSettings.AllowPageRotation).EditValue;
            scanConfig.PageEditConfig.AllowPageArrangement = _settingsSharedPreferences.GetEditItem(EditSettings.AllowPageArrangement).EditValue;
            scanConfig.PageEditConfig.AllowPageCropping = _settingsSharedPreferences.GetEditItem(EditSettings.AllowPageCropping).EditValue;
            scanConfig.PageEditConfig.PageArrangementShowDeleteButton = _settingsSharedPreferences.GetEditItem(EditSettings.PageArrangementShowDeleteButton).EditValue;
            scanConfig.PageEditConfig.PageArrangementShowPageNumber = _settingsSharedPreferences.GetEditItem(EditSettings.PageArrangementShowPageNumber).EditValue;
            
            //color settings
            var colorPrimary = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorPrimary);
            scanConfig.ColorConfig.ColorPrimary = new DocutainColor(colorPrimary.LightCircle, colorPrimary.DarkCircle);
            var colorSecondary = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorSecondary);
            scanConfig.ColorConfig.ColorSecondary = new DocutainColor(colorSecondary.LightCircle, colorSecondary.DarkCircle);
            var colorOnSecondary = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorOnSecondary);
            scanConfig.ColorConfig.ColorOnSecondary = new DocutainColor(colorOnSecondary.LightCircle, colorOnSecondary.DarkCircle);
            var colorScanButtonsLayoutBackground = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorScanButtonsLayoutBackground);
            scanConfig.ColorConfig.ColorScanButtonsLayoutBackground = new DocutainColor(colorScanButtonsLayoutBackground.LightCircle, colorScanButtonsLayoutBackground.DarkCircle);
            var colorScanButtonsForeground = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorScanButtonsForeground);
            scanConfig.ColorConfig.ColorScanButtonsForeground = new DocutainColor(colorScanButtonsForeground.LightCircle, colorScanButtonsForeground.DarkCircle);
            var colorScanPolygon = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorScanPolygon);
            scanConfig.ColorConfig.ColorScanPolygon = new DocutainColor(colorScanPolygon.LightCircle, colorScanPolygon.DarkCircle);
            var colorBottomBarBackground = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorBottomBarBackground);
            scanConfig.ColorConfig.ColorBottomBarBackground = new DocutainColor(colorBottomBarBackground.LightCircle, colorBottomBarBackground.DarkCircle);
            var colorBottomBarForeground = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorBottomBarForeground);
            scanConfig.ColorConfig.ColorBottomBarForeground = new DocutainColor(colorBottomBarForeground.LightCircle, colorBottomBarForeground.DarkCircle);
            var colorTopBarBackground = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorTopBarBackground);
            scanConfig.ColorConfig.ColorTopBarBackground = new DocutainColor(colorTopBarBackground.LightCircle, colorTopBarBackground.DarkCircle);
            var colorTopBarForeground = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorTopBarForeground);
            scanConfig.ColorConfig.ColorTopBarForeground = new DocutainColor(colorTopBarForeground.LightCircle, colorTopBarForeground.DarkCircle);

            // alter the onboarding image source if you like
            //scanConfig.OnboardingImageSource = ...

            

            // detailed information about theming possibilities can be found here: https://docs.docutain.com/docs/Xamarin/theming
            //scanConfig.Theme = ...
                   
            //start the document scanner
            bool success = await UI.ScanDocument(this, scanConfig);
            if (success)
                HandleScanResult();
            else
                System.Console.WriteLine("canceled scan process");
        }

        private async void StartPDFImport()
        {
            FileResult pdfFile = null;
            switch (selectedOption)
            {
                case ItemType.PDF_GENERATING:
                    System.Console.WriteLine("Generating a PDF from a file which is already a PDF makes no sense, please scan a document or import an image.");
                    break;
                case ItemType.DATA_EXTRACTION:
                    pdfFile = await FilePicker.PickAsync(new PickOptions
                    {
                        FileTypes = FilePickerFileType.Pdf
                    });
                    if(pdfFile != null)
                        OpenDataResultActivity(pdfFile.FullPath);
                    else
                        System.Console.WriteLine("canceled PDF import");
                    break;
                case ItemType.TEXT_RECOGNITION:
                    pdfFile = await FilePicker.PickAsync(new PickOptions
                    {
                        FileTypes = FilePickerFileType.Pdf
                    });
                    if (pdfFile != null)
                        OpenTextResultActivity(pdfFile.FullPath);
                    else
                        System.Console.WriteLine("canceled PDF import");
                    break;
                default:
                    System.Console.WriteLine("Select an input option first");
                    break;
            }
        }        

        private void StartDataExtraction()
        {
            ShowInputOptionAlert();
        }

        private void StartTextRecognition()
        {
            ShowInputOptionAlert();
        }

        private void StartPDFGenerating()
        {
            ShowInputOptionAlert(false);
        }

        private void ShowInputOptionAlert(bool showPDFImport = true)
        {
            var items = showPDFImport ? new string[] { GetString(Resource.String.input_option_scan), GetString(Resource.String.input_option_image), GetString(Resource.String.input_option_PDF) } :
                new string[] { GetString(Resource.String.input_option_scan), GetString(Resource.String.input_option_image) };
            var builder = new MaterialAlertDialogBuilder(this);
            builder.SetTitle(Resource.String.title_input_option)
                   .SetItems(items, (sender, args) =>
                   {
                       switch (args.Which)
                       {
                           case 0:
                               StartScan(false);
                               break;
                           case 1:
                               StartScan(true);
                               break;
                           case 2:
                               StartPDFImport();
                               break;                          
                       }
                   });
            builder.Create().Show();
        }

        private void GeneratePDF(string filePath)
        {
            Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    // If a filePath is available, it means we have imported a file. If so, we need to load it into the SDK first.
                    if (!DocumentDataReader.LoadFile(filePath))
                    {
                        // An error occurred, get the latest error message.
                        System.Console.WriteLine($"DocumentDataReader.LoadFile failed, last error: {DocutainSDK.LastError}");
                        return;
                    }
                }

                // Define the output file for the PDF.
                var file = new File(FilesDir, "SamplePDF");
                // Generate the PDF from the currently loaded document.
                // The generated PDF also contains the detected text, making the PDF searchable.
                // See https://docs.docutain.com/docs/Xamarin/pdfCreation for more details.
                var pdfFile = Document.WritePDF(file, true, Document.PDFPageFormat.A4);
                if (pdfFile == null)
                {
                    // An error occurred, get the latest error message.
                    System.Console.WriteLine($"DocumentDataReader.loadFile failed, last error: {DocutainSDK.LastError}");
                    return;
                }
                else
                {
                    //display the PDF by using the system's default viewer for demonstration purposes
                    var pdfUri = FileProvider.GetUriForFile(this, "de.docutain.Docutain_SDK_Example_Xamarin_Android.attachments", pdfFile);
                    var intent = new Intent(Intent.ActionView);
                    intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                    intent.SetDataAndType(pdfUri, "application/pdf");
                    try
                    {
                        StartActivity(intent);
                    }
                    catch (ActivityNotFoundException ex)
                    {
                        System.Console.WriteLine("No Activity available for displaying the PDF");
                    }
                }               
            });            
        }

        private void OpenDataResultActivity(string filePath)
        {
            var intent = new Intent(this, typeof(DataResultActivity));
            if (!string.IsNullOrEmpty(filePath))
                intent.PutExtra("filePath", filePath);
            StartActivity(intent);
        }

        private void OpenTextResultActivity(string filePath)
        {
            var intent = new Intent(this, typeof(TextResultActivity));
            if (!string.IsNullOrEmpty(filePath))
                intent.PutExtra("filePath", filePath);
            StartActivity(intent);
        }

        private void ShowLicenseEmptyInfo()
        {
            new MaterialAlertDialogBuilder(this)
                .SetTitle("License empty")
                .SetMessage("A valid license key is required. Please click \"GET LICENSE\" in order to create a free trial license key on our website.")
                .SetPositiveButton("Get License", (sender, args) =>
                {
                    Intent intent = new Intent(Intent.ActionView, Uri.Parse("https://sdk.docutain.com/TrialLicense?Source=1311235"));
                    StartActivity(intent);
                    Finish();
                })
                .SetCancelable(false)
                .Show();
        }

        private void ShowLicenseErrorInfo()
        {
            new MaterialAlertDialogBuilder(this)
                .SetTitle("License error")
                .SetMessage("A valid license key is required. Please contact our support to get an extended trial license.")
                .SetPositiveButton("Contact Support", (sender, args) =>
                {
                    Intent intent = new Intent(Intent.ActionSendto, Uri.Parse("mailto:support.sdk@Docutain.com"));
                    intent.PutExtra(Intent.ExtraSubject, "Trial License Error");
                    intent.PutExtra(Intent.ExtraText, $"Please keep your following trial license key in this e-mail: {licenseKey}");
                    if (intent.ResolveActivity(PackageManager) != null)
                    {
                        StartActivity(intent);
                        Finish();
                    }
                    else
                    {
                        System.Console.WriteLine("No Mail App available, please contact us manually via sdk@Docutain.com");
                    }
                })
                .SetCancelable(false)
                .Show();
        }
    }
}