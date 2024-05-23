namespace AdeNote.Models.DTOs
{
    public class TranslationDto
    {
        public TranslationDto(string originalText, 
            string translatedText, string originalLanguage,string translationLanaguage)
        {
            OriginalPage = originalText;
            TranslatedPage = translatedText;
            OriginalLanguage = originalLanguage;
            TranslationLanguage = translationLanaguage; 
        }
        public string OriginalPage { get; }
        public string OriginalLanguage { get; }
        public string TranslatedPage { get; }
        public string TranslationLanguage { get; }
    }
}
