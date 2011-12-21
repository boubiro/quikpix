using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuikPix.Core.TransferObjects;
using System.Windows.Threading;

namespace QuikPix.Views
{
    public delegate void ChangedDataItem(object sender, EventArgs e);

    /// <summary>
    /// Interaction logic for ItemScroller.xaml
    /// </summary>
    public partial class ItemScroller : UserControl
    {
        public const double ItemPadding = 0.0;

        public const double OpacityLevel0 = 1.0;
        public const double BubbleLevel0 = OpacityLevel0;
        public const double OpacityLevel1 = 0.85;
        public const double BubbleLevel1 = OpacityLevel1;
        public const double OpacityLevel2 = 0.70;
        public const double BubbleLevel2 = OpacityLevel2;

        private LinkedList<DisplayItem> displayItems = new LinkedList<DisplayItem>();

        public ItemScroller()
        {
            InitializeComponent();
        }

        public void BindData(IEnumerable<IDisplayItem> displayItems)
        {
            if (displayItems == null)
                throw new ArgumentException();

            ClearDisplayItems();
            foreach (var item in displayItems)
                AddDisplayItem(item);

            var itemCount = displayItems.Count();
            PositionDisplayItems();
        }

        public event ChangedDataItem CurrentDataItemChanged;

        private void OnChanged(EventArgs e)
        {
            if (CurrentDataItemChanged != null)
                CurrentDataItemChanged(this, e);
        }


        public IDisplayItem CurrentDataItem { get; private set; }


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            PositionDisplayItems();
        }

        private void ShowLoadingIndicator()
        {
            //ScrollCanvas.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(delegate(Object state)
            //{
            //    LoadingIndicator.Opacity = 1.0;

            //    return null;
            //}), null);
        }

        private void HideLoadingIndicator()
        {
            //ScrollCanvas.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(delegate(Object state)
            //{
            //    LoadingIndicator.Opacity = 0.0;

            //    return null;
            //}), null);
        }

        private void PositionDisplayItems()
        {
            if (displayItems != null && displayItems.Count > 0)
            {
                ShowLoadingIndicator();
                ScrollCanvas.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(Object state)
                {


                    double primaryHeight = this.ActualHeight;
                    double primaryWidth = primaryHeight / 1.5;
                    double displayCenter = this.ActualWidth / 2;

                    double bubbleWidth =
                        (primaryWidth * BubbleLevel0) +
                        ((primaryWidth * BubbleLevel1) * 2) +
                        ItemPadding * 4;

                    double displayLeft = displayCenter - (bubbleWidth / 2);
                    int centerItemPosition = (int)Math.Ceiling(displayLeft / ((primaryWidth * BubbleLevel2) + ItemPadding)) + 2; // +offsetPosition;
                    displayLeft = displayLeft - ((primaryWidth * BubbleLevel2) + ItemPadding) * ((centerItemPosition - 2));

                    var node = displayItems.First;
                    int displayPosition = (centerItemPosition - 1);

                    while (node != null)
                    {
                        double itemHeight = 0.0, itemWidth = 0.0, itemOpacity = 0.0, itemTop = 0.0;

                        if (displayPosition == 0)
                        {
                            this.CurrentDataItem = (IDisplayItem)node.Value.DataContext;
                            OnChanged(EventArgs.Empty);

                            itemHeight = primaryHeight * BubbleLevel0;
                            itemWidth = primaryWidth * BubbleLevel0;
                            itemOpacity = OpacityLevel0;
                        }
                        else if (displayPosition > 1 || displayPosition < -1)    // Level 2
                        {
                            itemHeight = primaryHeight * BubbleLevel2;
                            itemWidth = primaryWidth * BubbleLevel2;
                            itemOpacity = OpacityLevel2;
                        }
                        else if (displayPosition == 1 || displayPosition == -1) // Level 1
                        {
                            itemHeight = primaryHeight * BubbleLevel1;
                            itemWidth = primaryWidth * BubbleLevel1;
                            itemOpacity = OpacityLevel2;
                        }

                        node.Value.Height = itemHeight;
                        node.Value.Width = itemWidth;
                        node.Value.Opacity = itemOpacity;

                        itemTop = (primaryHeight - itemHeight) / 2;

                        Canvas.SetTop(node.Value, itemTop);

                        if (node.Previous != null)
                            Canvas.SetLeft(node.Value, Canvas.GetLeft(node.Previous.Value) + node.Previous.Value.Width + ItemPadding);
                        else
                            Canvas.SetLeft(node.Value, displayLeft);

                        node.Value.Visibility = System.Windows.Visibility.Visible;

                        node = node.Next;
                        displayPosition--;
                    }

                   return null;
                }), null);

                HideLoadingIndicator();
            }
        }

        public void ShiftLeft()
        {
            if (displayItems != null && displayItems.Count > 0)
            {
                var lastNode = displayItems.Last;
                displayItems.Remove(lastNode);
                displayItems.AddFirst(lastNode);

                PositionDisplayItems();
            }
        }

        public void ShiftRight()
        {
            if (displayItems != null && displayItems.Count > 0)
            {
                var firstNode = displayItems.First;
                displayItems.Remove(firstNode);
                displayItems.AddLast(firstNode);

                PositionDisplayItems();
            }
        }



        private void ClearDisplayItems()
        {
            ScrollCanvas.Children.Clear();
            displayItems.Clear();
        }

        private void AddDisplayItem(IDisplayItem displayItem)
        {
            var item = new DisplayItem(displayItem);

            item.Visibility = System.Windows.Visibility.Hidden;

            ScrollCanvas.Children.Add(item);

            Canvas.SetTop(item, 0);
            Canvas.SetLeft(item, 0);

            displayItems.AddLast(item);
        }
    }
}

