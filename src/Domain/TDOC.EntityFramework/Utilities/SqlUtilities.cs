namespace TDOC.EntityFramework.Utilities
{
    /// <summary>
    ///
    /// </summary>
    public static class SqlUtilities
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public static int? SQLKeyId(int? keyId)
        {
            if ((keyId ?? 0) > 0)
                return (int)keyId;
            else
            {
                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SQLStringValue(string value)
        {
            if (value != "")
                return value;
            else
            {
                return null;
            }
        }
    }
}