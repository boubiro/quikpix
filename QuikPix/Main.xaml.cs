using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QuikPix.Core;
using QuikPix.Core.TransferObjects;
using System.Threading;
using System.Diagnostics;
using QuikPix.Views;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace QuikPix
{

    public enum CurrentStates : short
    {
        RootGenres = 0,
        SubGenres = 1,
        Titles = 2,
        Reviews = 3
    }

    public enum DisplayActions : short 
    {
        Select = 0,
        Return = 1
    }

    public class DisplayState {
        public DisplayState() { CurrentState = CurrentStates.RootGenres; }

        public CurrentStates CurrentState { get; set; }
        public string RootGenreTitle { get; set; }
        public string RootGenreId { get; set; }
        public string SubGenreTitle { get; set; }
        public string SubGenreId { get; set; }
        public string Title { get; set; }
        public string TitleId { get; set; }

        public void ClearTitle() {
            Title = "";
            TitleId = "";
        }

        public void ClearSubGenre() {
            ClearTitle();
            SubGenreTitle = "";
            SubGenreId = "";
        }

        public void ClearRootGenre() {
            ClearSubGenre();
            RootGenreTitle = "";
            RootGenreId = "";
        }
    }

    public class DisplayRequest
    {
        public DisplayState DisplayState { get; set; }
        public DisplayActions DisplayAction { get; set; }
        public IDisplayItem CurrentDisplayItem { get; set; }
    }

    public class DisplayResponse
    {
        public IEnumerable<IDisplayItem> DisplayItems { get; set; }
        public string HeaderText { get; set; }
    }

    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private BackgroundWorker displayWorker = new BackgroundWorker();
        private Storyboard errorMessageStoryboard = new Storyboard();
        private DisplayState displayState = new DisplayState();
        private bool captureKeyEvents = true;
        
        public Main()
        {
            this.InitializeComponent();

            LayoutRoot.Visibility = Visibility.Hidden;

            displayWorker.DoWork += new DoWorkEventHandler(UpdateDisplayWorker);
            displayWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UpdateDisplayWorkerCompleted);

            EventManager.RegisterClassHandler(typeof(Window),
                 Keyboard.KeyUpEvent, new KeyEventHandler(keyUp), true);

            ItemScroller.CurrentDataItemChanged += new ChangedDataItem(ItemScroller_Changed);

            UpdateDisplay(CurrentStates.RootGenres, DisplayActions.Return);
        }

        private void UpdateDisplayWorker(object sender, DoWorkEventArgs e)
        {
            var request = e.Argument as DisplayRequest;
            if (request == null)
                throw new InvalidCastException("Unable to cast argument to DisplayRequest.");

            var response = new DisplayResponse();

            switch (request.DisplayState.CurrentState)
            {
                case CurrentStates.RootGenres:
                    request.DisplayState.ClearRootGenre();
                    response.HeaderText = "Select a Genre";
                    response.DisplayItems = QuikPixCore.Current.GetRootGenres();
                    break;
                case CurrentStates.SubGenres:
                    var dataItemA = request.CurrentDisplayItem as GenreDisplayItem;
                    if (request.DisplayAction == DisplayActions.Select && dataItemA != null)
                    {
                        request.DisplayState.RootGenreId = dataItemA.GenreId;
                        request.DisplayState.RootGenreTitle = dataItemA.Title;
                    }
                    request.DisplayState.ClearSubGenre();

                    response.HeaderText = string.Format("{0}: Select a Sub-Genre", request.DisplayState.RootGenreTitle);
                    response.DisplayItems = QuikPixCore.Current.GetSubGenres(request.DisplayState.RootGenreId);
                    break;
                case CurrentStates.Titles:
                    var dataItemB = ItemScroller.CurrentDataItem as GenreDisplayItem;
                    if (request.DisplayAction == DisplayActions.Select && dataItemB != null)
                    {
                        request.DisplayState.SubGenreId = dataItemB.GenreId;
                        request.DisplayState.SubGenreTitle = dataItemB.Title;
                    }
                    request.DisplayState.ClearTitle();

                    response.HeaderText = string.Format("{0}: {1}: Select a Title", request.DisplayState.RootGenreTitle, request.DisplayState.SubGenreTitle);
                    response.DisplayItems = QuikPixCore.Current.GetTitles(request.DisplayState.SubGenreId);
                    break;
                default:
                    Beep();
                    break;
            }

            e.Result = response;
        }

        private void UpdateDisplayWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null && !e.Cancelled) {
                var response = (e.Result as DisplayResponse);

                ItemScroller.BindData(response.DisplayItems);

                var itemCount = response.DisplayItems.Count();
                if (itemCount > 0)
                {
                    if (displayState.CurrentState == CurrentStates.RootGenres)
                        ResultSize.Text = string.Format("Choose from {0} Genre{1}", itemCount, (itemCount != 1 ? "s" : ""));
                    else if (displayState.CurrentState == CurrentStates.SubGenres)
                        ResultSize.Text = string.Format("Choose from {0} Sub-Genre{1}", itemCount, (itemCount != 1 ? "s" : ""));
                    else if (displayState.CurrentState == CurrentStates.Titles)
                        ResultSize.Text = string.Format("Choose from {0} Title{1}", itemCount, (itemCount != 1 ? "s" : ""));
                    else
                        ResultSize.Text = "";
                }
                breadcrumb.Text = response.HeaderText;

                LayoutRoot.Visibility = System.Windows.Visibility.Visible;
            }

            HideLoading();
        }

        private void ShowErrorMessage(string message)
        {
            Beep();
            ErrorMessageText.Text = message;
            
            //http://wpf-samples.blogspot.com/2007/01/programmatic-fade-in-out-sample.html
            // Create a storyboard to contain the animations.
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0.0, 1.0, new TimeSpan(0, 0, 1));
            Storyboard.SetTargetName(fadeInAnimation, ErrorMessage.Name);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(Control.OpacityProperty));
            errorMessageStoryboard.Children.Add(fadeInAnimation);

            DoubleAnimation delayAnimation = new DoubleAnimation(1.0, 1.0, new TimeSpan(0, 0, 24));
            Storyboard.SetTargetName(delayAnimation, ErrorMessage.Name);
            Storyboard.SetTargetProperty(delayAnimation, new PropertyPath(Control.OpacityProperty));
            errorMessageStoryboard.Children.Add(delayAnimation);

            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1.0, 0.0, new TimeSpan(0, 0, 5));
            Storyboard.SetTargetName(fadeOutAnimation, ErrorMessage.Name);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(Control.OpacityProperty));
            errorMessageStoryboard.Children.Add(fadeOutAnimation);

            // Begin the storyboard
            errorMessageStoryboard.Begin(this);
        }

        private void HideErrorMessage()
        {
            errorMessageStoryboard.Remove();
            ErrorMessage.Opacity = 0.0;
        }




        private void FadeIn(string name)
        {
            //http://wpf-samples.blogspot.com/2007/01/programmatic-fade-in-out-sample.html
            // Create a storyboard to contain the animations.
            Storyboard storyboard = new Storyboard();
            TimeSpan duration = new TimeSpan(0, 0, 0,0,250 );

            // Create a DoubleAnimation to fade the not selected option control
            DoubleAnimation animation = new DoubleAnimation();

            animation.From = 0.0;
            animation.To = 1.0;
            animation.Duration = new Duration(duration);
            // Configure the animation to target de property Opacity
            Storyboard.SetTargetName(animation, name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));
            // Add the animation to the storyboard
            storyboard.Children.Add(animation);

            // Begin the storyboard
            storyboard.Begin(this);
        }

        private void FadeOut(string name)
        {
            //http://wpf-samples.blogspot.com/2007/01/programmatic-fade-in-out-sample.html
            // Create a storyboard to contain the animations.
            Storyboard storyboard = new Storyboard();
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 750);

            // Create a DoubleAnimation to fade the not selected option control
            DoubleAnimation animation = new DoubleAnimation();

            animation.From = 1.0;
            animation.To = 0.0;
            animation.Duration = new Duration(duration);
            // Configure the animation to target de property Opacity
            Storyboard.SetTargetName(animation, name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));
            // Add the animation to the storyboard
            storyboard.Children.Add(animation);

            // Begin the storyboard
            storyboard.Begin(this);
        }

        private void ShowLoading()
        {
            this.FadeIn(LoadingSpinner.Name);
        }

        private void HideLoading()
        {
            this.FadeOut(LoadingSpinner.Name);
            if (LoadingCatalog.Opacity > 0.0)
                this.FadeOut(LoadingCatalog.Name);
        }

        private void UpdateDisplay(CurrentStates newState, DisplayActions displayAction)
        {
            ShowLoading();
            if (!displayWorker.IsBusy)
            {
                displayState.CurrentState = newState;

                var request = new DisplayRequest()
                {
                    DisplayState = displayState,
                    CurrentDisplayItem = ItemScroller.CurrentDataItem,
                    DisplayAction = displayAction
                };

                displayWorker.RunWorkerAsync(request);
            }
            else
            {
                Beep();
            }
        }

        void ItemScroller_Changed(object sender, EventArgs e)
        {
            if (displayState.CurrentState == CurrentStates.RootGenres || displayState.CurrentState == CurrentStates.SubGenres)
            {
                MiniTitlesPanel.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(Object state)
                {
                    MiniTitles.Visibility = Visibility.Visible;
                    TitleDetail.Visibility = Visibility.Collapsed;

                    var dataItem = ((sender as ItemScroller).CurrentDataItem as GenreDisplayItem);
                    if (dataItem != null)
                    {
                        MiniTitlesHeader.Text = string.Format("Top {0} Titles", dataItem.Title);
                        MiniTitlesPanel.Children.Clear();

                        var margin = new Thickness(0.0, 0.0, 10.0, 0.0);
                        var boxArtUris = dataItem.MiniTitles.Select(x => x.BoxArt).Distinct();

                        foreach (var boxArtUri in boxArtUris)
                        {
                            var bi = new BitmapImage();
                            bi.BeginInit();
                            bi.UriSource = new Uri(boxArtUri, UriKind.Absolute);
                            bi.EndInit();

                            if (bi.Height > 0.0)
                                MiniTitlesPanel.Children.Add(new Image() { Source = bi, Margin = margin });
                        }
                    }
                    return null;
                }), null);
            }
            else if (displayState.CurrentState == CurrentStates.Titles)
            {
                TitleDetail.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(Object state)
                {
                    MiniTitles.Visibility = Visibility.Hidden;
                    TitleDetail.Visibility = Visibility.Visible;

                    var dataItem = ((sender as ItemScroller).CurrentDataItem as TitleDisplayItem);

                    TitleDetail.DataContext = dataItem;

                    return null;
                }), null);
            }
        }

        public void StartCapturingKeyEvents()
        {
            captureKeyEvents = true;
        }

        public void StopCapturingKeyEvents()
        {
            captureKeyEvents = false;
        }

        private bool duplicateMenu = false;
        private void keyUp(object sender, KeyEventArgs e)
        {
            if (captureKeyEvents)
            {
                e.Handled = true;

                if (e.Key == Key.Left)
                    ItemScroller.ShiftLeft();
                else if (e.Key == Key.Right)
                    ItemScroller.ShiftRight();
                else if (e.Key == Key.Up)
                {
                    HideErrorMessage();
                    switch (displayState.CurrentState)
                    {
                        case CurrentStates.SubGenres:
                            UpdateDisplay(CurrentStates.RootGenres, DisplayActions.Return);
                            break;
                        case CurrentStates.Titles:
                            UpdateDisplay(CurrentStates.SubGenres, DisplayActions.Return);
                            break;
                        case CurrentStates.Reviews:
                            UpdateDisplay(CurrentStates.Titles, DisplayActions.Return);
                            break;
                        default:
                            ShowErrorMessage("Unable to go up, please choose a genre.");
                            break;
                    }
                }
                else if (e.Key == Key.Down)
                {
                    HideErrorMessage();
                    switch (displayState.CurrentState)
                    {
                        case CurrentStates.RootGenres:
                            UpdateDisplay(CurrentStates.SubGenres, DisplayActions.Select);
                            break;
                        case CurrentStates.SubGenres:
                            UpdateDisplay(CurrentStates.Titles, DisplayActions.Select);
                            break;
                        case CurrentStates.Titles:
                            ShowErrorMessage("Unable to go down, press menu to play movie.");
                            break;
                        default:
                            Beep();
                            break;
                    }
                }
                else if (e.Key == Key.Enter)
                {
                    if (!duplicateMenu)
                    {
                        duplicateMenu = true;
                        string titleId = null;

                        if (displayState.CurrentState >= CurrentStates.Titles)
                            titleId = (ItemScroller.CurrentDataItem as TitleDisplayItem).TitleId;

                        var menu = new MainMenu(titleId);
                        menu.ShowInTaskbar = false;
                        StopCapturingKeyEvents();
                        menu.ShowDialog();
                        if (!string.IsNullOrWhiteSpace(menu.MessageText))
                            this.ShowErrorMessage(menu.MessageText);
                        menu.Close();
                        StartCapturingKeyEvents();
                    }
                    else
                    {
                        duplicateMenu = false;
                    }
                }
            }
        }

        //http://stackoverflow.com/questions/2751686/how-can-i-execute-a-non-blocking-system-beep
        private void Beep()
        {
            new Thread(() => Console.Beep()).Start();
        }
    }
}