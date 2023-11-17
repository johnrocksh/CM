
namespace CastManager.Models.Extension
{
    internal static class EnumExtension
    {
        public static TEnumResult ToEnum<TEnumFrom, TEnumResult>(this TEnumFrom e)
            where TEnumFrom : System.Enum
            where TEnumResult : System.Enum
        {
            var eStr = e.ToString();
            foreach (TEnumResult eRes in Enum.GetValues(typeof(TEnumResult)))
            {
                var eResStr = eRes.ToString();
                if (eResStr.Equals(eStr))
                    return eRes;
            }

            return default(TEnumResult);
        }
        public static bool ToEnum<TEnumFrom, TEnumResult>(this TEnumFrom e, out TEnumResult eR) 
            where TEnumFrom : System.Enum
            where TEnumResult : System.Enum
        {
            eR = ToEnum<TEnumFrom, TEnumResult>(e);
            return eR != null;
        }

        public static TEnumResult ToEnum<TEnumFrom, TEnumResult>(this TEnumFrom e, TEnumResult _)
            where TEnumFrom : System.Enum
            where TEnumResult : System.Enum
        {
            return ToEnum<TEnumFrom, TEnumResult>(e);
        }
    }
}
