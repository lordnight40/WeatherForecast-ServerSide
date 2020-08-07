using Grabber;

using Model;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide.Service
{
    public class DbService
    {
        private readonly string ConnectionString = "Server=localhost;Database=weather;port=3306;User Id=root;password=root;CharSet=utf8";
        MySqlConnection connection;

        public DbService ()
        {
            connection = new MySqlConnection(ConnectionString);
        }

        public async Task Save(List<WeatherRecord> records, string city, string date)
        {
            MySqlCommand command = new MySqlCommand()
            {
                Connection = connection,
                CommandType = System.Data.CommandType.Text
            };

            StringBuilder sql = new StringBuilder("insert into weather.forecast (city, temperature, pressure, humidity, windspeed, precipitation, time, date) values ");

            for (int i = 0; i < records.Count; i++)
            {
                sql.Append($"(@city{i}, @temperature{i}, @pressure{i}, @humidity{i}, @windspeed{i}, @precipitation{i}, @time{i}, @date{i}), ");
            }

            sql.Length -= 2;
            command.CommandText = sql.ToString();
            
            for (int i = 0; i < records.Count; i++)
            {
                command.Parameters.Add(new MySqlParameter($"@city{i}", city));
                command.Parameters.Add(new MySqlParameter($"@temperature{i}", records[i].Temperature));
                command.Parameters.Add(new MySqlParameter($"@pressure{i}", records[i].Pressure));
                command.Parameters.Add(new MySqlParameter($"@humidity{i}", records[i].Humidity));
                command.Parameters.Add(new MySqlParameter($"@windspeed{i}", records[i].WindSpeed));
                command.Parameters.Add(new MySqlParameter($"@precipitation{i}", records[i].Precipiation));
                command.Parameters.Add(new MySqlParameter($"@time{i}", records[i].Time));
                command.Parameters.Add(new MySqlParameter($"@date{i}", date));
            }

            try
            {
                await connection.OpenAsync();

                await command.PrepareAsync();
                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
