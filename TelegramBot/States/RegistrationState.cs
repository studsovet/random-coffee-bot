using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.States;

/// <summary>
/// Класс состояния регистрации.
/// </summary>
public class RegistrationState : BaseState
{
    /// <summary>
    /// Имя.
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// Фамилия.
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// Дата рождения.
    /// </summary>
    public DateOnly? BirthDate { get; set; }
    
    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public RegistrationState() : base(0)
    {
        
    }

    /// <summary>
    /// Конструктор с идентификатором чата.
    /// </summary>
    public override async Task InitializeState()
    {
        await SendMessageAsync("Введите ваше имя:");
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
        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Message is not { } message)
            return;
        
        // Only process text messages
        if (message.Text is not { } messageText)
            return;
        
        // Only process with user
        if (message.From is not { } user)
            return;
        
        if (FirstName == null)
        {
            FirstName = messageText;
            await SendMessageAsync("Введите вашу фамилию:");
        }
        else if (LastName == null)
        {
            LastName = messageText;
            await SendMessageAsync("Введите ваш день рождения: (формат \"dd.mm.yyyy\")");
        }
        else if (BirthDate == null)
        {
            if (DateOnly.TryParse(messageText, out DateOnly date))
            {
                BirthDate = date;
            }
            else if (messageText.Length == 10 && messageText[2] == '.' && messageText[5] == '.')
            {
                string[] parts = messageText.Split('.');
                
                if (int.TryParse(parts[0], out int day) && int.TryParse(parts[1], out int month) && int.TryParse(parts[2], out int year))
                {
                    BirthDate = new DateOnly(year, month, day);
                } 
                else
                {
                    await SendMessageAsync("Неверный формат даты. Попробуйте еще раз.");
                    return;
                }
            }
            else
            {
                await SendMessageAsync("Неверный формат даты. Попробуйте еще раз.");
                return;
            }
            
            await SendMessageAsync("Спасибо за регистрацию!");
            
            SetData(user);
            
            UpdateState<MainState>();
        }
    }

    /// <summary>
    /// Установить данные.
    /// </summary>
    /// <param name="user">
    /// Пользователь.
    /// </param>
    public void SetData(Telegram.Bot.Types.User? user) 
    {
        StorageUtils.User userInDb = new StorageUtils.User(user);
            
        userInDb.FirstName = FirstName;
        userInDb.LastName = LastName;
        userInDb.LanguageCode = user.LanguageCode ?? "ru";
        userInDb.Username = user.Username ?? "-";
        userInDb.BirthDate = BirthDate ?? new DateOnly(1900, 1, 1);
        
        userInDb.Save();
    }
}