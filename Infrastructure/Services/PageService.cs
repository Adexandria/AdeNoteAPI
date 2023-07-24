using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Mapster;
using System.Net;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public class PageService : IPageService
    {
        public PageService(IPageRepository _pageRepository, IBookRepository _bookRepository,ILabelRepository _labelRepository, ILabelPageRepository _labelPageRepository)
        {
            pageRepository = _pageRepository;
            bookRepository = _bookRepository;
            labelRepository = _labelRepository;
            labelPageRepository = _labelPageRepository;
            
        }
        public async Task<ActionResult> Add(Guid bookId, Guid userId, PageCreateDTO createPage)
        {

            try
            {
                if (bookId == Guid.Empty || userId == Guid.Empty)
                    return await Task.FromResult(ActionResult.Failed("Invalid id"));

                var currentBook = bookRepository.GetAsync(bookId, userId);
                if (currentBook == null)
                    return await Task.FromResult(ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound));

                var page = createPage.Adapt<Page>();
                page.BookId = bookId;

                var commitStatus = await pageRepository.Add(page);
                if (!commitStatus)
                    return ActionResult.Failed("Failed to add page");

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
           
        }

        public async Task<ActionResult<IEnumerable<PageDTO>>> GetAll(Guid bookId)
        {
            if (bookId == Guid.Empty)
                return await Task.FromResult(ActionResult<IEnumerable<PageDTO>>.Failed("Invalid id"));

            var currentBookPages = pageRepository.GetBookPages(bookId);
            var currentBookPagesDTO = currentBookPages.Adapt<IEnumerable<PageDTO>>(MappingService.PageLabelsConfig());

            return await Task.FromResult(ActionResult<IEnumerable<PageDTO>>.SuccessfulOperation(currentBookPagesDTO));
        }

        public async Task<ActionResult<PageDTO>> GetById(Guid bookId, Guid pageId)
        {
            if (bookId == Guid.Empty || pageId == Guid.Empty)
                return await Task.FromResult(ActionResult<PageDTO>.Failed("Invalid id"));

            var currentBookPage = await pageRepository.GetBookPage(bookId, pageId);
            if (currentBookPage == null)
                return await Task.FromResult(ActionResult<PageDTO>.Failed("page doesn't exist", (int)HttpStatusCode.NotFound));
            var currentBookPageDTO = currentBookPage.Adapt<PageDTO>();

            return ActionResult<PageDTO>.SuccessfulOperation(currentBookPageDTO);
        }

        public async Task<ActionResult> Remove(Guid bookId, Guid userId, Guid pageId)
        {
            try
            {
                if (bookId == Guid.Empty || pageId == Guid.Empty || userId == Guid.Empty)
                    return await Task.FromResult(ActionResult.Failed("Invalid id"));

                var currentBook = bookRepository.GetAsync(bookId, userId);
                if (currentBook == null)
                    return await Task.FromResult(ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound));

                var currentBookPage = await pageRepository.GetBookPage(bookId, pageId);
                if (currentBookPage == null)
                    return await Task.FromResult(ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound));

                var commitStatus = await pageRepository.Remove(currentBookPage);
                if (!commitStatus)
                    return ActionResult.Failed("Failed to delete a page");

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
            
        }

        public async  Task<ActionResult> Update(Guid bookId, Guid userId, Guid pageId, PageUpdateDTO updatePage)
        {

            try
            {
                if (bookId == Guid.Empty || pageId == Guid.Empty || userId == Guid.Empty)
                    return await Task.FromResult(ActionResult.Failed("Invalid id"));

                var currentBook = await bookRepository.GetAsync(bookId, userId);
                if (currentBook == null)
                    return await Task.FromResult(ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound));

                var currentBookPage = await pageRepository.GetBookPage(bookId, pageId);
                if (currentBookPage == null)
                    return await Task.FromResult(ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound));

                var page = updatePage.Adapt<Page>(MappingService.UpdateLabelConfig());
                page.Id = pageId;
                page.BookId = bookId;

                var commitStatus = await pageRepository.Update(page);
                if (!commitStatus)
                    return ActionResult.Failed("Failed to update page");

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
            
        }
        public async Task<ActionResult> AddLabels(Guid bookId,Guid userId,Guid pageId,List<string> Labels)
        {
            var currentBook = await bookRepository.GetAsync(bookId, userId);
            if (currentBook == null)
                return await Task.FromResult(ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound));

            var currentBookPage = await pageRepository.GetBookPage(bookId, pageId);
            if (currentBookPage == null)
                return await Task.FromResult(ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound));
            if (Labels != null)
            {
                foreach (var label in Labels)
                {
                    var currentLabel = await labelRepository.GetByNameAsync(label);
                    if (currentLabel == null)
                        return ActionResult.Failed("Label doesn't exist", (int)HttpStatusCode.NotFound);

                    if (currentBookPage.Labels.Contains(currentLabel))
                        return ActionResult.Failed("Label has been added", (int)HttpStatusCode.BadRequest);

                    var status = await labelPageRepository.AddLabelToPage(pageId, currentLabel.Id);
                    if (!status)
                        return await Task.FromResult(ActionResult.Failed("Failed to add label"));
                }

            }

            return ActionResult.Successful();
        }

        public async Task<ActionResult> RemoveAllPageLabels(Guid bookId,Guid userId, Guid pageId)
        {
            if (bookId == Guid.Empty || pageId == Guid.Empty || userId == Guid.Empty)
                return await Task.FromResult(ActionResult.Failed("Invalid id"));

            var currentBook = bookRepository.GetAsync(bookId, userId);
            if (currentBook == null)
                return await Task.FromResult(ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound));

            var currentBookPage = await pageRepository.GetBookPage(bookId, pageId);
            if (currentBookPage == null)
                return await Task.FromResult(ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound));

            var pageLabels = await labelPageRepository.GetLabels(pageId);
            if(pageLabels == null)
                return ActionResult.Failed("Labels doesn't exist for this page", (int) HttpStatusCode.NotFound);

            var commitStatus = await labelPageRepository.DeleteLabelsFromPage(pageLabels);
            if (!commitStatus)
                return ActionResult.Failed("Failed to delete labels");

            return ActionResult.Successful();
        }

        public async Task<ActionResult> RemovePageLabel(Guid bookId, Guid userId, Guid pageId, string title)
        {
            if (bookId == Guid.Empty || pageId == Guid.Empty || userId == Guid.Empty)
                return await Task.FromResult(ActionResult.Failed("Invalid id"));

            var currentBook = bookRepository.GetAsync(bookId, userId);
            if (currentBook == null)
                return await Task.FromResult(ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound));

            var currentBookPage = await pageRepository.GetBookPage(bookId, pageId);
            if (currentBookPage == null)
                return await Task.FromResult(ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound));

            var currentLabel = currentBookPage.Labels.Where(s=>s.Title == title).Select(s=>s.Id).FirstOrDefault();
            if(currentLabel == Guid.Empty)
                return await Task.FromResult(ActionResult.Failed("label doesn't exist in this page", (int)HttpStatusCode.NotFound));

            var currentLabelPage = await labelPageRepository.GetLabel(pageId, currentLabel);

            var commitStatus = await labelPageRepository.DeleteLabelFromPage(currentLabelPage);

            if (!commitStatus)
                return ActionResult.Failed("Failed to update page");

            return ActionResult.Successful();
        }

        private IPageRepository pageRepository;
        private IBookRepository bookRepository;
        private ILabelRepository labelRepository;
        private ILabelPageRepository labelPageRepository;

    }
}
