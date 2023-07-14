using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdeNote.Controllers
{
    [Route("api/labels")]
    [ApiController]
    [Authorize]
    public class LabelController : ControllerBase
    {
        private readonly ILabelService _labelService;
        private readonly Guid CurrentUser;
        public LabelController(ILabelService labelService, IUserIdentity userIdentity)
        {
            CurrentUser = userIdentity.UserId;
            _labelService = labelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLabels()
        {
            var response = await _labelService.GetAll();
            return response.Response();
        }

        [HttpGet("{labelId}")]
        public async Task<IActionResult> GetLabel(Guid labelId)
        {
            var response = await _labelService.GetById(labelId);
            return response.Response();
        }

        [HttpPost]
        public async Task<IActionResult> CreateLabel(LabelCreateDTO createLabel)
        {
            var response = await _labelService.Add(createLabel);
            return response.Response();
        }

        [HttpPut("{labelId}")]
        public async Task<IActionResult> UpdateLabel(Guid labelId,LabelUpdateDTO updateLabel)
        {
            var response = await _labelService.Update(labelId, updateLabel);
            return response.Response();
        }

        [HttpDelete("{labelId}")]
        public async Task<IActionResult> DeleteLabel(Guid labelId)
        {
            var response = await _labelService.Remove(labelId);
            return response.Response();
        }
    }
}
