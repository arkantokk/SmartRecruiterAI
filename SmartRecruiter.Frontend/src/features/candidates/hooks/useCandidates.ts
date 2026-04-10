import {useQuery} from "@tanstack/react-query";
import {candidatesService} from "../candidatesService.ts";

export const useCandidates = () => {
    return useQuery({
        queryKey: ["candidates"],
        queryFn: candidatesService.getCandidates,
        refetchInterval: 30000,
        retry: 1
    })
}