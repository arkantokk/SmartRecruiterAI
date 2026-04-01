import { apiClient } from "../../api/axiosInstance.ts";

export const CandidateStatusMap = {
    Applied: 'Applied',
    Screening: 'Screening',
    ManualReview: 'ManualReview',
    Interview: 'Interview',
    Offer: 'Offer',
    Rejected: 'Rejected',
    Hired: 'Hired'
} as const;

export interface Candidate {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    score: number;
    resumeUrl?: string;
    summary: string;
    skills: string[];
    status: string;
}

export const candidatesService = {
    getCandidates: async () => {
        const response = await apiClient.get<Candidate[]>("Candidates");
        return response.data;
    },

    getCandidatesByVacancyId: async (vacancyId: string) => {
        const response = await apiClient.get<Candidate[]>(`JobVacancies/${vacancyId}/candidates`);
        return response.data;
    },

    updateCandidateStatus: async (data: { id: string; status: number }) => {
        const response = await apiClient.patch(`Candidates/${data.id}/status`, data.status, {
            headers: { 'Content-Type': 'application/json' }
        });
        return response.data;
    }
};