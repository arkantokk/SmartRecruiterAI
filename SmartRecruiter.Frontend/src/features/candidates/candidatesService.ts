import { apiClient } from "../../api/axiosInstance.ts";

export interface Candidate {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    status: string;
}

export const candidatesService = {
    getCandidates: async () => {
        const response = await apiClient.get<Candidate[]>("Candidates");
        return response.data;
    }
}