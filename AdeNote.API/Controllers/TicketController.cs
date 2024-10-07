using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Requests.CreateTicket;
using AdeNote.Infrastructure.Requests.FetchAllTickets;
using AdeNote.Infrastructure.Requests.FetchAllTicketsByName;
using AdeNote.Infrastructure.Requests.FetchTicketById;
using AdeNote.Infrastructure.Requests.FetchUserTicketById;
using AdeNote.Infrastructure.Requests.SearchTickets;
using AdeNote.Infrastructure.Requests.SearchTicketsByDate;
using AdeNote.Infrastructure.Requests.UpdateTicket;
using AdeNote.Infrastructure.Services.TicketSettings;
using AdeNote.Infrastructure.Utilities;
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

        public TicketController(IUserIdentity userIdentity, ITicketService _ticketService, 
            Application application) : base(userIdentity, application)
        {
            ticketService = _ticketService;
        }

        [HttpGet]
        [Authorize("Owner")]
        public async Task<IActionResult> GetAllTickets(int pageSize = 20, int pageNumber = 1)
        {
            var response = await Application.SendAsync<FetchAllTicketsRequest, 
                PaginatedResponse<TicketsDTO>>(new FetchAllTicketsRequest() 
                { 
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });


            return response.Response();
        }

        [HttpGet("users")]
        [Authorize("Owner")]
        public async Task<IActionResult> GetUserTickets([Required(ErrorMessage = "Invalid name")] string name, int pageNumber = 1, int pageSize = 20)
        {
            var response = await Application.SendAsync<FetchAllTicketsByNameRequest,
                PaginatedResponse<TicketsDTO>>(new FetchAllTicketsByNameRequest() 
                { 
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    Name = name
                    
                });


            return response.Response();
        }

        [HttpGet("date")]
        [Authorize("Owner")]
        public async Task<IActionResult> SearchUserTicketsByDate([ValidDateTime("Invalid date and time")] string created, int pageNumber = 1, int pageSize = 20)
        {
            var response = await Application.SendAsync<SearchTicketsByDateRequest,
                PaginatedResponse<TicketsDTO>>(new SearchTicketsByDateRequest() 
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    Created = created
                });


            return response.Response();
        }

        [HttpGet("status")]
        [Authorize("Owner")]
        public async Task<IActionResult> SearchUserTickets([Allow("Invalid status", "Pending", "Inreview", "Resolved", "Unresolved")] string status, int pageNumber = 1, int pageSize = 20)
        {
            var response = await Application.SendAsync<SearchTicketsRequest,
                PaginatedResponse<TicketsDTO>>(new SearchTicketsRequest() 
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    Status = status
                });


            return response.Response();
        }

        [HttpGet("{ticketId}")]
        [Authorize("Owner")]
        public async Task<IActionResult> GetTicket([ValidGuid("Invalid ticket id")] Guid ticketId)
        {
            var response = await Application.SendAsync<FetchTicketByIdRequest, TicketDTO>
                (new FetchTicketByIdRequest()
                {
                    TicketId = ticketId
                });

            return response.Response();
        }

        [HttpGet("search/{ticketId}")]
        [Authorize("User")]
        public async Task<IActionResult> GetTicket([ValidTicketId("Invalid ticket id")] string ticketId)
        {
            var response = await Application.SendAsync<FetchUserTicketByIdRequest, UserTicketDto>
                (new FetchUserTicketByIdRequest()
                {
                    TicketId  = ticketId
                });


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
           

            var response = await Application.SendAsync(new CreateTicketRequest()
            {
                Description = newTicket.Description,
                Image = ms,
                Issue = newTicket.Issue
            });

            return response.Response();
        }

        [HttpPut("{ticketId}")]
        [Authorize("Owner")]
        public async Task<IActionResult> UpdateTicketStatus([FromQuery] [Allow("Invalid status", "Pending", "Inreview", "Resolved", "Unresolved")]  string status,
            [ValidGuid("Invalid ticket id")] Guid ticketId, [FromBody] SolvedTicketDto solvedTicketDto)
        {
            var response = await Application.SendAsync(new UpdateTicketRequest()
            { 
                TicketId = ticketId,
                AdminId = CurrentUser,
                SolvedTicketDto = solvedTicketDto,
                Status= status
            });

            return response.Response();
        }
    }
}
