# Azure Function (Timer Trigger)

El siguiente proceso es un ejemplo muy básico que corresponse a una Azure Function tipo TimerTrigger que se estará ejecutando cada 5 minutos
y como caso de prueba hará incremento de 1 dia a la fecha correspondiente del campo LimitValidateAt si y solo si el esado sea 1 y las fechas coincidan con la actual.  

```c#

  // Formato de fecha CRON --> "0 */5 * * * *" 5 MINUTOS

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
```
