using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace TelegramForms.Registration;

public class NameForm : AutoCleanForm
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public bool IsFilled => this.Firstname != null && this.Lastname != null;
    
    public override async Task Load(MessageResult message)
    {
        if (string.IsNullOrWhiteSpace(message.MessageText)) return;

        if (this.Firstname == null)
        {
            this.Firstname = message.MessageText;
            return;
        }

        if (this.Lastname == null)
        {
            this.Lastname = message.MessageText;
            return;
        }
    }

    public override async Task Action(MessageResult message)
    {
        var call = message.GetData<CallbackData>();
        await message.ConfirmAction();

        if (call == null) return;

        switch (call.Value)
        {
            case "back":
                await this.NavigateTo(new NameForm());
                break;
        }
    }

    public override async Task Render(MessageResult message)
    {
        if (this.Firstname == null)
        {
            await this.Device.Send("Please sent your firstname:");
            return;
        }

        if (this.Lastname == null)
        {
            await this.Device.Send("Please sent your lastname:");
            return;
        }

        string s = "";

        s += "Firstname: " + this.Firstname + "\r\n";
        s += "Lastname: " + this.Lastname + "\r\n";

        ButtonForm bf = new ButtonForm();
        bf.AddButtonRow(new ButtonBase("Back", new CallbackData("a", "back").Serialize()));

        await this.Device.Send("Your details:\r\n" + s, bf);
    }
}