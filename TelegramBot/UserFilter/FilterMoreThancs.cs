namespace TelegramBot.UserFilter;

/// <summary>
/// Фильр для поиска машин по полю, значение которого больше заданного.
/// </summary>
/// <typeparam name="T">
/// Тип поля, по которому будет производиться фильтрация.
/// </typeparam>
public class FilterMoreThan<T> : FilterHelper<T> where T : IComparable<T>
{
    // Поле для хранения значения, с которым будет производиться сравнение.
    private readonly T _to;
    
    /// <summary>
    /// Конструктор фильтра.
    /// </summary>
    /// <param name="filterName">
    /// Название фильтра.
    /// </param>
    /// <param name="selectedFilterType">
    /// Тип фильтра.
    /// </param>
    /// <param name="to">
    /// Значение, с которым будет производиться сравнение.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Выбрасывается, если значение to равно null.
    /// </exception>
    public FilterMoreThan(string filterName, FilterType selectedFilterType, T to) : base(filterName, selectedFilterType)
    {
        _to = to ?? throw new ArgumentException();
    }
    
    /// <summary>
    /// Метод для валидации машины.
    /// </summary>
    /// <param name="user">
    /// Пользователь, которого нужно проверить.
    /// </param>
    /// <returns>
    /// Возвращает true, если машина прошла валидацию, иначе false.
    /// </returns>
    public override bool Validate(StorageUtils.User user)
    {
        List<T> filterField = GetFields(user);

        foreach (var item in filterField)
        {
            if (item.CompareTo(_to) >= 0)
            {
                return true;
            }
        }

        return false;
    }
}