# Patient Management System - React Frontend

## Phase 5: React Frontend Setup & Navigation

This is the frontend application for the Patient Management System, built with React 18, TypeScript, and Material-UI.

## Technology Stack

- **React 18.2** - UI library with hooks
- **TypeScript 6.0** - Strict mode enabled for type safety
- **Material-UI v9** - Component library
- **React Router v7** - Client-side routing
- **Axios** - HTTP client with interceptors
- **Vite** - Fast build tool
- **Vitest** - Unit testing framework

## Project Structure

```
client/
├── src/
│   ├── components/        # Reusable UI components
│   │   ├── Layout.tsx            # Main layout with navigation
│   │   └── ProtectedRoute.tsx    # Route guard for authentication
│   ├── contexts/          # React contexts
│   │   └── AuthContext.tsx       # Authentication state management
│   ├── pages/             # Page components
│   │   ├── Login.tsx             # Login page
│   │   ├── Dashboard.tsx         # Dashboard (home)
│   │   ├── Patients.tsx          # Patients page (Phase 6)
│   │   ├── Appointments.tsx      # Appointments page (Phase 7)
│   │   ├── Consultations.tsx     # Consultations page (Phase 8)
│   │   └── History.tsx           # History page (Phase 9)
│   ├── services/          # API and business logic
│   │   ├── api.service.ts        # Axios instance with interceptors
│   │   └── auth.service.ts       # Authentication service
│   ├── types/             # TypeScript type definitions
│   │   ├── api.types.ts          # API-related types
│   │   └── auth.types.ts         # Auth-related types
│   ├── theme/             # Material-UI theming
│   │   └── theme.ts              # Theme configuration
│   ├── config/            # Configuration constants
│   │   └── constants.ts          # App constants and routes
│   ├── test/              # Test files
│   │   ├── setup.ts              # Test setup
│   │   ├── Login.test.tsx        # Login component tests
│   │   ├── AuthContext.test.tsx  # Auth context tests
│   │   └── api.service.test.ts   # API service tests
│   ├── App.tsx            # Main app component
│   ├── main.tsx           # Application entry point
│   └── index.css          # Global styles
├── .env                   # Environment variables
├── .env.development       # Development environment
├── .env.production        # Production environment
├── package.json           # Dependencies and scripts
├── tsconfig.json          # TypeScript configuration
├── tsconfig.app.json      # App-specific TypeScript config
└── vitest.config.ts       # Test configuration
```

## Features Implemented (Phase 5)

### 1. TypeScript Strict Mode ✅
- All strict type checking enabled
- No implicit any
- Strict null checks
- Type-safe imports with `verbatimModuleSyntax`

### 2. Authentication Flow ✅
- Login page with form validation
- JWT token storage in localStorage
- Auth context for global state management
- Protected routes requiring authentication
- Automatic redirect to login for unauthenticated users
- Token-based session management (8-hour expiry)

### 3. API Client Service ✅
- Axios instance with base URL configuration
- Request interceptor for adding auth tokens
- Response interceptor for error handling:
  - 401: Automatic logout and redirect to login
  - 403: Forbidden access handling
  - 500+: Server error handling
- Helper methods for GET, POST, PUT, PATCH, DELETE

### 4. Layout & Navigation ✅
- Responsive layout with drawer navigation
- Material-UI AppBar and Drawer components
- Mobile-friendly hamburger menu
- Navigation items:
  - Patients
  - Appointments
  - Consultations
  - History
- Logout functionality in header and sidebar

### 5. Routing ✅
- React Router v7 with protected routes
- Routes configured:
  - `/login` - Login page (public)
  - `/` - Dashboard (protected)
  - `/patients` - Patients page (protected, Phase 6)
  - `/appointments` - Appointments page (protected, Phase 7)
  - `/consultations` - Consultations page (protected, Phase 8)
  - `/history` - History page (protected, Phase 9)

### 6. Testing ✅
- Vitest configured with jsdom
- Testing Library for React components
- Mock API services for testing
- Test coverage:
  - Login component (4 tests)
  - Auth context (3 tests)
  - API service (3 tests)
- **All 10 tests passing ✓**

## Environment Variables

Create a `.env` file in the client directory:

```env
VITE_API_BASE_URL=http://localhost:5000/api/v1
```

## Available Scripts

```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build

# Run tests
npm test

# Run tests with UI
npm run test:ui

# Run tests with coverage
npm run test:coverage

# Preview production build
npm run preview

# Lint code
npm run lint
```

## Development Server

Start the development server:

```bash
npm run dev
```

The application will be available at `http://localhost:5173/`

## Default Credentials

Since this is a single-user system with hardcoded credentials:

- **Username:** `admin`
- **Password:** `Admin@123`

*Note: These credentials are configured in the backend API*

## API Integration

The frontend communicates with the backend API at:
- **Development:** `http://localhost:5000/api/v1`
- **Production:** Configured via `VITE_API_BASE_URL` environment variable

All API calls automatically include the JWT token in the `Authorization` header.

## State Management Decision (Step 28)

**Decision:** Using React Context API

**Rationale:**
- Phase 5 only requires authentication state management
- React Context is sufficient for single-user application
- Simpler setup and maintenance compared to Redux
- Can be upgraded to Redux/Zustand in later phases if needed

## Testing

Run tests with:

```bash
npm test
```

Test results:
- **Test Files:** 3 passed
- **Tests:** 10 passed
- **Duration:** ~10s

## Build

Build the application for production:

```bash
npm run build
```

Build output:
- TypeScript compilation successful
- Bundle size: ~519 KB (minified)
- Output directory: `dist/`

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Dependencies

### Production Dependencies
- `react` ^19.2.6
- `react-dom` ^19.2.6
- `react-router-dom` ^7.15.1
- `@mui/material` ^9.0.1
- `@mui/icons-material` ^9.0.1
- `@emotion/react` ^11.14.0
- `@emotion/styled` ^11.14.1
- `axios` ^1.16.1

### Development Dependencies
- `typescript` ~6.0.2
- `vite` ^8.0.12
- `vitest` ^4.1.6
- `@testing-library/react` ^16.3.2
- `@testing-library/jest-dom` ^6.9.1
- `@testing-library/user-event` ^14.6.1
- `jsdom` ^29.1.1

## Next Phases

### Phase 6: Patient Management (To be implemented)
- Patient search functionality
- Patient registration form
- Patient profile view/edit
- Recent patients quick access
- API integration

### Phase 7: Appointment Management
- Appointment scheduling
- Daily appointment list
- Status updates
- Filtering and sorting

### Phase 8: Consultation Workflow
- Vitals capture
- Complaints/symptoms entry
- Diagnosis documentation
- Prescription entry

### Phase 9: History & Export
- Patient history timeline
- Visit details
- CSV/PDF export
- Prescription printing

## Assumptions & Dependencies

1. **Backend API** must be running on `http://localhost:5000/api/v1`
2. **Authentication endpoint** `/auth/login` must be available
3. **JWT tokens** expire after 8 hours (as per backend configuration)
4. **Single-user system** - no user registration or multi-user support
5. **Material-UI v9** - Using latest version with updated API

## Known Issues & Limitations

1. **Phase 5 Scope:** Only authentication and navigation implemented
2. **Placeholder Pages:** Patients, Appointments, Consultations, History pages show placeholders
3. **Dashboard Metrics:** Dashboard shows static "0" values (will be dynamic in later phases)
4. **No Error Boundary:** Global error boundary not yet implemented
5. **No Offline Support:** Requires active API connection

## Phase 5 Completion Status

✅ Step 23: Initialize React 18 project with TypeScript (strict mode)  
✅ Step 24: Create base layout and navigation structure  
✅ Step 25: Set up API client service layer (Axios with interceptors)  
✅ Step 26: Implement authentication flow (login, token storage, logout)  
✅ Step 27: Create context/state for user session  
✅ Step 28: Finalize state management approach (Context vs Redux)

**Phase 5 Status:** ✅ COMPLETE

All deliverables implemented, tested, and validated. Ready for Phase 6 implementation.
