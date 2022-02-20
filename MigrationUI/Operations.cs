using ConsoleTables;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MigrationUI
{
    public class Operations
    {
        public bool DuplicateValueInsertion { get; set; }   
        public void InputValidation(out int startNo , out int endNo)
        {
            Console.Write("Enter Start Number : ");
            var userInput = Console.ReadLine();

            while (!int.TryParse(userInput, out startNo) || startNo < 0 || startNo > 1000000)
            {
                Console.WriteLine("Enter Valid Number!\n");
                Console.Write("Enter Start Number : ");
                userInput = Console.ReadLine();
            }

            Console.Write("Enter End Number : ");
            userInput = Console.ReadLine();

            while (!int.TryParse(userInput, out endNo) || endNo < 0 || endNo > 1000000)
            {
                Console.WriteLine("Enter Valid Number!\n");
                Console.Write("Enter End Number : ");
                userInput = Console.ReadLine();
            }
        }
        public async Task AddDataToTable(int startNo , int endNo,CancellationToken cancellationToken)
        {
            int totalRows = endNo - startNo + 1;
            int temp = startNo;
            List<Task> tasks = new List<Task>();    
            if (!cancellationToken.IsCancellationRequested)
            {
                while (totalRows != 0)
                {
                    var dataInSourceTable = new List<SourceTable>();
                    try
                    {
                        int temp1 = (totalRows > 100) ? (temp + 100) : (endNo + 1);
                        var newContext = new MigrationContext();
                        dataInSourceTable = await newContext.SourceTable.Where(s => (s.Id >= temp && s.Id < temp1)).Distinct().ToListAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Something unusual found..." + ex.Message);
                    }
                    if (temp > 100)
                    {
                        temp += 100;
                        totalRows -= 100;
                    }
                    else
                    {
                        totalRows = 0;
                    }
                    tasks.Add(AddEntries(dataInSourceTable, cancellationToken));
                }

                    Task task = Task.WhenAll(tasks);
                    try
                    {
                        await task;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                    }
                
            }
        }
        public async Task AddEntries(List<SourceTable> sourceTableData , CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                var IdFromSource = sourceTableData.Select(s => s.Id);
                var DestinationTableData = new List<DestinationTable>();
                foreach (var item in sourceTableData)
                {
                    DestinationTableData.Add(new DestinationTable()
                    {
                        SourceTableId = item.Id,
                        Sum = await Sum(item.FirstNumber, item.SecondNumber)
                    });
                }
                try
                {
                    using (var newContext = new MigrationContext())
                    {
                        await newContext.DestinationTable.AddRangeAsync(DestinationTableData,cancellationToken);
                        newContext.SaveChanges();
                        DuplicateValueInsertion = false;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Exception while migrating data"+ex.Message);
                    DuplicateValueInsertion = true;
                }
            }
        }
        public async Task<int> Sum(int firstNumber, int secondNumber)
        {
            await Task.Delay(50);
            return firstNumber + secondNumber;
        }
        public MigrationStatusTable NewMigration(int startNo,int endNo)
        {
            var newMigrationStatusTableEntry = new MigrationStatusTable()
            {
                StartingNum = startNo,
                EndingNum = endNo,
                Status = "Running"
            };
            using (var newContext = new MigrationContext())
            {
                newContext.MigrationStatus.Add(newMigrationStatusTableEntry);
                newContext.SaveChanges();
                return newMigrationStatusTableEntry;
            }
        }
        public void ShowStatus()
        {
            var newContext = new MigrationContext();
            var MigrataionTableData = newContext.MigrationStatus.ToList();

            var table = new ConsoleTable("Id", "Start", "End", "Status");
            foreach (var status in MigrataionTableData)
            {
                table.AddRow($"{status.Id}", $"{status.StartingNum}", $"{status.EndingNum}", $"{status.Status}");
            }
            Console.WriteLine(table);
        }
        public void CanceledMigration(int instanceId)
        {
            using (var newContext = new MigrationContext())
            {
                var migrationTableData = newContext.MigrationStatus.Find(instanceId);
                migrationTableData.Status = "Canceled";
                newContext.MigrationStatus.Attach(migrationTableData);
                newContext.SaveChanges();
            }
        }
        public void CompletedMigration(int startNo , int endNo, int instanceId)
        {
            Console.WriteLine($"The migration from {startNo} to {endNo} has been completed.");
            using (var newContext = new MigrationContext())
            {
                var migrationTableData= newContext.MigrationStatus.Find(instanceId);
                migrationTableData.Status = "Completed";
                newContext.MigrationStatus.Attach(migrationTableData);
                newContext.SaveChanges();
            }
        }
    }
}
