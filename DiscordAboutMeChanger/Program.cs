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
        var aboutMeString = $"**Jack of all trades, master of none, but oftentimes better than a master of one.** \n:moneybag: Account Balance (as of **<t:{unixTimestamp}:R>**): **{userInput}** \n`This value gets updated every 1-3 days`";
        Console.WriteLine("Your new about me string is: ");
        Console.WriteLine(aboutMeString);
        ClipboardService.SetText(aboutMeString);
    }
}
