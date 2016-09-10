using System.IO;
using System.Text.RegularExpressions;

namespace vk_search_v3.Util
{
    internal class FileUtil
    {
        private static readonly Regex IllegalFilenameRegex = new Regex(string.Format("[{0}]", new string(Path.GetInvalidFileNameChars())));

        public static string FilterIllegalFilenameCharacters(string input)
        {
            return IllegalFilenameRegex.Replace(input, "");
        }
    }
}