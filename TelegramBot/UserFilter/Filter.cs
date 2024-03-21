namespace TelegramBot.UserFilter;

/// <summary>
/// Тип фильтра.
/// </summary>
public abstract class Filter
{
    /// <summary>
    /// Название фильтра.
    /// </summary>
    protected string FilterName { get; init; } = "Фильтр";
    
    /// <summary>
    /// Тип фильтра.
    /// </summary>
    protected FilterType SelectedFilterType { get; init; } = FilterType.ByAge;

    /// <summary>
    /// Валидация.
    /// </summary>
    /// <param name="value">
    /// Значение.
    /// </param>
    /// <returns>
    /// Результат валидации.
    /// </returns>
    public virtual bool Validate(StorageUtils.User value)
    {
        return true;
    }

    /// <summary>
    /// Строковое представление.
    /// </summary>
    /// <returns>
    /// Строковое представление.
    /// </returns>
    public override string ToString()
    {
        return FilterName;
    }
}