using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.States;

namespace TelegramBot;

/// <summary>
/// Мой телеграм бот.
/// </summary>
public class MyTelegramBot
{
    private ITelegramBotClient _botClient;
    private CancellationTokenSource _cts;
    private ReceiverOptions _receiverOptions;
    
    private Dictionary<long, BaseState> _states = new ();
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="token">
    /// Токен бота.
    /// </param>
    public MyTelegramBot(string token)
    {
        _botClient = new TelegramBotClient(token);
        _cts = new ();
        _receiverOptions = new ()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };
    }
    
    /// <summary>
    /// Установить состояние.
    /// </summary>
    /// <param name="chatId">
    /// Идентификатор чата.
    /// </param>
    /// <param name="state">
    /// Состояние.
    /// </param>
    public void SetState(long chatId, BaseState state)
    {
        Console.WriteLine($"Setting state for chat {chatId} to {state.GetType().Name}.");
        _states[chatId] = state;
    }
    
    /// <summary>
    /// Запустить опрос.
    /// </summary>
    public async Task StartPolling()
    {
        BaseState.SetUpdateStateDelegate(SetState);
        BaseState.SetBotClient(_botClient);
        
        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: _receiverOptions,
            cancellationToken: _cts.Token
        );
        
        var me = await _botClient.GetMeAsync();
        
        Console.WriteLine($"Start listening for @{me.Username}");
    }
    
    /// <summary>
    /// Остановить опрос.
    /// </summary>
    public async Task StopPolling()
    {
        _cts.Cancel();
    }
    
    /// <summary>
    /// Обработать обновление.
    /// </summary>
    /// <param name="botClient">
    /// Клиент бота.
    /// </param>
    /// <param name="update">
    /// Обновление.
    /// </param>
    /// <param name="cancellationToken">
    /// Токен отмены.
    /// </param>
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        long chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.Message?.Chat.Id ?? 0;
        
        if (chatId == 0)
            return;

        if (!_states.ContainsKey(chatId))
            SetState(chatId, new WelcomeState(chatId));
            
        await _states[chatId].HandleUpdateAsync(botClient, update, cancellationToken);     
    }

    /// <summary>
    /// Обработать ошибку опроса.
    /// </summary>
    /// <param name="botClient">
    /// Клиент бота.
    /// </param>
    /// <param name="exception">
    /// Исключение.
    /// </param>
    /// <param name="cancellationToken">
    /// Токен отмены.
    /// </param>
    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        string ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
    }
}