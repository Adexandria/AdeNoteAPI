using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using Excelify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula;

namespace AdeNote.Controllers
{
    [Authorize]
    [Route("api/excel")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        public ExcelController(IExcel excelService, IUserIdentity userIdentity)
        {
            _excelService = excelService;
            _userIdentity = userIdentity;
        }

    

        private readonly IExcel _excelService;
        private readonly IUserIdentity _userIdentity;
    }
}
