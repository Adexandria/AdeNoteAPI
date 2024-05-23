namespace AdeNote.Models.DTOs
{
    public class TranslationDto
    {
        public TranslationDto(string originalText, 
            string translatedText, string originalLanguage,
            string translationLanaguage, string transliteratedPage)
        {
            OriginalPage = originalText;
            TranslatedPage = translatedText;
            OriginalLanguage = originalLanguage;
            TransliteratedPage = transliteratedPage;
            TranslationLanguage = translationLanaguage; 

        }
        public string OriginalPage { get; }
        public string OriginalLanguage { get; }
        public string TranslatedPage { get; }
        public string TransliteratedPage { get; }
        public string TranslationLanguage { get; }
    }
}
