using Docutain.SDK.Xamarin.Android;
using System;

public abstract class SettingsMultiItems
{
    public class TitleItem : SettingsMultiItems
    {
        public int Title { get; }

        public TitleItem(int title)
        {
            Title = title;
        }
    }

    public class ColorItem : SettingsMultiItems
    {
        public int Title { get; }
        public int Subtitle { get; }
        public string LightCircle { get; }
        public string DarkCircle { get; }
        public ColorSettings ColorKey { get; }

        public ColorItem(int title, int subtitle, string lightCircle, string darkCircle, ColorSettings colorKey)
        {
            Title = title;
            Subtitle = subtitle;
            LightCircle = lightCircle;
            DarkCircle = darkCircle;
            ColorKey = colorKey;
        }
    }

    public class ScanSettingsItem : SettingsMultiItems
    {
        public int Title { get; }
        public int Subtitle { get; }
        public bool CheckValue { get; }
        public ScanSettings ScanKey { get; }

        public ScanSettingsItem(int title, int subtitle, bool checkValue, ScanSettings scanKey)
        {
            Title = title;
            Subtitle = subtitle;
            CheckValue = checkValue;
            ScanKey = scanKey;
        }
    }

    public class ScanFilterItem : SettingsMultiItems
    {
        public int Title { get; }
        public int Subtitle { get; }
        public ScanFilter ScanValue { get; }
        public ScanSettings FilterKey { get; }

        public ScanFilterItem(int title, int subtitle, ScanFilter scanValue, ScanSettings filterKey)
        {
            Title = title;
            Subtitle = subtitle;
            ScanValue = scanValue;
            FilterKey = filterKey;
        }
    }

    public class EditItem : SettingsMultiItems
    {
        public int Title { get; }
        public int Subtitle { get; }
        public bool EditValue { get; set; }
        public EditSettings EditKey { get; }

        public EditItem(int title, int subtitle, bool editValue, EditSettings editKey)
        {
            Title = title;
            Subtitle = subtitle;
            EditValue = editValue;
            EditKey = editKey;
        }
    }
}

