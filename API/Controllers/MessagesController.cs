using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMapper _mapper;
        public IUnitOfWork UnitOfWork { get; }
        public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.UnitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var userName = User.getUserName();

            if (userName.ToLower() == createMessageDto.RecipientUsername.ToLower()) return BadRequest("Same user");

            var sender = await UnitOfWork.UserRepository.GetUserByNameAsync(userName);
            var recipient = await UnitOfWork.UserRepository.GetUserByNameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            UnitOfWork.MessageRepository.AddMessage(message);

            if (await UnitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageparams)
        {
            messageparams.Username = User.getUserName();
            var messages = await UnitOfWork.MessageRepository.GetMessagesForUser(messageparams);

            Response.AddPaginationHeaders(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThreadX(string username)
        {
            var currentUsername = User.getUserName();

            return Ok(await UnitOfWork.MessageRepository.GetMessageThread(currentUsername, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.getUserName();

            var message = await UnitOfWork.MessageRepository.GetMessage(id);

            if (message.Sender.UserName != username && message.Recipient.UserName != username)
                return Unauthorized();

            if (message.Sender.UserName == username) message.SenderDeleted = true;

            if (message.Recipient.UserName == username) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
                UnitOfWork.MessageRepository.DeleteMessage(message);

            if (await UnitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }

}