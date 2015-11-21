using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;






// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace tileReminder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
      
        
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
       
        private const string JSONFILENAME = "data.json";
        private const string XMLFILENAME = "data.xml";
        
        int id = 1;
        int eventid = 0;
       
        //isolated storage
        private async Task writeJsonAsync(int tid,int eid)
        {
            List<int> ids = new List<int>();
            ids.Add(tid);
            ids.Add(eid);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<TileInfo>));

            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(JSONFILENAME, CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, ids);
            }
            
        }
        //read from isolatedstorage
        private async Task<string> readJsonAsync()
        {
            string content = String.Empty;

            var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(JSONFILENAME);

            using (var reader = new StreamReader(myStream))
            {
                content = await reader.ReadToEndAsync();
            }
            return content;
        }


       
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }
        // TODO solve hub issue  , solve chosen time prolem , the home and about pages , redirect to first page after pinig the tile or faliur cope ? ;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isAppointmentValid = true;
            var appointment = new Windows.ApplicationModel.Appointments.Appointment();
       
            
            // StartTime
            var date = endDate.Date;
            var time = endTime.Time;
            //var timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            //var startTime = new DateTimeOffset(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0, timeZoneOffset);
            //appointment.StartTime = startTime;

            // Subject
            appointment.Subject = name.Text;

            if (appointment.Subject.Length > 255)
            {
                isAppointmentValid = false;
              //  ResultTextBlock.Text = "The subject cannot be greater than 255 characters.";
            }

            // Location
            //appointment.Location = LocationTextBox.Text;
            /*
            if (appointment.Location.Length > 32768)
            {
                isAppointmentValid = false;
                ResultTextBlock.Text = "The location cannot be greater than 32,768 characters.";
            }
             * */

            // Details
            appointment.Details = desciption.Text;

            if (appointment.Details.Length > 1073741823)
            {
                isAppointmentValid = false;
                //ResultTextBlock.Text = "The details cannot be greater than 1,073,741,823 characters.";
            }


            /*
            // All Day
            appointment.AllDay = AllDayCheckBox.IsChecked.Value;

            // Reminder
            if (ReminderCheckBox.IsChecked.Value)
            {
                switch (ReminderComboBox.SelectedIndex)
                {
                    case 0:
                        appointment.Reminder = TimeSpan.FromMinutes(15);
                        break;
                    case 1:
                        appointment.Reminder = TimeSpan.FromHours(1);
                        break;
                    case 2:
                        appointment.Reminder = TimeSpan.FromDays(1);
                        break;
                }
            }

            */

            if (isAppointmentValid)
            {
                var rect = GetElementRect(sender as FrameworkElement);

                // ShowAddAppointmentAsync returns an appointment id if the appointment given was added to the user's calendar.
                // This value should be stored in app data and roamed so that the appointment can be replaced or removed in the future.
                // An empty string return value indicates that the user canceled the operation before the appointment was added.
                String appointmentId = await Windows.ApplicationModel.Appointments.AppointmentManager.ShowAddAppointmentAsync(
                                       appointment, rect, Windows.UI.Popups.Placement.Default);
                // ResultTextBlock.Text = "The event was created successfully and a tile was placed.";
               
                 
                
              //  String tileid = long.Parse(appointmentId).ToString();
               // int tidl = tileid.Length;
                if (appointmentId != null) { 
                //id = id + 1;

                await TileInfoDataSource.AddTile(new TileInfo() 
                {
                    Name = name.Text
                });

                
                string tileid = TileInfoDataSource.latestID.ToString();
                
                //pining the tile
                Uri square150x150Logo = new Uri("ms-appx:///Assets/square150x150Tile-sdk.png");

                String tileActivationArguments = tileid + " WasPinnedAt=" + DateTime.Now.ToLocalTime().ToString();

                SecondaryTile secondaryTile = new SecondaryTile(tileid,
                                                                    name.Text,
                                                                     tileActivationArguments,
                                                                     square150x150Logo,
                                                                     TileSize.Square150x150);

                 secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
                 secondaryTile.VisualElements.ForegroundText = ForegroundText.Dark;
                 await secondaryTile.RequestCreateAsync();
            }
            }
           // myHub.
        }
        private async void Butto_emptyn_Click(object sender, RoutedEventArgs e)
        {
            await TileInfoDataSource.AddTile(new TileInfo()
            {
                Name = ""
            });


            string tileid = TileInfoDataSource.latestID.ToString();
            Uri square150x150Logo = new Uri("ms-appx:///Assets/square150x150Tile-sdk.png");

            string tileActivationArguments = tileid + " WasPinnedAt=" + DateTime.Now.ToLocalTime().ToString();

            SecondaryTile secondaryTile = new SecondaryTile(tileid,
                                                                "",
                                                                tileActivationArguments,
                                                                square150x150Logo,
                                                                TileSize.Wide310x150);

            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;

            secondaryTile.VisualElements.ForegroundText = ForegroundText.Dark;
            await secondaryTile.RequestCreateAsync();

        }
       
        //send the button as a frimware
        private Windows.Foundation.Rect GetElementRect(FrameworkElement element)
        {
            Windows.UI.Xaml.Media.GeneralTransform buttonTransform = element.TransformToVisual(null);
            Windows.Foundation.Point point = buttonTransform.TransformPoint(new Windows.Foundation.Point());
            return new Windows.Foundation.Rect(point, new Windows.Foundation.Size(element.ActualWidth, element.ActualHeight));
        }

        private void HomeAppBarButton_Click(object sender, RoutedEventArgs e)
        {
           Frame.Navigate(typeof(BlankPage1));
        }

        private void aboutAppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private TextBox name;
        private DatePicker endDate;
        private TimePicker endTime;
        private TextBox desciption;

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            TileInfo newTile = new TileInfo();

            foreach(var item in (sender as Grid).Children)
            {
                if(item is TextBox)
                {
                    TextBox t = item as TextBox;
                    if(t.Name == "SubjectTextBox")
                    {
                        name = t;
                    }
                    else if(t.Name == "DetailsTextBox")
                    {
                        desciption = t;
                    }
                }

                if(item is TimePicker)
                {
                    endTime = item as TimePicker;
                }

                if(item is DatePicker)
                {
                    endDate = item as DatePicker;
                }
            }

        }

   

    }
}
