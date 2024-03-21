using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.UserFilter;

namespace TelegramBot.States;

/// <summary>
/// Состояние сборки фильтров.
/// </summary>
public class FilterComposerState: BaseState
{
    private Filter[] _filters;
    
    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public FilterComposerState() : base(0)
    {
        _filters = new Filter[] { };
    }
    
    /// <summary>
    /// Конструктор с идентификатором чата.
    /// </summary>
    /// <param name="chatId">
    /// Идентификатор чата.
    /// </param>
    public FilterComposerState(long chatId) : base(chatId)
    {
        _filters = new Filter[] { };
    }
    
    /// <summary>
    /// Запросить фильтр.
    /// </summary>
    public async Task AskForFilter()
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Фильтр по возрасту", "FilterByAge")
            }
        });
        
        await SendMessageAsync("Выберите фильтр для поиска собеседника:", keyboard);
    }

    /// <summary>
    /// Инициализация состояния.
    /// </summary>
    public override async Task InitializeState()
    {
        await AskForFilter();
    }
    
    /// <summary>
    /// Обработать запрос.
    /// </summary>
    /// <param name="query">
    /// Запрос.
    /// </param>
    private async Task HandleQuery(string query)
    {
        switch (query)
        {
            case "FilterByAge":
                _filters = new Filter[] { new FilterHolder("Фильтр по возрасту", FilterType.ByAge) };
                break;
        }
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
        if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
        {
            await HandleQuery(update.CallbackQuery.Data ?? "");
            return;
        }
    }
}