import {apiClient} from "../../api/axiosInstance.ts";
import type {ApiResponse} from "../../models/ApiResponse/ConnectGmailReponse.ts";
import type {IntegrationStatusResponse} from "../../models/ApiResponse/StatusResponse.ts"

export const integrationService = {
    connectGmail: async () => {
        try {
            const response = await apiClient.get<ApiResponse>('/Integrations/google/connect');
            const url = response.data.url;
            if (url) {
                window.location.href = url;
            } else {
                console.error("Could not connect to Google");
            }
        } catch (err) {
            console.error("Could not connect to Google");
        }
    },
    status: async (): Promise<IntegrationStatusResponse> => {
        const response = await apiClient.get<IntegrationStatusResponse>('/Integrations/google/status');
        return response.data;

    }
}