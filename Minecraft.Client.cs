using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// Use http://json2csharp.com/
namespace MineLib.ClientWrapper
{
    public partial class Minecraft
    {
        public List<ChatMessage> ChatMessage1 = new List<ChatMessage>(); 

        public struct Extra
        {
            [JsonProperty("color")]
            public string Color { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }

        }

        public struct ClickEvent
        {
            [JsonProperty("action")]
            public string Action { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        public struct HoverEvent
        {
            [JsonProperty("action")]
            public string Action { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        public struct ChatMessage
        {
            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("id")]
            public string Translate { get; set; }

            [JsonProperty("with")]
            public List<object> With { get; set; }

            [JsonProperty("extra")]
            public List<object> Extra { get; set; }

            [JsonProperty("bold")]
            public bool Bold { get; set; }

            [JsonProperty("italic")]
            public bool Italic { get; set; }

            [JsonProperty("underlined")]
            public bool Underlined { get; set; }

            [JsonProperty("strikethrough")]
            public bool StrikeThrough { get; set; }

            [JsonProperty("obfuscated")]
            public bool Obfuscated { get; set; }

            [JsonProperty("color")]
            public string Color { get; set; }

            [JsonProperty("hoverEvent")]
            public HoverEvent HoverEvent { get; set; }

            [JsonProperty("clickEvent")]
            public ClickEvent ClickEvent { get; set; }

        }


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
           var Text = JsonConvert.DeserializeObject<ChatMessage>(message);

            ChatMessage1.Add(Text);
        }

        private void EditSign(int x, int y, int z)
        {
        }
    }
}