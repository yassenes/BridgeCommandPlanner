using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BridgeCommandPlanner.SQL
{
    public class CommandPlanner
    {
        private DbConnection dbConnection_;
        private DateTime lastDateExecute_;
        public CommandPlanner(DbConnection dbConnection)
        {
            dbConnection_ = dbConnection;
            lastDateExecute_ = new DateTime(
                                    1970,
                                    3,
                                    1,
                                    1,
                                    1,
                                    1,
                                    1,
                                    lastDateExecute_.Kind);
        }
        public async Task Read()
        {
            try
            {
                using (var connection = dbConnection_.Get())
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT ID, CommandID, ISNULL(Data1, ''), ISNULL(DateToExecute, '') FROM _BridgeCommands_Planned WHERE DateToExecute <= GETDATE() AND DateToExecute >= @Date";
                        command.Parameters.AddWithValue("@Date", lastDateExecute_);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int id = reader.GetInt32(0);
                                int commandID = reader.GetInt32(1);
                                string data1 = reader.GetString(2);
                                DateTime date = reader.GetDateTime(3);

                                if (commandID == 100)
                                {
                                    Console.WriteLine($"[{id}] {DateTime.Now.ToString()}: Query [ {data1} ]");

                                    using (var plannedCommand = connection.CreateCommand())
                                    {
                                        plannedCommand.CommandText = "EXECUTE (@query)";
                                        plannedCommand.Parameters.AddWithValue("@query", data1);

                                        await plannedCommand.ExecuteNonQueryAsync();

                                        lastDateExecute_ = date;

                                        using (var deletePlannedCommand = connection.CreateCommand())
                                        {
                                            deletePlannedCommand.CommandText = "DELETE FROM _BridgeCommands_Planned WHERE ID = @ID";
                                            deletePlannedCommand.Parameters.AddWithValue("@ID", id);

                                            await deletePlannedCommand.ExecuteNonQueryAsync();
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
