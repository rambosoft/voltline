using System.Text.RegularExpressions;

namespace Voltline.Data
{
    public static class StableIdUtility
    {
        private static readonly Regex StableIdPattern = new(@"^[a-z0-9]+(?:\.[a-z0-9-]+)+$", RegexOptions.Compiled);

        public static bool IsValid(string candidate)
        {
            return !string.IsNullOrWhiteSpace(candidate) && StableIdPattern.IsMatch(candidate);
        }
    }
}