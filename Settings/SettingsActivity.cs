using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docutain_SDK_Example_Xamarin_Android
{
    [Activity(Label = "SettingsActivity")]
    public class SettingsActivity : AppCompatActivity
    {
        private SettingsSharedPreferences _settingsSharedPreferences;
        private SettingsMultiViewsAdapter _settingsAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_settings);
            InitToolbar();
            InitRestButton();

            _settingsSharedPreferences = new SettingsSharedPreferences(this);

            if (_settingsSharedPreferences.IsEmpty())
                _settingsSharedPreferences.DefaultSettings();

            _settingsAdapter = new SettingsMultiViewsAdapter(PreparingData(), _settingsSharedPreferences);
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.settings_recycler_view);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            recyclerView.SetAdapter(_settingsAdapter);

        }

        private List<SettingsMultiItems> PreparingData()
        {
            var colorPrimaryItem = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorPrimary);
            var colorSecondaryItem = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorSecondary);
            var colorOnSecondaryItem = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorOnSecondary);
            var colorScanButtonsLayoutBackground = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorScanButtonsLayoutBackground);
            var colorScanButtonsForeground = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorScanButtonsForeground);
            var colorScanPolygon = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorScanPolygon);
            var colorBottomBarBackground = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorBottomBarBackground);
            var colorBottomBarForeground = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorBottomBarForeground);
            var colorTopBarBackground = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorTopBarBackground);
            var colorTopBarForeground = _settingsSharedPreferences.GetColorItem(ColorSettings.ColorTopBarForeground);

            var allowCaptureModeSetting = _settingsSharedPreferences.GetScanItem(ScanSettings.AllowCaptureModeSetting).CheckValue;
            var autoCapture = _settingsSharedPreferences.GetScanItem(ScanSettings.AutoCapture).CheckValue;
            var autoCrop = _settingsSharedPreferences.GetScanItem(ScanSettings.AutoCrop).CheckValue;
            var multiPage = _settingsSharedPreferences.GetScanItem(ScanSettings.MultiPage).CheckValue;
            var preCaptureFocus = _settingsSharedPreferences.GetScanItem(ScanSettings.PreCaptureFocus).CheckValue;
            var defaultScanFilter = _settingsSharedPreferences.GetScanFilterItem(ScanSettings.DefaultScanFilter).ScanValue;

            var allowPageFilter = _settingsSharedPreferences.GetEditItem(EditSettings.AllowPageFilter).EditValue;
            var allowPageRotation = _settingsSharedPreferences.GetEditItem(EditSettings.AllowPageRotation).EditValue;
            var allowPageArrangement = _settingsSharedPreferences.GetEditItem(EditSettings.AllowPageArrangement).EditValue;
            var allowPageCropping = _settingsSharedPreferences.GetEditItem(EditSettings.AllowPageCropping).EditValue;
            var pageArrangementShowDeleteButton = _settingsSharedPreferences.GetEditItem(EditSettings.PageArrangementShowDeleteButton).EditValue;
            var pageArrangementShowPageNumber = _settingsSharedPreferences.GetEditItem(EditSettings.PageArrangementShowPageNumber).EditValue;

            var items = new List<SettingsMultiItems>
            {
                new SettingsMultiItems.TitleItem(Resource.String.color_settings),
                new SettingsMultiItems.ColorItem(Resource.String.color_primary_title,
                                                    Resource.String.color_primary_subtitle,
                                                    colorPrimaryItem.LightCircle, colorPrimaryItem.DarkCircle,
                                                    ColorSettings.ColorPrimary),
                new SettingsMultiItems.ColorItem(Resource.String.color_secondary_title,
                                                    Resource.String.color_secondary_subtitle,
                                                    colorSecondaryItem.LightCircle, colorSecondaryItem.DarkCircle,
                                                    ColorSettings.ColorSecondary),
                new SettingsMultiItems.ColorItem(Resource.String.color_on_secondary_title,
                                                    Resource.String.color_on_secondary_subtitle,
                                                    colorOnSecondaryItem.LightCircle, colorOnSecondaryItem.DarkCircle,
                                                    ColorSettings.ColorOnSecondary),
                new SettingsMultiItems.ColorItem(Resource.String.color_scan_layout_title,
                                                    Resource.String.color_scan_layout_subtitle,
                                                    colorScanButtonsLayoutBackground.LightCircle,
                                                    colorScanButtonsLayoutBackground.DarkCircle,
                                                    ColorSettings.ColorScanButtonsLayoutBackground),
                new SettingsMultiItems.ColorItem(Resource.String.color_scan_foreground_title,
                                                    Resource.String.color_scan_foreground_subtitle,
                                                    colorScanButtonsForeground.LightCircle, colorScanButtonsForeground.DarkCircle,
                                                    ColorSettings.ColorScanButtonsForeground),
                new SettingsMultiItems.ColorItem(Resource.String.color_scan_polygon_title,
                                                    Resource.String.color_scan_polygon_subtitle,
                                                    colorScanPolygon.LightCircle, colorScanPolygon.DarkCircle,
                                                    ColorSettings.ColorScanPolygon),
                new SettingsMultiItems.ColorItem(Resource.String.color_bottom_bar_background_title,
                                                    Resource.String.color_bottom_bar_background_subtitle,
                                                    colorBottomBarBackground.LightCircle, colorBottomBarBackground.DarkCircle,
                                                    ColorSettings.ColorBottomBarBackground),
                new SettingsMultiItems.ColorItem(Resource.String.color_bottom_bar_forground_title,
                                                    Resource.String.color_bottom_bar_forground_subtitle,
                                                    colorBottomBarForeground.LightCircle, colorBottomBarForeground.DarkCircle,
                                                    ColorSettings.ColorBottomBarForeground),
                new SettingsMultiItems.ColorItem(Resource.String.color_top_bar_background_title,
                                                    Resource.String.color_top_bar_background_subtitle,
                                                    colorTopBarBackground.LightCircle, colorTopBarBackground.DarkCircle,
                                                    ColorSettings.ColorTopBarBackground),
                new SettingsMultiItems.ColorItem(Resource.String.color_top_bar_forground_title,
                                                    Resource.String.color_top_bar_forground_subtitle,
                                                    colorTopBarForeground.LightCircle, colorTopBarForeground.DarkCircle,
                                                    ColorSettings.ColorTopBarForeground),
                new SettingsMultiItems.TitleItem(Resource.String.scan_settings),
                new SettingsMultiItems.ScanSettingsItem(Resource.String.capture_mode_setting_title,
                                                            Resource.String.capture_mode_setting_subtitle,
                                                            allowCaptureModeSetting,
                                                            ScanSettings.AllowCaptureModeSetting),
                new SettingsMultiItems.ScanSettingsItem(Resource.String.auto_capture_setting_title,
                                                            Resource.String.auto_capture_setting_subtitle,
                                                            autoCapture,
                                                            ScanSettings.AutoCapture),
                new SettingsMultiItems.ScanSettingsItem(Resource.String.auto_crop_setting_title,
                                                            Resource.String.auto_crop_setting_subtitle,
                                                            autoCrop,
                                                            ScanSettings.AutoCrop),
                new SettingsMultiItems.ScanSettingsItem(Resource.String.multi_page_setting_title,
                                                            Resource.String.multi_page_setting_subtitle,
                                                            multiPage,
                                                            ScanSettings.MultiPage),
                new SettingsMultiItems.ScanSettingsItem(Resource.String.pre_capture_setting_title,
                                                            Resource.String.pre_capture_setting_subtitle,
                                                            preCaptureFocus,
                                                            ScanSettings.PreCaptureFocus),
                new SettingsMultiItems.ScanFilterItem(Resource.String.default_scan_setting_title,
                                                            Resource.String.default_scan_setting_subtitle,
                                                            defaultScanFilter,
                                                            ScanSettings.DefaultScanFilter),
                new SettingsMultiItems.TitleItem(Resource.String.edit_settings),
                new SettingsMultiItems.EditItem(Resource.String.allow_page_filter_setting_title,
                                                    Resource.String.allow_page_filter_setting_subtitle,
                                                    allowPageFilter,
                                                    EditSettings.AllowPageFilter),
                new SettingsMultiItems.EditItem(Resource.String.allow_page_rotation_setting_title,
                                                    Resource.String.allow_page_rotation_setting_subtitle,
                                                    allowPageRotation,
                                                    EditSettings.AllowPageRotation),
                new SettingsMultiItems.EditItem(Resource.String.allow_page_arrangement_setting_title,
                                                    Resource.String.allow_page_arrangement_setting_subtitle,
                                                    allowPageArrangement,
                                                    EditSettings.AllowPageArrangement),
                new SettingsMultiItems.EditItem(Resource.String.allow_page_cropping_setting_title,
                                                    Resource.String.allow_page_cropping_setting_subtitle,
                                                    allowPageCropping,
                                                    EditSettings.AllowPageCropping),
                new SettingsMultiItems.EditItem(Resource.String.page_arrangement_delete_setting_title,
                                                    Resource.String.page_arrangement_delete_setting_subtitle,
                                                    pageArrangementShowDeleteButton,
                                                    EditSettings.PageArrangementShowDeleteButton),
                new SettingsMultiItems.EditItem(Resource.String.page_arrangement_number_setting_title,
                                                    Resource.String.page_arrangement_number_setting_subtitle,
                                                    pageArrangementShowPageNumber,
                                                    EditSettings.PageArrangementShowPageNumber)
            };

            return items;
        }


        private void InitRestButton()
        {
            var restButton = FindViewById<AppCompatButton>(Resource.Id.rest_button_settings);
            restButton.Click += (sender, e) =>
            {
                _settingsSharedPreferences.DefaultSettings();
                _settingsAdapter.Refresh(PreparingData().ToList());
            };
        }

        private void InitToolbar()
        {
            SupportActionBar?.SetTitle(Resource.String.title_settings);
            SupportActionBar?.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar?.SetDisplayShowHomeEnabled(true);
            SupportActionBar?.SetHomeAsUpIndicator(Resource.Drawable.baseline_arrow_back_24);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}