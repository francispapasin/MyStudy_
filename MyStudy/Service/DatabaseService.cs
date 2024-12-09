using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStudy.Model;


namespace MyStudy.Service
{
    internal class DatabaseService
    {

        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "Schedule.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Schedule>().Wait();
        }

        public Task<List<Schedule>> GetSchedulesAsync()
        {
            return _database.Table<Schedule>().ToListAsync();
        }

        public Task<Schedule> GetScheduleAsync(int id)
        {
            return _database.FindAsync<Schedule>(id);
        }

        public Task<int> AddScheduleAsync(Schedule schedule)
        {
            return _database.InsertAsync(schedule);
        }

        public Task<int> UpdateScheduleAsync(Schedule schedule)
        {
            return _database.UpdateAsync(schedule);
        }

        public Task<int> DeleteScheduleAsync(int id)
        {
            return _database.DeleteAsync<Schedule>(id);
        }
    }
}