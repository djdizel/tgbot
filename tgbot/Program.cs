using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;

namespace TelegramBot
{
class Program
{
    static string BotToken = "7829510317:AAGOiFPrk68q8vYpkFUS5NxuM7RM02N-YNA";
    static ITelegramBotClient botClient;

    static async Task Main(string[] args)
    {
        botClient = new TelegramBotClient(BotToken);
        var me = await botClient.GetMeAsync();
        Console.WriteLine(me.FirstName);
        
        var cancellationTokenSource = new CancellationTokenSource();
        
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() 
        };
        
        botClient.StartReceiving(HandleUpdateAsync, HandelPollingErrorAsync, 
        receiverOptions,cancellationToken: cancellationTokenSource.Token);
            
        Console.WriteLine("Bot is up and running. Press Enter to exit.");
        Console.ReadLine();
        cancellationTokenSource.Cancel();
    }

    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            var message = update.Message;
            Console.WriteLine($"Message: {message.Chat.FirstName}: {message.Text}");
            if (message.Text.StartsWith("/start"))
            {
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Бот запущен!", cancellationToken: cancellationToken);
            }
            else if (message.Text.StartsWith("/help"))
            {
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Список доступных команд:\n/start - начать работу с ботом\n/help - получить помощь\n/time - текущее время", cancellationToken: cancellationToken);

            }
            else if (message.Text.StartsWith("/time"))
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss"); 
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Текущее время: {currentTime}", cancellationToken: cancellationToken);
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Такой команды нет, используйте команду /help", cancellationToken: cancellationToken);
            }
        }
    }

    static Task HandelPollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    
}
}