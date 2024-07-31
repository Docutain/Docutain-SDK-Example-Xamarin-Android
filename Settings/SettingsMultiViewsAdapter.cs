using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Dialog;
using Google.Android.Material.TextField;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Docutain.SDK.Xamarin.Android;

namespace Docutain_SDK_Example_Xamarin_Android
{
    public class SettingsMultiViewsAdapter : RecyclerView.Adapter
    {
        private readonly List<SettingsMultiItems> _items;
        readonly SettingsSharedPreferences _settingsSharedPreferences;

        public SettingsMultiViewsAdapter(List<SettingsMultiItems> items, SettingsSharedPreferences settingsSharedPreferences)
        {
            _items = items;
            _settingsSharedPreferences = settingsSharedPreferences;
        }

        public override int ItemCount => _items.Count;

        public override int GetItemViewType(int position)
        {
            return _items[position] switch
            {
                SettingsMultiItems.TitleItem _ => TYPE_TITLE,
                SettingsMultiItems.ColorItem _ => TYPE_COLOR,
                SettingsMultiItems.ScanSettingsItem _ => TYPE_SCAN_SETTINGS,
                SettingsMultiItems.ScanFilterItem _ => TYPE_SCAN_FILTER,
                SettingsMultiItems.EditItem _ => TYPE_EDIT,
                _ => throw new ArgumentException("Invalid view type")
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            View itemView = viewType switch
            {
                TYPE_TITLE => inflater.Inflate(Resource.Layout.title_view_settings_item, parent, false),
                TYPE_COLOR => inflater.Inflate(Resource.Layout.color_settings_item, parent, false),
                TYPE_SCAN_SETTINGS => inflater.Inflate(Resource.Layout.scan_view_settings_item, parent, false),
                TYPE_SCAN_FILTER => inflater.Inflate(Resource.Layout.scan_filter_settings, parent, false),
                TYPE_EDIT => inflater.Inflate(Resource.Layout.edit_view_settings_item, parent, false),
                _ => throw new ArgumentException("Invalid view type")
            };
            return viewType switch
            {
                TYPE_TITLE => new TitleViewHolder(itemView),
                TYPE_COLOR => new ColorSettingsViewHolder(itemView, _settingsSharedPreferences),
                TYPE_SCAN_SETTINGS => new ScanSettingsViewHolder(itemView, _settingsSharedPreferences),
                TYPE_SCAN_FILTER => new ScanFilterViewHolder(itemView, _settingsSharedPreferences),
                TYPE_EDIT => new EditSettingsViewHolder(itemView, _settingsSharedPreferences),
                _ => throw new ArgumentException("Invalid view type")
            };
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            switch (_items[position])
            {
                case SettingsMultiItems.TitleItem titleItem:
                    ((TitleViewHolder)viewHolder).Bind(titleItem);
                    break;
                case SettingsMultiItems.ColorItem colorItem:
                    ((ColorSettingsViewHolder)viewHolder).Bind(colorItem);
                    break;
                case SettingsMultiItems.ScanSettingsItem scanSettingsItem:
                    ((ScanSettingsViewHolder)viewHolder).Bind(scanSettingsItem);
                    break;
                case SettingsMultiItems.ScanFilterItem scanFilterItem:
                    ((ScanFilterViewHolder)viewHolder).Bind(scanFilterItem);
                    break;
                case SettingsMultiItems.EditItem editItem:
                    ((EditSettingsViewHolder)viewHolder).Bind(editItem);
                    break;
            }
        }

        public void Refresh(List<SettingsMultiItems> newData)
        {
            _items.Clear();
            _items.AddRange(newData);
            NotifyDataSetChanged(); // Notify adapter about the changes
        }

        public class TitleViewHolder : RecyclerView.ViewHolder
        {
            readonly View itemview;
            public TitleViewHolder(View itemView) : base(itemView)
            {
                this.itemview = itemView;
            }

            public void Bind(SettingsMultiItems.TitleItem item)
            {
                TextView textView = itemview.FindViewById<TextView>(Resource.Id.title_view_settings);
                textView.SetText(item.Title);
            }
        }

        public class ColorSettingsViewHolder : RecyclerView.ViewHolder
        {
            readonly View itemview;
            readonly SettingsSharedPreferences _settingsSharedPreferences;
            SettingsMultiItems.ColorItem _colorItem;
            public ColorSettingsViewHolder(View itemView, SettingsSharedPreferences _settingsSharedPreferences) : base(itemView)
            {
                this.itemview = itemView;
                this._settingsSharedPreferences = _settingsSharedPreferences;
            }

            public void Bind(SettingsMultiItems.ColorItem item)
            {
                _colorItem = item;
                TextView title = itemview.FindViewById<TextView>(Resource.Id.title_settings_item);
                TextView subtitle = itemview.FindViewById<TextView>(Resource.Id.subtitle_settings_item);
                View lightView = itemview.FindViewById<View>(Resource.Id.light_circle_view);
                View darkView = itemview.FindViewById<View>(Resource.Id.dark_circle_view);

                var cachedItem = _settingsSharedPreferences.GetColorItem(item.ColorKey);

                CircleView(lightView, cachedItem.LightCircle);
                CircleView(darkView, cachedItem.DarkCircle);

                lightView.Click -= ShowColorPickerDialogLight;
                lightView.Click += ShowColorPickerDialogLight;

                darkView.Click -= ShowColorPickerDialogDark;
                darkView.Click += ShowColorPickerDialogDark;               

                title.SetText(item.Title);
                subtitle.SetText(item.Subtitle);
            }

            private void ShowColorPickerDialogLight(object sender, EventArgs args)
            {
                ColorPickerDialogAsync(sender as View, _colorItem, ColorType.Light, _settingsSharedPreferences);
            }

            private void ShowColorPickerDialogDark(object sender, EventArgs args)
            {
                ColorPickerDialogAsync(sender as View, _colorItem, ColorType.Dark, _settingsSharedPreferences);
            }
        }

        public class ScanSettingsViewHolder : RecyclerView.ViewHolder
        {
            private readonly View itemView;
            private readonly SettingsSharedPreferences _settingsSharedPreferences;
            private SettingsMultiItems.ScanSettingsItem currentItem;

            public ScanSettingsViewHolder(View itemView, SettingsSharedPreferences _settingsSharedPreferences) : base(itemView)
            {
                this.itemView = itemView;
                this._settingsSharedPreferences = _settingsSharedPreferences;
            }

            public void Bind(SettingsMultiItems.ScanSettingsItem item)
            {
                var cachedItem = _settingsSharedPreferences.GetScanItem(item.ScanKey);

                TextView title = itemView.FindViewById<TextView>(Resource.Id.title_scan_settings_item);
                TextView subtitle = itemView.FindViewById<TextView>(Resource.Id.subtitle_scan_settings_item);
                SwitchCompat switchButton = itemView.FindViewById<SwitchCompat>(Resource.Id.switch_scan_settings_item);

                title.SetText(item.Title);
                subtitle.SetText(item.Subtitle);

                switchButton.CheckedChange -= SwitchButton_CheckedChange;
                switchButton.Checked = cachedItem.CheckValue;
                switchButton.CheckedChange += SwitchButton_CheckedChange;

                currentItem = item;
            }

            private void SwitchButton_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                if (currentItem != null)
                {
                    _settingsSharedPreferences.SaveScanItem(currentItem.ScanKey, e.IsChecked);
                }
            }
        }

        public class ScanFilterViewHolder : RecyclerView.ViewHolder
        {
            readonly View itemview;
            readonly SettingsSharedPreferences _settingsSharedPreferences;
            SettingsMultiItems.ScanFilterItem _scanItem;

            public ScanFilterViewHolder(View itemView, SettingsSharedPreferences _settingsSharedPreferences) : base(itemView)
            {
                this.itemview = itemView;
                this._settingsSharedPreferences = _settingsSharedPreferences;
            }

            public void Bind(SettingsMultiItems.ScanFilterItem item)
            {
                _scanItem = item;
                var cachedItem = _settingsSharedPreferences.GetScanFilterItem(item.FilterKey);

                TextView title = itemview.FindViewById<TextView>(Resource.Id.title_filter_settings_item);
                TextView subtitle = itemview.FindViewById<TextView>(Resource.Id.subtitle_filter_settings_item);
                TextInputEditText inputScanFilter = itemview.FindViewById<TextInputEditText>(Resource.Id.input_filter_filter_dialog);

                title.SetText(item.Title);
                subtitle.SetText(item.Subtitle);

                ScanFilter filter = (ScanFilter)cachedItem.ScanValue;
                string filterString = filter.ToString();

                inputScanFilter.Text = filterString;
                //remove existing handlers
                inputScanFilter.Click -= ShowScanFilterDialog;
                inputScanFilter.Click += ShowScanFilterDialog;
            }

            private void ShowScanFilterDialog(object sender, EventArgs args)
            {
                ScanFilterDialog(itemview.Context, sender as TextInputEditText, _scanItem, _settingsSharedPreferences);
            }
        }

        public class EditSettingsViewHolder : RecyclerView.ViewHolder
        {
            private readonly View itemView;
            private readonly SettingsSharedPreferences _settingsSharedPreferences;
            private SettingsMultiItems.EditItem currentItem;

            public EditSettingsViewHolder(View itemView, SettingsSharedPreferences _settingsSharedPreferences) : base(itemView)
            {
                this.itemView = itemView;
                this._settingsSharedPreferences = _settingsSharedPreferences;
            }

            public void Bind(SettingsMultiItems.EditItem item)
            {
                var cachedItem = _settingsSharedPreferences.GetEditItem(item.EditKey);

                TextView title = itemView.FindViewById<TextView>(Resource.Id.title_edit_settings_item);
                TextView subtitle = itemView.FindViewById<TextView>(Resource.Id.subtitle_edit_settings_item);
                SwitchCompat switchButton = itemView.FindViewById<SwitchCompat>(Resource.Id.switch_edit_settings_item);

                title.SetText(item.Title);
                subtitle.SetText(item.Subtitle);

                switchButton.CheckedChange -= SwitchButton_CheckedChange;
                switchButton.Checked = cachedItem.EditValue;
                switchButton.CheckedChange += SwitchButton_CheckedChange;
                currentItem = item;
            }

            private void SwitchButton_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                if (currentItem != null)
                {
                    _settingsSharedPreferences.SaveEditItem(currentItem.EditKey, e.IsChecked);
                }
            }
        }

        private static void ScanFilterDialog(Android.Content.Context context, TextInputEditText inputText, SettingsMultiItems.ScanFilterItem item, SettingsSharedPreferences _settingsSharedPreferences)
        {
            string[] options = {
                context.GetString(Resource.String.auto_option),
                context.GetString(Resource.String.gray_option),
                context.GetString(Resource.String.black_and_white_option),
                context.GetString(Resource.String.original_option),
                context.GetString(Resource.String.text_option),
                context.GetString(Resource.String.auto_2_option),
                context.GetString(Resource.String.illustration_option)
            };
            var builder = new MaterialAlertDialogBuilder(context);
            builder.SetTitle(Resource.String.title_scan_dialog)
                   .SetItems(options, (sender, args) =>
                   {
                       string selectedOption = options[args.Which];
                       inputText.SetText(selectedOption, TextView.BufferType.Normal);
                       _settingsSharedPreferences.SaveScanFilterItem(item.FilterKey, args.Which);
                   })
                   .SetNegativeButton(Resource.String.cancel_scan_dialog, (sender, args) => { })
                   .Create().Show();
        }

        private static async Task ColorPickerDialogAsync(
            View view,
            SettingsMultiItems.ColorItem colorItem,
            ColorType colorType,
            SettingsSharedPreferences _settingsSharedPreferences
            )
        {
            var previousColorItem = _settingsSharedPreferences.GetColorItem(colorItem.ColorKey);
            String pickedColor = await DocutainColorPicker.PickColor(colorType == ColorType.Light ? previousColorItem.LightCircle : previousColorItem.DarkCircle);

            if (pickedColor != null)
            {

                CircleView(view, pickedColor);

                switch (colorType)
                {
                    case ColorType.Light:
                        _settingsSharedPreferences.SaveColorItemLight(
                            colorItem.ColorKey.ToString(),
                           pickedColor
                        );
                        break;
                    case ColorType.Dark:
                        _settingsSharedPreferences.SaveColorItemDark(
                            colorItem.ColorKey.ToString(),
                              pickedColor
                        );
                        break;
                }


            }
        }

        private static void CircleView(View view, string colorHex)
        {
            Color color = Color.ParseColor(colorHex);
            GradientDrawable gradientDrawable = new GradientDrawable();
            gradientDrawable.SetShape(ShapeType.Oval);
            gradientDrawable.SetStroke(2, Color.Gray);
            gradientDrawable.SetColor(color);
            view.SetBackgroundDrawable(gradientDrawable);
        }

        private const int TYPE_TITLE = 0;
        private const int TYPE_COLOR = 1;
        private const int TYPE_SCAN_SETTINGS = 2;
        private const int TYPE_SCAN_FILTER = 3;
        private const int TYPE_EDIT = 4;
        private const int TYPE_REST = 5;
    }
}