export interface LoginResponse{
    token: string;
}

export interface RegisterResponse{
    token: string;
}

export interface LogoutResponse{
    message: string;
}

export interface RefreshTokenResponse{
    token: string;
}

export interface MeRequestResponse{
    user: string;
}