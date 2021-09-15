using Remora.Discord.Core;

namespace Helix.Bot.Helpers.Formatters
{
    public static class MentionFormatter
    {
        public static string User(Snowflake snowflake) => $"<@{snowflake.Value}>";
        public static string Channel(Snowflake snowflake) => $"<#{snowflake.Value}>";
        public static string Role(Snowflake snowflake) => $"<@&{snowflake.Value}>";
    }
}
