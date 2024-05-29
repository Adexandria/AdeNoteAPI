using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Mapster;
using System.Net;

namespace AdeNote.Infrastructure.Services
{
    public class TicketService : ITicketService
    {
        protected TicketService()
        {
                
        }
        public TicketService(ITicketRepository _ticketRepository, IBlobService _blobService)
        {
            ticketRepository = _ticketRepository;
            blobService = _blobService;
        }
        public async Task<ActionResult> CreateTicket(TicketStreamDto newTicket, Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest);

                var ticket = newTicket.Adapt<Ticket>();

                ticket.Issuer = userId;

                var commitStatus = await ticketRepository.Add(ticket);

                if(newTicket.Image.Length != 0)
                {
                    var url = await blobService.UploadImage($"T{ticket.Id.ToString()[..4]}", newTicket.Image) ?? $"T{ticket.Id.ToString()[..4]}";

                    ticket.ImageUrl = url;
                }

                if (!commitStatus)
                    return ActionResult.Failed("Failed to create a ticket");

                return ActionResult.SuccessfulOperation();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);

            }
        }

        public ActionResult<PaginatedResponse<TicketsDTO>> FetchAllTickets(int pageNumber, int pageSize)
        {
            try
            {
                if (pageNumber == 0 || pageSize == 0)
                    return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid page number or page size", StatusCodes.Status400BadRequest);

                var tickets = ticketRepository.GetTickets(pageNumber, pageSize);

                if (tickets == null)
                    return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("There are no tickets", StatusCodes.Status400BadRequest);

                // Use mapster
                var currentTickets = tickets.Select(s =>
                    new TicketsDTO()
                    {
                        Issue = s.Issue,
                        TicketId = s.Id,
                        Status = s.Status.ToString(),
                        FirstName = s.User.FirstName,
                        LastName = s.User.LastName
                    });

                var paginatedResponse = new PaginatedResponse<TicketsDTO>(pageNumber, pageSize, currentTickets);

                return ActionResult<PaginatedResponse<TicketsDTO>>.SuccessfulOperation(paginatedResponse);
            }
            catch (Exception ex)
            {
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed(ex.Message);

            }
        }

        public ActionResult<PaginatedResponse<TicketsDTO>> FetchAllTickets(Guid userId, int pageNumber, int pageSize)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid id", StatusCodes.Status400BadRequest);

                if(pageNumber == 0 || pageSize == 0)
                    return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid page number or page size", StatusCodes.Status400BadRequest);

                var tickets = ticketRepository.GetTickets(userId, pageNumber, pageSize);

                if (tickets == null)
                    return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("There are no tickets", StatusCodes.Status400BadRequest);

                var currentTickets = tickets.Select(s =>
                   new TicketsDTO()
                   {
                       Issue = s.Issue,
                       TicketId = s.Id,
                       Status = s.Status.ToString(),
                       FirstName = s.User.FirstName,
                       LastName = s.User.LastName
                   });

                var paginatedResponse = new PaginatedResponse<TicketsDTO>(pageNumber, pageSize, currentTickets);

                return ActionResult<PaginatedResponse<TicketsDTO>>.SuccessfulOperation(paginatedResponse);
            }
            catch (Exception ex)
            {
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed(ex.Message);

            }
        }

        public async Task<ActionResult<TicketDTO>> FetchTicketById(Guid ticketId)
        {
            try
            {
                if (ticketId == Guid.Empty)
                    return ActionResult<TicketDTO>.Failed("Invalid ticket id", StatusCodes.Status400BadRequest);

                var ticket = await ticketRepository.GetTicket(ticketId);
                if (ticket == null)
                    return ActionResult<TicketDTO>.Failed("There are no existing tickets", StatusCodes.Status400BadRequest);

                var currentTicket = new TicketDTO()
                {
                    Description = ticket.Description,
                    Issue = ticket.Issue,
                    ImageUrl = ticket.ImageUrl,
                    Issuer = ticket.Issuer,
                    TicketId = ticket.Id,
                    Status = ticket.Status.ToString(),
                    FirstName = ticket.User.FirstName,
                    LastName = ticket.User.LastName
                };

                return ActionResult<TicketDTO>.SuccessfulOperation(currentTicket);
            }
            catch (Exception ex)
            {
                return ActionResult<TicketDTO>.Failed(ex.Message);
            }
        }

        public ActionResult<PaginatedResponse<TicketsDTO>> SearchTickets(DateTime created, int pageNumber, int pageSize)
        {
            try
            {
                if(created == DateTime.MinValue)
                {
                    return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid date", StatusCodes.Status400BadRequest);
                }

                if (pageNumber == 0 || pageSize == 0)
                    return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid page number or page size", StatusCodes.Status400BadRequest);

                var tickets = ticketRepository.SearchTickets(x => x.Created == created, pageNumber, pageSize);

                if (tickets == null)
                    return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("There are no tickets", StatusCodes.Status400BadRequest);

                var currentTickets = tickets.Select(s =>
                   new TicketsDTO()
                   {
                       Issue = s.Issue,
                       TicketId = s.Id,
                       Status = s.Status.ToString(),
                       FirstName = s.User.FirstName,
                       LastName = s.User.LastName
                   });

                var paginatedResponse = new PaginatedResponse<TicketsDTO>(pageNumber, pageSize, currentTickets);

                return ActionResult<PaginatedResponse<TicketsDTO>>.SuccessfulOperation(paginatedResponse);
            }
            catch (Exception ex)
            {
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed(ex.Message);

            }
        }

        public ActionResult<PaginatedResponse<TicketsDTO>> SearchTickets(string status, int pageNumber, int pageSize)
        {
            try
            {
                if (string.IsNullOrEmpty(status))
                {
                    return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid date", StatusCodes.Status400BadRequest);
                }

                if (pageNumber == 0 || pageSize == 0)
                    return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid page number or page size", StatusCodes.Status400BadRequest);

                var isStatus = Enum.TryParse(status, out Status newStatus);

                if (!isStatus)
                {
                    return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid status", StatusCodes.Status400BadRequest);
                }

                var tickets = ticketRepository.SearchTickets(x => x.Status == newStatus, pageNumber, pageSize);

                if (tickets == null)
                    return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("There are no tickets", StatusCodes.Status400BadRequest);

                var currentTickets = tickets.Select(s =>
                   new TicketsDTO()
                   {
                       Issue = s.Issue,
                       TicketId = s.Id,
                       Status = s.Status.ToString(),
                       FirstName = s.User.FirstName,
                       LastName = s.User.LastName
                   });

                var paginatedResponse = new PaginatedResponse<TicketsDTO>(pageNumber, pageSize, currentTickets);

                return ActionResult<PaginatedResponse<TicketsDTO>>.SuccessfulOperation(paginatedResponse);
            }
            catch (Exception ex)
            {
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed(ex.Message);

            }
        }



        public async Task<ActionResult> UpdateTicket(string status, Guid adminId, Guid ticketId)
        {
            try
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

                var ticket = currentTicket.Adapt<Ticket>();

                var commitStatus = await ticketRepository.Update(ticket);

                if(!commitStatus)
                {
                    return ActionResult.Failed("Failed to update ticket", StatusCodes.Status400BadRequest);
                }

                return ActionResult.SuccessfulOperation();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }

        public async Task<ActionResult> DeleteTicket(Guid ticketId)
        {
            try
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
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }



        private readonly ITicketRepository ticketRepository;
        private readonly IBlobService blobService;
    }
}
