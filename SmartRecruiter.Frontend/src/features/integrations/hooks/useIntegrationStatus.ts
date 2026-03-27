import { useQuery } from '@tanstack/react-query';
import {integrationService} from "../integrationService.ts";

export const useIntegrationStatus = () => {
    return useQuery({
        queryKey: ['integration-status'],
        queryFn: integrationService.status,
        staleTime: 1000 * 60 * 5,
        retry: false
    })
}