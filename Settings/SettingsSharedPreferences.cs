using Android.Content;
using Android.Content.Res;
using AndroidX.Core.Content;
using Docutain_SDK_Example_Xamarin_Android;
using static Android.Resource;
using System.Runtime.Remoting.Contexts;
using System.Globalization;
using Docutain.SDK.Xamarin.Android;

public enum ColorSettings
{
    ColorPrimary,
    ColorSecondary,
    ColorOnSecondary,
    ColorScanButtonsLayoutBackground,
    ColorScanButtonsForeground,
    ColorScanPolygon,
    ColorBottomBarBackground,
    ColorBottomBarForeground,
    ColorTopBarBackground,
    ColorTopBarForeground
}

public enum ColorType
{
    Light,
    Dark
}

public enum ScanSettings
{
    AllowCaptureModeSetting,
    AutoCapture,
    AutoCrop,
    MultiPage,
    PreCaptureFocus,
    DefaultScanFilter
}

public enum EditSettings
{
    AllowPageFilter,
    AllowPageRotation,
    AllowPageArrangement,
    AllowPageCropping,
    PageArrangementShowDeleteButton,
    PageArrangementShowPageNumber
}
public class SettingsSharedPreferences
{
    private Android.Content.Context _context;
    private readonly ISharedPreferences _sharedPreferences;

    public SettingsSharedPreferences(Android.Content.Context context)
    {
        _context = context;
        _sharedPreferences = _context.GetSharedPreferences(PrefFileName, FileCreationMode.Private);
    }

    private void SaveColorItem(ColorSettings key, string lightColor, string darkColor)
    {
        ISharedPreferencesEditor editor = _sharedPreferences.Edit();
        editor.PutString(key.ToString().ToLower() + LightColorKey, lightColor);
        editor.PutString(key.ToString().ToLower() + DarkColorKey, darkColor);
        editor.Apply();
    }

    public void SaveColorItemLight(string key, string color)
    {
        ISharedPreferencesEditor editor = _sharedPreferences.Edit();
        editor.PutString(key.ToLower() + LightColorKey, color);
        editor.Apply();
    }

    public void SaveColorItemDark(string key, string color)
    {
        ISharedPreferencesEditor editor = _sharedPreferences.Edit();
        editor.PutString(key.ToLower() + DarkColorKey, color);
        editor.Apply();
    }

    public void SaveEditItem(EditSettings key, bool value)
    {
        ISharedPreferencesEditor editor = _sharedPreferences.Edit();
        editor.PutBoolean(key.ToString().ToLower() + EditValueKey, value);
        editor.Apply();
    }

    public void SaveScanItem(ScanSettings key, bool value)
    {
        ISharedPreferencesEditor editor = _sharedPreferences.Edit();
        editor.PutBoolean(key.ToString().ToLower() + ScanValueKey, value);
        editor.Apply();
    }

    public void SaveScanFilterItem(ScanSettings key, int value)
    {
        ISharedPreferencesEditor editor = _sharedPreferences.Edit();
        editor.PutInt(key.ToString().ToLower() + FilterValueKey, value);
        editor.Apply();
    }

    public SettingsMultiItems.ColorItem GetColorItem(ColorSettings key)
    {
        string color1 = _sharedPreferences.GetString(key.ToString().ToLower() + LightColorKey, "");
        string color2 = _sharedPreferences.GetString(key.ToString().ToLower() + DarkColorKey, "");
        return new SettingsMultiItems.ColorItem(0, 0, color1, color2, key);
    }

    public SettingsMultiItems.EditItem GetEditItem(EditSettings key)
    {
        bool checkValue = _sharedPreferences.GetBoolean(key.ToString().ToLower() + EditValueKey, false);
        return new SettingsMultiItems.EditItem(0, 0, checkValue, key);
    }

    public SettingsMultiItems.ScanSettingsItem GetScanItem(ScanSettings key)
    {
        bool scanValue = _sharedPreferences.GetBoolean(key.ToString().ToLower() + ScanValueKey, false);
        return new SettingsMultiItems.ScanSettingsItem(0, 0, scanValue, key);
    }

    public SettingsMultiItems.ScanFilterItem GetScanFilterItem(ScanSettings key)
    {
        int scanValue = _sharedPreferences.GetInt(key.ToString().ToLower() + FilterValueKey, ScanFilter.Illustration.Ordinal());
        return new SettingsMultiItems.ScanFilterItem(0, 0, ScanFilter.Values()[scanValue], key);
    }

    public bool IsEmpty()
    {
        return _sharedPreferences.All.Count == 0;
    }

    public void DefaultSettings()
    {
        DefaultColorPrimary();
        DefaultColorSecondary();
        DefaultColorOnSecondary();
        DefaultColorScanButtonsLayoutBackground();
        DefaultColorScanButtonsForeground();
        DefaultColorScanPolygon();
        DefaultColorBottomBarBackground();
        DefaultColorBottomBarForeground();
        DefaultColorTopBarBackground();
        DefaultColorTopBarForeground();

        SaveScanItem(ScanSettings.AllowCaptureModeSetting, false);
        SaveScanItem(ScanSettings.AutoCapture, true);
        SaveScanItem(ScanSettings.AutoCrop, true);
        SaveScanItem(ScanSettings.MultiPage, true);
        SaveScanItem(ScanSettings.PreCaptureFocus, true);
        SaveScanFilterItem(ScanSettings.DefaultScanFilter, ScanFilter.Illustration.Ordinal());

        SaveEditItem(EditSettings.AllowPageFilter, true);
        SaveEditItem(EditSettings.AllowPageRotation, true);
        SaveEditItem(EditSettings.AllowPageArrangement, true);
        SaveEditItem(EditSettings.AllowPageCropping, true);
        SaveEditItem(EditSettings.PageArrangementShowDeleteButton, false);
        SaveEditItem(EditSettings.PageArrangementShowPageNumber, true);
    }


    private void DefaultColorPrimary(
        string lightColor = null,
        string darkColor = null
    )
    {
        lightColor ??= GetModeColor(Resource.Color.docutain_colorPrimary, false);
        darkColor ??= GetModeColor(Resource.Color.docutain_colorPrimary, true);

        SaveColorItem(ColorSettings.ColorPrimary, lightColor, darkColor);
    }

    private void DefaultColorSecondary(
        string lightColor = null,
        string darkColor = null
    )
    {
        lightColor ??= GetModeColor(Resource.Color.docutain_colorSecondary, false);
        darkColor ??= GetModeColor(Resource.Color.docutain_colorSecondary, true);

        SaveColorItem(ColorSettings.ColorSecondary, lightColor, darkColor);
    }

    private void DefaultColorOnSecondary(
        string lightColor = null,
        string darkColor = null
    )
    {
        lightColor ??= GetModeColor(Resource.Color.docutain_colorOnSecondary, false);
        darkColor ??= GetModeColor(Resource.Color.docutain_colorOnSecondary, true);

        SaveColorItem(ColorSettings.ColorOnSecondary, lightColor, darkColor);
    }

    private void DefaultColorScanButtonsLayoutBackground(
        string lightColor = null,
        string darkColor = null
    )
    {
        lightColor ??= GetModeColor(Resource.Color.docutain_colorScanButtonsLayoutBackground, false);
        darkColor ??= GetModeColor(Resource.Color.docutain_colorScanButtonsLayoutBackground, true);

        SaveColorItem(ColorSettings.ColorScanButtonsLayoutBackground, lightColor, darkColor);
    }

    private void DefaultColorScanButtonsForeground(
        string lightColor = null,
        string darkColor = null
    )
    {
        lightColor ??= GetModeColor(Resource.Color.docutain_colorScanButtonsForeground, false);
        darkColor ??= GetModeColor(Resource.Color.docutain_colorScanButtonsForeground, true);

        SaveColorItem(ColorSettings.ColorScanButtonsForeground, lightColor, darkColor);
    }

    private void DefaultColorScanPolygon(
        string lightColor = null,
        string darkColor = null
    )
    {
        lightColor ??= GetModeColor(Resource.Color.docutain_colorScanPolygon, false);
        darkColor ??= GetModeColor(Resource.Color.docutain_colorScanPolygon, true);

        SaveColorItem(ColorSettings.ColorScanPolygon, lightColor, darkColor);
    }

    private void DefaultColorBottomBarBackground(
        string lightColor = null,
        string darkColor = null
    )
    {
        lightColor ??= GetModeColor(Resource.Color.docutain_colorBottomBarBackground, false);
        darkColor ??= GetModeColor(Resource.Color.docutain_colorBottomBarBackground, true);

        SaveColorItem(ColorSettings.ColorBottomBarBackground, lightColor, darkColor);
    }

    private void DefaultColorBottomBarForeground(
        string lightColor = null,
        string darkColor = null
    )
    {
        lightColor ??= GetModeColor(Resource.Color.docutain_colorBottomBarForeground, false);
        darkColor ??= GetModeColor(Resource.Color.docutain_colorBottomBarForeground, true);

        SaveColorItem(ColorSettings.ColorBottomBarForeground, lightColor, darkColor);
    }

    private void DefaultColorTopBarBackground(
        string lightColor = null,
        string darkColor = null
    )
    {
        lightColor ??= GetModeColor(Resource.Color.docutain_colorTopBarBackground, false);
        darkColor ??= GetModeColor(Resource.Color.docutain_colorTopBarBackground, true);

        SaveColorItem(ColorSettings.ColorTopBarBackground, lightColor, darkColor);
    }

    private void DefaultColorTopBarForeground(
        string lightColor = null,
        string darkColor = null
    )
    {
        lightColor ??= GetModeColor(Resource.Color.docutain_colorTopBarForeground, false);
        darkColor ??= GetModeColor(Resource.Color.docutain_colorTopBarForeground, true);

        SaveColorItem(ColorSettings.ColorTopBarForeground, lightColor, darkColor);
    }


    public  string GetModeColor( int colorResId, bool isNight)
    {
        Configuration newConfig = new Configuration(_context.Resources.Configuration);
        newConfig.UiMode = isNight ? UiMode.NightYes : UiMode.NightNo;

        Android.Content.Context nightModeContext = _context.CreateConfigurationContext(newConfig);
        int color = ContextCompat.GetColor(nightModeContext, colorResId);

        string colorHex = $"#{color:X}".ToUpper();

        return colorHex;
    }

    public const string PrefFileName = "settings_prefs";
    public const string LightColorKey = "_light_color";
    public const string DarkColorKey = "_dark_color";
    public const string EditValueKey = "_edit_value";
    public const string ScanValueKey = "_scan_value";
    public const string FilterValueKey = "_filter_value";
}
