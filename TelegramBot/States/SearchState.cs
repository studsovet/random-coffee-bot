using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.UserFilter;

namespace TelegramBot.States;

/// <summary>
/// Делегат обработки немедленного поиска.
/// </summary>
delegate void HandleImmidiateSearchDelegate(SearchState user);

/// <summary>
/// Состояние поиска.
/// </summary>
public class SearchState: BaseState
{
    private StorageUtils.User _user;
    private static HandleImmidiateSearchDelegate? _handleImmidiateSearchDelegate;
    private bool _isSearching = false;
    private Filter[] _filters = new Filter[] { };
    
    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public SearchState() : base(0)
    {
    }
    
    /// <summary>
    /// Проверить пользователя на соответствие фильтрам.
    /// </summary>
    /// <param name="user">
    /// Пользователь.
    /// </param>
    /// <returns>
    /// Результат проверки.
    /// </returns>
    public bool IsValidForSearch(StorageUtils.User user)
    {
        foreach (var filter in _filters)
        {
            if (!filter.Validate(user))
            {
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Конструктор с идентификатором чата.
    /// </summary>
    /// <param name="chatId">
    /// Идентификатор чата.
    /// </param>
    public SearchState(long chatId) : base(chatId)
    {
    }
    
    /// <summary>
    /// Конструктор с идентификатором чата и фильтрами.
    /// </summary>
    /// <param name="chatId">
    /// Идентификатор чата.
    /// </param>
    /// <param name="filterses">
    /// Фильтры.
    /// </param>
    public SearchState(long chatId, Filter[] filterses) : base(chatId)
    {
        _filters = filterses;
    }
    
    /// <summary>
    /// Остановить поиск.
    /// </summary>
    /// <param name="user">
    /// Пользователь, который остановил поиск.
    /// </param>
    private void StopSearch(SearchState user)
    {
        _handleImmidiateSearchDelegate -= HandleImmidiateSearchDelegate;
        _isSearching = false;
        SendMessageAsync($"Мы нашли вам собеседника!\nВот контакт: @{user._user.Username} ({user._user.FirstName} {user._user.LastName})").Wait();
        UpdateState<MainState>();
    }
    
    /// <summary>
    /// Обработать немедленный поиск.
    /// </summary>
    /// <param name="user">
    /// Пользователь.
    /// </param>
    private void HandleImmidiateSearchDelegate(SearchState user)
    {
        if (!_isSearching || !user._isSearching)
            return;
        
        if (!user.IsValidForSearch(_user) || !IsValidForSearch(user._user))
            return;
        
        user.StopSearch(this);
        StopSearch(user);
    }

    /// <summary>
    /// Инициализация состояния.
    /// </summary>
    public override async Task InitializeState()
    {
        _isSearching = true;
        _user = new StorageUtils.User(ChatId);
        
        if (!_user.IsExists)
        {
            UpdateState<WelcomeState>();
            return;
        }

        _handleImmidiateSearchDelegate?.Invoke(this);

        if (!_isSearching)
        {
            return;
        }
        
        _handleImmidiateSearchDelegate += HandleImmidiateSearchDelegate;
        
        await SendMessageAsync("Поиск собеседника...", new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Отмена", callbackData: "cancel")
            }
        }));
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
                case "cancel":
                    _isSearching = false;
                    UpdateState<MainState>();
                    break;
            }
            
            return;
        }
        
        if (update.Message is not { } message)
            return;
        
        if (message.Text is not { } messageText)
            return;
        
        if (message.From is not { } user)
            return;
        
        await SendMessageAsync("Мы все еще ищем вам собеседника...\nНо, если что, есть кнопка ниже.", new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Отмена", callbackData: "cancel")
            }
        }));
    }
}