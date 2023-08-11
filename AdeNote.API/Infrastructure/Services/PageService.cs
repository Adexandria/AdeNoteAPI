using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Mapster;
using System.Net;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    /// <summary>
    /// An implementation of the interface
    /// </summary>
    public class PageService : IPageService
    {
        /// <summary>
        /// A constructor
        /// </summary>
        protected PageService()
        {

        }
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="_pageRepository">>Handles persisting and querying pages</param>
        /// <param name="_bookRepository">>Handles persisting and querying books</param>
        /// <param name="_labelRepository">>Handles persisting and querying labels</param>
        /// <param name="_labelPageRepository">>Handles persisting and querying page labels</param>
        public PageService(IPageRepository _pageRepository, IBookRepository _bookRepository,ILabelRepository _labelRepository, ILabelPageRepository _labelPageRepository)
        {
            pageRepository = _pageRepository;
            bookRepository = _bookRepository;
            labelRepository = _labelRepository;
            labelPageRepository = _labelPageRepository;
            
        }

        /// <summary>
        /// Adds a new page to a book
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="createPage">A object to create a new page</param>
        /// <returns>An action result</returns>
        public async Task<ActionResult> Add(Guid bookId, Guid userId, PageCreateDTO createPage)
        {

            try
            {
                if (bookId == Guid.Empty || userId == Guid.Empty)
                    return await Task.FromResult(ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest));

                var currentBook = await bookRepository.GetAsync(bookId, userId);
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
        /// <summary>
        /// Gets all pages in a book
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <returns>An action result</returns>
        public async Task<ActionResult<IEnumerable<PageDTO>>> GetAll(Guid bookId)
        {
            if (bookId == Guid.Empty)
                return await Task.FromResult(ActionResult<IEnumerable<PageDTO>>.Failed("Invalid id", (int)HttpStatusCode.BadRequest));

            var currentBookPages = pageRepository.GetBookPages(bookId);
            var currentBookPagesDTO = currentBookPages.Adapt<IEnumerable<PageDTO>>(MappingService.PageLabelsConfig());

            return await Task.FromResult(ActionResult<IEnumerable<PageDTO>>.SuccessfulOperation(currentBookPagesDTO));
        }

        /// <summary>
        /// Get a page by id
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="pageId">page id</param>
        /// <returns>An action result</returns>
        public async Task<ActionResult<PageDTO>> GetById(Guid bookId, Guid pageId)
        {
            if (bookId == Guid.Empty || pageId == Guid.Empty)
                return await Task.FromResult(ActionResult<PageDTO>.Failed("Invalid id", (int)HttpStatusCode.BadRequest));

            var currentBookPage = await pageRepository.GetBookPage(bookId, pageId);
            if (currentBookPage == null)
                return await Task.FromResult(ActionResult<PageDTO>.Failed("page doesn't exist", (int)HttpStatusCode.NotFound));
            var currentBookPageDTO = currentBookPage.Adapt<PageDTO>();

            return ActionResult<PageDTO>.SuccessfulOperation(currentBookPageDTO);
        }

        /// <summary>
        /// Removes a particular page
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="pageId">Page id</param>
        /// <returns>An action result</returns>
        public async Task<ActionResult> Remove(Guid bookId, Guid userId, Guid pageId)
        {
            try
            {
                if (bookId == Guid.Empty || pageId == Guid.Empty || userId == Guid.Empty)
                    return await Task.FromResult(ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest));

                var currentBook = await bookRepository.GetAsync(bookId, userId);
                if (currentBook == null)
                    return await Task.FromResult(ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound));

                var currentBookPage = await pageRepository.GetBookPage(bookId, pageId);
                if (currentBookPage == null)
                    return await Task.FromResult(ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound));

                var commitStatus = await pageRepository.Remove(currentBookPage);
                if (!commitStatus)
                    return ActionResult.Failed("Failed to delete page");

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
            
        }

        /// <summary>
        /// Updates an existing page
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="pageId">Page id</param>
        /// <param name="updatePage">An object to update an existing page</param>
        /// <returns>An action result</returns>
        public async  Task<ActionResult> Update(Guid bookId, Guid userId, Guid pageId, PageUpdateDTO updatePage)
        {

            try
            {
                if (bookId == Guid.Empty || pageId == Guid.Empty || userId == Guid.Empty)
                    return await Task.FromResult(ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest));

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

        /// <summary>
        /// Adds labels to a page
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="pageId">Page id</param>
        /// <param name="Labels">a list of labels</param>
        /// <returns>An action result</returns>
        public async Task<ActionResult> AddLabels(Guid bookId,Guid userId,Guid pageId,List<string> Labels)
        {

            if (bookId == Guid.Empty || pageId == Guid.Empty || userId == Guid.Empty)
                return await Task.FromResult(ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest));

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

                    if (currentBookPage.Labels != null )
                        if (currentBookPage.Labels.Any(s=>s.Title == currentLabel.Title))
                        {
                            return ActionResult.Failed("Label has been added", (int)HttpStatusCode.BadRequest);

                        }

                    var status = await labelPageRepository.AddLabelToPage(pageId, currentLabel.Id);
                    if (!status)
                        return await Task.FromResult(ActionResult.Failed("Failed to add label"));
                }

            }

            return ActionResult.Successful();
        }

        /// <summary>
        /// Removes all labels from a page
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="pageId">Page id</param>
        /// <returns>An action result</returns>
        public async Task<ActionResult> RemoveAllPageLabels(Guid bookId,Guid userId, Guid pageId)
        {
            if (bookId == Guid.Empty || pageId == Guid.Empty || userId == Guid.Empty)
                return await Task.FromResult(ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest));

            var currentBook = await bookRepository.GetAsync(bookId, userId);
            if (currentBook == null)
                return await Task.FromResult(ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound));

            var currentBookPage = await pageRepository.GetBookPage(bookId, pageId);
            if (currentBookPage == null)
                return await Task.FromResult(ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound));

            var pageLabels = await labelPageRepository.GetLabels(pageId);
            if(!pageLabels.Any())
                return ActionResult.Failed("Labels doesn't exist in this page", (int) HttpStatusCode.NotFound);

            var commitStatus = await labelPageRepository.DeleteLabelsFromPage(pageLabels);
            if (!commitStatus)
                return ActionResult.Failed("Failed to delete labels");

            return ActionResult.Successful();
        }

        /// <summary>
        /// Removes a particular label from a page
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="pageId">Page id</param>
        /// <param name="title">label</param>
        /// <returns>An action result</returns>
        public async Task<ActionResult> RemovePageLabel(Guid bookId, Guid userId, Guid pageId, string title)
        {
            if (bookId == Guid.Empty || pageId == Guid.Empty || userId == Guid.Empty)
                return await Task.FromResult(ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest));

            var currentBook = await bookRepository.GetAsync(bookId, userId);
            if (currentBook == null)
                return await Task.FromResult(ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound));

            var currentBookPage = await pageRepository.GetBookPage(bookId, pageId);
            if (currentBookPage == null)
                return await Task.FromResult(ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound));

            var currentLabel = currentBookPage.Labels.Where(s=>s.Title == title).Select(s=>s.Id).FirstOrDefault();
            if(currentLabel == Guid.Empty)
                return await Task.FromResult(ActionResult.Failed("Label doesn't exist in this page", (int)HttpStatusCode.NotFound));

            var currentLabelPage = await labelPageRepository.GetLabel(pageId, currentLabel);
            if(currentLabelPage == null)
                return ActionResult.Failed("Label doesn't exist in this page", (int)HttpStatusCode.NotFound);

            var commitStatus = await labelPageRepository.DeleteLabelFromPage(currentLabelPage);

            if (!commitStatus)
                return ActionResult.Failed("Failed to delete label");

            return ActionResult.Successful();
        }

        public IPageRepository pageRepository;
        public IBookRepository bookRepository;
        public ILabelRepository labelRepository;
        public ILabelPageRepository labelPageRepository;

    }
}
