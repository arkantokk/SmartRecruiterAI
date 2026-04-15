import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { LoginForm } from './LoginForm';
import { useAuthStore } from '../../../store/authStore';
import { authService } from '../authService';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { MemoryRouter } from 'react-router-dom';

vi.mock('../../../store/authStore', () => ({
    useAuthStore: vi.fn(),
}));

vi.mock('../authService', () => ({
    authService: {
        login: vi.fn(),
        googleLogin: vi.fn()
    }
}));

const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

describe('LoginForm Component', () => {
    beforeEach(() => {
        vi.clearAllMocks();

        (useAuthStore as any).mockReturnValue({
            loginSuccess: vi.fn(),
            isLoading: false,
            error: null,
        });

        (authService.login as any).mockResolvedValue({ token: 'fake-jwt-token' });
    });

    it('should render email and password inputs and a submit button', () => {
        render(
            <MemoryRouter>
                <LoginForm />
            </MemoryRouter>
        );

        expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
        expect(screen.getByRole('button', { name: /login/i })).toBeInTheDocument();
    });



    it('should call authService.login and navigate to home on successful submission', async () => {
        const user = userEvent.setup();
        render(
            <MemoryRouter>
                <LoginForm />
            </MemoryRouter>
        );

        const emailInput = screen.getByLabelText(/email/i);
        const passwordInput = screen.getByLabelText(/password/i);
        const submitButton = screen.getByRole('button', { name: /login/i });

        await user.type(emailInput, 'test@example.com');
        await user.type(passwordInput, 'Password123!');
        await user.click(submitButton);

        await waitFor(() => {
            expect(authService.login).toHaveBeenCalledWith({
                email: 'test@example.com',
                password: 'Password123!',
            });
        });

        expect(mockNavigate).toHaveBeenCalledWith('/candidates');
    });

    it('should display an error message if the login fails via the store', async () => {
        (useAuthStore as any).mockReturnValue({
            loginSuccess: vi.fn(),
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
            loginSuccess: vi.fn(),
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