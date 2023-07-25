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
        private string licenseKey = "YOUR_LICENSE_KEY_HERE";
        private ItemType selectedOption = ItemType.NONE;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            if (!DocutainSDK.InitSDK(Application, licenseKey))
            {
                //init of Docutain SDK failed, get the last error message
                System.Console.WriteLine("Initialization of the Docutain SDK failed: " + DocutainSDK.LastError);
                //your logic to deactivate access to SDK functionality
                if (licenseKey == "YOUR_LICENSE_KEY_HERE")
                {
                    ShowLicenseEmptyInfo();
                    return;
                }
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
                        StartScan();
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

        private async void StartScan()
        {
            // Define a DocumentScannerConfiguration to alter the scan process and define a custom theme to match your branding
            var scanConfig = new DocumentScannerConfiguration();
            scanConfig.AllowCaptureModeSetting = true; // defaults to false
            scanConfig.PageEditConfig.AllowPageFilter = true; // defaults to true
            scanConfig.PageEditConfig.AllowPageRotation = true; // defaults to true
            // alter the onboarding image source if you like
            //scanConfig.OnboardingImageSource = ...

            // detailed information about theming possibilities can be found here: https://docs.docutain.com/docs/Xamarin/theming
            scanConfig.Theme = Resource.Style.Theme_DocutainSDK;
                   
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

        private async void StartImageImport()
        {
            FileResult imageFile = null;
            switch (selectedOption)
            {
                case ItemType.PDF_GENERATING:
                    imageFile = await FilePicker.PickAsync(new PickOptions
                    {
                        FileTypes = FilePickerFileType.Images
                    });
                    if (imageFile != null)
                        GeneratePDF(imageFile.FullPath);
                    else
                        System.Console.WriteLine("canceled image import");
                    break;
                case ItemType.DATA_EXTRACTION:
                    imageFile = await FilePicker.PickAsync(new PickOptions
                    {
                        FileTypes = FilePickerFileType.Images
                    });
                    if (imageFile != null)
                        OpenDataResultActivity(imageFile.FullPath);
                    else
                        System.Console.WriteLine("canceled image import");
                    break;
                case ItemType.TEXT_RECOGNITION:
                    imageFile = await FilePicker.PickAsync(new PickOptions
                    {
                        FileTypes = FilePickerFileType.Images
                    });
                    if (imageFile != null)
                        OpenTextResultActivity(imageFile.FullPath);
                    else
                        System.Console.WriteLine("canceled image import");
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
                               StartScan();
                               break;
                           case 1:
                               StartImageImport();
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
            Task.Run(async () =>
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
                .SetMessage("A valid license key is required. Please contact us via sdk@Docutain.com to get a trial license.")
                .SetNegativeButton("Cancel", (sender, args) => { })
                .SetPositiveButton("Get License", (sender, args) =>
                {
                    var intent = new Intent(Intent.ActionSendto);
                    intent.SetData(Uri.Parse("mailto:"));
                    intent.PutExtra(Intent.ExtraEmail, new[] { "sdk@Docutain.com" });
                    intent.PutExtra(Intent.ExtraSubject, "Trial License Request");
                    if (intent.ResolveActivity(PackageManager) != null)
                    {
                        StartActivity(intent);
                    }
                    else
                    {
                        System.Console.WriteLine("No Mail App available, please contact us manually via sdk@Docutain.com");
                    }
                })
                .Show();
        }
    }
}