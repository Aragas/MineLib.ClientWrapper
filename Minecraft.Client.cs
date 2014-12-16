using System.Collections.Generic;
using MineLib.Network.Data;
using Newtonsoft.Json;

// -- Use http://json2csharp.com/
namespace MineLib.ClientWrapper
{
    public partial class Minecraft
    {
        // -- Debugging
        public readonly List<ChatMessage> ChatTextHistory = new List<ChatMessage>();
        // -- Debugging

        public List<string> ChatHistory = new List<string>(); 

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


        private void PlaySound(string soundName, Position coordinates, float volume, byte pitch) { }

        private void PlayEffect(int effectId, Position coordinates, int data, bool disableRelativeVolume) { }

        private void DisplayChatMessage(string message)
        {
           var Text = JsonConvert.DeserializeObject<ChatMessage>(message);

            ChatTextHistory.Add(Text);
        }

        private void EditSign(Position coordinates) { }
    }
}