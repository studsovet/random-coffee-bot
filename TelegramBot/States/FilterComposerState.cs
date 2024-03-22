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
    private Dictionary<string, Filter> _filters;
    private string FilterSource { get; set; }
    private string FilterValue { get; set; }
    
    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public FilterComposerState() : base(0)
    {
        _filters = new Dictionary<string, Filter>();
    }
    
    /// <summary>
    /// Конструктор с идентификатором чата.
    /// </summary>
    /// <param name="chatId">
    /// Идентификатор чата.
    /// </param>
    public FilterComposerState(long chatId) : base(chatId)
    {
        _filters = new Dictionary<string, Filter>();
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
                InlineKeyboardButton.WithCallbackData("Перейти к поиску", "StartSearch")
            },
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
                FilterSource = "FilterByAge";

                await SendMessageAsync("Напишите возраст для фильтрации: (например, 20 или 20-22)");
                break;
            case "StartSearch":
                UpdateState(new SearchState(ChatId, _filters.Values.ToArray()));
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

        if (update.Type == UpdateType.Message && update.Message != null)
        {
            if (FilterSource == "FilterByAge")
            {
                FilterValue = update.Message.Text ?? "";
                
                if (FilterValue.Contains("-"))
                {
                    string[] values = FilterValue.Split('-');
                    if (values.Length != 2)
                    {
                        await SendMessageAsync("Неверный формат. Попробуйте еще раз.");
                        return;
                    }
                    
                    uint minAge, maxAge;
                    
                    if (!uint.TryParse(values[0], out minAge) || !uint.TryParse(values[1], out maxAge) || minAge > maxAge)
                    {
                        await SendMessageAsync("Неверный формат. Попробуйте еще раз.");
                        return;
                    }
                    
                    _filters["Age"] = new FilterInRange<uint>("Возраст", FilterType.ByAge, minAge, maxAge);
                }
                else
                {
                    if (!uint.TryParse(FilterValue, out uint age))
                    {
                        await SendMessageAsync("Неверный формат. Попробуйте еще раз.");
                        return;
                    }
                    
                    _filters["Age"] = new FilterInList<uint>("Возраст", FilterType.ByAge, new List<uint> { age });
                }
                
                await SendMessageAsync("Фильтр успешно добавлен.");
                await AskForFilter();
            }
        }
    }
}