namespace CastManager.StringsResources
{
    using System.Windows;
    internal static class Strings
    {
        static public string ById(string id)
        {
            try
            {
                return Application.Current.FindResource(id).ToString();
            }
            catch
            {

            }
            return $"ResById:'{id}'";
        }

        internal static string Wrng_TabloIsNotRunning => ById("Wrng_TabloIsNotRunning");
        internal static string Expected => ById("Expected_Str");
        internal static string NotFound => ById("NotFound_Str");
        internal static string SelectNetwork => ById("Select_NetworkStr");
        internal static string EnterPassword => ById("EnterPassword_Str");
        internal static string ConnectToPicasterError => ById("ConnectToPicasterError_Str");
        internal static string IPAddress => ById("IPAddress_Str");

        

    }
}
