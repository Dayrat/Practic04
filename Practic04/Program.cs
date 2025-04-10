
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
namespace Practic04
{
    class Program
    {


        private static readonly string BotToken = "7477335429:AAEk2lI53GgIzYlNu340AkIP7iMf4Wa2_Ao";
        private static ITelegramBotClient Bot;

        static async Task Main()
        {
            Bot = new TelegramBotClient(BotToken);

            var cts = new System.Threading.CancellationTokenSource();
            Bot.StartReceiving(HandlerUpdateAsync, HandlerErrorAsync, cancellationToken: cts.Token);

            Console.WriteLine("Bot is started...");
            Console.ReadLine();
            cts.Cancel();
        }

        private static async Task HandlerUpdateAsync(ITelegramBotClient bot, Telegram.Bot.Types.Update update, System.Threading.CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message?.Text != null)
            {
                var messageText = update.Message.Text.ToLower();

                if (messageText == "/start")
                {
                    await bot.SendTextMessageAsync(
                        chatId: update.Message.Chat,
                        text: "Привет! Выберите опцию:",
                        replyMarkup: GetReplyKeyboard(),
                        cancellationToken: cancellationToken);
                }
                else if (messageText == "просмотр занятий")
                    await SendGroupSelectionMessage(update.Message.Chat.Id, cancellationToken);
                else if (messageText == "просмотр успеваемости")
                    await SendStudentSelectionMessage(update.Message.Chat.Id, cancellationToken);
                else
                    await bot.SendTextMessageAsync(update.Message.Chat, "Пожалуйста, выберите одну из кнопок на клавиатуре.", cancellationToken: cancellationToken);
                
            }
            else if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
                {
                    // Обрабатываем нажатия на инлайн-кнопки
                    await HandleCallbackQuery(update.CallbackQuery, cancellationToken);
                }
        }

        private static ReplyKeyboardMarkup GetReplyKeyboard()
        {
            var keyboard = new[]
            {
            new[]
            {
                new KeyboardButton("Просмотр занятий"),
                new KeyboardButton("Просмотр успеваемости"),
            },
        };

            return new ReplyKeyboardMarkup(keyboard)
            {
                ResizeKeyboard = true // Автоматически подстраивает размер клавиатуры
            };
        }
        private static async Task HandleCallbackQuery(Telegram.Bot.Types.CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            if (callbackQuery.Data.StartsWith("student_"))
            {

                string message = callbackQuery.Data switch
                {
                    "student_1" => "Тверитин Богдан Сергеевич был на занятии 1 сентября 9:00",
                    "student_2" => "Кобилова Шукрона Исломджоновна не была на занятии 3 сентября 9:00",
                    "student_3" => "Информация о Рекко Кирилле Сергеевиче ещё не внесена",
                    "student_4" => "Информация о Лавренкове Дмитрие Алексеевиче ещё не внесена",
                    "student_5" => "Информация о Сябро Илье Родионовиче ещё не внесена",
                    "student_6" => "Информация о Шишкине Илье Дмитриевиче ещё не внесена",
                    _ => "Неизвестный студент"

                };
                await Bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, message, cancellationToken: cancellationToken);
            }
            else if (callbackQuery.Data.StartsWith("group_"))
            {
                string groupNumber = callbackQuery.Data.Split('_')[1];
                string classesMessage = GetClassesForGroup(groupNumber);
                await Bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, classesMessage, cancellationToken: cancellationToken);
            }

            
        }

        private static async Task SendStudentSelectionMessage(long chatId, CancellationToken cancellationToken)
        {
            var inlineKeyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Тверитин", "student_1"),
                InlineKeyboardButton.WithCallbackData("Кобилова", "student_2")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Рекко", "student_3"),
                InlineKeyboardButton.WithCallbackData("Лавренков", "student_4")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Сябро", "student_5"),
                InlineKeyboardButton.WithCallbackData("Шишкин", "student_6")
            },
        });

            await Bot.SendTextMessageAsync(
                chatId: chatId,
                text: "Выберите студента:",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
        private static async Task SendGroupSelectionMessage(long chatId, CancellationToken cancellationToken)
        {
            var inlineKeyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("44ИС", "group_1"),
                InlineKeyboardButton.WithCallbackData("23КС", "group_2")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("11Л", "group_3"),
                InlineKeyboardButton.WithCallbackData("32Э", "group_4")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("42ИС", "group_5")
            }
        });

            await Bot.SendTextMessageAsync(
                chatId: chatId,
                text: "Выберите группу:",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }

        private static string GetClassesForGroup(string groupNumber)
        {
            return groupNumber switch
            {
                "1" => "Занятия группы 44ИС:\n- Экономика 1 сентября 9:00\n- Информационные системы 1 сентября 11:00",
                "2" => "Занятия группы 23КС:\n- Экономика 3 сентября 9:00",
                "3" => "Занятия группы 11Л:\n- Занятия ещё не выбраны",
                "4" => "Занятия группы 32Э:\n- Русский язык 2 сентября 13:00",
                "5" => "Занятия группы 42ИС:\n- Занятия ещё не выбраны",
                _ => "Неизвестная группа"
            };
        }




        private static Task HandlerErrorAsync(ITelegramBotClient botClient, Exception exception, System.Threading.CancellationToken cancellationToken)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
