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
        public PageService(IPageRepository _pageRepository, IBookRepository _bookRepository)
        {
            pageRepository = _pageRepository;
            bookRepository = _bookRepository;
            
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
                    return ActionResult.Failed("Failed to update book");

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
            var currentBookPagesDTO = currentBookPages.Adapt<IEnumerable<PageDTO>>();

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
                    return ActionResult.Failed("Failed to update book");

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

                var currentBook = bookRepository.GetAsync(bookId, userId);
                if (currentBook == null)
                    return await Task.FromResult(ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound));

                var currentBookPage = await pageRepository.GetBookPage(bookId, pageId);
                if (currentBookPage == null)
                    return await Task.FromResult(ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound));

                var page = updatePage.Adapt<Page>();
                page.Id = pageId;
                page.BookId = bookId;
                page.AddLabels(updatePage.Labels);

                var commitStatus = await pageRepository.Update(currentBookPage);
                if (!commitStatus)
                    return ActionResult.Failed("Failed to update book");

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
            
        }


        private IPageRepository pageRepository;
        private IBookRepository bookRepository;

    }
}
