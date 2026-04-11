import {keepPreviousData, useQuery} from "@tanstack/react-query";
import {candidatesService} from "../candidatesService.ts";

export const useCandidates = (pageNumber: number, pageSize: number, searchTerm: string, sortBy: string) => {
    return useQuery({
        queryKey: ["candidates", pageNumber, pageSize, searchTerm, sortBy],
        queryFn: () => candidatesService.getCandidates(pageNumber, pageSize, searchTerm, sortBy),
        refetchInterval: 30000,
        placeholderData: keepPreviousData,
        retry: 1
    })
}