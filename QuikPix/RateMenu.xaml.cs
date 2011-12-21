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
using System.Windows.Shapes;
using System.Threading;
using QuikPix.Core.Catalog;
using QuikPix.Core;
using System.Diagnostics;

namespace QuikPix
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class RateMenu : Window
    {

        public string Rating { get; private set; }

        public RateMenu() {
            InitializeComponent();

            this.Rating = null;

            buttonGoBack.Click += new RoutedEventHandler(buttonGoBack_Click);


            buttonRate1.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(buttonRate1_PreviewGotKeyboardFocus);
            buttonRate2.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(buttonRate2_PreviewGotKeyboardFocus);
            buttonRate3.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(buttonRate3_PreviewGotKeyboardFocus);
            buttonRate4.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(buttonRate4_PreviewGotKeyboardFocus);
            buttonRate5.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(buttonRate5_PreviewGotKeyboardFocus);

            buttonRate1.Click += new RoutedEventHandler(buttonRate1_Click);
            buttonRate2.Click += new RoutedEventHandler(buttonRate2_Click);
            buttonRate3.Click += new RoutedEventHandler(buttonRate3_Click);
            buttonRate4.Click += new RoutedEventHandler(buttonRate4_Click);
            buttonRate5.Click += new RoutedEventHandler(buttonRate5_Click);

            buttonRate3.Focus();
        }

        void buttonGoBack_Click(object sender, RoutedEventArgs e)
        {
            this.Rating = null;
            this.Hide();
        }

        void HiliteButtons(int rating)
        {
            var offBitmap = new BitmapImage();
            offBitmap.BeginInit();
            offBitmap.UriSource = new Uri("/QuikPix;component/Images/menu_rate_off.png", UriKind.Relative);
            offBitmap.EndInit();

            var onBitmap = new BitmapImage();
            onBitmap.BeginInit();
            onBitmap.UriSource = new Uri("/QuikPix;component/Images/menu_rate.png", UriKind.Relative);
            onBitmap.EndInit();

            imageRate2.Source = offBitmap;
            imageRate3.Source = offBitmap;
            imageRate4.Source = offBitmap;
            imageRate5.Source = offBitmap;

            if (rating > 1)
                imageRate2.Source = onBitmap;

            if (rating > 2)
                imageRate3.Source = onBitmap;

            if (rating > 3)
                imageRate4.Source = onBitmap;

            if (rating > 4)
                imageRate5.Source = onBitmap;
        }

        void buttonRate1_Click(object sender, RoutedEventArgs e)
        {
            this.Rating = "1 - Hated It";
            this.Hide();
        }

        void buttonRate1_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            HiliteButtons(1);
        }

        void buttonRate2_Click(object sender, RoutedEventArgs e)
        {
            this.Rating = "2 - Didn't Like It";
            this.Hide();
        }

        void buttonRate2_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            HiliteButtons(2);
        }

        void buttonRate3_Click(object sender, RoutedEventArgs e)
        {
            this.Rating = "3 - Liked It";
            this.Hide();
        }

        void buttonRate3_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            HiliteButtons(3);
        }

        void buttonRate4_Click(object sender, RoutedEventArgs e)
        {
            this.Rating = "4 - Really Liked It";
            this.Hide();
        }

        void buttonRate4_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            HiliteButtons(4);
        }

        void buttonRate5_Click(object sender, RoutedEventArgs e)
        {
            this.Rating = "5 - Loved It";
            this.Hide();
        }

        void buttonRate5_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            HiliteButtons(5);
        }
    }
}

