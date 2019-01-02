using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 字符串信息验证-扩展方法
    /// </summary>
    public  static class StringValidatorExtension
    {
        public static bool ValidateNotNull(this string Text)
        {
            return Text != null;
        }

        public static bool ValidateStrictNotNull(this string Text)
        {
            return Text != null && Text != "";
        }

        public static bool ValidateInt(this string Text)
        {
            try
            {
                int.Parse(Text);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool ValidateDouble(this string Text)
        {
            try
            {
                double.Parse(Text);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool ValidateBool(this string Text)
        {
            try
            {
                bool.Parse(Text.ToLower());
            }
            catch
            {
                return false;
            }
            return true;
        }

        private static bool ContainSpecialChar(this string Text, char[] IgnoreChars)
        {
            if (Text == null || Text == "")
            {
                return false;
            }
            else
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                if (IgnoreChars != null)
                {
                    foreach (char c in IgnoreChars)
                    {
                        builder.Append(c);
                    }
                }
                string ignore = builder.ToString();

                char[] specialChars = new char[] { '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '[', '{', ']', '}', '\\', '|', ';', ':', '\'', '\"', ',', '<', '.', '>', '/', '?', ' ' };
                foreach (char specialChar in specialChars)
                {
                    if (ignore.IndexOf(specialChar) == -1 && Text.IndexOf(specialChar) != -1)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static bool ValidateNotNullStrictText(this string Text, char[] IgnoreChars)
        {
            if (!ValidateStrictNotNull(Text))
            {
                return false;
            }
            else if (ContainSpecialChar(Text, IgnoreChars))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool ValidateNotNullStrictText(this string Text)
        {
            return ValidateNotNullStrictText(Text, null);
        }

        public static bool ValidateStrictText(this string Text, char[] IgnoreChars)
        {
            if (Text == null || Text == "")
            {
                return true;
            }
            else if (ContainSpecialChar(Text, IgnoreChars))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool ValidateStrictText(this string Text)
        {
            return ValidateStrictText(Text, null);
        }

        public static bool ValidateAvoidSQL(this string Text)
        {
            if (Text == null || Text == "")
            {
                return true;
            }
            else
            {
                char[] specialChars = new char[] { '\'', '@', '<', '>', '!', '&', '|' };
                foreach (char specialChar in specialChars)
                {
                    if (Text.IndexOf(specialChar) != -1)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
