using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services.TicketSettings;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Infrastructure.Utilities.ValidationAttributes;
using AdeNote.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        public IActionResult GetUserTickets([Required(ErrorMessage = "Invalid name")] string name, int pageNumber = 1, int pageSize = 20)
        {
            var response = ticketService.FetchAllTickets(name, pageNumber, pageSize);

            return response.Response();
        }

        [HttpGet("date")]
        [Authorize("Owner")]
        public IActionResult SearchUserTicketsByDate([ValidDateTime("Invalid date and time")] string created, int pageNumber = 1, int pageSize = 20)
        {
            var response = ticketService.SearchTicketsByDate(created, pageNumber, pageSize);

            return response.Response();
        }

        [HttpGet("status")]
        [Authorize("Owner")]
        public IActionResult SearchUserTickets([Allow("Invalid status", "Pending", "Inreview", "Solved", "Unresolved")] string status, int pageNumber = 1, int pageSize = 20)
        {
            var response = ticketService.SearchTickets(status, pageNumber, pageSize);

            return response.Response();
        }

        [HttpGet("{ticketId}")]
        [Authorize("Owner")]
        public async Task<IActionResult> GetTicket([ValidGuid("Invalid ticket id")] Guid ticketId)
        {
            var response = await ticketService.FetchTicketById(ticketId);

            return response.Response();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateTickets([FromForm]TicketCreateDto newTicket, [Required(ErrorMessage = "Invalid email")]  string email)
        {
            var ms = new MemoryStream
            {
                Position = 0
            };

            if(newTicket.Image != null)
            {
                await newTicket.Image?.CopyToAsync(ms);
            }
           
            var ticket = new TicketStreamDto()
            { 
                Description = newTicket.Description,
                Image = ms,
                Issue = newTicket.Issue
            };

            var response = await ticketService.CreateTicket(ticket, email);

            return response.Response();
        }

        [HttpPut("{ticketId}")]
        [Authorize("Owner")]
        public async Task<IActionResult> UpdateTicketStatus([FromQuery] [Allow("Invalid status", "Pending", "Inreview", "Solved", "Unresolved")]  string status,
            [ValidGuid("Invalid ticket id")] Guid ticketId)
        {
            var response = await ticketService.UpdateTicket(status, CurrentUser, ticketId);

            return response.Response();
        }
    }
}
