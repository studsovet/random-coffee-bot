namespace StorageUtils;

public class User
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string LanguageCode { get; set; }
    public DateOnly BirthDate { get; set; }

    public bool IsExists
    {
        get
        {
            if (Id == 0)
                return false;
            
            var reader = Database.ExecuteReader($"SELECT * FROM users WHERE id = {Id};");
            
            var tmp = reader.Read();
            
            reader.Close();
            
            return tmp;
        }
    }
    
    public User(long id, string firstName, string lastName, string username, string languageCode)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Username = username;
        LanguageCode = languageCode;
    }

    public User(long id)
    {
        var reader = Database.ExecuteReader($"SELECT * FROM users WHERE id = {id};");
        
        if (reader.Read())
        {
            Id = reader.GetInt64(0);
            FirstName = reader.GetString(1);
            LastName = reader.GetString(2);
            Username = reader.GetString(3);
            LanguageCode = reader.GetString(4);
        }
        
        reader.Close();
    }
    
    public User(Telegram.Bot.Types.User user)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName ?? "";
        Username = user.Username ?? "";
        LanguageCode = user.LanguageCode ?? "RU";
    }
    
    public void Save()
    {
        if (IsExists)
        {
            Database.ExecuteNonQuery($"UPDATE users SET first_name = '{FirstName}', last_name = '{LastName}', username = '{Username}', language_code = '{LanguageCode}', birthdate = '{BirthDate}' WHERE id = {Id}");
        }
        else
        {
            Database.ExecuteNonQuery($"INSERT INTO users (id, first_name, last_name, username, language_code, birthdate) VALUES ({Id}, '{FirstName}', '{LastName}', '{Username}', '{LanguageCode}', '{BirthDate}')");
        }
    }
}