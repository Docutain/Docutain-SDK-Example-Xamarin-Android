using Android.Views;
using Android.Widget;
using static AndroidX.RecyclerView.Widget.RecyclerView;

namespace Docutain_SDK_Example_Xamarin_Android
{
    class ListItemViewHolder : ViewHolder
    {

        public ImageView Icon { get; }
        public TextView Title { get; }
        public TextView Secondary { get; }

        public ListItemViewHolder(View itemView) : base(itemView)
        {
            Icon = itemView.FindViewById<ImageView>(Resource.Id.list_item_icon);
            Title = itemView.FindViewById<TextView>(Resource.Id.list_item_text);
            Secondary = itemView.FindViewById<TextView>(Resource.Id.list_item_secondary_text);
        }

        public static ListItemViewHolder Create(ViewGroup parent)
            {
                return new ListItemViewHolder(LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.list_item, parent, false));
            }
        }
    }