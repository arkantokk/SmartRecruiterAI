import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { vacanciesService } from "../vacanciesService.ts";
import { candidatesService } from "../../candidates/candidatesService.ts";

export const useVacancy = (id: string | null | undefined) => {
    return useQuery({
        queryKey: ["vacancy", id],
        queryFn: () => vacanciesService.getVacancyById(id as string),
        staleTime: 1000 * 60 * 5,
        retry: 1,
        enabled: !!id
    });
};

export const useVacancyCandidates = (vacancyId: string | null | undefined) => {
    return useQuery({
        queryKey: ["candidates", "vacancy", vacancyId],
        queryFn: () => candidatesService.getCandidatesByVacancyId(vacancyId as string),
        staleTime: 1000 * 60 * 5,
        retry: 1,
        enabled: !!vacancyId
    });
};

export const useUpdateCandidateStatus = (vacancyId: string | null | undefined) => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: candidatesService.updateCandidateStatus,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["candidates", "vacancy", vacancyId] });
        }
    });
};