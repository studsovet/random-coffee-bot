using Npgsql;

namespace StorageUtils;

/// <summary>
/// Класс для работы с базой данных.
/// </summary>
public static class Database
{
    private static string _host;
    private static string _user;
    private static string _dbName;
    private static string _password;
    private static string _port;
    
    private static string _connectionString = $"Host={_host};Username={_user};Password={_password};Database={_dbName};Port={_port}";
    
    private static NpgsqlConnection _connection;
    
    /// <summary>
    /// Инициализация базы данных.
    /// </summary>
    /// <param name="host">
    /// Хост.
    /// </param>
    /// <param name="user">
    /// Пользователь.
    /// </param>
    /// <param name="dbName">
    /// Название базы данных.
    /// </param>
    /// <param name="password">
    /// Пароль.
    /// </param>
    /// <param name="port">
    /// Порт.
    /// </param>
    public static void Initialize(string host, string user, string dbName, string password, string port)
    {
        _host = host;
        _user = user;
        _dbName = dbName;
        _password = password;
        _port = port;
        
        _connectionString = $"Host={_host};Username={_user};Password={_password};Database={_dbName};Port={_port}";
        
        _connection = new NpgsqlConnection(_connectionString);
        _connection.Open();
        
        InitializeDatabase();
    }
    
    /// <summary>
    /// Инициализация базы данных.
    /// </summary>
    private static void InitializeDatabase()
    {
        Console.WriteLine("Initializing database...");
        ExecuteNonQuery("CREATE TABLE IF NOT EXISTS users (id BIGINT, first_name TEXT, last_name TEXT, username TEXT, language_code TEXT, birthdate DATE)");
    }
    
    /// <summary>
    /// Выполнить запрос без возвращаемого значения.
    /// </summary>
    /// <param name="sql">
    /// SQL-запрос.
    /// </param>
    public static void ExecuteNonQuery(string sql)
    {
        using var cmd = new NpgsqlCommand(sql, _connection);
        cmd.ExecuteNonQuery();
    }
    
    /// <summary>
    /// Выполнить запрос с возвращаемым значением.
    /// </summary>
    /// <param name="sql">
    /// SQL-запрос.
    /// </param>
    /// <returns>
    /// Результат запроса.
    /// </returns>
    public static NpgsqlDataReader ExecuteReader(string sql)
    {
        using var cmd = new NpgsqlCommand(sql, _connection);
        return cmd.ExecuteReader();
    }
}