using MyStudy.Model;
using MyStudy.Service;
using MyStudy.View;
using System.Collections.ObjectModel;


namespace MyStudy
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        // Property to track button selection state
        public bool IsAddButtonSelected { get; set; }

        public ObservableCollection<Schedule> Schedules { get; set; }

      
        public MainPage()
        {
            InitializeComponent();
       _databaseService = new DatabaseService();
            Schedules = new ObservableCollection<Schedule>();
            BindingContext = this;

            // Subscribe to the RefreshSchedules message
            // Subscribe to the RefreshSchedules message
            MessagingCenter.Subscribe<AddEditSchedulePage>(this, "RefreshSchedules", async (sender) =>
            {
                await LoadSchedulesAsync(); // Refresh schedules when a message is received
            });

            // Load schedules initially
            Task.Run(LoadSchedulesAsync);
        }
        private int _totalEntries;
        public int TotalEntries
        {
            get => _totalEntries;
            set
            {
                if (_totalEntries != value)
                {
                    _totalEntries = value;
                    OnPropertyChanged();  // Notifies the UI of the property change
                }
            }
        }

        private async Task LoadSchedulesAsync()
        {
            try
            {
                var schedules = await _databaseService.GetSchedulesAsync();

                // Update the ObservableCollection
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Schedules.Clear();
                    foreach (var schedule in schedules)
                    {
                        Schedules.Add(schedule);
                    }

                    // Update the TotalEntries property
                    TotalEntries = Schedules.Count;
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load schedules: {ex.Message}", "OK");
            }
        }



        private async void OnAddScheduleClicked(object sender, EventArgs e)
        {
            // Toggle the selection state of the button
            IsAddButtonSelected = !IsAddButtonSelected;

            // Navigate to the Add/Edit Schedule page
            await Navigation.PushAsync(new AddEditSchedulePage());
            LoadSchedulesAsync(); // Refresh after adding
        }

        private async void OnEditSchedule(object sender, EventArgs e)
        {
            var id = (int)((Button)sender).CommandParameter;
            await Navigation.PushAsync(new AddEditSchedulePage(id));
            LoadSchedulesAsync(); // Refresh after editing
        }

        private async void OnDeleteSchedule(object sender, EventArgs e)
        {
            var id = (int)((Button)sender).CommandParameter;

            // Confirmation before deleting
            var confirm = await DisplayAlert("Delete Schedule", "Are you sure you want to delete this schedule?", "Yes", "No");
            if (!confirm)
                return;

            try
            {
                await _databaseService.DeleteScheduleAsync(id);
                await LoadSchedulesAsync(); // Refresh after deleting
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to delete schedule: {ex.Message}", "OK");
            }
        }
    }
}

