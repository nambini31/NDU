using System;

namespace SAIM.Core.Utilities
{
    public class Remise
    {
        public static string CreateRemiseId(string pcName)
        {
            var remiseId = string.Empty;
            var dayOfYear = DateTime.Today.DayOfYear.ToString();
            try 
            {
                remiseId = "DOC." + DateTime.Now.Year % 10 + DateTime.Now.DayOfYear.ToString("000") + "." + pcName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return remiseId;
        }

        //public static string getLastNumRemise()
        //{
        //    ////date + last sinon 1.
        //    //string 
        //    //try
        //    //{

        //    //}
        //    //catch (Exception ex)
        //    //{

        //    //}
        //}
    }
}
