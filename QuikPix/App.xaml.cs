using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using QuikPix.Core;

namespace QuikPix
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var splashScreen = new SplashScreen("resources/quikpix-noback.png");
            splashScreen.Show(false);

            var main = new Main();
            main.Show();

            //var browser = new MovieViewer();
            //browser.Show();

            splashScreen.Close(TimeSpan.FromSeconds(2));
        }

        public void StartMovieViewer(Uri movieUri)
        {
            var movieViewer = new MovieViewer();
            movieViewer.movieBrowser.Source = movieUri;
        }
    }
}
