# AdeNote API
This is a note management system API. It provides user with secured access to notes and also offers a to-do list feature.

![Git-Pages](https://github.com/Adeola-Aderibigbe/AdeNoteAPI/actions/workflows/dotnet.yml/badge.svg)              ![Git-Pages](https://github.com/Adeola-Aderibigbe/AdeNoteAPI/actions/workflows/build.yml/badge.svg)

### Packages
- Entity Framework core
- Swashbuckle
- TasksLibrary by Paprika
- Mapster

### Architecture
The API uses a monolith structure. It includes:

#### Infrastucture
This layer contains any infrastructure code and features. This serves as the bridge between the db layer and the domain layer.

#### Db
This layer contains data abstraction layer to connnect to an external database. This layer handles database operations.

#### Models
This layer contains all the business logic or domain of the application. It also include the Data transfer object used to interact with the users.

###  [Swagger Documentation](https://adenoteapi.azurewebsites.net/swagger/index.html)
