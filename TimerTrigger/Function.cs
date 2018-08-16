using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace TimerTrigger
{
    public static class Function
    {
        [FunctionName("AddOneDayLimitValidateAt_Example")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            var DBConnectionString = ConfigurationManager.
                ConnectionStrings["sqlserver_azure"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(DBConnectionString))
            {
                connection.Open();
                var query = @" 
                    UPDATE [dbo].[Users] 
                    SET LimitValidateAt = DATEADD(DAY, 1, LimitValidateAt)
                    WHERE CONCAT(YEAR(CreateAt),'-',MONTH(CreateAt)) = CONCAT(YEAR(GETDATE()), '-', MONTH(GETDATE()))
                    AND Status = 1";

                using(SqlCommand cmd = new SqlCommand(query, connection))
                {
                    var rows = await cmd.ExecuteNonQueryAsync();
                    log.Info($"{rows} rows has been updated");
                }
                
            }

                
        }
    }
}
