namespace Loan_Backend.SharedKernel.Model.Request
{
    public class PasswordChangeRequest
    {
        public string Password { get; set; } = string.Empty;
        public string ConfirmedPassword { get; set; } = string.Empty;
    }
}
