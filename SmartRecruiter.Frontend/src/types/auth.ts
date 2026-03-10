export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    email: string;
    password: string;
}

export interface AuthResult{
    succeeded: boolean;
    errors: string[];
    token: string;
}