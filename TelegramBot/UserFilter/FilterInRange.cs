namespace TelegramBot.UserFilter;

/// <summary>
/// Фильтр для проверки нахождения значения в диапазоне.
/// </summary>
/// <typeparam name="T">
/// Тип данных, с которым работает фильтр.
/// </typeparam>
public class FilterInRange<T> : FilterHelper<T> where T : IComparable<T>
{
    // Поля для хранения границ диапазона.
    private readonly T _from, _to;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="filterName">
    /// Название фильтра.
    /// </param>
    /// <param name="selectedFilterType">
    /// Тип фильтра.
    /// </param>
    /// <param name="from">
    /// Начало диапазона.
    /// </param>
    /// <param name="to">
    /// Конец диапазона.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Вызывается, если начало диапазона больше конца.
    /// </exception>
    public FilterInRange(string filterName, FilterType selectedFilterType, T from, T to) : base(filterName, selectedFilterType)
    {
        if (_to == null || _from == null || _from.CompareTo(_to) < 0)
        {
            throw new ArgumentException();
        }
        
        _from = from;
        _to = to;
    }
    
    /// <summary>
    /// Метод для проверки нахождения значения в диапазоне.
    /// </summary>
    /// <param name="user">
    /// Пользователь, которого нужно проверить.
    /// </param>
    /// <returns>
    /// Результат проверки.
    /// </returns>
    public override bool Validate(StorageUtils.User user)
    {
        List<T> filterField = GetFields(user);

        foreach (var item in filterField)
        {
            if (item.CompareTo(_to) * item.CompareTo(_from) <= 0)
            {
                return true;
            }
        }

        return false;
    }
}