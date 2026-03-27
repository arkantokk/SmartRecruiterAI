import {apiClient} from "../../api/axiosInstance.ts";
import type {JobVacancy} from "../../models/JobVacancy/JobVacancy.ts";
import type {JobVacancyResponse} from "../../models/JobVacancy/JobVacancyResponse.ts";

export const vacanciesService = {
    postVacancy: async (jobVacancy: JobVacancy) => {
        const response = await apiClient.post<string>('/JobVacancies', jobVacancy);
        return response.data;
    },
    getVacancies: async () => {
        const response = await apiClient.get<JobVacancyResponse[]>('/JobVacancies');
        return response.data;
    }
}