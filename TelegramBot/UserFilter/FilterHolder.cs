namespace TelegramBot.UserFilter;

/// <summary>
/// Заглушка фильтра.
/// </summary>
public class FilterHolder: Filter
{
    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public FilterHolder()
    {
        FilterName = "Фильтр";
        SelectedFilterType = FilterType.ByAge;
    }
    
    /// <summary>
    /// Конструктор с параметрами.
    /// </summary>
    /// <param name="filterName">
    /// Название фильтра.
    /// </param>
    /// <param name="selectedFilterType">
    /// Тип фильтра.
    /// </param>
    public FilterHolder(string filterName, FilterType selectedFilterType)
    {
        FilterName = filterName;
        SelectedFilterType = selectedFilterType;
    }
    
    /// <summary>
    /// Валидация.
    /// </summary>
    /// <param name="value">
    /// Значение.
    /// </param>
    /// <returns>
    /// Результат валидации.
    /// </returns>
    public override bool Validate(StorageUtils.User value)
    {
        return true;
    }
}