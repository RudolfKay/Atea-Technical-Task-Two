# Atea Task 2

Steps to run:

1) Start the program with "dotnet run" (or run it in debug mode).
2) Navigate to project folder "weather-app" within the terminal.
3) Install necessary packages with "npm install"
4) Start the front-end app with "npm start".
5) Wait a few minutes for data to gather.
6) If not open already, navigate to "localhost:3000" on your browser of choice.
7) Enjoy!

The graphs displaying weather min/max temps and timestamps by location refresh every minute (both BE, and FE update requests) to provide the most current data from OpenWeatherMap.org

What would I improve next:
1. Refactor job/service to be handled by an azure function.
2. Implement unit tests using xUnit/nUnit (attempted to add test project but cut short by time and dependency issues).
3. If not refactor to azure function, then implement auto-retry functionality for the polling job.
4. UI, perhaps add more data (humidity and current temperature data already available for the front-end).
5. Add more validation.
6. Use custom errors.
7. Add Serilog or equivalent.
8. Cloud storage. 🌈
