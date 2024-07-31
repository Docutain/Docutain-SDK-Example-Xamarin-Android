using Com.Github.Dhaval2404.Colorpicker;
using Com.Github.Dhaval2404.Colorpicker.Listener;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Docutain_SDK_Example_Xamarin_Android
{
    internal class ColorPickerListener : Java.Lang.Object, IDismissListener, IColorListener
    {
        private readonly TaskCompletionSource<String> tcs;
        private String selectedColor = null;

        public ColorPickerListener(TaskCompletionSource<String> tcs)
        {
            this.tcs = tcs;
        }

        public void OnColorSelected(int color, string colorHex)
        {
            selectedColor = colorHex;
        }

        public void OnDismiss()
        {
            tcs.TrySetResult(selectedColor);
        }
    }

    public static partial class DocutainColorPicker
    {
        static TaskCompletionSource<String> tcs;
        public static async Task<String> PickColor(String defaultColor = null)
        {
            tcs = new TaskCompletionSource<String>();
            var listener = new ColorPickerListener(tcs);
            var dialog = new ColorPickerDialog
                .Builder(Platform.CurrentActivity)
                .SetColorListener(listener)
                .SetDismissListener(listener);
            if (defaultColor != null)
                dialog.SetDefaultColor(defaultColor);
            dialog.Show();
            return await tcs.Task;
        }
    }
}