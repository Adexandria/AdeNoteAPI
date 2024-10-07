using AdeCache.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services.TranslationAI;
using AdeNote.Infrastructure.Utilities;

using AdeNote.Models.DTOs;
using AdeNote.Models;
using AdeText.Models;
using MediatR;
using System.Net;

namespace AdeNote.Infrastructure.Requests.TranslatePage
{
    public class TranslatePageRequestHandler : IRequestHandler<TranslatePageRequest, ActionResult>
    {
        public TranslatePageRequestHandler(IPageRepository pageRepository,
          IBookRepository bookRepository,
          ICacheService cacheService, ILabelPageRepository labelPageRepository,
          ILabelRepository labelRepository,
            CachingKeys cachingKeys, ITextTranslation textTranslation)
        {
            this.pageRepository = pageRepository;
            this.bookRepository = bookRepository;
            this.cacheService = cacheService;
            _bookCacheKey = cachingKeys.BookCacheKey;
            _pageCacheKey = cachingKeys.PageCacheKey;
            this.textTranslation = textTranslation;
        }
        public async Task<ActionResult> Handle(TranslatePageRequest request, CancellationToken cancellationToken)
        {
            var currentBook = cacheService.Get<Book>($"{_bookCacheKey}:{request.UserId}:{request.BookId}")
                ?? await bookRepository.GetAsync(request.BookId, request.UserId);

            if (currentBook == null)
                return ActionResult<TranslationDto>.Failed("Book doesn't exist", (int)HttpStatusCode.NotFound);

            var currentBookPage = cacheService.Get<Page>($"{_pageCacheKey}:{request.BookId}:{request.PageId}") 
                ?? await pageRepository.GetBookPage(request.BookId, request.PageId);

            if (currentBookPage == null)
                return ActionResult<TranslationDto>.Failed("page doesn't exist", (int)HttpStatusCode.NotFound);

            var existingTranslatedPage = cacheService.Get<TranslationDto>($"{_pageCacheKey}:{request.BookId}:{request.PageId}:{request.TranslatedLanguage.ToLower()}");

            if (existingTranslatedPage != null)
            {
                return ActionResult<TranslationDto>.SuccessfulOperation(existingTranslatedPage);
            }

            var translationLanguages = cacheService.Get<Dictionary<string, string>>("translation_languages");

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

            var translationLanguage = translationLanguages.FirstOrDefault(s => s.Key.ToUpper() == request.TranslatedLanguage.ToUpper()).Value;

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

            request.TranslatedLanguage = translationLanguages.FirstOrDefault(s => s.Key.ToUpper() == request.TranslatedLanguage.ToUpper()).Key;

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
                detectedLanguage, request.TranslatedLanguage, transliterationResponse.Data);

                cacheService.Set($"{_pageCacheKey}:{request.BookId}:{request.PageId}:{request.TranslatedLanguage.ToLower()}", translatedPage, DateTime.UtcNow.AddMinutes(10));

                return ActionResult<TranslationDto>.SuccessfulOperation(translatedPage);
            }
            translatedPage = new TranslationDto(currentBookPage.Content,
            translatedResponse.Data[0],
              detectedLanguage, request.TranslatedLanguage);

            cacheService.Set($"{_pageCacheKey}:{request.BookId}:{request.PageId}:{request.TranslatedLanguage.ToLower()}", translatedPage, DateTime.UtcNow.AddMinutes(10));

            return ActionResult<TranslationDto>.SuccessfulOperation(translatedPage);
        }

        private readonly ICacheService cacheService;
        private readonly IPageRepository pageRepository;
        private readonly IBookRepository bookRepository;
        private readonly string _pageCacheKey;
        private readonly string _bookCacheKey;
        private readonly ITextTranslation textTranslation;
    }
}
