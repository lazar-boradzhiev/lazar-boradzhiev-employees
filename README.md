# Getting Started

## Prerequisites

Before starting, ensure you have the following installed:

- [Docker](https://www.docker.com/) (for running with Docker)
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) (for running with the .NET CLI)
- A modern web browser (for accessing the API and Swagger UI)

---

## Running the Project

### Option 1: Using Docker

1. **Build and Start the Docker Container**
   ```bash
   docker-compose up
   ```
   This will:
   - Build the Docker image for the `overlap-api` service.
   - Start the container and expose the API on:
     - `http://localhost:5000` (HTTP)
     - `https://localhost:5001` (HTTPS)

2. **Access the API**
   - Open your browser and navigate to:
     - Swagger UI: [http://localhost:5000/index.html](http://localhost:5000/index.html)
     - Or use an HTTP client like `curl` or Postman to interact with the API.

---

### Option 2: Using the .NET CLI

1. **Restore Dependencies**
   Navigate to the `api/src/Employees.Api` directory and run:
   ```bash
   dotnet restore
   ```

2. **Build the Project**
   ```bash
   dotnet build -c Release
   ```

3. **Run the API**
   ```bash
   dotnet run
   ```
   By default, the API will start on:
   - `http://localhost:5000` (HTTP)
   - `https://localhost:5001` (HTTPS)

4. **Access the API**
   - Open your browser and navigate to:
     - Swagger UI: [http://localhost:5000/index.html](http://localhost:5000/index.html)
     - Or use an HTTP client like `curl` or Postman to interact with the API.

---

## Running Tests

To ensure the project is working as expected, you can run the tests using the .NET CLI:

1. **Navigate to the Test Project Directory**
   Go to the directory containing the test project, for example:
   ```bash
   cd api/tests/Employees.Api.Tests
   ```

2. **Run the Tests**
   Execute the following command:
   ```bash
   dotnet test
   ```
   This will:
   - Build the test project.
   - Run all the tests and display the results in the terminal.

3. **View Test Results**
   - If the tests pass, you will see a summary indicating success.
   - If any tests fail, review the output for details on the failures.

---

## Notes

- If you encounter issues with HTTPS in development, you can bypass SSL verification using the `-k` flag in `curl` or configure your HTTP client to ignore SSL errors.
- Ensure that ports `5000` and `5001` are not in use by other applications.