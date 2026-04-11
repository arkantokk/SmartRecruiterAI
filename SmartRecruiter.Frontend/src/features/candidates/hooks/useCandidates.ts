import {keepPreviousData, useQuery} from "@tanstack/react-query";
import {candidatesService} from "../candidatesService.ts";

export const useCandidates = (pageNumber: number, pageSize: number) => {
    return useQuery({
        queryKey: ["candidates", pageNumber, pageSize],
        queryFn: () => candidatesService.getCandidates(pageNumber, pageSize),
        refetchInterval: 30000,
        placeholderData: keepPreviousData,
        retry: 1
    })
}