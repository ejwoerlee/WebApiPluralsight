using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Windows.Input;
using CountingKsClient.Model;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace CountingKsClient.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        // Basic Authentication In WebAPI -> https://www.c-sharpcorner.com/blogs/basic-authentication-in-webapi
        //https://docs.microsoft.com/en-us/aspnet/web-api/overview/mobile-clients/calling-web-api-from-a-windows-phone-8-application#STEP2

        const string apiUrl = @"http://localhost/CountingKs/Api/Foods";

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<Food> Items { get; private set; }

        public bool IsDataLoaded { get; private set; }

        public ICommand CallServiceCommand { get; private set; }

        public MainViewModel()
        {
            this.Items = new ObservableCollection<Food>();
            CallServiceCommand = new Command(LoadData);
        }
        public void LoadData()
        {
            if (this.IsDataLoaded == false) {
                this.Items.Clear();
                this.Items.Add(new Food() { Id = 0, Description = ""});
                WebClient webClient = new WebClient();               
                webClient.Headers["Accept"] = "application/json";
                
                //webClient.UseDefaultCredentials = false;
                //webClient.Credentials = new NetworkCredential("egbert-jan.woerlee@cgm.com", "???");
                //string authInfo = "name:pass";
                //authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                //webClient.Headers["Authorization"] = "Basic " + authInfo;


                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadCatalogCompleted);
                webClient.DownloadStringAsync(new Uri(apiUrl));
            }
        }

        private void webClient_DownloadCatalogCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                this.Items.Clear();
                if (e.Result != null)
                {
                    var foods = JsonConvert.DeserializeObject<Food[]>(e.Result);
                    int id = 0;
                    foreach (Food food in foods)
                    {
                        this.Items.Add(new Food()
                        {
                            Id = food.Id,
                            Description = food.Description
                        });
                    }

                    this.IsDataLoaded = true;
                }
            }
            catch (Exception ex)
            {
                this.Items.Add(new Food()
                {
                    Id = -1,
                    Description = String.Format("Additional inner exception information: {0}", ex.InnerException.Message)
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
