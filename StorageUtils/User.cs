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
        Id = id;
        
        Load();
    }
    
    public User(Telegram.Bot.Types.User user): this(user.Id) {}

    private void Load()
    {
        var reader = Database.ExecuteReader($"SELECT * FROM users WHERE id = {Id};");
        
        if (reader.Read())
        {
            FirstName = reader.GetString(1);
            LastName = reader.GetString(2);
            Username = reader.GetString(3);
            LanguageCode = reader.GetString(4);

            var date = reader.GetDateTime(5);
            BirthDate = DateOnly.FromDateTime(date);
        }
        
        reader.Close();
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