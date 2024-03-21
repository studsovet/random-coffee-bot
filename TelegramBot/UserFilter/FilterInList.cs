namespace TelegramBot.UserFilter;

/// <summary>
/// Фильтрация по списку.
/// </summary>
/// <typeparam name="T">
/// Тип данных для фильтрации.
/// </typeparam>
public class FilterInList<T> : FilterHelper<T> where T : IComparable<T>
{
    // Список для фильтрации.
    private readonly List<T> _list;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="filterName">
    /// Название фильтра.
    /// </param>
    /// <param name="selectedFilterType">
    /// Тип фильтра.
    /// </param>
    /// <param name="list">
    /// Список для фильтрации.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Выбрасывается, если список для фильтрации не был передан.
    /// </exception>
    public FilterInList(string filterName, FilterType selectedFilterType, List<T> list) : base(filterName, selectedFilterType)
    {
        _list = list ?? throw new ArgumentException();
    }
    
    /// <summary>
    /// Проверка на валидность.
    /// </summary>
    /// <param name="user">
    /// Пользователь, которого нужно проверить.
    /// </param>
    /// <returns>
    /// Возвращает true, если машина прошла фильтрацию, иначе false.
    /// </returns>
    public override bool Validate(StorageUtils.User user)
    {
        List<T> filterField = GetFields(user);

        foreach (var item in filterField)
        {
            foreach (var ourItem in _list)
            {
                if (ourItem.CompareTo(item) == 0)
                {
                    return true;
                }
            }
        }

        return false;
    }
}