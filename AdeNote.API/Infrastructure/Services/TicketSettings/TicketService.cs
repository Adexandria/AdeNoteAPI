﻿using AdeMessaging.Services;
using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services.Blob;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.EventSystem;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Automappify.Services;
using System.Net;
using System.Text.Json;

namespace AdeNote.Infrastructure.Services.TicketSettings
{
    public class TicketService : ITicketService
    {
        protected TicketService()
        {

        }
        public TicketService(ITicketRepository _ticketRepository, IUserRepository _userRepository, 
            IBlobService _blobService
            , IMessagingService _messagingService, IConfiguration config)
        {
            ticketRepository = _ticketRepository;
            userRepository = _userRepository;
            blobService = _blobService;
            messagingService = _messagingService;
            eventConfiguration = config.GetSection("Events")
                .GetSection("Ticket")
                .Get<EventConfiguration>() ?? throw new NullReferenceException(nameof(eventConfiguration));
          
        }

        public async Task<ActionResult> CreateTicket(TicketStreamDto newTicket, string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(email))
                return ActionResult.Failed("Invalid email", (int)HttpStatusCode.BadRequest);


            var currentUser = await userRepository.GetUserByEmail(email);

            if (currentUser == null)
            {
                return ActionResult.Failed("Invalid user", StatusCodes.Status400BadRequest);
            }

            var ticket = newTicket.Map<TicketStreamDto,Ticket>(MappingService.TicketStreamConfig());

            ticket.Issuer = currentUser.Id;

            if (newTicket.Image.Length != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var url = await blobService.UploadImage($"T{Guid.NewGuid().ToString()[..4]}", newTicket.Image, cancellationToken) ?? $"T{Guid.NewGuid().ToString()[..4]}";

                if (string.IsNullOrEmpty(url))
                {
                    return ActionResult<string>.Failed(url, 400);
                }

                ticket.ImageUrl = url;
            }

            var commitStatus = await ticketRepository.Add(ticket);

            if (!commitStatus)
                return ActionResult.Failed("Failed to create a ticket");

            return ActionResult.SuccessfulOperation();
        }

        public ActionResult<PaginatedResponse<TicketsDTO>> FetchAllTickets(int pageNumber, int pageSize)
        {

            if (pageNumber == 0 || pageSize == 0)
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid page number or page size", StatusCodes.Status400BadRequest);

            var tickets = ticketRepository.GetTickets(pageNumber, pageSize).ToList();

            if (!tickets.Any())
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("There are no tickets", StatusCodes.Status400BadRequest);

            var currentTickets = tickets.Map<IEnumerable<Ticket>, IEnumerable<TicketsDTO>>(MappingService.TicketsConfig());

            var paginatedResponse = new PaginatedResponse<TicketsDTO>(pageNumber, pageSize, currentTickets);

            return ActionResult<PaginatedResponse<TicketsDTO>>.SuccessfulOperation(paginatedResponse);
        }

        public ActionResult<PaginatedResponse<TicketsDTO>> FetchAllTickets(string name, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(name))
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid id", StatusCodes.Status400BadRequest);

            if (pageNumber == 0 || pageSize == 0)
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid page number or page size", StatusCodes.Status400BadRequest);

            var tickets = ticketRepository.GetTickets(name, pageNumber, pageSize);

            if (!tickets.Any())
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("There are no tickets", StatusCodes.Status400BadRequest);

            var currentTickets = tickets.Map<IEnumerable<Ticket>, IEnumerable<TicketsDTO>>(MappingService.TicketsConfig());

            var paginatedResponse = new PaginatedResponse<TicketsDTO>(pageNumber, pageSize, currentTickets);

            return ActionResult<PaginatedResponse<TicketsDTO>>.SuccessfulOperation(paginatedResponse);
        }

        public async Task<ActionResult<TicketDTO>> FetchTicketById(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
                return ActionResult<TicketDTO>.Failed("Invalid ticket id", StatusCodes.Status400BadRequest);

            var ticket = await ticketRepository.GetTicket(ticketId);
            if (ticket == null)
                return ActionResult<TicketDTO>.Failed("There are no existing tickets", StatusCodes.Status400BadRequest);

            var ticketAdmin = await userRepository.GetUser(ticket.AdminId.GetValueOrDefault());

            var currentTicket = ticket.Map<Ticket,TicketDTO>(MappingService.TicketConfig());

            currentTicket.Map(ticketAdmin, MappingService.TicketAdminConfig());
                
            return ActionResult<TicketDTO>.SuccessfulOperation(currentTicket);
        }

        public async Task<ActionResult<UserTicketDto>> FetchTicketById(string ticketId)
        {
            var ticket = await ticketRepository.GetTicket(ticketId);
            if (ticket == null)
                return ActionResult<UserTicketDto>.Failed("There are no existing tickets", StatusCodes.Status400BadRequest);

            var ticketAdmin = await userRepository.GetUser(ticket.AdminId.GetValueOrDefault());

            var currentTicket = ticket.Map<Ticket, UserTicketDto>(MappingService.UserTicketConfig());

            currentTicket.TicketId = ticketId;

            currentTicket.Map(ticketAdmin, MappingService.TicketAdminConfig());

            return ActionResult<UserTicketDto>.SuccessfulOperation(currentTicket);
        }

        public ActionResult<PaginatedResponse<TicketsDTO>> SearchTicketsByDate(string created, int pageNumber, int pageSize)
        {
            DateTime.TryParse(created, out var date);

            if (pageNumber == 0 || pageSize == 0)
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid page number or page size", StatusCodes.Status400BadRequest);

            var tickets = ticketRepository.SearchTickets(x => x.Created == date, pageNumber, pageSize);

            if (!tickets.Any())
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("There are no tickets", StatusCodes.Status400BadRequest);

            var currentTickets = tickets.Map<IEnumerable<Ticket>, IEnumerable<TicketsDTO>>(MappingService.TicketsConfig());

            var paginatedResponse = new PaginatedResponse<TicketsDTO>(pageNumber, pageSize, currentTickets);

            return ActionResult<PaginatedResponse<TicketsDTO>>.SuccessfulOperation(paginatedResponse);
        }

        public ActionResult<PaginatedResponse<TicketsDTO>> SearchTickets(string status, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(status))
            {
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid date", StatusCodes.Status400BadRequest);
            }

            if (pageNumber == 0 || pageSize == 0)
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid page number or page size", StatusCodes.Status400BadRequest);

            if (!Enum.TryParse(status, out Status newStatus))
            {
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid status", StatusCodes.Status400BadRequest);
            }

            var tickets = ticketRepository.SearchTickets(x => x.Status == newStatus, pageNumber, pageSize).ToList();

            if (!tickets.Any())
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("There are no tickets", StatusCodes.Status400BadRequest);

            var currentTickets = tickets.Map<IEnumerable<Ticket>, IEnumerable<TicketsDTO>>(MappingService.TicketsConfig());

            var paginatedResponse = new PaginatedResponse<TicketsDTO>(pageNumber, pageSize, currentTickets);

            return ActionResult<PaginatedResponse<TicketsDTO>>.SuccessfulOperation(paginatedResponse);
        }



        public async Task<ActionResult> UpdateTicket(string status, Guid adminId, Guid ticketId, SolvedTicketDto? solvedTicketDto)
        {
            if (ticketId == Guid.Empty)
                return ActionResult.Failed("Invalid ticket id", StatusCodes.Status400BadRequest);

            var currentTicket = await ticketRepository.GetTicket(ticketId);
            if (currentTicket == null)
                return ActionResult.Failed("There are no existing ticket", StatusCodes.Status400BadRequest);

            var isStatus = Enum.TryParse(status, out Status newStatus);

            if (!isStatus)
            {
                return ActionResult.Failed("Invalid status", StatusCodes.Status400BadRequest);
            }

            currentTicket.Status = newStatus;

            currentTicket.AdminId = adminId;

            currentTicket.SetModifiedDate();

            var commitStatus = await ticketRepository.Update(currentTicket);

            if (!commitStatus)
            {
                return ActionResult.Failed("Failed to update ticket", StatusCodes.Status400BadRequest);
            }

            var admin = await userRepository.GetUser(adminId);


            string message = JsonSerializer.Serialize(new
            {
                status = newStatus.GetDescription(),
                firstName = currentTicket.User.FirstName,
                lastName = currentTicket.User.LastName,
                ticketId = $"tk{currentTicket.Id.ToString("N")[^5..]}",
                emailAddress = currentTicket.User.Email,
                issue = currentTicket.Issue,
                lastUpdated = currentTicket.Modified.ToShortDateString(),
                dateSubmitted = currentTicket.Created.ToShortDateString(),
                agent = admin.FirstName + " " + admin.LastName,
                resolvedDetails = solvedTicketDto?.ResolutionDetails
            });

            messagingService.Publish(message, eventConfiguration.Exchange, eventConfiguration.RoutingKey);

            return ActionResult.SuccessfulOperation();
        }

        public async Task<ActionResult> DeleteTicket(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
                return ActionResult.Failed("Invalid ticket id", StatusCodes.Status400BadRequest);

            var currentTicket = await ticketRepository.GetTicket(ticketId);
            if (currentTicket == null)
                return ActionResult.Failed("There are no existing ticket", StatusCodes.Status400BadRequest);

            var commitStatus = await ticketRepository.Remove(currentTicket);

            if (!commitStatus)
            {
                return ActionResult.Failed("Failed to delete ticket", StatusCodes.Status400BadRequest);
            }

            return ActionResult.SuccessfulOperation();
        }



        private readonly ITicketRepository ticketRepository;
        private readonly IUserRepository userRepository;
        private readonly IBlobService blobService;
        private readonly IMessagingService messagingService;
        private readonly EventConfiguration eventConfiguration;
    }
}
