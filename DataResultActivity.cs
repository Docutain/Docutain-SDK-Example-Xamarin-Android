using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Android.Util;
using Google.Android.Material.TextField;
using Org.Json;
using System.Text.RegularExpressions;
using Docutain.SDK.Xamarin.Android;
using System.Threading.Tasks;

namespace Docutain_SDK_Example_Xamarin_Android
{
    [Activity(Label = "DataResultActivity")]
    public class DataResultActivity : AppCompatActivity
    {
        private TextInputLayout textViewName1;
        private TextInputLayout textViewName2;
        private TextInputLayout textViewName3;
        private TextInputLayout textViewZipcode;
        private TextInputLayout textViewCity;
        private TextInputLayout textViewStreet;
        private TextInputLayout textViewPhone;
        private TextInputLayout textViewCustomerID;
        private TextInputLayout textViewIBAN;
        private TextInputLayout textViewBIC;
        private TextInputLayout textViewDate;
        private TextInputLayout textViewAmount;
        private TextInputLayout textViewInvoiceId;
        private TextInputLayout textViewReference;
        private TextInputLayout textViewPaymentState;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_data_result);

            // Initialize the textViews which display the detected data
            textViewName1 = FindViewById<TextInputLayout>(Resource.Id.textField_name1);
            textViewName2 = FindViewById<TextInputLayout>(Resource.Id.textField_name2);
            textViewName3 = FindViewById<TextInputLayout>(Resource.Id.textField_name3);
            textViewZipcode = FindViewById<TextInputLayout>(Resource.Id.textField_zipcode);
            textViewCity = FindViewById<TextInputLayout>(Resource.Id.textField_city);
            textViewStreet = FindViewById<TextInputLayout>(Resource.Id.textField_street);
            textViewPhone = FindViewById<TextInputLayout>(Resource.Id.textField_phone);
            textViewCustomerID = FindViewById<TextInputLayout>(Resource.Id.textField_customerId);
            textViewIBAN = FindViewById<TextInputLayout>(Resource.Id.textField_IBAN);
            textViewBIC = FindViewById<TextInputLayout>(Resource.Id.textField_BIC);
            textViewDate = FindViewById<TextInputLayout>(Resource.Id.textField_Date);
            textViewAmount = FindViewById<TextInputLayout>(Resource.Id.textField_Amount);
            textViewInvoiceId = FindViewById<TextInputLayout>(Resource.Id.textField_InvoiceId);
            textViewReference = FindViewById<TextInputLayout>(Resource.Id.textField_reference);
            textViewPaymentState = FindViewById<TextInputLayout>(Resource.Id.textField_paymentState);

            // Analyze the document and load the detected data
            LoadData();
        }

        private void LoadData()
        {
            Task.Run(() =>
            {
                var filePath = Intent.GetStringExtra("filePath");
                if (filePath != null)
                {
                    // If a uri is available, it means we have imported a file. If so, we need to load it into the SDK first
                    if (!DocumentDataReader.LoadFile(filePath))
                    {
                        // An error occurred, get the latest error message
                        System.Console.WriteLine($"DocumentDataReader.LoadFile failed, last error: {DocutainSDK.LastError}");
                        return;
                    }
                }

                // Analyze the currently loaded document and get the detected data
                string analyzeData = DocumentDataReader.Analyze();
                if (string.IsNullOrEmpty(analyzeData))
                {
                    // No data detected
                    return;
                }

                // Detected data is returned as JSON, so serializing the data in order to extract the key value pairs
                // see https://docs.docutain.com/docs/Xamarin/dataExtraction
                var jsonArray = new JSONObject(analyzeData);
                var address = new JSONObject(jsonArray.GetString("Address"));
                var name1 = address.GetString("Name1");
                var name2 = address.GetString("Name2");
                var name3 = address.GetString("Name3");
                var zipcode = address.GetString("Zipcode");
                var city = address.GetString("City");
                var street = address.GetString("Street");
                var phone = address.GetString("Phone");
                var customerId = address.GetString("CustomerId");
                var bank = address.GetJSONArray("Bank");
                var IBAN = "";
                var BIC = "";

                // TODO: handle multiple bank accounts
                if (bank.Length() > 0)
                {
                    var object1 = bank.GetJSONObject(0);
                    IBAN = object1.GetString("IBAN");
                    BIC = object1.GetString("BIC");
                }

                var date = jsonArray.GetString("Date");
                var amount = jsonArray.GetString("Amount");
                var invoiceId = jsonArray.GetString("InvoiceId");
                var reference = jsonArray.GetString("Reference");
                var paid = jsonArray.OptString("PaymentState");

                // Load the text into the textfields if value is detected
                RunOnUiThread(() =>
                {
                    if (!string.IsNullOrEmpty(name1))
                    {
                        textViewName1.EditText.Text = name1;
                        textViewName1.Visibility = ViewStates.Visible;
                    }
                    if (!string.IsNullOrEmpty(name2))
                    {
                        textViewName2.EditText.Text = name2;
                        textViewName2.Visibility = ViewStates.Visible;
                    }
                    if (!string.IsNullOrEmpty(name3))
                    {
                        textViewName3.EditText.Text = name3;
                        textViewName3.Visibility = ViewStates.Visible;
                    }
                    if (!string.IsNullOrEmpty(zipcode))
                    {
                        textViewZipcode.EditText.Text = zipcode;
                        textViewZipcode.Visibility = ViewStates.Visible;
                    }
                    if (!string.IsNullOrEmpty(city))
                    {
                        textViewCity.EditText.Text = city;
                        textViewCity.Visibility = ViewStates.Visible;
                    }
                    if (!string.IsNullOrEmpty(street))
                    {
                        textViewStreet.EditText.Text = street;
                        textViewStreet.Visibility = ViewStates.Visible;
                    }
                    if (!string.IsNullOrEmpty(phone))
                    {
                        textViewPhone.EditText.Text = phone;
                        textViewPhone.Visibility = ViewStates.Visible;
                    }
                    if (!string.IsNullOrEmpty(customerId))
                    {
                        textViewCustomerID.EditText.Text = customerId;
                        textViewCustomerID.Visibility = ViewStates.Visible;
                    }
                    if (!string.IsNullOrEmpty(IBAN))
                    {
                        var regex = new Regex(".{4}");
                        textViewIBAN.EditText.Text = regex.Replace(IBAN, "$0 ");
                        textViewIBAN.Visibility = ViewStates.Visible;
                    }
                    if (!string.IsNullOrEmpty(BIC))
                    {
                        textViewBIC.EditText.Text = BIC;
                        textViewBIC.Visibility = ViewStates.Visible;
                    }
                    if (!string.IsNullOrEmpty(date))
                    {
                        textViewDate.EditText.Text = date;
                        textViewDate.Visibility = ViewStates.Visible;
                    }
                    if (!string.IsNullOrEmpty(amount) && amount != "0.00")
                    {
                        textViewAmount.EditText.Text = amount;
                        textViewAmount.Visibility = ViewStates.Visible;
                    }
                    if (!string.IsNullOrEmpty(invoiceId))
                    {
                        textViewInvoiceId.EditText.Text = invoiceId;
                        textViewInvoiceId.Visibility = ViewStates.Visible;
                    }
                    if (!string.IsNullOrEmpty(reference))
                    {
                        textViewReference.EditText.Text = reference;
                        textViewReference.Visibility = ViewStates.Visible;
                    }
                    if (!string.IsNullOrEmpty(paid))
                    {
                        textViewPaymentState.EditText.Text = paid;
                        textViewPaymentState.Visibility = ViewStates.Visible;
                    }
                    FindViewById<ProgressBar>(Resource.Id.activity_indicator).Visibility = ViewStates.Gone;
                });
            });            
        }
    }
}