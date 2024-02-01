using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ChatSystem
{
    public static class RichTextFiltering
    {

        public static string FilterRichText(string inputText, string[] whitelist)
        {
            string filteredText = inputText;

            // Define the opening and closing tag patterns
            string openTagPattern = @"<\s*([^\/s>]+)[^>]*>";
            string closeTagPattern = @"<\/\s*([^\s>]+)>";

            // Create regex patterns for opening and closing tags
            Regex openTagRegex = new Regex(openTagPattern);
            Regex closeTagRegex = new Regex(closeTagPattern);

            // Match opening tags
            MatchCollection openTagMatches = openTagRegex.Matches(filteredText);
            foreach (Match match in openTagMatches)
            {
                string tag = match.Groups[1].Value;

                // Check if the tag is not in the whitelist
                if (!IsTagInWhitelist(tag, whitelist))
                {
                    // Remove the opening tag
                    filteredText = filteredText.Replace(match.Value, "");
                }
            }

            // Match closing tags
            MatchCollection closeTagMatches = closeTagRegex.Matches(filteredText);
            foreach (Match match in closeTagMatches)
            {
                string tag = match.Groups[1].Value;

                // Check if the tag is not in the whitelist
                if (!IsTagInWhitelist(tag, whitelist))
                {
                    // Remove the closing tag
                    filteredText = filteredText.Replace(match.Value, "");
                }
            }

            return filteredText;
        }

        private static bool IsTagInWhitelist(string tag, string[] whitelist)
        {
            foreach (string allowedTag in whitelist)
            {
                if (tag.StartsWith(allowedTag))
                {
                    return true;
                }
            }
            return false;
        }
    }
}