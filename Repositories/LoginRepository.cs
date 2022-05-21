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
using MySqlConnector;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using System.Collections.Generic;

namespace ElementsTheAPI.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly MailSettings _mailSettings;
        private readonly IUserDataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IJwtAuth _jwtAuth;

        private const long otpTimeOut = (long)600000000 * 5;

        public LoginRepository(IUserDataContext context, IConfiguration configuration, IOptions<MailSettings> mailSettings, IJwtAuth jwtAuth)
        {
            _context = context;
            _configuration = configuration;
            _mailSettings = mailSettings.Value;
            _jwtAuth = jwtAuth;
        }


        public async Task<UserData> GetById(string id)
        {
            IAsyncCursor<UserData> userAsyncCursor = await _context.UserDataCollection.FindAsync(p => p.Id == id);
            return userAsyncCursor.FirstOrDefault();
        }

        public async Task<bool> SaveToLog(LogData logData)
        {
            await _context.LogCollection.InsertOneAsync(logData);
            return true;
        }

        public async Task<LoginResponse> LoginUser(LoginRequest loginRequest)
        { 
            Console.WriteLine("VersionCheck");
            var appVersion = new Version(loginRequest.AppVersion);
            IAsyncCursor<EnvFlags> envCursor = await _context.EnvFlagCollection.FindAsync(p => p.Id == "627d2a5e66c8edf696d0c7db");
            var minVersion = new Version(envCursor.FirstOrDefault().MinAppVersion);

            var result = appVersion.CompareTo(minVersion);
            if (result < 0)
            {
                return new LoginResponse()
                {
                    ErrorMessage = ErrorCases.AppUpdateRequired
                };
            }

            IAsyncCursor<UserData> userAsyncCursor = await _context.UserDataCollection.FindAsync(p => p.Username == loginRequest.Username);
            UserData userData = userAsyncCursor.FirstOrDefault();

            if(userData == null)
            {
                return new LoginResponse()
                {
                    ErrorMessage = ErrorCases.UserDoesNotExist
                };
            }
            string hashPass = loginRequest.Password.EncryptPassword(userData.Salt);

            if(userData.Password != hashPass)
            {
                return new LoginResponse()
                {
                    ErrorMessage = ErrorCases.IncorrectPassword
                };
            }

            if (!userData.IsVerified)
            {
                return new LoginResponse()
                {
                    ErrorMessage = ErrorCases.AccountNotVerified
                };
            }


            IAsyncCursor<SavedData> savedDataCursor = await _context.SavedDataCollection.FindAsync(g => g.Id == userData.SavedDataId);

            string token = _jwtAuth.Authentication(loginRequest.Username, userData.Id);
            LoginResponse returnValue = new LoginResponse()
            {
                PlayerData = savedDataCursor.SingleOrDefault(),
                PlayerId = userData.Id,
                EmailAddress = userData.EmailAddress,
                Token = token,
                ErrorMessage = ErrorCases.AllGood
            };

            await SaveToLog(new LogData()
            {
                AppVersion = loginRequest.AppVersion,
                Platform = loginRequest.Platform,
                Username = loginRequest.Username,
                Time = DateTime.Now.ToString(),
                PlayerId = userData.Id,
                SaveId = userData.SavedDataId
            });
            return returnValue;
        }

        public async Task<LoginResponse> RegisterUser(LoginRequest loginRequest)
        {

            if(loginRequest.EmailAddress != "")
            {

                if (!loginRequest.EmailAddress.IsValidEmail())
                {
                    return new LoginResponse()
                    {
                        ErrorMessage = ErrorCases.IncorrectEmail
                    };
                }
            }
            string userSalt = ExtendMethods.GenerateRndSalt();
            var hashPass = loginRequest.Password.EncryptPassword(userSalt); //$"{loginRequest.Password}{_configuration.GetValue<string>("Salt")}".Sha256();
            IAsyncCursor<UserData> userAsyncCursor = await _context.UserDataCollection.FindAsync(p => p.Username == loginRequest.Username);

            UserData userData = userAsyncCursor.FirstOrDefault();

            if(userData != null)
            {
                return new LoginResponse()
                {
                    ErrorMessage = ErrorCases.UserNameInUse
                };
            }

            SavedData newSavedData = new SavedData();


            await _context.SavedDataCollection.InsertOneAsync(newSavedData);
            UserData newUser = new UserData()
            {
                SavedDataId = newSavedData.Id,
                Username = loginRequest.Username,
                Password = hashPass,
                EmailAddress = loginRequest.EmailAddress,
                Salt = userSalt,
                CodeGenerateTime = DateTime.Now.Ticks.ToString(),
                IsVerified = true
            };

            await _context.UserDataCollection.InsertOneAsync(newUser);
            string token = _jwtAuth.Authentication(loginRequest.Username, newUser.Id);

            //var email = new MimeMessage();
            //email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            //email.To.Add(MailboxAddress.Parse(loginRequest.EmailAddress));
            //email.Subject = "Elements The Game Verification Code";
            //var builder = new BodyBuilder();

            //builder.HtmlBody = $"Use this code to verify your account: {OtpCode}";
            //email.Body = builder.ToMessageBody();
            //using var smtp = new SmtpClient();
            //smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            //smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            //smtp.CheckCertificateRevocation = false;
            //await smtp.SendAsync(email);

            LoginResponse returnValue = new LoginResponse()
            {
                PlayerId = newUser.Id,
                PlayerData = newSavedData,
                ErrorMessage = ErrorCases.AllGood,
                Token = token
            };

            await SaveToLog(new LogData() 
                { 
                    AppVersion = loginRequest.AppVersion, 
                    Platform = loginRequest.Platform, 
                    Username = loginRequest.Username, 
                    Time = DateTime.Now.ToString(),
                    PlayerId = userData.Id,
                    SaveId = userData.SavedDataId
                });
            return returnValue;
        }


        private async void SendOtpEmail(string OtpCode, string emailAddress)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(emailAddress));
            email.Subject = "Pet Cavern Verification Code";
            var builder = new BodyBuilder();

            builder.HtmlBody = $"Use this code to verify your account: {OtpCode}";
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.CheckCertificateRevocation = false;
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
        }

        public async Task<LoginResponse> CheckAppVersion(LoginRequest loginRequest)
        {
            string newString = "Data Source=208.88.227.38\\MSSQLSERVER2017;User ID=sparklmo_mainadmin;Password=Drakonus!1993;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection Connection = new SqlConnection(newString);

            using (IDbConnection connection = new SqlConnection(newString))
            {
                var result = connection.Query<UserDataSQL>(SQLQueryHelper.GetPlayerDataWithUsername(loginRequest.Username)).FirstOrDefault();

                string hashPass = loginRequest.Password.EncryptPassword(result.Salt);

                var saveDataResult = connection.Query<SavedDataSQL>(SQLQueryHelper.GetSaveDataWithID(result.ID)).FirstOrDefault();
                List<string> cardsIDs = saveDataResult.DeckCards.Split(",").ToList(); 
                List<CardObjectSQL> cards = connection.Query<CardObjectSQL>(SQLQueryHelper.GetCardWithIDQuery(saveDataResult.DeckCards)).ToList();

                List<CardObject> cardListFinal = new List<CardObject>();
                List<string> cardsInventoryIDs = saveDataResult.InventoryCards.Split(",").ToList();
                List<CardObjectSQL> cardsInventory = connection.Query<CardObjectSQL>(SQLQueryHelper.GetCardWithIDQuery(saveDataResult.DeckCards)).ToList();
                List<CardObject> inventoryListFinal = new List<CardObject>();

                foreach (var cardId in cardsIDs)
                {
                    cardListFinal.Add(new CardObject(cards.First(q => q.ID == int.Parse(cardId))));
                }

                foreach (var cardId in cardsInventoryIDs)
                {
                    inventoryListFinal.Add(new CardObject(cards.First(q => q.ID == int.Parse(cardId))));
                }

                if (result.UserPassKey != hashPass)
                {
                    return new LoginResponse()
                    {
                        ErrorMessage = ErrorCases.IncorrectPassword
                    };
                }

                return new LoginResponse()
                {
                    PlayerId = Newtonsoft.Json.JsonConvert.SerializeObject(cardListFinal),
                    PlayerData = SQLMapper.MapSQLToSavedDataUnity(saveDataResult, cardListFinal, inventoryListFinal),
                    EmailAddress = Connection.Database,
                    ErrorMessage = ErrorCases.AllGood
                };
            }


            //Console.WriteLine("VersionCheck");
            //var appVersion = new Version(loginRequest.AppVersion);
            //IAsyncCursor<EnvFlags> envCursor = await _context.EnvFlagCollection.FindAsync(p => p.Id == "627d2a5e66c8edf696d0c7db");
            //var minVersion = new Version(envCursor.FirstOrDefault().MinAppVersion);

            //var result = appVersion.CompareTo(minVersion);
            //if(result < 0)
            //{
            //    return new LoginResponse()
            //    {
            //        ErrorMessage = ErrorCases.AppUpdateRequired
            //    };
            //}
            //else
            //{
            //    return new LoginResponse()
            //    {
            //        ErrorMessage = ErrorCases.AllGood
            //    };
            //}
        }
    }
}
