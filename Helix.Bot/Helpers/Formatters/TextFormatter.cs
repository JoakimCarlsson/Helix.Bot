using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Bot.Helpers.Formatters
{
    public static class TextFormatter
    {
        public static string Bold(string text) => $"**{text}**";
        public static string Italics(string text) => $"*{text}*";
        public static string Underline(string text) => $"__{text}__";
        public static string Strikethrough(string text) => $"~~{text}~~";
        public static string Spoiler(string text) => $"||{text}||";
        public static string EscapeUrl(string url) => $"<{url}>";
        public static string Quote(string text) => $">>> {text}";
    }
}
