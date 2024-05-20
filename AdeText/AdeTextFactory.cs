using AdeText.Services;

namespace AdeText
{
    public static class AdeTextFactory
    {

        public static ITranslate BuildClient(ITranslateConfiguration configuration)
        {
            return new Translate(configuration);
        }
    }
}
