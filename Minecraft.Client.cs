using System;

namespace MineLib.ClientWrapper
{
    public partial class Minecraft
    {
        private void PlaySound(string SoundName, int X, int Y, int Z,
            float Volume, byte Pitch)
        {
        }

        private void PlayEffect(int EffectID, int X, byte Y, int Z,
            int Data, bool DisableRelativeVolume)
        {
        }

        private void DisplayChatMessage(string message)
        {
            bool bold;
            bool italic;
            bool underlined;
            bool strikethrough;
            bool obfuscated;

            string text;
            string colour;
            string[] temp;


            temp = message.Split(new string[] { "text\":\"" }, StringSplitOptions.RemoveEmptyEntries);
            text = temp[1].Split('"')[0];

            //temp = message.Split(new string[] { "bold\":\"" }, StringSplitOptions.RemoveEmptyEntries);
            //bold = Boolean.Parse(temp[1].Split('"')[0]);

            //temp = message.Split(new string[] { "italic\":\"" }, StringSplitOptions.RemoveEmptyEntries);
            //italic = Boolean.Parse(temp[1].Split('"')[0]);

            //temp = message.Split(new string[] { "underlined\":\"" }, StringSplitOptions.RemoveEmptyEntries);
            //underlined = Boolean.Parse(temp[1].Split('"')[0]);

            //temp = message.Split(new string[] { "strikethrough\":\"" }, StringSplitOptions.RemoveEmptyEntries);
            //strikethrough = Boolean.Parse(temp[1].Split('"')[0]);

            //temp = message.Split(new string[] { "obfuscated\":\"" }, StringSplitOptions.RemoveEmptyEntries);
            //obfuscated = Boolean.Parse(temp[1].Split('"')[0]);

            temp = message.Split(new string[] { "color\":\"" }, StringSplitOptions.RemoveEmptyEntries);
            colour = temp[1].Split('"')[0];

            //Console.WriteLine(message);
            Console.WriteLine(text);
            //Console.WriteLine(colour);
            //Console.Read();
        }

        private void EditSign(int x, int y, int z)
        {
        }
    }
}