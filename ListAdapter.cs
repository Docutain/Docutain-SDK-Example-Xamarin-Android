using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docutain_SDK_Example_Xamarin_Android
{

    public class ListItem
    {
        public int Title { get; set; }
        public int Subtitle { get; set; }
        public int Icon { get; set; }
        public ItemType Type { get; set; }
    }

    public enum ItemType
    {
        NONE,
        DOCUMENT_SCAN,
        DATA_EXTRACTION,
        TEXT_RECOGNITION,
        PDF_GENERATING,
        SETTINGS
    }

    internal class ListAdapter : RecyclerView.Adapter
    {
        Context context;
        private readonly Action<ListItem> onItemClicked;

        private List<ListItem> items = new List<ListItem>
        {
            new ListItem
            {
               Title = Resource.String.title_document_scan,
               Subtitle = Resource.String.subtitle_document_scan,
               Icon = Resource.Drawable.document_scanner,
               Type = ItemType.DOCUMENT_SCAN
            },
            new ListItem
            {
               Title = Resource.String.title_data_extraction,
               Subtitle = Resource.String.subtitle_data_extraction,
               Icon = Resource.Drawable.data_extraction,
               Type = ItemType.DATA_EXTRACTION
            },
            new ListItem
            {
               Title = Resource.String.title_text_recognition,
               Subtitle = Resource.String.subtitle_text_recognition,
               Icon = Resource.Drawable.ocr,
               Type = ItemType.TEXT_RECOGNITION
            },
            new ListItem
            {
               Title = Resource.String.title_PDF_generating,
               Subtitle = Resource.String.subtitle_PDF_generating,
               Icon = Resource.Drawable.pdf,
               Type = ItemType.PDF_GENERATING
            },
            new ListItem {
               Title = Resource.String.title_settings,
               Subtitle = Resource.String.subtitle_settings,
               Icon = Resource.Drawable.settings,
               Type = ItemType.SETTINGS
            }

        };

        public ListAdapter(Action<ListItem> onItemClicked)
        {
            this.onItemClicked = onItemClicked;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item, parent, false);
            return new ListItemViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListItemViewHolder itemViewHolder = (ListItemViewHolder)holder;
            ListItem item = items[position];

            itemViewHolder.Title.SetText(item.Title);
            itemViewHolder.Secondary.SetText(item.Subtitle);
            itemViewHolder.Icon.SetImageResource(item.Icon);

            holder.ItemView.Click += (sender, e) => onItemClicked(item);
        }

        public override int ItemCount => items.Count;
    }
}