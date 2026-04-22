# My-Worksheet

<p align="center">
  <img src="MyWorksheet.Website/Client/wwwroot/Icons/my-worksheetIcon.svg" alt="My-Worksheet" width="220" />
</p>

My-Worksheet is a free, open-source time tracking and project management tool built for freelancers, the self-employed, and small companies. The idea is simple: you need to know where your time goes, what to bill, and how your projects are doing — without depending on some third-party SaaS subscription you don't control.

You host it yourself. Your data stays yours.

> [!WARNING]
> My-Worksheet as a public FOSS project has just been open sourced from multiple years of running just for myself. There might be start issues i forgotten. For all intents and purposes treat this as an Beta software. I am aware of the current start issues and i am working on it to make my-worksheet info a fully selfhosted application.


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
    image: ghcr.io/jpvenson/my-worksheet:main
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

### appsettings.json for Docker

When running with Docker, mount your own `appsettings.json` into the container.

This full template includes all current keys with inline comments. The snippet uses `jsonc` (JSON with comments) for readability in the README.

```jsonc
{
  // Application logging verbosity
  "Logging": {
    "LogLevel": {
      "Default": "Information", // Global fallback log level
      "Microsoft": "Warning", // ASP.NET Core framework logs
      "Microsoft.Hosting.Lifetime": "Information" // Startup/shutdown lifecycle logs
    }
  },

  // Database connection (from appsettings.Development.json)
  "ConnectionStrings": {
    "DefaultConnection": "User ID=postgres;Password=postgres;Host=db;Port=5432;Database=MyWorksheet;" // PostgreSQL connection string
  },

  // Hostname filter. Set to your real domain in production.
  "AllowedHosts": "myworksheet.example.com",

  // Internal transformation metadata used by the app/runtime
  "transformation": {
    "state": "debug", // Example: debug, staging, production
    "realm": "test", // Environment/realm marker
    "version": "beta" // App or rollout channel marker
  },

  // JWT token configuration for auth
  "TokenSettings": {
    "Issuer": "myworksheet.example.com", // Public issuer/host users access
    "Audience": "API", // Token audience expected by API
    "Key": "replace-with-a-long-random-secret" // Change this to a long random secret
  },

  // Mail settings (note: key names are part of current config contract)
  "mail": {
    "recive": {
      "mainMailAddress": "noreply@example.com" // Default inbound address
    },
    "send": {
      "realm": "smtp.example.com", // SMTP host name
      "port": 587, // SMTP port
      "sender": "noreply@example.com", // From address
      "username": "smtp-user", // SMTP user
      "password": "smtp-password", // SMTP password/secret
      "dns": true // Enable domain checks for sending
    }
  },

  // Maintenance mode message templates
  "maintainace": {
    "templates": {
      "scheduled": "", // Template/message for planned maintenance
      "unScheduled": "" // Template/message for unplanned downtime
    }
  },

  // Main server options
  "server": {
    "realm": {
      "primaryRealm": "https://www.my-worksheet.com" // Primary realm URL used by the server
    },
    "storage": {
      "default": {
        "path": "/opt/myworksheet" // Default base storage path
      },
      "sql": {
        "maxReportSize": 25600 // Max generated report payload size
      },
      "file": {
        "token": {
          "maxttl": 1200 // File token TTL in seconds
        },
        "location": "/file-store/", // Persistent storage mount path
        "temp": "/tmp/mw-temp/" // Temporary file path (must be writable)
      }
    },
    "user": {
      "create": {
        "defaultRegion": "00000000-0000-0000-0000-000000000000", // Default region id for new users
        "defaultRoles": "Kunde,WorksheetActionsUser,OrganisationAdmin,SettingsUsers,ProjectManager,WorksheetUser,WorksheetAdmin" // Comma-separated default roles
      }
    },
    "external": {
      "user": {
        "callthreshold": 20 // Threshold for external user related checks
      },
      "uriRules": {
        "allowLoopback": false, // Allow calls to 127.0.0.1 / localhost
        "allowSameNetwork": false, // Allow calls to private LAN targets
        "allowRelative": false // Allow relative target URLs
      }
    },
    "featureSwitch": {
      "registration": {
        "enabled": false // Public self-registration
      },
      "testRegistration": {
        "enabled": false // Test-user registration
      }
    }
  },

  // Google reCAPTCHA keys (from appsettings.Development.json)
  "google": {
    "recaptcha": {
      "keys": {
        "public": "6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI", // Site key
        "secret": "6LeIxAcTAAAAAGG-vFI1TnRWxMZNFuojJ4WifJWe" // Secret key
      }
    }
  },

  // CDN fallback cache
  "Cdn": {
    "Cache": {
      "Path": "/Content/CdnCache/", // Cache folder
      "Enumeration": [] // Optional predefined CDN entries
    }
  },

  // Activity/monitoring settings
  "activitys": {
    "trackerStillRunning": {
      "fallbackMwt": 60 // Minutes before long-running tracker warning
    }
  },

  // Feature switches for optional "is it" integration
  "is": {
    "it": {
      "free": true // Example feature flag
    }
  },

  // Default quotas applied to new accounts/users
  "account": {
    "quota": {
      "default": {
        "Project": -1, // -1 means unlimited
        "Worksheet": -1, // -1 means unlimited
        "LocalFile": 50000, // File quota (project-specific unit)
        "Webhooks": 20, // Max webhook count
        "ConcurrentReports": 3 // Max concurrent report jobs
      }
    }
  }
}
```

---

## AI Usage

This project has been created long before AI was a mainstream thing back in 2012 so both the predecesor written in AngularJS and its Blazor Rewrite are the complete result of my work. However certain rewrites like the move from my own ORM to EFCore and certain other modules have been rewritten with the help of Anthropic Claude. 

---

## License

See [LICENSE](LICENSE).
