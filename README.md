# 🧠 SmartRecruiterAI

[](https://www.google.com/search?q=https://github.com/your-username/SmartRecruiterAI/actions)
[](https://azure.microsoft.com/)

SmartRecruiterAI is an AI-powered applicant tracking system (ATS) designed to streamline the hiring process. It automatically evaluates candidate resumes against job vacancy requirements, providing recruiters with instant matching scores and actionable insights.

## 🚀 Features

  * **AI Resume Evaluation:** Automatically parses candidate resumes and generates a match score (0-100) based on job vacancy descriptions.
  * **Secure Email Integration & Automation:** Users securely connect their email accounts, triggering automated background workers to process incoming communications and sync candidate data asynchronously.
  * **Secure Resume Storage:** Candidate CVs and related documents are securely uploaded and stored using Azure Blob Storage for high availability and compliance.
  * **Candidate Pipeline Management:** Track candidates through stages (Applied, Interviewing, Hired, Rejected) using a streamlined Kanban-style interface.
  * **Secure Authentication:** JWT-based custom email/password login alongside Google OAuth 2.0 integration.
  * **Real-time Analytics Dashboard:** Visual indicators (like color-coded score circles) to quickly assess candidate quality.

-----

## 🛠 Tech Stack

### Frontend

  * **Framework:** React 18 with TypeScript
  * **Build Tool:** Vite
  * **Data Fetching & Caching:** TanStack Query (React Query)
  * **State Management:** Zustand (Client state & Auth)
  * **Styling:** Tailwind CSS
  * **Form Handling:** React Hook Form + Zod (Validation)
  * **Auth:** Google OAuth (`@react-oauth/google`)

### Backend

  * **Framework:** .NET 8 (C\#) Web API
  * **Architecture:** Clean Architecture (Domain, Application, Infrastructure, API layers)
  * **Asynchronous Processing:** .NET BackgroundService (Background Workers)
  * **Storage:** Azure Blob Storage (CV & Document Storage)
  * **Patterns:** Repository Pattern, Dependency Injection (DI)
  * **AI Integration:** Large Language Model evaluation service (Prompt Engineering)

### Testing & DevOps

  * **Backend Testing:** xUnit, Moq (AAA pattern)
  * **Frontend Testing:** Vitest, React Testing Library, User-Event
  * **CI/CD:** GitHub Actions (Automated testing and deployment gates)
  * **Cloud Hosting:** Azure Web Apps

-----

## 🔄 Architecture & Data Flow

1.  **User Registration & Authentication:** Users register and authenticate via the React frontend. The `.NET API` validates the request and issues a secure JWT token, which is stored securely.
2.  **Secure Email Integration:** Post-registration, the recruiter connects their email account via a secure integration flow.
3.  **Background Worker Processing:** Once integrated, a .NET Background Worker continuously polls and processes email communications asynchronously, preventing UI blocking and ensuring real-time syncing of candidate interactions.
4.  **Job Posting & Application:** Recruiters create a `JobVacancy` with specific requirements. Candidates submit their details and upload their CVs. The original CV files are securely stored in **Azure Blob Storage**, while the extracted `ResumeText` is processed by the system.
5.  **AI Evaluation Node:** \* The backend triggers the `IAiService`.
      * The service compares the extracted `ResumeText` against the `JobVacancy` context.
      * An `AiAnalysisResult` is generated, returning a score and a list of matching/missing skills.
6.  **Data Synchronization:** The frontend utilizes **TanStack Query** to fetch, cache, and synchronize the paginated list of candidates from the backend. This ensures the UI is always up-to-date with the latest AI evaluation scores and background worker updates without unnecessary network requests.
7.  **Dynamic Rendering:** The frontend dynamically renders visual indicators (e.g., `ScoreCircle` components mapping to Tailwind color classes based on the AI score) using the cached data.

-----

## 🧪 Testing Suite

This project maintains strict quality gates using automated testing for both the backend logic and frontend UI components.

**Run Backend Tests (xUnit)**

```bash
dotnet test SmartRecruiterAI.sln
```

  * **Coverage:** Business logic, `CandidateService`, Entity state changes, and mocked AI service interactions.

**Run Frontend Tests (Vitest)**

```bash
cd SmartRecruiter.Frontend
npm run test
```

  * **Coverage:** Component rendering, dynamic Tailwind CSS class assignments, form validation states, and mocked Zustand store interactions using `@testing-library/user-event`.

-----

## 🚀 CI/CD Pipeline

The project utilizes **GitHub Actions** for continuous integration and continuous deployment, ensuring zero downtime and protecting the main branch from broken code.

  * **Job 1: Build & Validate:** Triggered on PRs and merges to `main`. Restores dependencies and runs both `.NET` and `Vitest` test suites.
  * **Job 2: Deploy to Azure:** Triggered *only* if Job 1 passes successfully on the `main` branch. Compiles the Vite frontend and publishes the .NET backend to an Azure Web App environment.

-----

## 💻 Local Setup

### Prerequisites

  * Node.js v20+
  * .NET 8 SDK
  * Azure/Local SQL Server
  * Azure Storage Account (for Blob Storage)

### Installation Steps

1.  **Clone the repository**

    ```bash
    git clone https://github.com/your-username/SmartRecruiterAI.git
    cd SmartRecruiterAI
    ```

2.  **Setup Backend**

    ```bash
    dotnet restore
    # Update appsettings.json with your DB connection string, AI keys, Email Integration secrets, and Azure Blob Storage connection string
    dotnet run --project src/SmartRecruiter.API
    ```

3.  **Setup Frontend**

    ```bash
    cd SmartRecruiter.Frontend
    npm install
    # Create a .env file with VITE_API_BASE_URL and VITE_GOOGLE_CLIENT_ID
    npm run dev
    ```
