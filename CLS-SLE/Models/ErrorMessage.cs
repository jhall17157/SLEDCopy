namespace CLS_SLE.Models
{
    public class ErrorMessage
    {
        public string Message { get; set; }

        public string LogoutURL { get; set; }

        public bool HasErrorMessage
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(this.Message));
            }
        }
    }
}