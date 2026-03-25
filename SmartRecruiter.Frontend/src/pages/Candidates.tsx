import Header from "../components/Header";
import {apiClient} from "../api/axiosInstance.ts";
import type {ApiResponse} from "../models/ApiResponse/ConnectGmailReponse.ts"

export const Candidates = () => {
    const handleConnectGmail  = async () => {
        try{
            const response = await apiClient.get<ApiResponse>('/integrations/google/connect')
            const url = response.data.url;
            if (url) {
                window.location.href = url;
            } else{
                console.error("Could not connect to Google");
            }

        } catch (e) {

        }

    }

    return (
        <div className="min-h-screen bg-gray-50">
            <Header />


            <button onClick={handleConnectGmail}>Connect gmail</button>
        </div>
    );
}

export default Candidates;