using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        public IMapper Mapper { get; }
        public IHubContext<PresenceHub> PresenceHub { get; set; }
        private readonly PresenceTracker presenceTracker;
        private readonly IUnitOfWork unitOfWork;
        public MessageHub(IMapper mapper, IUnitOfWork unitOfWork
                          , IHubContext<PresenceHub> presenceHub, PresenceTracker presenceTracker)
        {
            this.unitOfWork = unitOfWork;
            this.presenceTracker = presenceTracker;
            this.PresenceHub = presenceHub;
            this.Mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUserName = httpContext.Request.Query["user"].ToString();

            var groupName = getGroupName(httpContext.User.getUserName(), otherUserName);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(Context, groupName);

            var MsgThreads = unitOfWork.MessageRepository.GetMessageThread(Context.User.getUserName(), otherUserName);
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", MsgThreads);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var userName = Context.User.getUserName();

            if (userName.ToLower() == createMessageDto.RecipientUsername.ToLower()) throw new HubException("Same user");

            var sender = await unitOfWork.UserRepository.GetUserByNameAsync(userName);
            var recipient = await unitOfWork.UserRepository.GetUserByNameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) throw new HubException("Not Found User");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName = getGroupName(sender.UserName, recipient.UserName);
            var group = await unitOfWork.MessageRepository.GetMessageGroup(groupName);

            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await presenceTracker.GetConnectionsForUser(recipient.UserName);
                if (connections != null)
                {
                    await PresenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new
                    {
                        userName = sender.UserName,
                        knownAs = sender.KnownAs
                    });
                }
            }

            unitOfWork.MessageRepository.AddMessage(message);

            if (await unitOfWork.Complete())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", Mapper.Map<MessageDto>(message));
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromMessageGroup(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }


        public async Task<Boolean> AddToGroup(HubCallerContext context, string groupName)
        {
            var group = await unitOfWork.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(context.ConnectionId, context.User.getUserName());

            if (group == null)
            {
                group = new Group(groupName);
                unitOfWork.MessageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);
            return await unitOfWork.Complete();
        }

        private async Task RemoveFromMessageGroup(string connectionId)
        {
            var Connection = await unitOfWork.MessageRepository.GetConnection(connectionId);
            unitOfWork.MessageRepository.RemoveConnection(Connection);
            await unitOfWork.Complete();
        }

        private string getGroupName(string userName, string otherUserName)
        {
            var stringCompare = string.CompareOrdinal(userName, otherUserName) < 0;
            return stringCompare ? $"{userName}-{otherUserName}" : $"{otherUserName}-{userName}";
        }
    }
}