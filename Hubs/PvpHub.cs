using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using ElementsTheAPI.Helpers;
using System.Web.Http;
using System.Security.Claims;
using System.Linq;
using ElementsTheAPI.Data;
using MongoDB.Driver;
using ElementsTheAPI.Entities;
using ElementsTheAPI.Services;

namespace ElementsTheAPI.Hubs
{
    [Authorize]
    public class PvpHub : Hub
    {
        IPvpHubContext _pvpHubContext;
        IPvpRoomService _pvpRoomService;
        public PvpHub(IPvpHubContext pvpHubContext, IPvpRoomService pvpRoomService)
        {
            _pvpHubContext = pvpHubContext;
            _pvpRoomService = pvpRoomService;
        }

        public override Task OnConnectedAsync()
        {
            //var userClaim = Context.User.Claims
            Console.WriteLine("Unity Connected" + Context.ConnectionId);
            (string, string) pIdAndUsername = GetPIdAndUsername();
            ConnectedUser connectedUser = _pvpHubContext.ConnectedUserCollection.Find(p => p.PlayerId == pIdAndUsername.Item1).FirstOrDefault();

            UserData userData = _pvpHubContext.UserDataCollection.Find(x => x.Id == pIdAndUsername.Item1).FirstOrDefault();
            SavedData savedData = _pvpHubContext.SavedDataCollection.Find(x => x.Id == userData.SavedDataId).FirstOrDefault();

            if (connectedUser == null)
            {
                connectedUser = new ConnectedUser
                {
                    PlayerId = pIdAndUsername.Item1,
                    Username = pIdAndUsername.Item2,
                    DeckList = savedData.CurrentDeck,
                    ElementMark = savedData.MarkElement,
                    ConnectionType = ConnectedType.NoPvp,
                    Win = savedData.GamesWon,
                    Lose = savedData.GamesLost,
                    Score = savedData.PlayerScore,
                    ConnectionId = Context.ConnectionId
                };

                _pvpHubContext.ConnectedUserCollection.InsertOne(connectedUser);
                return base.OnConnectedAsync();
            }
            connectedUser.ConnectionType = ConnectedType.NoPvp;
            connectedUser.ConnectionId = Context.ConnectionId;
            _pvpHubContext.ConnectedUserCollection.ReplaceOne(filter: g => g.PlayerId == pIdAndUsername.Item1, replacement: connectedUser);
            //Clients.Client(Context.ConnectionId).SendAsync("ReceiveConnID", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public async Task SendPvpAction(Guid roomId, PvP_Action pvP_Action, ConnectedType connectedType)
        {
            (string, string) pIdAndUsername = GetPIdAndUsername();

            PvpRoom pvpRoom = await _pvpRoomService.GetRoomWithId(roomId, connectedType);
            if (pvP_Action.OriginId != null)
            {
                pvP_Action.OriginId.Owner = pvP_Action.OriginId.Owner.Equals(OwnerEnum.Player) ? OwnerEnum.Opponent : OwnerEnum.Player;
            }
            if(pvP_Action.TargetId != null)
            {
                pvP_Action.TargetId.Owner = pvP_Action.TargetId.Owner.Equals(OwnerEnum.Player) ? OwnerEnum.Opponent : OwnerEnum.Player;
            }
            string jsonString = JsonConvert.SerializeObject(pvP_Action);
            if (pvpRoom.FirstConnectedPlayer.PlayerId == pIdAndUsername.Item1)
            {
                await Clients.Client(pvpRoom.SecondConnectedPlayer.ConnectionId).SendAsync("ReceivePvpAction", jsonString);
            }
            else
            {
                await Clients.Client(pvpRoom.FirstConnectedPlayer.ConnectionId).SendAsync("ReceivePvpAction", jsonString);
            }
        }

        public async Task SendShuffledDeck(Guid roomId, List<CardObject> shuffledDeck, ConnectedType connectedType)
        {
            (string, string) pIdAndUsername = GetPIdAndUsername();

            PvpRoom pvpRoom = await _pvpRoomService.GetRoomWithId(roomId, connectedType);

            if (pvpRoom.FirstConnectedPlayer.PlayerId == pIdAndUsername.Item1)
            {
                await Clients.Client(pvpRoom.SecondConnectedPlayer.ConnectionId).SendAsync("ReceiveOpponentDeck", shuffledDeck);
            }
            else
            {
                await Clients.Client(pvpRoom.FirstConnectedPlayer.ConnectionId).SendAsync("ReceiveOpponentDeck", shuffledDeck);
            }
        }

        public async Task EndPlayerTurn(Guid roomkey, List<QuantaObject> quantaObjects, ConnectedType connectedType)
        {
            
            (string, string) pIdAndUsername = GetPIdAndUsername();

            PvpRoom pvpRoom = await _pvpRoomService.GetRoomWithId(roomkey, connectedType);

            if (pvpRoom.FirstConnectedPlayer.PlayerId == pIdAndUsername.Item1)
            {
                await Clients.Client(pvpRoom.SecondConnectedPlayer.ConnectionId).SendAsync("ReceiveQuantaObjects", quantaObjects);
            }
            else
            {
                await Clients.Client(pvpRoom.FirstConnectedPlayer.ConnectionId).SendAsync("ReceiveQuantaObjects", quantaObjects);
            }
        }

        public async Task StartPvpConnection(int connectionType)
        {
            (string, string) pIdAndUsername = GetPIdAndUsername();

            ConnectedUser connectedUser = _pvpHubContext.ConnectedUserCollection.Find(p => p.PlayerId == pIdAndUsername.Item1).FirstOrDefault();
            ConnectedType type = (ConnectedType)connectionType;

            connectedUser.ConnectionType = type;
            _pvpHubContext.ConnectedUserCollection.ReplaceOne(filter: g => g.PlayerId == pIdAndUsername.Item1, replacement: connectedUser);

            var roomId = await _pvpRoomService.GetFirstAvailableRoom(connectedUser);

            if(roomId == Guid.Empty)
            {
                roomId = await _pvpRoomService.CreateRoom(connectedUser);
                await Clients.Client(Context.ConnectionId).SendAsync("UpdatePvpOpScreen", roomId, null);
                return;
            }

            PvpRoom pvpRoom = await _pvpRoomService.GetRoomWithId(roomId, connectedUser.ConnectionType);

            await Clients.Client(pvpRoom.FirstConnectedPlayer.ConnectionId).SendAsync("UpdatePvpOpScreen", roomId, new PvpUserInfo(pvpRoom.SecondConnectedPlayer));
            await Clients.Client(pvpRoom.SecondConnectedPlayer.ConnectionId).SendAsync("UpdatePvpOpScreen", roomId, new PvpUserInfo(pvpRoom.FirstConnectedPlayer));
        }

        public async Task ConfirmOpponentConnection(Guid roomKey, int connectionType)
        {
            (string, string) pIdAndUsername = GetPIdAndUsername();

            PvpRoom pvpRoom = await _pvpRoomService.GetRoomWithId(roomKey, (ConnectedType)connectionType);

            if(pvpRoom.FirstConnectedPlayer.PlayerId == pIdAndUsername.Item1)
            {
                pvpRoom.PlayerOneConnected = true;
                pvpRoom.FirstConnectedPlayer.DeckList = new List<CardObject>(pvpRoom.FirstConnectedPlayer.DeckList.Shuffle());
            }
            else
            {
                pvpRoom.PlayerTwoConnected = true;
                pvpRoom.SecondConnectedPlayer.DeckList = new List<CardObject>(pvpRoom.SecondConnectedPlayer.DeckList.Shuffle());
            }

            if(pvpRoom.PlayerTwoConnected && pvpRoom.PlayerOneConnected)
            {
                Random r = new Random();
                int flipResult = r.Next(0, 14);
                bool playerOneStarts = flipResult % 2 == 0;
                await Clients.Client(pvpRoom.FirstConnectedPlayer.ConnectionId).SendAsync("GetCoinFlipCount", flipResult, playerOneStarts, pvpRoom.FirstConnectedPlayer.DeckList, pvpRoom.SecondConnectedPlayer.DeckList);
                await Clients.Client(pvpRoom.SecondConnectedPlayer.ConnectionId).SendAsync("GetCoinFlipCount", flipResult + 1, !playerOneStarts, pvpRoom.SecondConnectedPlayer.DeckList, pvpRoom.FirstConnectedPlayer.DeckList);
            }
        }

        public async Task DisconnectFromHub(Guid roomKey)
        {
            (string, string) pIdAndUsername = GetPIdAndUsername();

            ConnectedUser connectedUser = _pvpHubContext.ConnectedUserCollection.Find(p => p.PlayerId == pIdAndUsername.Item1).FirstOrDefault();
            string opConnID = connectedUser.OpponentConnectionId;
            if (connectedUser != null)
            {
                connectedUser.ConnectionType = ConnectedType.Disconnected;
                connectedUser.OpponentConnectionId = "";
                _pvpHubContext.ConnectedUserCollection.ReplaceOne(filter: g => g.PlayerId == pIdAndUsername.Item1, replacement: connectedUser);
            }
            _pvpRoomService.RemoveFromRoom(roomKey);
            await Clients.Client(opConnID).SendAsync("OpDisconnect");
        }

        public async Task SendMessageAsync(string message, string targetClient)
        {
            Console.WriteLine("Message Recevied on: " + Context.ConnectionId);

            if(targetClient == "")
            {
                await Clients.All.SendAsync("ReceiveMessage", message);
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", message);
            }
        }

        private (string,string) GetPIdAndUsername()
        {
            var identity = (ClaimsIdentity)Context.User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var playerId = claims.First(claim => claim.Type == "playerID").Value;
            var userName = claims.First(claim => claim.Type == "name").Value;

            return (playerId, userName);
        }
    }
}
