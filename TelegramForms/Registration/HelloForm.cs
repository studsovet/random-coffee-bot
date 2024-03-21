using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace TelegramForms.Registration;

public class WelcomeForm : FormBase
{
    public override async Task Load(MessageResult message)
    {
        await this.Device.Send("Привет! Я Random Coffee Bot, созданный в стенах КпЦ. Я помогу тебе найти того, " +
                               "с кем ты сможешь выпить кофе. Для начала, давай познакомимся.");
        
        await this.NavigateTo(new NameForm());
    }
}

