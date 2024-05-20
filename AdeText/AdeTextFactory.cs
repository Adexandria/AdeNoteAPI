using AdeText.Services;

namespace AdeText
{
    public static class AdeTextFactory
    {

        public static ITranslateClient BuildClient(ITranslateConfiguration configuration)
        {
            return new TranslateClient(configuration);
        }
    }
}
