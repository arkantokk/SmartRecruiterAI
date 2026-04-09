import { apiClient } from "../../api/axiosInstance";
import type { authFormValues } from "./authSchema";
import type {LoginResponse, LogoutResponse, RegisterResponse, MeRequestResponse} from "../../models/ApiResponse/AuthResponse.ts";

export const authService = {
    login: async (data: authFormValues): Promise<LoginResponse> => {
        const response = await apiClient.post("Auth/login", data, {withCredentials: true});
        if (response.data && response.data.token) {
            localStorage.setItem("token", response.data.token);
        }
        return response.data;
    },
    register: async (data: authFormValues): Promise<RegisterResponse> => {
        const response = await apiClient.post("Auth/register", data, {withCredentials: true});
        return response.data;
    },
    logout: async (): Promise<LogoutResponse> => {
        const response = await apiClient.post("Auth/logout", {}, {withCredentials: true});
        return response.data;
    },
    me: async (): Promise<MeRequestResponse> => {
        const response = await apiClient.get("Auth/me", {withCredentials: true});
        return response.data;
    }
}