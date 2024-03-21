using TelegramBot;

StorageUtils.Database.Initialize(
    Environment.GetEnvironmentVariable("DB_HOST"),
    Environment.GetEnvironmentVariable("DB_USER"),
    Environment.GetEnvironmentVariable("DB_NAME"),
    Environment.GetEnvironmentVariable("DB_PASSWORD"),
    Environment.GetEnvironmentVariable("DB_PORT")
);

var botClient = new MyTelegramBot(Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN"));

await botClient.StartPolling();

while (true);

// await botClient.StopPolling();
