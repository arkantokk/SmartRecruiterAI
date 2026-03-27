import {useQuery} from "@tanstack/react-query";
import {candidatesService} from "../candidatesService.ts";

export const useCandidates = () => {
    return useQuery({
        queryKey: ["candidates"],
        queryFn: candidatesService.getCandidates,
        staleTime: 1000 * 60 * 5,
        retry: 1
    })
}