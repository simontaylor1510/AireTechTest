# Patient Appointment Network Data Application

See https://github.com/airelogic/tech-test-portal/blob/main/Patient-Appointment-Backend/README.md for details of requirements

## Running locally

Install the Aspire CLI tool:

```powershell
irm https://aspire.dev/install.ps1 | iex
```

Ensure that Docker Desktop is installed and running.

Run the application:

```powershell
aspire run
```

When the application is running click the Dashboard link to view the dashboard in the browser.

![img.png](img.png)

The database will start up in a Docker container and once it is ready the API will be available by clicking on the `Scalar` link in the Dashboard.

You can then interact with the API using the Scalar UI.