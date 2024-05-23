namespace AdeNote.Models.DTOs
{
    public class TranslationDto
    {
        public TranslationDto(string originalText, 
            string translatedText, string originalLanguage,string translationLanaguage)
        {
            OriginalText = originalText;
            TranslatedText = translatedText;
            OriginalLanguage = originalLanguage;
            TranslationLanguage = translationLanaguage; 
        }
        public string OriginalText { get; }
        public string OriginalLanguage { get; }
        public string TranslatedText { get; }
        public string TranslationLanguage { get; }
    }
}
