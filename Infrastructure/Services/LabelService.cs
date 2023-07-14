using AdeNote.Infrastructure.Repository;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Mapster;
using System.Net;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public class LabelService : ILabelService
    {
        public LabelService(LabelRepository _labelRepository)
        {
            labelRepository = _labelRepository;
        }
        public async Task<ActionResult> Add(LabelCreateDTO createLabel)
        {
            try
            {
                var label = createLabel.Adapt<Label>();
                var commitStatus = await labelRepository.Add(label);
                if (!commitStatus)
                    return ActionResult.Failed("Failed to update book");

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }

        public async Task<ActionResult<IEnumerable<LabelDTO>>> GetAll()
        {
           var currentLabels = labelRepository.GetAll();
           var currentLabelsDTO = currentLabels.Adapt<IEnumerable<LabelDTO>>();
           return await Task.FromResult(ActionResult<IEnumerable<LabelDTO>>.SuccessfulOperation(currentLabelsDTO));
        }

        public async Task<ActionResult<LabelDTO>> GetById(Guid labelId)
        {

           if (labelId == Guid.Empty)
              return await Task.FromResult(ActionResult<LabelDTO>.Failed("Invalid id"));

            var currentLabel = await labelRepository.GetAsync(labelId);
            var currentLabelDTO = currentLabel.Adapt<LabelDTO>();
            return await Task.FromResult(ActionResult<LabelDTO>.SuccessfulOperation(currentLabelDTO));
        }

        public async Task<ActionResult> Remove(Guid labelId)
        {
            try
            {
                if (labelId == Guid.Empty)
                    return await Task.FromResult(ActionResult.Failed("Invalid id"));

                var currentLabel = await labelRepository.GetAsync(labelId);
                if(currentLabel == null)
                    return await Task.FromResult(ActionResult.Failed("label doesn't exist", (int)HttpStatusCode.NotFound));

                var commitStatus = await labelRepository.Remove(currentLabel);
                if (!commitStatus)
                    return ActionResult.Failed("Failed to update book");

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }

        public async Task<ActionResult> Update(Guid labelId, LabelUpdateDTO updateLabel)
        {
            try
            {
                if (labelId == Guid.Empty)
                    return await Task.FromResult(ActionResult.Failed("Invalid id"));

                var currentLabel = await labelRepository.GetAsync(labelId);
                if (currentLabel == null)
                    return await Task.FromResult(ActionResult.Failed("label doesn't exist", (int)HttpStatusCode.NotFound));

                var label = updateLabel.Adapt<Label>();
                label.Id = labelId;

                var commitStatus = await labelRepository.Update(label);
                if (!commitStatus)
                    return ActionResult.Failed("Failed to update book");

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }
        private LabelRepository labelRepository;
    }
}
