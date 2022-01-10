namespace Dominio.Extensions
{
    public static class StringExtensions
    {
        public static string ToUpperIgnoreNull(this string value)
        {
            if (value != null)
            {
                value = value.ToUpper();
            }

            return value;

        }
    }
}
