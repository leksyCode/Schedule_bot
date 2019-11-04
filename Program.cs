using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SheduleBot.Common;
using SheduleBot.Models;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace SheduleBot
{
    public class Program
    {
        private static ITelegramBotClient botClient;
        private static List<User> usersList = new List<User>();

        public static void Main(string[] args)
        {
            botClient = new TelegramBotClient("820903390:AAHmpT-wxce7Pz-aAu2XhcerWoPAvYvbO0s");
            botClient.OnMessage += Bot_OnMessage;
            botClient.OnCallbackQuery += Bot_OnCallBackQuery;
            botClient.StartReceiving();

            CreateWebHostBuilder(args).Build().Run();
            Thread.Sleep(int.MaxValue);
        }

        static void Bot_OnCallBackQuery(object sender, CallbackQueryEventArgs e)
        {
            string str = e.CallbackQuery.Data;
        }

        static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (usersList.Find(x => x.Id == e.Message.From.Id) == null)
            {                
                usersList.Add(new User(e.Message.From.Id));
            }

            string command = " ";

            if (e.Message.Type == MessageType.Text && e.Message.Text[0] == '/')
            {
                command = e.Message.Text;
            }
            else if (e.Message.Type == MessageType.Photo && usersList.Find(x => x.Id == e.Message.From.Id).SetShedule == true)
            {
                usersList.Find(x => x.Id == e.Message.From.Id).Shedule_FileId = e.Message.Photo[2].FileId;
                SendTextMessage(e, "Done! You have new schedule.");
                usersList.Find(x => x.Id == e.Message.From.Id).SetShedule = false;
            }
            else if (e.Message.Type == MessageType.Text && usersList.Find(x => x.Id == e.Message.From.Id).SetTimeline == true)
            {
                usersList.Find(x => x.Id == e.Message.From.Id).TimeLine.AddRange(e.Message.Text.Split('\n'));


                Parser p = new Parser();
                List<Business> businessList = p.ParseText(usersList.Find(x => x.Id == e.Message.From.Id).TimeLine);
                usersList.Find(x => x.Id == e.Message.From.Id).Doings.AddRange(businessList);
                SendTextMessage(e, "Done! Your timeline is ready to use. Do not miss your business.");
                usersList.Find(x => x.Id == e.Message.From.Id).SetTimeline = false;
            }
            else
            {
                SendTextMessage(e, "Sorry. It's not a command. Be sure that string starts with '/'");
            }


            if (command == "/schedule" || command == "/schedule@YourSchedule_bot")
            {
                SendFileMessage(e, "http://fmi-plovdiv.org/GetResource?id=3315", MessageType.Document, "");
            }
            else if (command == "/show_schedule" || command == "/show_schedule@YourSchedule_bot")
            {
                if (usersList.Find(x => x.Id == e.Message.From.Id).Shedule_FileId != null)
                {
                    string caption = "";
                    for (int i = 0; i < usersList.Find(x => x.Id == e.Message.From.Id).TimeLine.Count; i++)
                    {
                        caption += i+1 + ". " + usersList.Find(x => x.Id == e.Message.From.Id).TimeLine[i] + " \n";
                    }

                    SendFileMessage(e, usersList.Find(x => x.Id == e.Message.From.Id).Shedule_FileId.ToString(), MessageType.Photo, 
                        "next: " + usersList.Find(x => x.Id == e.Message.From.Id).Doings.ElementAt(0).Name + " - " +
                       usersList.Find(x => x.Id == e.Message.From.Id).Doings.ElementAt(0).Weekday + ", " + usersList.Find(x => x.Id == e.Message.From.Id).Doings.ElementAt(0).Time);
                }
                else
                {
                    SendTextMessage(e, "Sorry. At first you need to drop me a schedule!");
                }
            }
            else if (command == "/upload_schedule" || command == "/upload_schedule@YourSchedule_bot")
            {
                usersList.Find(x => x.Id == e.Message.From.Id).SetShedule = true;
                SendTextMessage(e, "Please send me photo of your schedule so I remember it.");
            }
            else if (command == "/set_timeline" || command == "/set_timeline@YourSchedule_bot")
            {
                usersList.Find(x => x.Id == e.Message.From.Id).SetTimeline = true;
                SendTextMessage(e, "Ok. Send me a timeline list of things to do. Please use this format: \n\n" +
                    "thing, weekday, from-to(hh:mm), time to notification (min) \n" +
                    "Lect. Mobile Aps, monday, 17:30-19:00, 30");
            }
        }

        public async static void SendTextMessage(MessageEventArgs e, string text)
        {
            List<KeyboardButton> btns = new List<KeyboardButton>();
            btns.Add(new KeyboardButton("/schedule"));
            btns.Add(new KeyboardButton("/show_schedule"));
            btns.Add(new KeyboardButton("/upload_schedule"));


            await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: text,
                        parseMode: ParseMode.Html,
                        replyMarkup: new ReplyKeyboardRemove()
                        
                        //replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton().Text = "inline button")
                        );

        }

        public async static void SendFileMessage(MessageEventArgs e, string url, MessageType type, string caption)
        {
            InputOnlineFile inputOnlineFile = new InputOnlineFile(url);

            switch (type)
            {
                case MessageType.Photo:
                    {
                        await botClient.SendPhotoAsync(e.Message.Chat, inputOnlineFile, caption: caption, parseMode: ParseMode.Html);
                    }
                    break;
                case MessageType.Document:
                    {
                        await botClient.SendDocumentAsync(e.Message.Chat, inputOnlineFile,
                             caption: caption,
                             parseMode: ParseMode.Html,
                             replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Visit FMI web", "http://fmi-plovdiv.org/index.jsp?id=2583&ln=1")));
                    }
                    break;
            }

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .UseStartup<Startup>();
    }
}
