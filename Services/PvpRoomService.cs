using ElementsTheAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Services
{
    public class PvpRoomService : IPvpRoomService
    {
        private readonly Dictionary<Guid, PvpRoom> _pvpRoomOne
            = new Dictionary<Guid, PvpRoom>();

        private readonly Dictionary<Guid, PvpRoom> _pvpRoomTwo
            = new Dictionary<Guid, PvpRoom>();

        public Task<PvpRoom> GetRoomWithId(Guid roomKey, ConnectedType connectedType)
        {
            switch (connectedType)
            {
                case ConnectedType.PvpOne:
                    return Task.FromResult(_pvpRoomOne[roomKey]);
                case ConnectedType.PvpTwo:
                    return Task.FromResult(_pvpRoomTwo[roomKey]);
                default:
                    break;
            }
            return null;
        }

        public Task<Guid> CreateRoom(ConnectedUser firstConnectedPlayer)
        {
            var id = Guid.NewGuid();
            switch (firstConnectedPlayer.ConnectionType)
            {
                case ConnectedType.PvpOne:
                    _pvpRoomOne[id] = new PvpRoom
                    {
                        FirstConnectedPlayer = firstConnectedPlayer
                    };
                    break;
                case ConnectedType.PvpTwo:
                    _pvpRoomTwo[id] = new PvpRoom
                    {
                        FirstConnectedPlayer = firstConnectedPlayer
                    };
                    break;
                default:
                    break;
            }

            return Task.FromResult(id);
        }

        public Task<Guid> GetFirstAvailableRoom(ConnectedUser secondConnectedPlayer)
        {
            switch (secondConnectedPlayer.ConnectionType)
            {
                case ConnectedType.PvpOne:
                    foreach (KeyValuePair<Guid, PvpRoom> room in _pvpRoomOne)
                    {
                        if (room.Value.SecondConnectedPlayer == null && room.Value.FirstConnectedPlayer.PlayerId != secondConnectedPlayer.PlayerId)
                        {
                            room.Value.SecondConnectedPlayer = secondConnectedPlayer;
                            return Task.FromResult(room.Key);
                        }
                    }
                    break;
                case ConnectedType.PvpTwo:
                    foreach (KeyValuePair<Guid, PvpRoom> room in _pvpRoomTwo)
                    {
                        if (room.Value.SecondConnectedPlayer == null && room.Value.FirstConnectedPlayer.PlayerId != secondConnectedPlayer.PlayerId)
                        {
                            room.Value.SecondConnectedPlayer = secondConnectedPlayer;
                            return Task.FromResult(room.Key);
                        }
                    }
                    break;
                default:
                    break;
            }
            
            return Task.FromResult(Guid.Empty);
        }

        public void RemoveFromRoom(Guid roomKey)
        {
            if(_pvpRoomOne[roomKey] != null)
            {
                _pvpRoomOne.Remove(roomKey);
            }
            else
            {
                _pvpRoomTwo.Remove(roomKey);
            }
        }
    }
}
