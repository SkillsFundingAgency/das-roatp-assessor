using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ViewModelBase
    {
        public Dictionary<string, string> ErrorDictionary { get; set; }

        protected ViewModelBase()
        {
            ErrorDictionary = new Dictionary<string, string>();
        }

        protected string GetErrorMessage(string propertyName)
        {
            return (ErrorDictionary.Any() && ErrorDictionary.TryGetValue(propertyName, out var value) ? value : "") ?? string.Empty;
        }
    }
}
