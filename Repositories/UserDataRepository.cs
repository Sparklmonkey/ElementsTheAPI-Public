using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ElementsTheAPI.Settings;
using ElementsTheAPI.Models;
using ElementsTheAPI.Entities;
using ElementsTheAPI.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using ElementsTheAPI.Data;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ElementsTheAPI.Filters;

namespace ElementsTheAPI.Repositories
{
    public class UserDataRepository : IUserDataRepository
    {
        private readonly IUserDataContext _context;
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _appSettings;
        private readonly IJwtAuth _jwtAuth;

        public UserDataRepository(IUserDataContext context, IConfiguration configuration, IOptions<JwtSettings> appSettings, IJwtAuth jwtAuth)
        {
            _context = context;
            _configuration = configuration;
            _appSettings = appSettings.Value;
            _jwtAuth = jwtAuth;
        }


        public async Task<UserData> GetById(string id)
        {
            IAsyncCursor<UserData> userAsyncCursor = await _context.UserDataCollection.FindAsync(p => p.Id == id);
            return userAsyncCursor.FirstOrDefault();
        }

        private (string, string) GetUserInfoFromJWT(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            string authHeader = jwtToken;
            var jsonToken = handler.ReadToken(authHeader);
            var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
            var playerId = tokenS.Claims.First(claim => claim.Type == "playerID").Value;
            var userName = tokenS.Claims.First(claim => claim.Type == "name").Value;

            return (userName, playerId);
        }

        public async Task<AccountResponse> SaveUserData(AccountRequest accountRequest)
        {
            //Generate response
            AccountResponse response = new AccountResponse();

            //Find User in MongoDB
            IAsyncCursor<UserData> userAsyncCursor = await _context.UserDataCollection.FindAsync(p => p.Id == accountRequest.PlayerId);
            UserData userData = userAsyncCursor.FirstOrDefault();

            //Null check user 
            if (userData == null)
            {
                response.ErrorMessage = ErrorCases.UserDoesNotExist;
                return response;
            }

            //Confirm JWT belongs to current user
            var userAndID = GetUserInfoFromJWT(accountRequest.Token);
            string userName = userAndID.Item1;
            string playerId = userAndID.Item2;

            if(userData.Username != userName || userData.Id != playerId)
            {
                response.ErrorMessage = ErrorCases.UserMismatch;
                return response;
            }
            //Create new Token
            string token = _jwtAuth.Authentication(userName, playerId);
            response.Token = token;


            //Update Saved data
            var updateResult = await _context.SavedDataCollection.ReplaceOneAsync(filter: g => g.Id == userData.SavedDataId, replacement: accountRequest.SavedData);
            //Check if update complete and either update was made or at least a match was found(in case data is identical)
            if (updateResult.IsAcknowledged && (updateResult.ModifiedCount > 0 || updateResult.MatchedCount > 0))
            {
                response.ErrorMessage = ErrorCases.AllGood;
                
                return response;
            }

            //return unknown error if update failed
            response.ErrorMessage = ErrorCases.UnknownError;
            return response;
        }

        public async Task<AccountResponse> ResetUserData(AccountRequest accountRequest)
        {
            //Generate response
            AccountResponse response = new AccountResponse();

            //Find User in MongoDB
            IAsyncCursor<UserData> userAsyncCursor = await _context.UserDataCollection.FindAsync(p => p.Id == accountRequest.PlayerId);
            UserData userData = userAsyncCursor.FirstOrDefault();

            //Null Check User
            if (userData == null)
            {
                response.ErrorMessage = ErrorCases.UserDoesNotExist;
                return response;
            }

            //Confirm JWT belongs to current user
            var userAndID = GetUserInfoFromJWT(accountRequest.Token);
            string userName = userAndID.Item1;
            string playerId = userAndID.Item2;

            if (userData.Username != userName || userData.Id != playerId)
            {
                response.ErrorMessage = ErrorCases.UserMismatch;
                return response;
            }
            //Create new Token
            string token = _jwtAuth.Authentication(userName, playerId);
            response.Token = token;
            //Create new Saved Data
            SavedData newSavedData = new SavedData().GetDefault();
            //Update MongoDB with new saved data
            var updateResult = await _context.SavedDataCollection.ReplaceOneAsync(filter: g => g.Id == userData.SavedDataId, replacement: newSavedData);

            if (updateResult.IsAcknowledged && (updateResult.ModifiedCount > 0 || updateResult.MatchedCount > 0))
            {
                response.PlayerData = newSavedData;
                response.ErrorMessage = ErrorCases.AllGood;
                return response;
            }

            //return unknown error if update failed
            response.ErrorMessage = ErrorCases.UnknownError;
            return response;
        }

        public async Task<AccountResponse> UpdateUserDetails(AccountRequest accountRequest)
        {
            //Generate response
            AccountResponse response = new AccountResponse();

            //Get User from MongoDB
            IAsyncCursor<UserData> userAsyncCursor = await _context.UserDataCollection.FindAsync(p => p.Id == accountRequest.PlayerId);
            UserData userData = userAsyncCursor.FirstOrDefault();
            //Null check user
            if (userData == null)
            {
                response.ErrorMessage = ErrorCases.UserMismatch;
                return response;
            }

            //Confirm JWT belongs to current user
            var userAndID = GetUserInfoFromJWT(accountRequest.Token);
            string userName = userAndID.Item1;
            string playerId = userAndID.Item2;

            if (userData.Username != userName || userData.Id != playerId)
            {
                response.ErrorMessage = ErrorCases.UserMismatch;
                return response;
            }
            //Create new Token
            string token = _jwtAuth.Authentication(userName, playerId);
            response.Token = token;


            //Check if email should be changed
            if (accountRequest.NewEmailAddress != "")
            {
                //Confirm email is valid
                if (!accountRequest.NewEmailAddress.IsValidEmail())
                {
                    //If invalid return error
                    response.ErrorMessage = ErrorCases.IncorrectEmail;
                    return response;
                }

                //Update old email with new email
                userData.EmailAddress = accountRequest.NewEmailAddress;
            }

            //Check if passwordShould be updated
            if(accountRequest.NewPassword != "" && accountRequest.OldPassword != "")
            {
                //Confirm Old password is valid
                string hashPass = accountRequest.OldPassword.EncryptPassword(userData.Salt);
                if(userData.Password != hashPass)
                {
                    //If invalid return error
                    response.ErrorMessage = ErrorCases.IncorrectPassword;
                    return response;
                }

                //Update password
                hashPass = accountRequest.NewPassword.EncryptPassword(userData.Salt);
                userData.Password = hashPass;
            }

            //Check if Username shoud be changed
            if(userData.Username != accountRequest.Username)
            {
                //Check if username is in user
                IAsyncCursor<UserData> usernameCheckAsyncCursor = await _context.UserDataCollection.FindAsync(p => p.Username == accountRequest.Username);
                UserData usernameCheck = usernameCheckAsyncCursor.FirstOrDefault();
                //Null check user
                if (usernameCheck != null)
                {
                    response.ErrorMessage = ErrorCases.UserNameInUse;
                    return response;
                }

                //Update Username
                userData.Username = accountRequest.Username;
            }

            //Update MongoDB with new Information
            var replaceResult = await _context.UserDataCollection.ReplaceOneAsync(p => p.Id == userData.Id, userData);

            if (replaceResult.IsAcknowledged && (replaceResult.ModifiedCount > 0 || replaceResult.MatchedCount > 0))
            {
                response.ErrorMessage = ErrorCases.AllGood;
                return response;
            }

            //return unknown error if update failed
            response.ErrorMessage = ErrorCases.UnknownError;
            return response;
        }

        public Task<string> RefreshToken(string token)
        {
            var userAndID = GetUserInfoFromJWT(token.Replace("Bearer ",""));
            return Task.FromResult(_jwtAuth.Authentication(userAndID.Item1, userAndID.Item2));

        }
    }
}
