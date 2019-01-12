namespace WhatANerd.Extensions
{
    using Discord;

    public static class MessageExtension
    {
        public static bool HasMentionPrefix(this IUserMessage message, IUser user, out string output)
        {
            var content = message.Content;
            output = string.Empty;

            if (string.IsNullOrEmpty(content) || content.Length <= 3 || content[0] != '<' || content[1] != '@')
                return false;

            var endPos = content.IndexOf('>');
            if (endPos == -1)
                return false;

            if (content.Length < endPos + 2 || content[endPos + 1] != ' ')
                return false;

            if (!MentionUtils.TryParseUser(content.Substring(0, endPos + 1), out var userId))
                return false;

            if (userId != user.Id)
                return false;

            output = content.Substring(endPos + 2);
            return true;
        }
    }
}