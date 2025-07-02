using System;
using System.Linq;

namespace SAIM.Core.Utilities
{
    public class PasswordGenerator
    {
        public static string Generate()
        {
            var password = "";
            const string symbols = "`~!@#$%^&*()_-+=/*-+':;/?.>,<";
            const string characters = "qwertyuiopasdfghjklzxcvbnm";
            const string numbers = "0123456789";
            var randomLength = new Random().Next(8, 16);
            var random = new Random();
            for (var i = 0; i < 2; i++)
            {
                var indiceSymbol = random.Next(0, symbols.Length - 1);
                password += symbols[indiceSymbol].ToString();
            }
            random = new Random();
            for (var i = 0; i < 3; i++)
            {
                var indiceCharacter = random.Next(0, characters.Length - 1);
                password += characters[indiceCharacter].ToString();
            }
            random = new Random();
            for (var i = 0; i < 3; i++)
            {
                var indiceNumber = random.Next(0, numbers.Length - 1);
                password += numbers[indiceNumber].ToString();
            }
            random = new Random();
            for (var i = 0; i < randomLength - 8; i++)
            {
                var indiceUpperCase = random.Next(0, characters.Length - 1);
                password += characters[indiceUpperCase].ToString().ToUpper();
            }
            random = new Random();
            return new string(password.ToCharArray().OrderBy(x => random.Next()).ToArray());
        }
    }
}