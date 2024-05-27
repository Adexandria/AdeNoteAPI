# Specify the .net version image to use
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# A work directory to instructs the docker to use this path for the subsequent commands.
WORKDIR /app

# Copy the only the project file then run dotnet restore
COPY ./ .

RUN dotnet restore


RUN  dotnet publish -c Release -o out

# Specify the .net version to use as runtime
FROM  mcr.microsoft.com/dotnet/sdk:6.0

# A work directory to instructs the docker to use this path for the subsequent commands.
WORKDIR /app

# Copy the directory from the build-env stage into runtime image
COPY --from=build-env /app/out .

# Expose port 80 to incoming requests
EXPOSE 80

# Runs the application using project dll file.
ENTRYPOINT ["dotnet", "AdeNote.dll"]