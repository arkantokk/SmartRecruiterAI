import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { LoginForm } from './LoginForm';
import { useAuthStore } from '../../../store/authStore';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { MemoryRouter } from 'react-router-dom';

// 1. Mock the Auth Store
// We mock the whole module so we can control the 'login' function
vi.mock('../../../store/authStore', () => ({
    useAuthStore: vi.fn(),
}));

// 2. Mock Navigate (since the form redirects after login)
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

describe('LoginForm Component', () => {
    const mockLogin = vi.fn();

    beforeEach(() => {
        vi.clearAllMocks();
        // Set up the store mock to return our spy function
        (useAuthStore as any).mockReturnValue({
            login: mockLogin,
            isLoading: false,
            error: null,
        });
    });

    it('should render email and password inputs and a submit button', () => {
        render(
            <MemoryRouter>
                <LoginForm />
            </MemoryRouter>
        );

        expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
        expect(screen.getByRole('button', { name: /sign in/i })).toBeInTheDocument();
    });

    it('should show validation errors when fields are empty and submitted', async () => {
        render(
            <MemoryRouter>
                <LoginForm />
            </MemoryRouter>
        );

        const submitButton = screen.getByRole('button', { name: /sign in/i });
        await userEvent.click(submitButton);

        // Check for validation messages (based on your Zod/Hook Form schema)
        await waitFor(() => {
            expect(screen.getByText(/email is required/i)).toBeInTheDocument();
            expect(screen.getByText(/password is required/i)).toBeInTheDocument();
        });
    });

    it('should call login and navigate to home on successful submission', async () => {
        const user = userEvent.setup();
        render(
            <MemoryRouter>
                <LoginForm />
            </MemoryRouter>
        );

        const emailInput = screen.getByLabelText(/email/i);
        const passwordInput = screen.getByLabelText(/password/i);
        const submitButton = screen.getByRole('button', { name: /sign in/i });

        // Act: Type into inputs
        await user.type(emailInput, 'test@example.com');
        await user.type(passwordInput, 'Password123!');
        await user.click(submitButton);

        // Assert: Verify store was called with correct data
        await waitFor(() => {
            expect(mockLogin).toHaveBeenCalledWith({
                email: 'test@example.com',
                password: 'Password123!',
            });
        });

        // Assert: Verify navigation happened
        expect(mockNavigate).toHaveBeenCalledWith('/');
    });

    it('should display an error message if the login fails', async () => {
        // Override the mock for this specific test
        (useAuthStore as any).mockReturnValue({
            login: mockLogin,
            isLoading: false,
            error: 'Invalid credentials',
        });

        render(
            <MemoryRouter>
                <LoginForm />
            </MemoryRouter>
        );

        expect(screen.getByText(/invalid credentials/i)).toBeInTheDocument();
    });

    it('should disable the button and show loading state while submitting', () => {
        (useAuthStore as any).mockReturnValue({
            login: mockLogin,
            isLoading: true,
            error: null,
        });

        render(
            <MemoryRouter>
                <LoginForm />
            </MemoryRouter>
        );

        const button = screen.getByRole('button');
        expect(button).toBeDisabled();
        expect(button).toHaveTextContent(/signing in/i);
    });
});