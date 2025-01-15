using TextCopy;

namespace DiscordAboutMeChanger;

class Program
{
    static void Main(string[] args)
    {
        var userInput = "";
        var unixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        Console.WriteLine("Enter your account balance: ");
        userInput = Console.ReadLine();
        var aboutMeString = $"Known as HakuSystems & lyze but you can call me Noah\n**Please get to know me properly before you put me in a box.**\n**{userInput}** (as of **<t:{unixTimestamp}:R>**)";
        Console.WriteLine("Your new about me string is: ");
        Console.WriteLine(aboutMeString);
        ClipboardService.SetText(aboutMeString);
    }
}
