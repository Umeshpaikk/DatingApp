using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DatatContext _context;
        public IMapper _Mapper { get; }
        public MessageRepository(DatatContext context, IMapper mapper)
        {
            _Mapper = mapper;
            _context = context;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(u => u.Sender)
                .Include(u => u.Recipient)
                .SingleOrDefaultAsync(x => x.Id == id);
        }
        
        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(m => m.Recipient.UserName == messageParams.Username
                && m.RecipientDeleted == false),
                "Outbox" => query.Where(m => m.Sender.UserName == messageParams.Username
                && m.SenderDeleted == false),
                _ => query.Where(m => m.DateRead == null && m.Recipient.UserName == messageParams.Username
                && m.RecipientDeleted == false)
            };

            var messages = query.ProjectTo<MessageDto>(_Mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages,messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string RecipientUsername)
        {
            var messages = await _context.Messages
            //.Include( u => u.Sender).ThenInclude( p => p.Photos)
            //.Include( u => u.Recipient).ThenInclude( p => p.Photos)
            .Where( m => 
            (m.Recipient.UserName == currentUsername && m.Sender.UserName == RecipientUsername && m.RecipientDeleted ==false)
            || (m.Recipient.UserName == RecipientUsername && m.Sender.UserName == currentUsername && m.SenderDeleted == false)
             )
             .OrderBy( m => m.MessageSent)
             .ProjectTo<MessageDto>(_Mapper.ConfigurationProvider)
             .ToListAsync();

             var unreadmsgs = messages.Where( m => m.DateRead == null && m.RecipientUsername == currentUsername).ToList();
             foreach(var msg in unreadmsgs){
                 msg.DateRead = DateTime.UtcNow;
             }
             await _context.SaveChangesAsync();

            return (messages);
        }

        // public async Task<bool> SaveAllAsync()
        // {
        //     return await _context.SaveChangesAsync() > 0;
        // }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                        .Include(x => x.Connections)
                        .FirstOrDefaultAsync(x=>x.Name == groupName);
        }

        public Task<Group> GetGroupForConnection(string connectionId)
        {
            throw new NotImplementedException();
        }
    }
}