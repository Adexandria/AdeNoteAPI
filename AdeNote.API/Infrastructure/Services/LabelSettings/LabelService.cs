using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Mapster;
using System.Net;

namespace AdeNote.Infrastructure.Services.LabelSettings
{

    /// <summary>
    /// An implementaion of the interface
    /// </summary>
    public class LabelService : ILabelService
    {
        /// <summary>
        /// A constructor
        /// </summary>
        protected LabelService()
        {

        }
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="_labelRepository">Handles persisting and querying</param>
        public LabelService(ILabelRepository _labelRepository)
        {
            labelRepository = _labelRepository;
        }

        /// <summary>
        /// Adds new label
        /// </summary>
        /// <param name="createLabel">An object used to create new object</param>
        /// <returns>Action result</returns>
        public async Task<ActionResult> Add(LabelCreateDTO createLabel)
        {
            var label = createLabel.Adapt<Label>();

            var commitStatus = await labelRepository.Add(label);
            if (!commitStatus)
                return ActionResult.Failed("Failed to add label");

            return ActionResult.SuccessfulOperation();
        }

        /// <summary>
        /// Gets all the labels
        /// </summary>
        /// <returns>Action result</returns>
        public async Task<ActionResult<IEnumerable<LabelDTO>>> GetAll()
        {

            var currentLabels = labelRepository.GetAll();

            var currentLabelsDTO = currentLabels.Adapt<IEnumerable<LabelDTO>>(MappingService.LabelConfig());

            return ActionResult<IEnumerable<LabelDTO>>.SuccessfulOperation(currentLabelsDTO);

        }

        /// <summary>
        /// Gets a particular label by id
        /// </summary>
        /// <param name="labelId">label id</param>
        /// <returns>Action result</returns>
        public async Task<ActionResult<LabelDTO>> GetById(Guid labelId)
        {
            if (labelId == Guid.Empty)
                return ActionResult<LabelDTO>.Failed("Invalid id", (int)HttpStatusCode.BadRequest);

            var currentLabel = await labelRepository.GetNoTrackingAsync(labelId);
            if (currentLabel == null)
                return ActionResult<LabelDTO>.Failed("Label doesn't exist", (int)HttpStatusCode.NotFound);

            var currentLabelDTO = currentLabel.Adapt<LabelDTO>();
            return ActionResult<LabelDTO>.SuccessfulOperation(currentLabelDTO);


        }

        /// <summary>
        /// Removes an existing label
        /// </summary>
        /// <param name="labelId">label id</param>
        /// <returns>Action result</returns>
        public async Task<ActionResult> Remove(Guid labelId)
        {
            if (labelId == Guid.Empty)
                return ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest);

            var currentLabel = await labelRepository.GetNoTrackingAsync(labelId);
            if (currentLabel == null)
                return ActionResult.Failed("label doesn't exist", (int)HttpStatusCode.NotFound);

            var commitStatus = await labelRepository.Remove(currentLabel);
            if (!commitStatus)
                return ActionResult.Failed("Failed to remove label");

            return ActionResult.SuccessfulOperation();
        }

        /// <summary>
        /// Updates an existing label
        /// </summary>
        /// <param name="labelId">label id</param>
        /// <param name="updateLabel">An object to update a label</param>
        /// <returns>Action result</returns>
        public async Task<ActionResult> Update(Guid labelId, LabelUpdateDTO updateLabel)
        {
            if (labelId == Guid.Empty)
                return ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest);

            var currentLabel = await labelRepository.GetAsync(labelId);
            if (currentLabel == null)
                return ActionResult.Failed("label doesn't exist", (int)HttpStatusCode.NotFound);

            var label = updateLabel.Adapt<Label>();

            label.Id = labelId;

            label.SetModifiedDate();

            var commitStatus = await labelRepository.Update(label, currentLabel);
            if (!commitStatus)
                return ActionResult.Failed("Failed to update label");

            return ActionResult.SuccessfulOperation();
        }
        public ILabelRepository labelRepository { get; set; }
    }
}
