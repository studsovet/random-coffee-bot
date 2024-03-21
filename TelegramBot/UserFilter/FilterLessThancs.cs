namespace TelegramBot.UserFilter;

/// <summary>
/// Фильтр для сравнения меньше.
/// </summary>
/// <typeparam name="T">
/// Тип данных для сравнения.
/// </typeparam>
public class FilterLessThan<T> : FilterHelper<T> where T : IComparable<T>
{
    // Поле для сравнения.
    private readonly T _from;
    
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
    /// Значение для сравнения.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Выбрасывается, если значение для сравнения не задано.
    /// </exception>
    public FilterLessThan(string filterName, FilterType selectedFilterType, T from) : base(filterName, selectedFilterType)
    {
        _from = from ?? throw new ArgumentException();
    }
    
    /// <summary>
    /// Проверка на валидность.
    /// </summary>
    /// <param name="user">
    /// Машина, для которой проверяется валидность.
    /// </param>
    /// <returns>
    /// Результат проверки.
    /// </returns>
    public override bool Validate(StorageUtils.User user)
    {
        List<T> filterField = GetFields(user);

        foreach (var item in filterField)
        {
            if (item.CompareTo(_from) <= 0)
            {
                return true;
            }
        }

        return false;
    }
}