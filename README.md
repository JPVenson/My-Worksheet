# My-Worksheet

<p align="center">
  <img src="MyWorksheet.Website/Client/wwwroot/Icons/my-worksheetIcon.svg" alt="My-Worksheet" width="220" />
</p>

My-Worksheet is a free, open-source time tracking and project management tool built for freelancers, the self-employed, and small companies. The idea is simple: you need to know where your time goes, what to bill, and how your projects are doing — without depending on some third-party SaaS subscription you don't control.

You host it yourself. Your data stays yours.

---

## Background

This started over 13 years ago as an AngularJS 1.0 application, born out of a personal need for something flexible and self-hostable for tracking billable hours. Over the years it grew well beyond a simple timer application, and eventually the whole thing was rewritten as a modern [Blazor WebAssembly](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) application on top of ASP.NET Core — keeping the same goals: extendable, self-hosted, and actually useful for day-to-day freelance work.

---

## What It Does

### Time Tracking
The core of the app. You create worksheets tied to projects and log your time as items with start/end times, comments, billing rates, and statuses. You can run multiple live timers at once, stop them when you're done, and everything flows into your worksheets automatically.

### Projects & Clients
Projects connect to organisations (your clients), carry their own billing rates and tax rates, and can have a team of users assigned to them. Each project can have a budget — time-based, monetary, or both — with optional deadlines and overbooking flags so you always know when you're heading into trouble.

### Billing & Invoicing
Billing rates are flexible per project and per charge type. Payment conditions are configurable. Invoices and reports are generated from templates using [Morestachio](https://github.com/JPVenson/morestachio) — meaning you can build your own document layouts without touching any code. Sequential invoice numbering via configurable number ranges is included. PayPal integration is available for payment tracking.

### Reporting
The reporting engine is template-driven. You write a Morestachio template, point it at a data source (work items, projects, aggregated stats), and get back whatever output format you need — HTML, PDF, DOCX (Soon), or anything else your templates produce. Aggregate database views pre-crunch the numbers for per-day, per-project, and per-worksheet summaries. You can also share reports publicly via a share key, no login required on the recipient's end.

### Dashboard
A customisable grid dashboard with drag-and-drop widgets. Widgets cover active worksheets, running timers, today's workload, earnings, project breakdowns, and more. Each user gets their own layout.

### Workflows & Automation
Worksheets can follow a workflow — a configurable state machine that defines which statuses are valid and what transitions are allowed. Outgoing webhooks with signed secrets can fire on worksheet events, so you can wire things up to external systems when statuses change.

### Overtime Tracking
Time booked on specific projects can be routed to an overtime account. Transactions accrue over time, giving you a running total of overtime per user.

### User & Organisation Management
Role-based access control, JWT authentication with email verification, user invitations, and an association model so users can be linked to each other and to organisations. Registration can be open, invite-only, or completely closed depending on how you run your instance.

### Multi-Language Support
The entire interface is backed by a database-driven translation system. Languages live in the database, not in compiled resource files, so they can be updated at runtime without a redeploy.

### Storage & Mail
Remote/cloud storage backends are configurable. SMTP and IMAP mail accounts can be connected for outgoing notifications and incoming mail handling. Mail blacklists and domain validation are built in. Storage of all invoices and really anything My-Worksheet produces can be stored on a number of differnt Providers.

### Admin Tools
Server log viewer, scheduled background task monitor, activity audit log, maintenance mode, and per-user quota management. Everything you need to run a production instance without flying blind.

---

## Self-Hosting

My-Worksheet is designed to be self-hosted via Docker and with an PostgreSQL database. 

Example Docker-Compose.yml file:

```yml

services:
  my-worksheet:
    image: ghcs.io/myworksheet:latest
    hostname: my-worksheet
    restart: unless-stopped
    networks:
      - my_worksheet_network
    ports:
      - 80:8080
    volumes:
      - ./appsettings.json:/app/appsettings.json
  my-worksheet-db:
    image: postgres:14.3
    hostname: my-worksheet-db
    volumes:
      - ../../data/myWorksheet/database:/var/lib/postgresql/data
    environment:
      POSTGRES_PASSWORD: postgres
      POSTGRES_USER: postgres
      POSTGRES_DB: MyWorksheet
    networks:
      - my_worksheet_network
networks:
  my_worksheet_network:
    name: "compose_myworksheet_network"

```

---

## AI Usage

This project has been created long before AI was a mainstream thing back in 2012 so both the predecesor written in AngularJS and its Blazor Rewrite are the complete result of my work. However certain rewrites like the move from my own ORM to EFCore and certain other modules have been rewritten with the help of Anthropic Claude. 

---

## License

See [LICENSE](LICENSE).
