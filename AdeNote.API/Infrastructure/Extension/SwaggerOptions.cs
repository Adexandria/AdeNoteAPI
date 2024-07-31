using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace AdeNote.Infrastructure.Extension
{
    public class SwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        public SwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }
        /// <summary>
        ///  Configure swagger options for api versioning.
        ///  This method was inherited
        /// </summary>
        /// <param name="name">Api docs name</param>
        /// <param name="options">swagger gen option</param>
        public void Configure(string name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        /// <summary>
        ///  Configure swagger options for api versioning.
        ///  This method was inherited
        /// </summary>
        /// <param name="options">swagger gen option</param>
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
            }
        }

        /// <summary>
        /// Creates a version Info for the api versions on swagger.
        /// </summary>
        /// <param name="versionDescription">Includes the group name and description of the version</param>
        /// <returns>The info of the version</returns>
        private OpenApiInfo CreateVersionInfo(ApiVersionDescription versionDescription)
        {
            var info = new OpenApiInfo()
            {
                Title = "AdeNote API",
                Version = versionDescription.ApiVersion.ToString(),
                Contact = new OpenApiContact()
                {
                    Email = "adeolaaderibigbe09@gmail.com",
                    Name = "Adeola Wuraola Aderibigbe"

                },
                Description = "This is a simple note API"
            };

            if (versionDescription.IsDeprecated)
            {
                info.Description += "This API version has been deprecated.";
            }
            return info;
        }
    }
}
