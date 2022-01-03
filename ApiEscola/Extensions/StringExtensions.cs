namespace ApiEscola.Extensions
{
    public static class StringExtensions
    {
        internal static string ToUpperIgnoreNull(this string value)
        {
            if (value != null)
            {
                value = value.ToUpper();
            }

            return value;

        }
    }
}
