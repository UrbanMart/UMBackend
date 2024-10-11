# UrbanMart Backend

The UrbanMart Backend is developed using .NET Core 8 and serves as the API for the UrbanMart e-commerce platform. It utilizes MongoDB for data storage and includes support for both production and development environments. The backend is deployed on Azure and runs on an IIS server, with cron jobs for scheduled tasks.

## Features

- **API Documentation:** Access the API documentation via Swagger [here](https://urbanmartapi-bqdwczd9gqcrdkc7.eastus-01.azurewebsites.net/swagger/index.html).
- **Production and Development Environments:** Supports multiple environments for testing and deployment.
- **MongoDB Integration:** Uses MongoDB for data storage and management.
- **Cron Jobs:** Implements scheduled tasks for various backend operations.

## Tech Stack

- **Framework:** .NET Core 8
- **Database:** MongoDB
- **Hosting:** Azure
- **Web Server:** IIS

## Links

- **Frontend Admin Portal:** [UrbanMart Admin Portal](https://urbanmart-dev.netlify.app/)

## Installation and Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/urbanmart-backend.git
   cd urbanmart-backend
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Publish the application:
   ```bash
   dotnet publish --configuration Release --output ./publish
   ```

5. Run the application locally:
   ```bash
   dotnet run
   ```

6. Configure the application settings for production or development.

## Contributing

Contributions are welcome! Feel free to fork the repository and submit pull requests.

## License

This project is licensed under the MIT License.
