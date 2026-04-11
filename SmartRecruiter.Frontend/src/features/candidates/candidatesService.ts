import {apiClient} from "../../api/axiosInstance.ts";

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

export interface PagedResponse<T>{
    items: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
}

export const candidatesService = {
    getCandidates: async (pageNumber: number, pageSize: number) => {
        const response = await apiClient.get<PagedResponse<Candidate>>("Candidates", {params: {pageNumber, pageSize}});
        return response.data;
    },

    getCandidatesByVacancyId: async (
        vacancyId: string,
        pageNumber: number,
        pageSize: number,
        search?: string,
        sort?: string,
        tab?: string,
        archiveFilter?: string
    ) => {
        const response = await apiClient.get(`JobVacancies/${vacancyId}/candidates`, {
            params: {
                pageNumber,
                pageSize,
                search: search || undefined,
                sort: sort || undefined,
                tab,
                archiveFilter
            }
        });
        return response.data;
    },

    updateCandidateStatus: async (data: { id: string; status: number }) => {
        const response = await apiClient.patch(`Candidates/${data.id}/status`, data.status, {
            headers: {'Content-Type': 'application/json'}
        });
        return response.data;
    }
};