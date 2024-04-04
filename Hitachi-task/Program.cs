using System.Text.RegularExpressions;
using Hitachi_task.CsvHandler;
using Hitachi_task.EmailSender;
using Hitachi_task.Exceptions;

namespace Hitachi_task;

class Program
{
    private static bool _isEnglish = true;
    private static bool _firstTimeLanguageChosen = false;

    static void Main(string[] args)
    {
        if (_firstTimeLanguageChosen == false)
        {
            _firstTimeLanguageChosen = true;
            ChangeLanguage();
        }

        Start(out string pathToFolder, out string senderEmail, out string password, out string receiverEmail);
        var csvDict = CsvDeserializer.Csv(pathToFolder); //change this before commiting
        CsvReader csvReader = new CsvReader(csvDict);
        var bestDaysDict = csvReader.BestDays();
        CsvWriter csvWriter = new CsvWriter(bestDaysDict, _isEnglish);
        var createdCsvPath = csvWriter.CreateCsv();
        Sender emailSender = new Sender(bestDaysDict, _isEnglish);
        emailSender.SendEmail(senderEmail, password, receiverEmail, createdCsvPath);
        Console.WriteLine(_isEnglish
            ? "The app has finished execution. Press any key to exit . . ."
            : "Die Ausführung der App ist beendet. Drücken Sie eine beliebige Taste zum Beenden . . .");
        Console.ReadKey(true);
    }

    static void ChangeLanguage()
    {
        ConsoleKeyInfo key;
        Console.WriteLine(_isEnglish
            ? "Please choose the application UI language.\nPress \"e\" for English and \"g\" for German.\n(note you can always change the language by pressing \"ctrl + l\"), doing so will make you lose your current information!"
            : "Bitte wählen Sie die Sprache der Benutzeroberfläche der Anwendung.\nDrücken Sie \"e\" für Englisch und \"g\" für Deutsch\n(beachten Sie, dass Sie die Sprache jederzeit durch Drücken von \"ctrl + l\" ändern können), sonst gehen Ihre aktuellen Informationen verloren!");
        while (true)
        {
            key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.E)
            {
                _isEnglish = true;
                break;
            }
            else if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.L)
            {
                continue;
            }
            else if (key.Key == ConsoleKey.G)
            {
                _isEnglish = false;
                break;
            }
            else
            {
                Console.WriteLine(_isEnglish
                    ? "\nInvalid symbol! Please type \"e\" for English or \"g\" for German"
                    : "\nUngültiges Symbol! Bitte geben Sie \"e\" für Englisch oder \"g\" für Deutsch ein.");
            }
        }

        Console.WriteLine();
        Console.Clear();
    }

    static void Start(out string pathToFolder, out string senderEmail, out string password, out string receiverEmail)
    {
        string pathMessage = _isEnglish
            ? "Enter the path to the folder which contains the csv files"
            : "Geben Sie den Pfad zu dem Ordner an, der die csv-Dateien enthält";
        string senderEmailMessage =
            _isEnglish ? "Enter the sender email address" : "Geben Sie die Absender-E-Mail-Adresse ein";
        string senderPasswordMessage = _isEnglish
            ? "Enter the password for the sender's email"
            : "Geben Sie das Passwort für die E-Mail des Absenders ein";
        string receiverEmailMessage = _isEnglish ? "Enter the receiver email" : "Geben Sie die Empfänger-E-Mail ein";
        Console.WriteLine(pathMessage);
        pathToFolder = PathToFolderInput();
        Console.WriteLine(senderEmailMessage);
        senderEmail = SenderEmailInput();
        Console.WriteLine(senderPasswordMessage);
        password = PasswordInput();
        Console.WriteLine(receiverEmailMessage);
        receiverEmail = ReceiverEmailInput();
    }

    private static string PathToFolderInput()
    {
        ConsoleKeyInfo key;
        string pathToFolder = "";
        while (true)
        {
            try
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    if (string.IsNullOrEmpty(pathToFolder))
                    {
                        Console.WriteLine();
                        string exceptionMessage = _isEnglish
                            ? "The path cannot be empty! Please enter a valid path"
                            : "Der Pfad darf nicht leer sein! Bitte geben Sie einen gültigen Pfad ein";
                        throw new CustomException(exceptionMessage);
                    }

                    if (Directory.Exists(pathToFolder))
                    {
                        var filenames = Directory.GetFiles(pathToFolder);
                        foreach (var filename in filenames)
                        {
                            string fileExtension = Path.GetExtension(filename);
                            if (fileExtension != ".csv")
                            {
                                Console.WriteLine();
                                string exceptionMessage = _isEnglish
                                    ? "There is a file in the folder which is not of type \"csv\"! Please delete it and relaunch the application"
                                    : "Im Ordner befindet sich eine Datei, die nicht vom Typ \"csv\" ist! Bitte löschen Sie sie und starten Sie die Anwendung erneut";
                                throw new CustomException(exceptionMessage);
                            }
                        }

                        break;
                    }
                    else
                    {
                        Console.WriteLine();
                        string exceptionMessage = _isEnglish
                            ? "Invalid path! Please provide a valid path the the directory which contains the csvs"
                            : "Ungültiger Pfad! Bitte geben Sie einen gültigen Pfad zu dem Verzeichnis an, das die csvs enthält";
                        throw new CustomException(exceptionMessage);
                    }
                }
                else if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.L)
                {
                    ChangeLanguage();
                    Console.Clear();
                    Main(null); //fix this as whenever i choose a new language it asks me to enter a language again
                    return "";
                }
                else if (key.Key != ConsoleKey.Backspace)
                {
                    Console.Write(key.KeyChar);
                    pathToFolder += key.KeyChar;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(pathToFolder))
                    {
                        pathToFolder = pathToFolder.Substring(0, pathToFolder.Length - 1);
                        int pos = Console.CursorLeft;
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
            }
            catch (CustomException e)
            {
                pathToFolder = "";
                Console.WriteLine(e.Message);
                if (e.Message ==
                    "There is a file in the folder which is not of type \"csv\"! Please delete it and relaunch the application" ||
                    e.Message ==
                    "Im Ordner befindet sich eine Datei, die nicht vom Typ \"csv\" ist! Bitte löschen Sie sie und starten Sie die Anwendung erneut")
                {
                    Console.WriteLine(_isEnglish
                        ? "Press any key to exit the app . . ."
                        : "Drücken Sie eine beliebige Taste, um die App zu verlassen . . .");
                    Console.ReadKey(true);
                    Environment.Exit(1);
                }
            }
        }

        Console.WriteLine();
        return pathToFolder;
    }

    private static string SenderEmailInput()
    {
        ConsoleKeyInfo key;
        string senderEmail = "";
        while (true)
        {
            try
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    if (string.IsNullOrEmpty(senderEmail))
                    {
                        Console.WriteLine();
                        string exceptionMessage =
                            _isEnglish
                                ? "The email cannot be empty! Please enter a valid email"
                                : "Die E-Mail kann nicht leer sein! Bitte geben Sie eine gültige E-Mail ein";
                        throw new CustomException(exceptionMessage);
                    }
                    else if (!Regex.IsMatch(senderEmail, "^[\\w.+\\-]+@gmail\\.com$"))
                    {
                        Console.WriteLine();
                        string exceptionMessage = _isEnglish
                            ? "The app only works with gmail addresses! Please enter a gmail address!"
                            : "Die App funktioniert nur mit gmail Adressen! Bitte geben Sie eine gmail Adresse ein!";
                        throw new CustomException(exceptionMessage);
                    }
                    else
                    {
                        break;
                    }
                }
                else if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.L)
                {
                    ChangeLanguage();
                    Console.Clear();
                    Main(null); //fix this as whenever i choose a new language it asks me to enter a language again
                    return "";
                }
                else if (key.Key != ConsoleKey.Backspace)
                {
                    Console.Write(key.KeyChar);
                    senderEmail += key.KeyChar;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(senderEmail))
                    {
                        senderEmail = senderEmail.Substring(0, senderEmail.Length - 1);
                        int pos = Console.CursorLeft;
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
            }
            catch (CustomException e)
            {
                senderEmail = "";
                Console.WriteLine(e.Message);
            }
        }

        Console.WriteLine();
        return senderEmail;
    }

    private static string PasswordInput()
    {
        ConsoleKeyInfo key;
        string password = "";
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
                        Console.WriteLine();
                        string exceptionMessage = _isEnglish
                            ? "The password cannot be empty! Please enter a valid password"
                            : "Das Passwort kann nicht leer sein! Bitte geben Sie ein gültiges Passwort ein";
                        throw new CustomException(exceptionMessage);
                    }
                    else
                    {
                        break;
                    }
                }
                else if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.L)
                {
                    ChangeLanguage();
                    Console.Clear();
                    Main(null); //fix this as whenever i choose a new language it asks me to enter a language again
                    return "";
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
                password = "";
                Console.WriteLine(e.Message);
            }
        }

        Console.WriteLine();
        return password;
    }

    private static string ReceiverEmailInput()
    {
        ConsoleKeyInfo key;
        string receiverEmail = "";
        while (true)
        {
            try
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    if (string.IsNullOrEmpty(receiverEmail))
                    {
                        Console.WriteLine();
                        string exceptionMessage = _isEnglish
                            ? "The receiver email cannot be empty! Please enter a valid email"
                            : "Die Empfänger-E-Mail kann nicht leer sein! Bitte geben Sie eine gültige E-Mail ein";
                        throw new CustomException(exceptionMessage);
                    }
                    else if (!Regex.IsMatch(receiverEmail, "^[\\w.+\\-]+@gmail\\.com$"))
                    {
                        Console.WriteLine();
                        string exceptionMessage = _isEnglish
                            ? "The app only works with gmail addresses! Please enter a gmail address!"
                            : "Die App funktioniert nur mit gmail Adressen! Bitte geben Sie eine gmail Adresse ein!";
                        throw new CustomException(exceptionMessage);
                    }
                    else
                    {
                        break;
                    }
                }
                else if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.L)
                {
                    ChangeLanguage();
                    Console.Clear();
                    Main(null); //fix this as whenever i choose a new language it asks me to enter a language again
                    return "";
                }
                else if (key.Key != ConsoleKey.Backspace)
                {
                    Console.Write(key.KeyChar);
                    receiverEmail += key.KeyChar;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(receiverEmail))
                    {
                        receiverEmail = receiverEmail.Substring(0, receiverEmail.Length - 1);
                        int pos = Console.CursorLeft;
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
            }
            catch (CustomException e)
            {
                receiverEmail = "";
                Console.WriteLine(e.Message);
            }
        }

        Console.WriteLine();
        return receiverEmail;
    }
}