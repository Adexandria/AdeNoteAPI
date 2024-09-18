using AdeCache.Services;
using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services.TranslationAI;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Automappify.Services;
using System.Net;

namespace AdeNote.Infrastructure.Services.PageSettings
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
        public PageService(IPageRepository _pageRepository,
            IBookRepository _bookRepository, ILabelRepository _labelRepository, ILabelPageRepository _labelPageRepository,
            ITextTranslation _textTranslation, ICacheService _cacheService)
        {
            pageRepository = _pageRepository;
            bookRepository = _bookRepository;
            labelRepository = _labelRepository;
            labelPageRepository = _labelPageRepository;
            textTranslation = _textTranslation;
            cacheService = _cacheService;
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
            if (bookId == Guid.Empty || userId == Guid.Empty)
                return ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest);

            var currentBook = cacheService.Get<Book>($"{_bookCacheKey}:{userId}:{bookId}") ?? await bookRepository.GetAsync(bookId, userId);

            if (currentBook == null)
                return ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound);

            var page = createPage.Map<PageCreateDTO,Page>();

            page.BookId = bookId;

            var commitStatus = await pageRepository.Add(page);
            if (!commitStatus)
                return ActionResult.Failed("Failed to add page");

            cacheService.Set($"{_pageCacheKey}:{bookId}:{page.Id}",page, DateTime.UtcNow.AddMinutes(30));

            return ActionResult.SuccessfulOperation();

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

            var currentPages = cacheService.Search<Page>(_pageCacheKey,"*");

            if(currentPages == null)
            {
                currentPages = pageRepository.GetBookPages(bookId).ToList();
                currentPages.Foreach(currentPage => cacheService.Set($"{_pageCacheKey}:{bookId}:{currentPage.Id}", currentPage, DateTime.UtcNow.AddMinutes(30)));
            }

            var currentBookPagesDTO = currentPages.Map<IEnumerable<Page>,IEnumerable<PageDTO>>(MappingService.PageLabelsConfig());

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


            var currentBookPage = cacheService.Get<Page>($"{_pageCacheKey}:{bookId}:{pageId}") ?? await pageRepository.GetBookPage(bookId, pageId);
            
            if (currentBookPage == null)
                return await Task.FromResult(ActionResult<PageDTO>.Failed("page doesn't exist", (int)HttpStatusCode.NotFound));
            
            var currentBookPageDTO = currentBookPage.Map<Page,PageDTO>(MappingService.PageLabelsConfig());

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
            if (bookId == Guid.Empty || pageId == Guid.Empty || userId == Guid.Empty)
                return ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest);

            var currentBook = cacheService.Get<Book>($"{_bookCacheKey}:{userId}:{bookId}") ?? await bookRepository.GetAsync(bookId, userId);

            if (currentBook == null)
                return ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound);

            var currentBookPage = cacheService.Get<Page>($"{_pageCacheKey}:{bookId}:{pageId}") ?? await pageRepository.GetBookPage(bookId, pageId,true);

            if (currentBookPage == null)
                return ActionResult<PageDTO>.Failed("page doesn't exist", (int)HttpStatusCode.NotFound);

            var commitStatus = await pageRepository.Remove(currentBookPage);
            if (!commitStatus)
                return ActionResult.Failed("Failed to delete page");

            cacheService.Remove($"{_pageCacheKey}:{bookId}:{pageId}");

            return ActionResult.SuccessfulOperation();

        }

        /// <summary>
        /// Updates an existing page
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="pageId">Page id</param>
        /// <param name="updatePage">An object to update an existing page</param>
        /// <returns>An action result</returns>
        public async Task<ActionResult> Update(Guid bookId, Guid userId, Guid pageId, PageUpdateDTO updatePage)
        {
            if (bookId == Guid.Empty || pageId == Guid.Empty || userId == Guid.Empty)
                return ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest);

            var currentBook = cacheService.Get<Book>($"{_bookCacheKey}:{userId}:{bookId}") ?? await bookRepository.GetAsync(bookId, userId);

            if (currentBook == null)
                return ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound);

            var currentBookPage = cacheService.Get<Page>($"{_pageCacheKey}:{bookId}:{pageId}") ?? await pageRepository.GetBookPage(bookId, pageId, true);
            if (currentBookPage == null)
                return ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound);

            var page = updatePage.Map<PageUpdateDTO,Page>();
            page.Id = pageId;
            page.BookId = bookId;
            page.SetModifiedDate();

            var commitStatus = await pageRepository.Update(page, currentBookPage);
            if (!commitStatus)
                return ActionResult.Failed("Failed to update page");

            cacheService.Set($"{_pageCacheKey}:{bookId}:{pageId}", page, DateTime.UtcNow.AddMinutes(30));

            return ActionResult.SuccessfulOperation();

        }

        /// <summary>
        /// Adds labels to a page
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="pageId">Page id</param>
        /// <param name="Labels">a list of labels</param>
        /// <returns>An action result</returns>
        public async Task<ActionResult> AddLabels(Guid bookId, Guid userId, Guid pageId, List<string> Labels)
        {
            if (bookId == Guid.Empty || pageId == Guid.Empty || userId == Guid.Empty)
                return ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest);

            var currentBook = cacheService.Get<Book>($"{_bookCacheKey}:{userId}:{bookId}") ?? await bookRepository.GetAsync(bookId, userId);

            if (currentBook == null)
                return ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound);

            var currentBookPage = cacheService.Get<Page>($"{_pageCacheKey}:{bookId}:{pageId}") ?? await pageRepository.GetBookPage(bookId, pageId);
            if (currentBookPage == null)
                return ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound);

            if (Labels != null)
            {
                foreach (var label in Labels)
                {
                    var currentLabels = cacheService.Get<IEnumerable<Label>>(_labelCacheKey);

                    Label currentLabel = null;

                    if (currentLabels != null)
                    {
                        currentLabel = currentLabels.FirstOrDefault(s => s.Title == label);
                    }
                    else
                    {
                        currentLabel = await labelRepository.GetByNameAsync(label);
                    }

                    if (currentLabel == null)
                        return ActionResult.Failed("Label doesn't exist", StatusCodes.Status404NotFound);
                        
                    if (currentBookPage.Labels != null)
                        if (currentBookPage.Labels.Any(s => s.Title == currentLabel.Title))
                        {
                            return ActionResult.Failed("Label has been added", (int)HttpStatusCode.BadRequest);
                        }

                    var status = await labelPageRepository.AddLabelToPage(pageId, currentLabel.Id);
                    if (!status)
                        return ActionResult.Failed("Failed to add label");
                }
            }

            return ActionResult.SuccessfulOperation();

        }

        /// <summary>
        /// Removes all labels from a page
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="pageId">Page id</param>
        /// <returns>An action result</returns>
        public async Task<ActionResult> RemoveAllPageLabels(Guid bookId, Guid userId, Guid pageId)
        {
            if (bookId == Guid.Empty || pageId == Guid.Empty || userId == Guid.Empty)
                return ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest);

            var currentBook = cacheService.Get<Book>($"{_bookCacheKey}:{userId}:{bookId}") ?? await bookRepository.GetAsync(bookId, userId);

            if (currentBook == null)
                return ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound);

            var currentBookPage = cacheService.Get<Page>($"{_pageCacheKey}:{bookId}:{pageId}") ?? await pageRepository.GetBookPage(bookId, pageId);
            if (currentBookPage == null)
                return ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound);

            var pageLabels = await labelPageRepository.GetLabels(pageId);

            if (!pageLabels.Any())
                return ActionResult.Failed("Labels doesn't exist in this page", (int)HttpStatusCode.NotFound);

            var commitStatus = await labelPageRepository.DeleteLabelsFromPage(pageLabels);
            if (!commitStatus)
                return ActionResult.Failed("Failed to delete labels");

            return ActionResult.SuccessfulOperation();
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
                return ActionResult.Failed("Invalid id", (int)HttpStatusCode.BadRequest);

            var currentBook = cacheService.Get<Book>($"{_bookCacheKey}:{userId}:{bookId}") ?? await bookRepository.GetAsync(bookId, userId);

            if (currentBook == null)
                return ActionResult.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound);

            var currentBookPage = cacheService.Get<Page>($"{_pageCacheKey}:{bookId}:{pageId}") ?? await pageRepository.GetBookPage(bookId, pageId);
            if (currentBookPage == null)
                return ActionResult.Failed("page doesn't exist", (int)HttpStatusCode.NotFound);

            var currentLabel = currentBookPage.Labels.Where(s => s.Title == title).Select(s => s.Id).FirstOrDefault();
            if (currentLabel == Guid.Empty)
                return ActionResult.Failed("Label doesn't exist in this page", (int)HttpStatusCode.NotFound);

            var currentLabelPage = await labelPageRepository.GetLabel(pageId, currentLabel);
            if (currentLabelPage == null)
                return ActionResult.Failed("Label doesn't exist in this page", (int)HttpStatusCode.NotFound);

            var commitStatus = await labelPageRepository.DeleteLabelFromPage(currentLabelPage);

            if (!commitStatus)
                return ActionResult.Failed("Failed to delete label");

            return ActionResult.SuccessfulOperation();
        }

        public async Task<ActionResult<TranslationDto>> TranslatePage(Guid bookId, Guid userId, Guid pageId, string translatedLanguage, CancellationToken cancellationToken = default)
        {
            if (bookId == Guid.Empty || pageId == Guid.Empty || userId == Guid.Empty)
                return ActionResult<TranslationDto>.Failed("Invalid id", StatusCodes.Status400BadRequest);

            var currentBook = cacheService.Get<Book>($"{_bookCacheKey}:{userId}:{bookId}") ?? await bookRepository.GetAsync(bookId, userId);

            if (currentBook == null)
                return ActionResult<TranslationDto>.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound);

            var currentBookPage = cacheService.Get<Page>($"{_pageCacheKey}:{bookId}:{pageId}") ?? await pageRepository.GetBookPage(bookId, pageId);
            if (currentBookPage == null)
                return ActionResult<TranslationDto>.Failed("page doesn't exist", (int)HttpStatusCode.NotFound);

            var existingTranslatedPage = cacheService.Get<TranslationDto>($"{_translatedPageCacheKey}:{bookId}:{pageId}:{translatedLanguage.ToLower()}");

            if(existingTranslatedPage != null)
            {
                return ActionResult<TranslationDto>.SuccessfulOperation(existingTranslatedPage);
            }

            var translationLanguages = cacheService.Get<Dictionary<string,string>>("translation_languages");

            var transliterationLanguages = cacheService.Get<Dictionary<string, string>>("transliteration_languages");

            if (translationLanguages == null)
            {
                var languagesResponse = textTranslation.GetSupportedLanguages("translation", "transliteration", cancellationToken: cancellationToken);
                if (languagesResponse.NotSuccessful)
                {
                    return ActionResult<TranslationDto>
                    .Failed("Failed to fetch languages", StatusCodes.Status400BadRequest);
                }
                cacheService.Set("translation_languages", languagesResponse.Data.TranslationLanguages);
                cacheService.Set("transliteration_languages", languagesResponse.Data.TransliterationLanguages);
                cacheService.Set("etag", languagesResponse.Data.ETag);

                translationLanguages = languagesResponse.Data.TranslationLanguages;
                transliterationLanguages = languagesResponse.Data.TransliterationLanguages;
            }

            var translationLanguage = translationLanguages.FirstOrDefault(s => s.Key.ToUpper() == translatedLanguage.ToUpper()).Value;

            if (translationLanguage == null)
            {
                return ActionResult<TranslationDto>
                    .Failed("The language is not supported", StatusCodes.Status400BadRequest);
            }

            var translatedResponse = await textTranslation.TranslatePage(currentBookPage.Content, translationLanguage, cancellationToken);

            if (translatedResponse.NotSuccessful)
            {
                return ActionResult<TranslationDto>
                  .Failed(translatedResponse.Errors.FirstOrDefault(), StatusCodes.Status400BadRequest);
            }

            translatedLanguage = translationLanguages.FirstOrDefault(s => s.Key.ToUpper() == translatedLanguage.ToUpper()).Key;

            var detectedLanguage = translationLanguages.FirstOrDefault(s => s.Value.ToUpper() == translatedResponse.Data[1].ToUpper()).Key ?? translatedResponse.Data[1];

            var transliterationLanguage = transliterationLanguages.FirstOrDefault(s => s.Value.ToUpper() == translationLanguage.ToUpper()).Key;

            TranslationDto translatedPage;

            if (transliterationLanguage != null)
            {
                var transliterationResponse = await textTranslation.TransliteratePage(translatedResponse.Data[0], translationLanguage, transliterationLanguage, cancellationToken);

                if (transliterationResponse.NotSuccessful)
                {
                    return ActionResult<TranslationDto>
                     .Failed(transliterationResponse.Errors.FirstOrDefault(), StatusCodes.Status400BadRequest);
                }

                translatedPage = new TranslationDto(currentBookPage.Content,
                translatedResponse.Data[0],
                detectedLanguage, translatedLanguage, transliterationResponse.Data);

                cacheService.Set($"{_translatedPageCacheKey}:{bookId}:{pageId}:{translatedLanguage.ToLower()}", translatedPage, DateTime.UtcNow.AddMinutes(10));

                return ActionResult<TranslationDto>.SuccessfulOperation(translatedPage);
            }

              translatedPage = new TranslationDto(currentBookPage.Content,
                translatedResponse.Data[0],
                detectedLanguage, translatedLanguage);

            cacheService.Set($"{_translatedPageCacheKey}:{bookId}:{pageId}:{translatedLanguage.ToLower()}", translatedPage, DateTime.UtcNow.AddMinutes(10));

            return ActionResult<TranslationDto>.SuccessfulOperation(translatedPage);
        }

        public ICacheService cacheService;
        public ITextTranslation textTranslation;
        public IPageRepository pageRepository;
        public IBookRepository bookRepository;
        public ILabelRepository labelRepository;
        public ILabelPageRepository labelPageRepository;

        private string _bookCacheKey = "Books";
        private string _pageCacheKey = "Pages";
        private string _translatedPageCacheKey = "Pages";
        private string _labelCacheKey = "Labels";

    }
}
