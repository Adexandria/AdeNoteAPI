using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    /// <summary>
    /// Note Management: 
    /// - Create a note
    /// - Update a note
    /// - Delete a note
    /// </summary>
    [Route("api/notes")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NoteController : BaseController
    {
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="container">A container that contains all the built dependency</param>
        /// <param name="application">An interface used to interact with the layers</param>
        /// <param name="userIdentity">An interface that interacts with the user. This fetches the current user details</param>
        public NoteController(IContainer container, ITaskApplication application, IUserIdentity userIdentity) : base(container, application, userIdentity)
        {
        }

        /// <summary>
        /// Fetches all notes
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             GET  /notes
        /// </remarks>
        /// <returns>a list of notes</returns>
        ///  <response code ="200"> Returns if Successful</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <response code ="404"> Returns if Not found</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult<List<NoteDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            var getAllTasksResponse = await Application.SendQuery<FetchAllNotesQuery, List<NoteDTO>>(Container,
                new FetchAllNotesQuery() { UserId = CurrentUser });
            return getAllTasksResponse.Response();
        }


        /// <summary>
        /// Fetches a particular note
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///         
        ///                 GET /notes/20b1204e-fad5-4a90-a78e-bc3b988afd60
        ///                 
        /// </remarks>
        /// <param name="noteId">A note id</param>
        /// <returns>A single note</returns>
        ///  <response code ="200"> Returns if Successful</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <response code ="404"> Returns if not found</response>
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult<NoteDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Creates a new note
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///             POST /notes
        /// </remarks>
        /// <param name="newNote"></param>
        ///  <response code ="200"> Returns if Successful</response>
        ///  <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <response code ="404"> Returns if not found</response>
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult<List<NoteDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Updates an existing note
        /// </summary>
        /// <remarks>
        /// Sample request: 
        ///     
        ///             PUT /notes
        ///             {
        ///                 "taskId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///                 "task": "string",
        ///                 "description": "string"
        ///              }
        /// </remarks>
        /// <param name="command">A model to udapte an existing note</param>
        ///  <response code ="200"> Returns if Successful</response>
        ///  <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult<List<NoteDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPut]
        public async Task<IActionResult> UpdateNote(UpdateTaskCommand command)
        {
            var updatedResponse = await Application.ExecuteCommand(Container, command);
            return updatedResponse.Response();
        }

        /// <summary>
        /// Deletes an existing note
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///         
        ///             DELETE /notes/20b1204e-fad5-4a90-a78e-bc3b988afd60
        /// </remarks>
        /// <param name="noteId">A note id</param>
        /// <response code ="200"> Returns if Successful</response>
        /// <response code ="404"> Returns if not found</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
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
