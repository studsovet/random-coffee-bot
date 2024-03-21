namespace TelegramBot.UserFilter;

/// <summary>
/// Фильтр для поиска по содержанию.
/// </summary>
public class FilterContains : FilterHelper<string>
{
    // Поле для хранения значения для поиска.
    private readonly string _contain;
    
    /// <summary>
    /// Конструктор для создания фильтра по содержанию.
    /// </summary>
    /// <param name="filterName">
    /// Название фильтра.
    /// </param>
    /// <param name="selectedFilterType">
    /// Тип фильтра.
    /// </param>
    /// <param name="contain">
    /// Значение для поиска.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Выбрасывается, если значение для поиска равно null.
    /// </exception>
    public FilterContains(string filterName, FilterType selectedFilterType, string contain) : base(filterName, selectedFilterType)
    {
        if (contain == null)
        {
            throw new ArgumentException();
        }

        _contain = contain;
    }
    
    /// <summary>
    /// Метод для валидации.
    /// </summary>
    /// <param name="user">
    /// Пользователь, которого нужно проверить.
    /// </param>
    /// <returns>
    /// Возвращает true, если значение содержится в поле, иначе false.
    /// </returns>
    public override bool Validate(StorageUtils.User user)
    {
        List<string> filterField = GetFields(user);

        foreach (var item in filterField)
        {
            if (item.Contains(_contain))
            {
                return true;
            }
        }

        return false;
    }
}