using System;
namespace ElementsTheAPI.Entities
{
    public enum ErrorCases
    {
        UserNameInUse,
        UserDoesNotExist,
        IncorrectPassword,
        AllGood,
        UserMismatch,
        UnknownError,
        IncorrectEmail,
        OtpIncorrect,
        OtpExpired,
        AccountNotVerified
    }
}
