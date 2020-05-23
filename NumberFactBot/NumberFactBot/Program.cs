using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace NumberFactBot
{
    public class Program
    {
        static TelegramBotClient Bot;
        public static void Main(string[] args)
        {
            Bot = new TelegramBotClient("1169172868:AAHrP1Cm_owpPaOmB7WZZmsht63_bRXPMKU");

            var me = Bot.GetMeAsync().Result;

            Bot.OnMessage += Bot_OnMessaageReceived;
            Bot.OnCallbackQuery += Bot_OnCallbackQueryRecievedAsync;
            

            Console.WriteLine(me.FirstName);

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();

            Console.ReadLine();
        }

        private static async void Bot_OnCallbackQueryRecievedAsync(object sender, CallbackQueryEventArgs e) //Buttons
        {
            string buttonText = e.CallbackQuery.Data;
            string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName} {e.CallbackQuery.From.Id}";

            Console.WriteLine(name + " " + "натиснув "  + buttonText);

            string[] comNum = buttonText.Split(" ");

            switch(comNum[0])
            {
                case "mathfact":
                    {
                        try
                        {
                            int number = Convert.ToInt32(comNum[1]);

                            UserInfo userInfo = new UserInfo();
                            userInfo.Number = number;

                            var json = JsonConvert.SerializeObject(userInfo);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");

                            using var client = new HttpClient();
                            var content = await client.PostAsync("https://numberfactsapi.azurewebsites.net/api/Number/math", data);


                            string result = content.Content.ReadAsStringAsync().Result;
                            UserItem useritem = JsonConvert.DeserializeObject<UserItem>(result);

                            await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, useritem.Fact);
                            break;

                        }
                        catch
                        {
                            await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Error! A wrong number is entered!");
                            break;
                        }
                    }
                case "triviafact":
                    {
                        try
                        {
                            int number = Convert.ToInt32(comNum[1]);

                            UserInfo userInfo = new UserInfo();
                            userInfo.Number = number;

                            var json = JsonConvert.SerializeObject(userInfo);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");

                            using var client = new HttpClient();
                            var content = await client.PostAsync("https://numberfactsapi.azurewebsites.net/api/Number/trivia", data);


                            string result = content.Content.ReadAsStringAsync().Result;
                            UserItem useritem = JsonConvert.DeserializeObject<UserItem>(result);

                            await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, useritem.Fact);
                            break;

                        }
                        catch
                        {
                            await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Error! A wrong number is entered!");
                            break;
                        }
                    }
                case "addfavoritenumber":
                    {
                        try
                        {
                            int number = Convert.ToInt32(comNum[1]);


                            UserInfo userInfo = new UserInfo();
                            userInfo.Id = Convert.ToString(e.CallbackQuery.From.Id);
                            userInfo.Number = number;

                            var json = JsonConvert.SerializeObject(userInfo);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");

                            using var client = new HttpClient();
                            var content = await client.PostAsync("https://numberfactsapi.azurewebsites.net/api/Number/AddFavorite", data);

                            string result = content.Content.ReadAsStringAsync().Result;

                            if (result == "Ok")
                            {
                                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Oк");
                                break;
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Error! Тhe number already exists in the list!");
                                break;
                            }
                        }
                        catch
                        {
                            await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Error! A wrong number is entered!");
                            break;
                        }
                    }
                case "deletefavoritenumber":
                    {
                        try
                        {
                            int number = Convert.ToInt32(comNum[1]);


                            UserInfo userInfo = new UserInfo();
                            userInfo.Id = Convert.ToString(e.CallbackQuery.From.Id);
                            userInfo.Number = number;

                            var json = JsonConvert.SerializeObject(userInfo);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");

                            using var client = new HttpClient();
                            var content = await client.PutAsync("https://numberfactsapi.azurewebsites.net/api/Number/DeleteFavorite", data);

                            string result = content.Content.ReadAsStringAsync().Result;

                            if (result == "Ok")
                            {
                                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Oк");
                                break;
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Error! The number does not exist in the list!");
                                break;
                            }
                        }
                        catch
                        {
                            await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Error! A wrong number is entered!");
                            break;
                        }
                    }
            }

        }

        private static async void Bot_OnMessaageReceived(object sender, MessageEventArgs email)
        {

            var message = email.Message;
            if (message == null || message.Type != MessageType.Text)
                return;
            string username = $"{message.From.FirstName} {message.From.LastName}";

            Console.WriteLine($"{username} send message '{message.Text}'");

            string[] userMessage = message.Text.Split(' ', '.');

            List<InlineKeyboardButton> inlineKeyboardList = new List<InlineKeyboardButton>();
          

            string[] Commands = { "mathfact", "triviafact", "addfavoritenumber", "deletefavoritenumber"};

            int num;
            bool isNum = new bool();

            for (int i = 0; i < userMessage.Length; i++) 
            {
                isNum = int.TryParse(userMessage[i], out num);
                if (isNum == false)
                    break;
            }
           
            if (isNum)
            {
                if (userMessage.Length == 2 && userMessage[0] != "/start" && userMessage[0] != "/getfavoritenumbers") // Date
                {
                    try
                    {
                        int day = Convert.ToInt32(userMessage[0]);
                        int month = Convert.ToInt32(userMessage[1]);

                        if ((month == 1 && day >= 1 && day <= 31) || (month == 2 && day >= 1 && day <= 29) || (month == 3 && day >= 1 && day <= 31) || (month == 4 && day >= 1 && day <= 30) || (month == 5 && day >= 1 && day <= 30) || (month == 6 && day >= 1 && day <= 31) || (month == 7 && day >= 1 && day <= 31) || (month == 8 && day >= 1 && day <= 31) || (month == 9 && day >= 1 && day <= 30) || (month == 10 && day >= 1 && day <= 31) || (month == 11 && day >= 1 && day <= 30) || (month == 12 && day >= 1 && day <= 31))
                        {
                            UserInfo userInfo = new UserInfo();
                            userInfo.Day = day;
                            userInfo.Month = month;

                            var json = JsonConvert.SerializeObject(userInfo);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");

                            using var client = new HttpClient();
                            var content = await client.PostAsync("https://numberfactsapi.azurewebsites.net/api/Number/date", data);


                            string result = content.Content.ReadAsStringAsync().Result;
                            UserItem useritem = JsonConvert.DeserializeObject<UserItem>(result);

                            await Bot.SendTextMessageAsync(message.From.Id, useritem.Fact);
                        }
                        else
                        {
                            await Bot.SendTextMessageAsync(message.From.Id, "This day is not supported");
                        }
                    }
                    catch
                    {
                        await Bot.SendTextMessageAsync(message.From.Id, "Error! A wrong number is entered!");
                    }
                }
                else if (userMessage.Length == 1 && userMessage[0] != "/start" && userMessage[0] != "/getfavoritenumbers") //Number
                {

                    for (int i = 0; i < Commands.Length; i++)
                    {
                        InlineKeyboardButton ts = new InlineKeyboardButton();
                        ts = (InlineKeyboardButton.WithCallbackData(Commands[i], Commands[i] + " " + userMessage[0]));
                        inlineKeyboardList.Add(ts);
                    }
                }

                if (userMessage.Length == 1 &&  userMessage[0] != "/start" && userMessage[0] != "/getfavoritenumbers")
                {
                    var inline = new InlineKeyboardMarkup(inlineKeyboardList);
                    await Bot.SendTextMessageAsync(email.Message.Chat.Id, "Select a command: \nmathfact - mathematical fact about number; \ntriviafact - ordinary fact about number; \naddfavoritenumber - add number to list of favorite number; \ndeletefavoritenumber - delete number from list of favorite number.", replyMarkup: inline);
                }
            }
            else
            {
                if (userMessage[0] != "/start" && userMessage[0] != "/getfavoritenumbers")
                {
                    await Bot.SendTextMessageAsync(email.Message.Chat.Id, "Error! A wrong number is entered!");
                }
              
            }


            switch (userMessage[0])
            {

                case "/start":
                    string starttext = @"Hi! I will tell you interesting facts about numbers! 
I can send you an interesting mathematical or ordinary fact about a number or a fact about a date entered. 
And I can create a list of your favorite numbers.
Enter a NUMBER for subsequent commands or a DATE (in XX.XX or XX XX format) to get the fact about it:)";
                    await Bot.SendTextMessageAsync(message.From.Id, starttext);
                    break;
                case "/getfavoritenumbers":
                    {
                        if (userMessage.Length > 0)
                        {
                            try
                            {

                                UserInfo userInfo = new UserInfo();
                                userInfo.Id = Convert.ToString(message.From.Id);

                                var json = JsonConvert.SerializeObject(userInfo);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");

                                using var client = new HttpClient();
                                var content = await client.PostAsync("https://numberfactsapi.azurewebsites.net/api/Number/GetFavorite", data);

                                string res = content.Content.ReadAsStringAsync().Result;
                                string result = Convert.ToString(res).Trim(new char[] { '[', ']' });

                                await Bot.SendTextMessageAsync(message.From.Id, result);
                                break;
                              
                            }
                            catch
                            {
                                await Bot.SendTextMessageAsync(message.From.Id, "Error! A wrong number is entered!");
                                break;
                            }
                        }
                        else
                        {
                            await Bot.SendTextMessageAsync(message.From.Id, "Error! A wrong number is entered!");
                            break;
                        }
                    }
                  
            }
        }
    }
}
