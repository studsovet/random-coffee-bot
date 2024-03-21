using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.States;

/// <summary>
/// Делегат обновления состояния.
/// </summary>
public delegate void UpdateStateDelegate(long chatId, BaseState newState);

/// <summary>
/// Базовое состояние.
/// </summary>
public abstract class BaseState
{
    private static UpdateStateDelegate? _updateStateDelegate;
    private static ITelegramBotClient? _botClient;
    protected bool AbortFlag = false;
    
    private long _chatId;
    
    /// <summary>
    /// Идентификатор чата.
    /// </summary>
    protected long ChatId
    {
        get => _chatId;
        set
        {
            _chatId = value;
            
            if (value != 0) Task.Run(() => InitializeState()).Wait();
        }
    }
    
    /// <summary>
    /// Инициализация состояния.
    /// </summary>
    public virtual async Task InitializeState()
    {
        
    }
    
    /// <summary>
    /// Установить делегат обновления состояния.
    /// </summary>
    /// <param name="updateStateDelegate"></param>
    public static void SetUpdateStateDelegate(UpdateStateDelegate updateStateDelegate)
    {
        _updateStateDelegate = updateStateDelegate;
    }
    
    /// <summary>
    /// Установить клиент бота.
    /// </summary>
    /// <param name="botClient">
    /// Клиент бота.
    /// </param>
    public static void SetBotClient(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }
    
    /// <summary>
    /// Обновить состояние.
    /// </summary>
    /// <param name="newState">
    /// Новое состояние.
    /// </param>
    protected void UpdateState(BaseState newState)
    {
        if (_updateStateDelegate == null)
        {
            Console.WriteLine("UpdateStateDelegate is null.");
            return;
        }
        
        AbortFlag = true;
        _updateStateDelegate.Invoke(ChatId, newState);
    }
    
    /// <summary>
    /// Обновить состояние.
    /// </summary>
    /// <typeparam name="T">
    /// Тип состояния.
    /// </typeparam>
    protected void UpdateState<T>() where T : BaseState, new()
    {
        if (_updateStateDelegate == null)
        {
            Console.WriteLine("UpdateStateDelegate is null.");
            return;
        }
        
        AbortFlag = true;
        
        var newState = new T();
        newState.ChatId = ChatId;
        
        if (!newState.AbortFlag) _updateStateDelegate.Invoke(ChatId, newState);
    }
    
    /// <summary>
    /// Отправить сообщение.
    /// </summary>
    /// <param name="message">
    /// Сообщение.
    /// </param>
    /// <param name="replyMarkup">
    /// Разметка ответа.
    /// </param>
    protected async Task SendMessageAsync(string message, IReplyMarkup? replyMarkup = null)
    {
        if (_botClient == null)
        {
            Console.WriteLine("BotClient is null.");
            return;
        }
        
        await _botClient.SendTextMessageAsync(
            ChatId, 
            message,
            replyMarkup: replyMarkup
            );
    }
    
    /// <summary>
    /// Обработать обновление.
    /// </summary>
    /// <param name="chatId">
    /// Идентификатор чата.
    /// </param>
    public BaseState(long chatId)
    {
        ChatId = chatId;
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
    public virtual async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        Console.WriteLine("BaseState was called.");
    }
}
