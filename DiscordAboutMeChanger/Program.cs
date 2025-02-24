using TextCopy;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

namespace DiscordAboutMeChanger
{
    class Program
    {
        // P/Invoke-Deklarationen für die Konsolenverwaltung.
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();
        
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        static void Main(string[] args)
        {
            EnsureNewConsole(); // Neue Konsole erzwingen.
            StoreUserCulture(); // Nutzersprache speichern.

            var deCulture = CultureInfo.GetCultureInfo("de-DE");
            var accountBalance = GetValidAccountBalance(deCulture);
            if (accountBalance == null)
                return;

            long unixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            string aboutMeString = CreateAboutMeString(accountBalance.Value, unixTimestamp);

            Console.WriteLine("\nYour new about me string is:\n" + aboutMeString);
            TryCopyToClipboard(aboutMeString);
        }

        // Erzwingt eine neue Konsole, indem vorhandene Konsolen getrennt werden.
        static void EnsureNewConsole()
        {
            FreeConsole();   // Vorhandene Konsole trennen.
            AllocConsole();  // Neue Konsole zuweisen.
        }

        // Speichert die ermittelte Nutzersprache in einer Einstellungsdatei.
        static void StoreUserCulture()
        {
            var detectedCulture = CultureInfo.CurrentCulture.Name;
            var settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "AppSettings.json");
            File.WriteAllText(settingsPath, $"{{ \"UserCulture\": \"{detectedCulture}\" }}");
            Console.WriteLine($"Detected your region as '{detectedCulture}' and stored settings.");
        }

        // Liest die Eingabe für den Kontostand ein und validiert diese.
        static decimal? GetValidAccountBalance(CultureInfo deCulture)
        {
            Console.Write("Enter your account balance (in Euro): ");
            var userInput = Console.ReadLine()?.Trim();

            // Entfernt das Euro-Zeichen.
            userInput = userInput?.Replace("€", "");

            // Falls die Eingabe nur Ziffern enthält und länger als 2 Zeichen ist,
            // wird angenommen, dass die letzten beiden Ziffern Dezimalstellen sind.
            // Beispiel: "249609" wird zu "2496,09"
            if (!string.IsNullOrEmpty(userInput) && 
                userInput.All(char.IsDigit) && 
                userInput.Length > 2)
            {
                userInput = userInput.Insert(userInput.Length - 2, ",");
            }
            
            if (string.IsNullOrEmpty(userInput) || !userInput.Any(char.IsDigit))
            {
                Console.WriteLine("Invalid input. Please enter a valid monetary amount in Euro.");
                return null;
            }

            if (!TryParseBalance(userInput, deCulture, out decimal balance))
            {
                Console.WriteLine("Invalid input. Please enter a valid monetary amount in Euro.");
                return null;
            }
            return balance;
        }

        // Versucht, den Kontostand mit de-DE-Formatierung zu parsen, inklusive Korrekturen.
        static bool TryParseBalance(string input, CultureInfo deCulture, out decimal accountBalance)
        {
            // Versuch mit de-DE-Formatierung.
            if (decimal.TryParse(input, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, deCulture, out accountBalance))
                return true;
            
            // Versuch der automatischen Korrektur der Dezimaltrennzeichen.
            string fixedInput;
            if (input.contains('.') && !input.contains(','))
            {
                fixedInput = input.Replace(".", ",");
                if (decimal.TryParse(fixedInput, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, deCulture, out accountBalance))
                {
                    Console.WriteLine($"Input auto-corrected from '{input}' to '{fixedInput}'.");
                    return true;
                }
            }
            else if (input.contains(',') && !input.contains('.'))
            {
                fixedInput = input.Replace(",", ".");
                if (decimal.TryParse(fixedInput, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.InvariantCulture, out accountBalance))
                {
                    Console.WriteLine($"Input auto-corrected from '{input}' to '{fixedInput}'.");
                    return true;
                }
            }

            return false;
        }

        // Erstellt den "about me"-String anhand des Kontostandes und Zeitstempels.
        static string CreateAboutMeString(decimal accountBalance, long unixTimestamp)
        {
            return $"Known as HakuSystems & lyze but you can call me Noah\n" +
                   $"**Please get to know me properly before you put me in a box.**\n" +
                   // Es wird nur der numerische Wert angezeigt.
                   $"**{accountBalance:N2}** (as of **<t:{unixTimestamp}:R>**)";
        }

        // Versucht, den Text in die Zwischenablage zu kopieren.
        static void TryCopyToClipboard(string text)
        {
            try
            {
                ClipboardService.SetText(text);
                Console.WriteLine("\nThe string has been copied to your clipboard successfully.");
                Console.WriteLine("Notification: Your string has been copied to the clipboard. You can now paste it into Discord.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFailed to copy string to clipboard: {ex.Message}");
            }
        }
    }
}
