namespace TelegramBot.UserFilter;

/// <summary>
/// Тип фильтра.
/// </summary>
/// <typeparam name="T">
/// Тип данных.
/// </typeparam>
public abstract class FilterHelper<T>: UserFilter.Filter
{
    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    /// <param name="filterName">
    /// Название фильтра.
    /// </param>
    /// <param name="selectedFilterType">
    /// Тип фильтра.
    /// </param>
    protected FilterHelper(string filterName, FilterType selectedFilterType)
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
    public bool Validate(T value)
    {
        return false;
    }
    
    /// <summary>
    /// Приведение типа.
    /// </summary>
    /// <param name="o">
    /// Объект.
    /// </param>
    /// <returns>
    /// Приведенный объект.
    /// </returns>
    protected T Cast(object o)
    {
        if (!(o is T)) Console.WriteLine("Что-то пошло не так");

        return (T)o;
    }
    
    /// <summary>
    /// Расчет возраста.
    /// </summary>
    /// <param name="age">
    /// Дата рождения.
    /// </param>
    /// <returns>
    /// Возраст.
    /// </returns>
    private static int CalculateAge(DateOnly age) {  
        DateTime ageWithTime = age.ToDateTime(TimeOnly.MinValue);
        return new DateTime(DateTime.Now.Subtract(ageWithTime).Ticks).Year - 1;  
    }   

    /// <summary>
    /// Получение данных.
    /// </summary>
    /// <param name="user">
    /// Пользователь.
    /// </param>
    /// <returns>
    /// Данные.
    /// </returns>
    protected List<T> GetFields(StorageUtils.User user)
    {
        List<T> data = new List<T>();

        switch (SelectedFilterType)
        {
            case FilterType.ByAge:
                data.Add(Cast(CalculateAge(user.BirthDate)));
                break;
        }

        return data;
    }
}