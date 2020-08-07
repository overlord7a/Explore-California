﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignalR_App.Models;

namespace SignalR_App.Services
{
    public class InMemoryChatRoomService : IChatRoomService
    {
        private readonly Dictionary<Guid, ChatRoom> _roomInfo = new Dictionary<Guid, ChatRoom>();
        private readonly Dictionary<Guid,List<ChatMessage>> _messageHistory = new Dictionary<Guid, List<ChatMessage>>();

        public Task AddMessage(Guid roomId, ChatMessage message)
        {
            if (!_messageHistory.ContainsKey(roomId))
            {
                _messageHistory[roomId] = new List<ChatMessage>();
            }
            _messageHistory[roomId].Add(message);
            return Task.CompletedTask;
        }

        public Task<Guid> CreateRoom(string connectionId)
        {
            var id = Guid.NewGuid();
            _roomInfo[id] = new ChatRoom
            {
                OwnerConnectionId = connectionId
            };
            return Task.FromResult(id);
        }

        public Task<List<ChatMessage>> GetMessageHistory(Guid roomId)
        {
            _messageHistory.TryGetValue(roomId, out var messages);
            messages ??= new List<ChatMessage>();
            messages = messages
                .OrderBy(c => c.SentAt)
                .ToList();
            return Task.FromResult(messages);
        }

        public Task<Guid> GetRoomForConnectionId(string connectionId)
        {
            var foundRoom = _roomInfo.FirstOrDefault(
                x => x.Value.OwnerConnectionId == connectionId);
            if (foundRoom.Key == Guid.Empty)
            {
                throw new ArgumentException("This was invalid connection Id");
            }

            return Task.FromResult(foundRoom.Key);
        }

        public Task SetRoomName(Guid roomId, string name)
        {
            if (!_roomInfo.ContainsKey(roomId))
                throw new ArgumentException("Room doesn't exists");
            _roomInfo[roomId].Name = name;
            return Task.CompletedTask;
        }
    }
}