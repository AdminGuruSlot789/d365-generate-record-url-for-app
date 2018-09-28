using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenerateAppSpecificRecordUrl
{
    public static class StringExtensions
    {
        public static string RemoveExtraWhiteSpaceInFetchXml(this string s)
        {
            // Remove new line characters.
            s = Regex.Replace(s, @"\t|\n|\r", "");

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                sb.Append(s[i]);

                // Skip unnecessary SPACE characters.
                if ((s[i] == '>' || s[i] == ' ') && (i + 2 < s.Length) && (s[i + 1] == ' '))
                {
                    while ((s[i] == '>' || s[i] == ' ') && (i + 2 < s.Length) && (s[i + 1] == ' '))
                    {
                        i++;
                        continue;
                    }
                }
            }
            return sb.ToString();
        }
    }
}
