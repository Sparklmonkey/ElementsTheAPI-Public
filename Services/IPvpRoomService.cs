using ElementsTheAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Services
{
    public interface IPvpRoomService
    {
        Task<Guid> CreateRoom(ConnectedUser firstConnectedUser);

        Task<Guid> GetFirstAvailableRoom(ConnectedUser secondConnectedUser);

        Task<PvpRoom> GetRoomWithId(Guid roomKey, ConnectedType connectedType);
        void RemoveFromRoom(Guid roomKey);
    }
}
