using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services;
using AdeNote.Models.DTOs;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdeNote.Controllers
{
    /// <summary>
    /// Handles creating, updating and deleting a label
    /// 
    /// 
    /// Supports version 1.0
    /// </summary>
    [Route("api/v{version:apiVersion}/labels")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class LabelController : BaseController
    {
        private readonly ILabelService _labelService;

        /// <summary>
        /// A Constructor
        /// </summary>
        /// <param name="labelService">An interface that interacts with the label tables</param>
        public LabelController(ILabelService labelService)
        {
            _labelService = labelService;
        }


        /// <summary>
        /// Fetches all label
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///         GET /labels
        ///         
        /// </remarks>
        /// <returns>A list of labels</returns>
        /// <response code ="200"> Returns if Successful</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<IEnumerable<LabelDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet]
        public async Task<IActionResult> GetAllLabels()
        {
            var response = await _labelService.GetAll();
            return response.Response();
        }

        /// <summary>
        /// Gets a single label
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     
        ///         GET /labels/20b1204e-fad5-4a90-a78e-bc3b988afd60
        /// </remarks>
        /// <param name="labelId">A label id</param>
        /// <returns>A single label</returns>
        /// <response code ="200"> Returns if Successful</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <response code ="404"> Returns if found</response>
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<LabelDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet("{labelId}")]
        public async Task<IActionResult> GetLabel(Guid labelId)
        {
            var response = await _labelService.GetById(labelId);
            return response.Response();
        }

        /// <summary>
        /// Creates a new label
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     
        ///             POST /labels
        ///             {
        ///              "title":"Ef Core"
        ///             }
        /// </remarks>
        /// <param name="createLabel">A model to create a new model</param>
        /// <response code ="200"> Returns if Successful</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost]
        public async Task<IActionResult> CreateLabel(LabelCreateDTO createLabel)
        {
            var response = await _labelService.Add(createLabel);
            return response.Response();
        }

        /// <summary>
        /// Updates an existing label
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     
        ///             PUT /labels/20b1204e-fad5-4a90-a78e-bc3b988afd60
        ///             {
        ///              "title":"Ef Core"
        ///             }
        /// </remarks>
        /// <param name="labelId">label id</param>
        /// <param name="updateLabel">A model used to update an existing label</param>
        /// <response code ="200"> Returns if Successful</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPut("{labelId}")]
        public async Task<IActionResult> UpdateLabel(Guid labelId,LabelUpdateDTO updateLabel)
        {
            var response = await _labelService.Update(labelId, updateLabel);
            return response.Response();
        }

        /// <summary>
        /// Deletes an existing label
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     
        ///             DELETE /labels/20b1204e-fad5-4a90-a78e-bc3b988afd60
        /// </remarks>
        /// <param name="labelId">label id</param>
        /// <response code ="200"> Returns if Successful</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <response code ="404"> Returns if not found</response>
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpDelete("{labelId}")]
        public async Task<IActionResult> DeleteLabel(Guid labelId)
        {
            var response = await _labelService.Remove(labelId);
            return response.Response();
        }
    }
}
