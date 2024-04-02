using System.Text.RegularExpressions;
using Hitachi_task.CsvHandler;
using Hitachi_task.Exceptions;


namespace Hitachi_task;

class Program
{
    static void Main(string[] args)
    {
        Start(out string pathToFolder, out string senderEmail, out string password, out string receiverEmail);
        var csvDict = CsvDeserializer.Csv(pathToFolder); //change this before commiting
        CsvReader csvReader = new CsvReader(csvDict);
        var bestDaysDict = csvReader.BestDays();
        CsvWriter csvWriter = new CsvWriter(bestDaysDict);
        csvWriter.CreateCsv();
        Console.ReadKey();
    }

    static void Start(out string pathToFolder, out string senderEmail, out string password, out string receiverEmail)
    {
        Console.WriteLine("Enter a path to the folder which contains the csv files");
        while (true)
        {
            try
            {
                pathToFolder = Console.ReadLine();
                if (string.IsNullOrEmpty(pathToFolder))
                {
                    throw new CustomException("The path cannot be empty! Please enter a valid path");
                }
                if (Directory.Exists(pathToFolder))
                {
                    var filenames = Directory.GetFiles(pathToFolder);
                    foreach (var filename in filenames)
                    {
                        string fileExtension = Path.GetExtension(filename);
                        if (fileExtension != ".csv")
                        {
                            throw new CustomException("There is a file in the folder which is not of type csv! Please delete it and relaunch the application");
                        }
                    }
                    break;
                }
            }
            catch (CustomException e)
            {
                Console.WriteLine(e.Message);
                if (e.Message == "There is a file in the folder which is not of type csv! Please delete it and relaunch the application")
                {
                    Console.WriteLine("Press any key to exit the app . . .");
                    Console.ReadKey(true);
                    Environment.Exit(1);
                }
            }
        }

        Console.WriteLine("Enter sender email address");
        while (true)
        {
            try
            {
                senderEmail = Console.ReadLine();
                if (string.IsNullOrEmpty(senderEmail))
                {
                    throw new CustomException("The email cannot be empty! Please enter a valid email");
                }
                else if (!Regex.IsMatch(senderEmail, "^[\\w.+\\-]+@gmail\\.com$"))
                {
                    throw new CustomException("The app only works with gmail addresses! Please enter a gmail address!");
                }
                else
                {
                    break;
                }
            }
            catch (CustomException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        Console.WriteLine("Enter the password for the sender's email");
        ConsoleKeyInfo key;
        password = "";
        while (true)
        {
            try
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    if (string.IsNullOrEmpty(password))
                    {
                        password = "";
                        throw new CustomException("The password cannot be empty! Please enter a valid password");
                    }
                    else
                    {
                        break;
                    }
                }
                else if (key.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += key.KeyChar;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring(0, password.Length - 1);
                        int pos = Console.CursorLeft;
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
            }
            catch (CustomException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        Console.WriteLine("\nEnter the receiver email");
        while (true)
        {
            try
            {
                receiverEmail = Console.ReadLine();
                if (string.IsNullOrEmpty(receiverEmail))
                {
                    throw new CustomException("The receiver email cannot be empty! Please enter a valid email");
                }
                else if (!Regex.IsMatch(receiverEmail, "^[\\w.+\\-]+@gmail\\.com$"))
                {
                    throw new CustomException("The app only works with gmail addresses! Please enter a gmail address!");
                }
                else
                {
                    break;
                }
            }
            catch (CustomException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}