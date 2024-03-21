using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.States;

/// <summary>
/// Состояние приветствия.
/// </summary>
public class WelcomeState: BaseState
{
    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public WelcomeState() : base(0)
    {
    }
    
    /// <summary>
    /// Конструктор с идентификатором чата.
    /// </summary>
    /// <param name="chatId">
    /// Идентификатор чата.
    /// </param>
    public WelcomeState(long chatId) : base(chatId)
    {
    }

    /// <summary>
    /// Инициализация состояния.
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
    public override async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Message is not { } message)
            return;
        
        // Only process text messages
        if (message.Text is not { } messageText)
            return;
        
        // Only process with user
        if (message.From is not { } user)
            return;
        
        await SendMessageAsync($"Привет! Я Random Coffee Bot. Я помогу тебе найти себе компанию для кофепития."); 

        StorageUtils.User userInDb = new StorageUtils.User(user);
        
        if (user.Username == null)
        {
            await SendMessageAsync("Пожалуйста, укажи свой username в настройках телеграма. Без него я не смогу тебе помочь.");
            return;
        }
        
        if (!userInDb.IsExists)
        {
            UpdateState<RegistrationState>();
        }
        else
        {
            UpdateState<MainState>();
        }
    }
}