using CLS_SLE.Controllers;

namespace CLS_SLE.Models
{
    public class ErrorMessage
    {
        public SLEError SLEError { get; set; }

        public string LogoutURL { get; set; }

        public bool HasErrorMessage
        {
            get
            {
                return (this.SLEError != SLEError.Undefined);
            }
        }
    }
}