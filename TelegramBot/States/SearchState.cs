using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.UserFilter;
using File = Telegram.Bot.Types.File;

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
    private async Task StopSearch(SearchState user)
    {
        _handleImmidiateSearchDelegate -= HandleImmidiateSearchDelegate;
        _isSearching = false;
        
        UserProfilePhotos photos = await user.GetUserProfilePhotos();
        if (photos.Photos.Length == 0)
        {
            await SendMessageAsync("Мы не смогли найти фотографии вашего собеседника. Но вот его контакт: @" + user._user.Username);
            return;
        }
        
        string fileId = photos.Photos[0][0].FileId;
        File? file = await user.GetFileById(fileId);
        
        if (file == null)
        {
            await SendMessageAsync("Мы не смогли найти фотографии вашего собеседника. Но вот его контакт: @" + user._user.Username);
            return;
        }
        
        SendPhotoAsync(file, $"Мы нашли вам собеседника!\nВот контакт: @{user._user.Username} ({user._user.FirstName} {user._user.LastName})").Wait();
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

        user.StopSearch(this).Wait();
        StopSearch(user).Wait();
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