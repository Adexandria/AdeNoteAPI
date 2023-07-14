using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using Autofac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksLibrary.Application.Commands.CreateTask;
using TasksLibrary.Application.Commands.DeleteTask;
using TasksLibrary.Application.Commands.UpdateTask;
using TasksLibrary.Application.Queries.FetchAllNotes;
using TasksLibrary.Application.Queries.FetchNoteById;
using TasksLibrary.Architecture.Application;

namespace AdeNote.Controllers
{
    [Route("api/notes")]
    [ApiController]
    [Authorize]
    public class NoteController : BaseController
    {
        public NoteController(IContainer container, ITaskApplication application, IUserIdentity userIdentity) : base(container, application, userIdentity)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            var getAllTasksResponse = await Application.SendQuery<FetchAllNotesQuery, List<NoteDTO>>(Container, 
                new FetchAllNotesQuery() { UserId = CurrentUser });
            return getAllTasksResponse.Response();
        }

        [HttpGet("{noteId}")]
        public async Task<IActionResult> GetNote(Guid noteId)
        {
            var command = new FetchNoteByIdQuery()
            {
                TaskId = noteId,
                UserId = CurrentUser
            };
            var taskResponse = await Application.SendQuery<FetchNoteByIdQuery, NoteDTO>(Container, command);
            return taskResponse.Response();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote(CreateNoteDTO newNote)
        {
            var taskCommand = new CreateTaskCommand()
            {
                Task = newNote.Task,
                Description = newNote.Description,
                UserId = CurrentUser
            };

            var createdResponse = await Application.ExecuteCommand<CreateTaskCommand, Guid>(Container, taskCommand);
            return createdResponse.Response();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNote(UpdateTaskCommand command)
        {
            var updatedResponse = await Application.ExecuteCommand(Container, command);
            return updatedResponse.Response();
        }


        [HttpDelete("{noteId}")]
        public async Task<IActionResult> DeleteNote(Guid noteId)
        {
            var command = new DeleteTaskCommand()
            {
                TaskId = noteId
            };

            var deletedResponse = await Application.ExecuteCommand(Container, command);
            return deletedResponse.Response();
        }
    }
}
