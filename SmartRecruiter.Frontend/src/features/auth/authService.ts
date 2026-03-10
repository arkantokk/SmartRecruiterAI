import { apiClient } from "../../api/axiosInstance";
import type { authFormValues } from "./authSchema";

export const authService = {
    login: async (data: authFormValues) => {
        const response = await apiClient.post("Auth/login", data);
        if (response.data && response.data.token) {
            localStorage.setItem("token", response.data.token);
        }
        return response.data;
    },
    register: async (data: authFormValues) => {
        const response = await apiClient.post("Auth/register", data);
        return response.data;
    }
}