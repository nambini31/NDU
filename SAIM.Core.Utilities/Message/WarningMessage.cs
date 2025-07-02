namespace SAIM.Core.Utilities.Message
{
    public class WarningMessage
    {
        public static string Get(string errorCode)
        {
            return errorCode switch
            {
                "F2" => "Test",
                _ => ""
            };
        }
    }
}