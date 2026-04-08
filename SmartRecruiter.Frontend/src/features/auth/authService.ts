import { apiClient } from "../../api/axiosInstance";
import type { authFormValues } from "./authSchema";

export const authService = {
    login: async (data: authFormValues) => {
        const response = await apiClient.post("Auth/login", data, {withCredentials: true});
        if (response.data && response.data.token) {
            localStorage.setItem("token", response.data.token);
        }
        return response.data;
    },
    register: async (data: authFormValues) => {
        const response = await apiClient.post("Auth/register", data, {withCredentials: true});
        return response.data;
    },
    logout: async () => {
        const response = await apiClient.post("Auth/logout", {}, {withCredentials: true});
        return response.data;
    },
    me: async () => {
        const response = await apiClient.get("Auth/me", {withCredentials: true});
        return response.data;
    }
}