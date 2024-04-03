using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Hitachi_task.CsvHandler;
using Hitachi_task.Exceptions;
using Hitachi_task.Islands;

namespace Hitachi_task.EmailSender;

public class Sender
{
    private readonly Dictionary<Island, Day> _bestDaysDict;

    public Sender(Dictionary<Island, Day> bestDaysDict)
    {
        _bestDaysDict = bestDaysDict;
    }

    public void SendEmail(string senderEmail, string senderEmailPassword, string receiverEmail, string createdCsvPath)
    {
        var bestCombinationIsland = _bestDaysDict.FirstOrDefault().Key.Name;
        var bestCombinationDay = _bestDaysDict.FirstOrDefault().Value.DayOfTheMonth;
        var fromAddress = new MailAddress(senderEmail, senderEmail);
        var toAddress = new MailAddress(receiverEmail, receiverEmail);
        string ordinalNumberSpelling = bestCombinationDay == 1 ? "st" :
            bestCombinationDay == 2 ? "nd" :
            bestCombinationDay == 3 ? "rd" : "th";
        string messageBody = $@"<head>
                            <style>
                                #email-image {{
                                    width: 200px;
                                }}
                                #container {{
                                    height: auto;
                                    display: flex;
                                    justify-content: center;
                                    align-items: center;
                                    text-align: center;
                                }}
                            </style>
                        </head>
                        <body>
                            <div id=""container"">
                                <a href=""#"">
                                    <img src=""https://www.hitachi-solutions.bg/wp-content/uploads/2023/03/SPACE-Icon-with-HS-logo-e1678100201133-1024x1024.png""alt=""hitachi space image""id=""email-image"">
                                </a>
                                <div id=""heading"">
                                    <h1>Hitachi - SPACE Shuttle Launch Report</h1>
                                    <h2>The best combination is: ""{bestCombinationIsland}"" - {bestCombinationDay}{ordinalNumberSpelling} of July</h2>
                                    <h3>A csv report can be found in the attached file</h3>
                                </div>
                            </div>
                        </body>";
        var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = "Hitachi - SPACE shuttle launch",
            Body = messageBody,
            IsBodyHtml = true,
            Attachments =
                { new Attachment(createdCsvPath, new ContentType() { MediaType = MediaTypeNames.Application.Octet }) }
        };
        using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
        {
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderEmailPassword);
            smtpClient.EnableSsl = true;
            try
            {
                Console.WriteLine("Sending the email . . .");
                smtpClient.Send(message);
            }
            catch (SmtpException)
            {
                try
                {
                    throw new CustomException("The provided password is incorrect for the sender email! Please enter the correct password");
                }
                catch (CustomException e)
                {
                    Console.WriteLine(e.Message);
                    ConsoleKeyInfo key;
                    senderEmailPassword = "";
                    while (true)
                    {
                        key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Enter)
                        {
                            if (string.IsNullOrEmpty(senderEmailPassword))
                            {
                                senderEmailPassword = "";
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
                            senderEmailPassword += key.KeyChar;
                        }
                        else if (key.Key == ConsoleKey.Backspace)
                        {
                            if (!string.IsNullOrEmpty(senderEmailPassword))
                            {
                                senderEmailPassword = senderEmailPassword.Substring(0, senderEmailPassword.Length - 1);
                                int pos = Console.CursorLeft;
                                Console.SetCursorPosition(pos - 1, Console.CursorTop);
                                Console.Write(" ");
                                Console.SetCursorPosition(pos - 1, Console.CursorTop);
                            }
                        }
                    }
                    Console.WriteLine();
                    SendEmail(senderEmail, senderEmailPassword, receiverEmail, createdCsvPath);
                    return;
                }
            }
        }

        Console.WriteLine($"Email has been successfully sent to \"{receiverEmail}\"!");
    }
}