using CommunityToolkit.Mvvm.Input;
using MyStudy.Service;
using MyStudy.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyStudy.View
{
    public partial class AddEditSchedulePage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private int? _scheduleId;

        public ObservableCollection<Schedule> Schedules { get; set; }
        public IRelayCommand AlarmCommand { get; }

        public AddEditSchedulePage(int? scheduleId = null)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _scheduleId = scheduleId;

            Schedules = new ObservableCollection<Schedule>();
            AlarmCommand = new RelayCommand(TriggerAlarm);

            // Load schedules for the alarm check
            LoadSchedulesAsync();

            // If editing an existing schedule, load it
            if (_scheduleId.HasValue)
            {
                LoadScheduleAsync();
            }

            // Start the background timer
            Device.StartTimer(TimeSpan.FromSeconds(30), () =>
            {
                AlarmCommand.Execute(null);
                return true; // Keep the timer running
            });
        }

        private async void LoadScheduleAsync()
        {
            try
            {
                var schedule = await _databaseService.GetScheduleAsync(_scheduleId.Value);

                // Pre-fill the form with the existing schedule data
                TaskNameEntry.Text = schedule.TaskName;
                TaskDescriptionEditor.Text = schedule.TaskDescription;
                DayOfWeekPicker.SelectedItem = schedule.DayOfWeek;
                StartTimePicker.Time = schedule.StartTime;
                EndTimePicker.Time = schedule.EndTime;
                PriorityPicker.SelectedItem = schedule.Priority;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load schedule: {ex.Message}", "OK");
            }
        }

        private async void LoadSchedulesAsync()
        {
            try
            {
                var schedules = await _databaseService.GetSchedulesAsync();
                foreach (var schedule in schedules)
                {
                    Schedules.Add(schedule);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load schedules: {ex.Message}", "OK");
            }
        }

        private async void TriggerAlarm()
        {
            var currentTime = DateTime.Now.TimeOfDay;
            var currentDay = DateTime.Now.DayOfWeek.ToString();

            var matchingSchedule = Schedules.FirstOrDefault(schedule =>
                schedule.DayOfWeek == currentDay &&
                schedule.StartTime.Hours == currentTime.Hours &&
                schedule.StartTime.Minutes == currentTime.Minutes);

            if (matchingSchedule != null)
            {
                await DisplayAlert("Alarm", $"It's time for: {matchingSchedule.TaskName}", "OK");
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Validate the user input
            if (string.IsNullOrWhiteSpace(TaskNameEntry.Text) || StartTimePicker.Time >= EndTimePicker.Time)
            {
                await DisplayAlert("Error", "Task name is required and start time must be before end time.", "OK");
                return;
            }

            var schedule = new Schedule
            {
                TaskName = TaskNameEntry.Text,
                TaskDescription = TaskDescriptionEditor.Text,
                StartTime = StartTimePicker.Time,
                EndTime = EndTimePicker.Time,
                DayOfWeek = DayOfWeekPicker.SelectedItem?.ToString(),
                Priority = PriorityPicker.SelectedItem?.ToString()
            };

            try
            {
                // Save or update the schedule
                if (_scheduleId.HasValue)
                {
                    schedule.Id = _scheduleId.Value;
                    await _databaseService.UpdateScheduleAsync(schedule);
                }
                else
                {
                    await _databaseService.AddScheduleAsync(schedule);
                }

                // Refresh the local collection
                Schedules.Clear();
                LoadSchedulesAsync();

                // Notify MainPage to reload the schedule list
                MessagingCenter.Send(this, "RefreshSchedules");

                // Navigate back to MainPage
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while saving the schedule: {ex.Message}", "OK");
            }
        }
    }
}
