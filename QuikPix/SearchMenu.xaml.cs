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
using dragonz.actb.core;
using System.ComponentModel;

namespace QuikPix
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class SearchMenu : Window
    {
        public string Query { get; private set; }
        public BackgroundWorker searchWorker = new BackgroundWorker();


        public SearchMenu() {
            InitializeComponent();

            buttonSearch.Click += new RoutedEventHandler(buttonSearch_Click);
            buttonGoBack.Click += new RoutedEventHandler(buttonGoBack_Click);

            txtSearch.KeyUp += new KeyEventHandler(txtSearch_KeyUp);

            lbSuggestion.KeyUp += new KeyEventHandler(lbSuggestion_KeyUp);
            txtSearch.Focus();

            searchWorker.DoWork += new DoWorkEventHandler(searchWorker_DoWork);
            searchWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(searchWorker_RunWorkerCompleted);
        }

        void lbSuggestion_KeyUp(object sender, KeyEventArgs e)
        {
            if (lbSuggestion.Items.Count > 0 && e.Key == Key.Enter)
            {
                txtSearch.Text = (lbSuggestion.SelectedItem as string);
                lbSuggestion.ItemsSource = null;
                lbSuggestion.Visibility = Visibility.Collapsed;
                buttonSearch.Focus();
            }
        }

        void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right && txtSearch.CaretIndex == txtSearch.Text.Length)
            {
                e.Handled = true;
                buttonSearch.Focus();
            }
            else if (e.Key == Key.Enter && txtSearch.Text.Length > 0)
            {
                buttonSearch_Click(null, null);
            }
            else if (e.Key == Key.Down && lbSuggestion.Visibility == Visibility.Visible)
            {
                lbSuggestion.Focus();
                lbSuggestion.SelectedIndex = 0;
            }
            else
            {
                if (txtSearch.Text.Length > 2 && !searchWorker.IsBusy)
                    searchWorker.RunWorkerAsync(txtSearch.Text);
            }
        }

        void searchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = QuikPixCore.Current.Autocomplete(e.Argument as string);
        }

        void searchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lbSuggestion.ItemsSource = e.Result as string[];

            if (lbSuggestion.Items.Count > 0)
                lbSuggestion.Visibility = Visibility.Visible;
            else
                lbSuggestion.Visibility = Visibility.Collapsed;
        }


        void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.Query = txtSearch.Text;
            this.Hide();
        }

        void buttonGoBack_Click(object sender, RoutedEventArgs e)
        {
            this.Query = null;
            this.Hide();
        }


    }
}

