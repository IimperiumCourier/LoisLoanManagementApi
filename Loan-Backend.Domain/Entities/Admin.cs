using Loan_Backend.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Entities
{
    public class Admin : BaseEntity<Guid>
    {
        public string FullName { get; private set; }
        public string EmailAddress { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Password { get; private set; }
        public string RefreshToken { get; private set; }
        public DateTime RefreshTokenExpireOn { get; private set; }
        public DateTime DateCreated { get; private set; }
        public bool IsActive { get; private set; }
        public int RetryCount { get; private set; }
        public DateTime LastDateUpdated { get; private set; }


        public Admin(Guid id):base(id)
        {
                
        }

        public Admin() : base(Guid.NewGuid())
        {

        }

        public static Admin Create(string fullName, string email, string phoneNo, string passWord)
        {
            return new Admin
            {
                EmailAddress = email.ToLower(),
                FullName = fullName.ToProperCase(),
                Password = Sha512HashGenerator.GenerateHash(Common.PWD_Salt+passWord),
                PhoneNumber = phoneNo,
                IsActive = true,
                DateCreated = DateTime.UtcNow.AddHours(1)
            };
        }

        public void ChangePassword(string password)
        {
            Password = Sha512HashGenerator.GenerateHash(Common.PWD_Salt + password);
            LastDateUpdated = DateTime.UtcNow.AddHours(1);
            ResetPasswordTrialCount();
        }

        public void ResetPasswordTrialCount()
        {
            RetryCount = 0;
        }

        public void Deactivate()
        {
            IsActive = false;
            LastDateUpdated = DateTime.UtcNow.AddHours(1);
        }

        public void Activate()
        {
            IsActive = true;
            LastDateUpdated = DateTime.UtcNow.AddHours(1);
        }

        public (bool isValid, string message) IsPasswordValid(string password)
        {

            bool isValidPassword = string.Equals(Password, Sha512HashGenerator.GenerateHash(Common.PWD_Salt + password));
            if (!isValidPassword)
            {
                RetryCount += 1;
            }

            if(RetryCount != 0)
            {
                RetryCount = 0;
            }

            return (isValidPassword, "Password verified successfully");
        }

        public (bool isPasswordRetryCountExceeded, string message) CheckRetryCount()
        {

            if (RetryCount >= Common.RetryCountLimit)
            {
                return (true, "Hi, retry count has been exceeded contact super admin to initiate password reset");
            }

            return (false, "");
        }

        public string SetRefreshToken()
        {
            RefreshToken = Guid.NewGuid().ToString("N");
            RefreshTokenExpireOn = DateTime.UtcNow.AddHours(24);

            return RefreshToken;
        }

        public bool RefreshTokenExpired()
        {
            return RefreshTokenExpireOn <= DateTime.UtcNow;
        }

        public AdminRole SetRole(Guid roleId, Guid grantedBy)
        {
            return AdminRole.Create(Id, roleId, grantedBy);
        }
    }
}
