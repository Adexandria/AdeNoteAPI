using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdeNote.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketController : BaseController
    {
        private readonly ITicketService ticketService;

        public TicketController(IUserIdentity userIdentity, ITicketService _ticketService) : base(userIdentity)
        {
            ticketService = _ticketService;
        }

        [HttpGet]
        [Authorize("Owner")]
        public IActionResult GetAllTickets(int pageSize = 20, int pageNumber = 1)
        {
            var response = ticketService.FetchAllTickets(pageNumber, pageSize);

            return response.Response();
        }

        [HttpGet("users")]
        [Authorize("Owner")]
        public IActionResult GetUserTickets(Guid userId, int pageNumber = 1, int pageSize = 20)
        {
            var response = ticketService.FetchAllTickets(userId, pageNumber, pageSize);

            return response.Response();
        }

        [HttpGet("date")]
        [Authorize("Owner")]
        public IActionResult SearchUserTickets(DateTime created, int pageNumber = 1, int pageSize = 20)
        {
            var response = ticketService.SearchTickets(created, pageNumber, pageSize);

            return response.Response();
        }

        [HttpGet("status")]
        [Authorize("Owner")]
        public IActionResult SearchUserTickets(string status, int pageNumber = 1, int pageSize = 20)
        {
            var response = ticketService.SearchTickets(status, pageNumber, pageSize);

            return response.Response();
        }

        [HttpGet("{ticketId}")]
        [Authorize("Owner")]
        public async Task<IActionResult> GetTicket(Guid ticketId)
        {
            var response = await ticketService.FetchTicketById(ticketId);

            return response.Response();
        }

        [HttpPost]
        [Authorize("User")]
        public async Task<IActionResult> GenerateTickets(TicketCreateDto newTicket)
        {
            var ms = new MemoryStream
            {
                Position = 0
            };

            await newTicket.Image?.CopyToAsync(ms);

            var ticket = new TicketStreamDto()
            { 
                Description = newTicket.Description,
                Image = ms,
                Issue = newTicket.Issue
            };

            var response = await ticketService.CreateTicket(ticket, CurrentUser);

            return response.Response();
        }

        [HttpPut("{ticketId}")]
        [Authorize("Owner")]
        public async Task<IActionResult> UpdateTicketStatus([FromQuery] string status, Guid ticketId)
        {
            var response = await ticketService.UpdateTicket(status, CurrentUser, ticketId);

            return response.Response();
        }
    }
}
