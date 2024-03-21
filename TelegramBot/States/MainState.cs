using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.States;

public class MainState : BaseState
{
    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public MainState() : base(0)
    {
    }
    
    /// <summary>
    /// Конструктор с идентификатором чата.
    /// </summary>
    /// <param name="chatId">
    /// Идентификатор чата.
    /// </param>
    public MainState(long chatId) : base(chatId)
    {
    }

    private async Task SendMenu()
    {
        await SendMessageAsync("Главное меню", new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Редактировать профиль", callbackData: "edit")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Найти собеседника", callbackData: "find")
            },
        }));
    }
    
    /// <summary>
    /// Инициализация состояния.
    /// </summary>
    public override async Task InitializeState()
    {
        await SendMenu();
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
    public override async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    { 
        if (update.CallbackQuery != null)
        {
            switch (update.CallbackQuery.Data)
            {
                case "edit":
                    UpdateState<RegistrationState>();
                    break;
                case "find":
                    UpdateState<SearchState>();
                    break;
            }
            
            return;
        }

        await SendMenu();
    }
}