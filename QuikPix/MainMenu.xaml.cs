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
    public partial class MainMenu : Window
    {
        public string MessageText { get; private set; }

        private Title currentTitle = null;




        public MainMenu(string titleId)
        {
            InitializeComponent();

            buttonPlay.Click += new RoutedEventHandler(buttonPlay_Click);
            buttonRate.Click += new RoutedEventHandler(buttonRate_Click);
            buttonQueue.Click += new RoutedEventHandler(buttonQueue_Click);

            buttonGoBack.Click += new RoutedEventHandler(buttonGoBack_Click);
            buttonSearch.Click += new RoutedEventHandler(buttonSearch_Click);
            buttonExit.Click += new RoutedEventHandler(buttonExit_Click);

            if (string.IsNullOrWhiteSpace(titleId))
            {
                buttonPlay.Opacity = 0.5;
                buttonPlay.IsEnabled = false;

                buttonQueue.Opacity = 0.5;
                buttonQueue.IsEnabled = false;

                buttonRate.Opacity = 0.5;
                buttonRate.IsEnabled = false;

                buttonGoBack.Focus();
            }
            else
            {
                buttonPlay.Focus();

                currentTitle = QuikPixCore.Current.GetTitle(titleId);
            }
        }

        void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            var search = new SearchMenu();

            this.Opacity = 0.2;
            search.ShowDialog();

            if (!string.IsNullOrWhiteSpace(search.Query))
            {
                this.MessageText = string.Format("The following results are for your search for '{0}'.", search.Query);
                this.Hide();
            }
            else
            {
                this.Opacity = 1.0;
                buttonSearch.Focus();
            }
        }

        void buttonRate_Click(object sender, RoutedEventArgs e)
        {
            if (currentTitle != null)
            {
                var rating = new RateMenu();

                this.Opacity = 0.2;
                rating.ShowDialog();

                if (!string.IsNullOrWhiteSpace(rating.Rating)) {
                    this.MessageText = string.Format("Your rating of '{0}' has been set for '{1}'.", rating.Rating, currentTitle.RegularTitle);
                    this.Hide();
                } else {
                    this.Opacity = 1.0;
                    buttonRate.Focus();
                }
            }
        }

        void buttonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (currentTitle != null)
            {
                var idPart = currentTitle.GetIdPart();
                if (string.IsNullOrWhiteSpace(idPart))
                {
                    this.MessageText = string.Format("There was an error getting the movie details for '{0}'. Please try another movie or contact Netflix for support.", currentTitle.RegularTitle);
                    this.Hide();
                }

                var movieUrl = string.Format("http://movies.netflix.com/WiPlayer?movieid={0}", idPart);
                this.Topmost = false;

                // Start Movie
                var proc = Process.Start(movieUrl);
                this.Hide();
            }
        }

        void buttonQueue_Click(object sender, RoutedEventArgs e)
        {
            if (currentTitle != null)
            {
                Thread.Sleep(500);
                this.MessageText = string.Format("'{0}' has been saved in the queue.", currentTitle.RegularTitle);
                this.Hide();
            }
        }

        void buttonGoBack_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Hide();
        }

        void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }



    }
}
