using Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MigrationUI
{
    public class Program : Operations
    {
        public static async Task Main()
        {
            var operation = new Operations();
            Console.WriteLine("***********************Data Migration Tool***********************");
            ConsoleKeyInfo Operation;
            do
            {
                var cancellationToken = new CancellationTokenSource();
                var token = cancellationToken.Token;
                int startNo , endNo ;
                Console.WriteLine("Enter Starrting and End Numbers");
                int counter = 0;
                do
                {
                    if (counter != 0)
                    {
                        Console.WriteLine("----------------------------------------------------");
                        Console.WriteLine("End Number must be greater than Start Number");
                        Console.WriteLine("----------------------------------------------------");
                    }
                    operation.InputValidation(out startNo , out endNo );
                    counter++;
                }while(startNo > endNo);
                Console.WriteLine("----------------------------------------------------");
                Console.WriteLine("Type CANCEL for Cancelling Migration.");
                Console.WriteLine("----------------------------------------------------");
                Console.WriteLine("Type STATUS to know Status of Migration done. ");
                Console.WriteLine("----------------------------------------------------");
                MigrationStatusTable migrationStatusTableData = operation.NewMigration(startNo , endNo );
                int instanceId = migrationStatusTableData.Id;
                Task addDataToTable = operation.AddDataToTable(startNo, endNo, token);
                Task cancelStatus = Task.Run(() =>
                {
                    while (true)
                    {
                        if (!addDataToTable.IsCompleted)
                        {
                            string str = Console.ReadLine();
                            if (str.Equals("CANCEL", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("\nMigration Canceled\n");
                                cancellationToken.Cancel();
                                break;
                            }
                            else if (str.Equals("STATUS", StringComparison.OrdinalIgnoreCase))
                            {
                                operation.ShowStatus();
                            }
                        }
                    }
                });
                Task task = await Task.WhenAny(addDataToTable, cancelStatus);
                if(operation.DuplicateValueInsertion || (task.IsCompleted && token.IsCancellationRequested))
                {
                    operation.CanceledMigration(instanceId);
                }
                else
                {
                    operation.CompletedMigration(startNo , endNo , instanceId);
                }
                Console.WriteLine("Press any key to Continue...");
                Console.WriteLine("Press Escape key to Exit...");
                Operation = Console.ReadKey();                  
            }while (Operation.Key != ConsoleKey.Escape);

            Console.WriteLine("Exiting Application........");
        }
    }
}
