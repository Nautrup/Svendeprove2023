using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Common
{
    public class ExceptionHttpHelper
    {
        public int StatusCode { get; private set; }
        public string StatusDescription { get; private set; }
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Hjælper klasse til nemmere og bruge WebExceptions
        /// </summary>
        /// <param name="ex">Webexception</param>
        /// <exception cref="Exception">Brug denne i en MessageBox</exception>
        public ExceptionHttpHelper(WebException ex)
        {
            try
            {
                using (HttpWebResponse res = ex.Response as HttpWebResponse)
                {
                    using (Stream test = res.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(test);
                        StatusCode = (int)res.StatusCode;
                        StatusDescription = res.StatusDescription;
                        ErrorMessage = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception exex)
            {
                throw new Exception(exex.Message);
            }
            
        }

    }
}
