import { useEffect, useState } from "react";
import { candidatesService } from "../features/candidates/candidatesService";
import type { Candidate } from "../features/candidates/candidatesService";
import Header from "../components/Header";

export const Candidates = () => {
    const [candidates, setCandidates] = useState<Candidate[]>([]);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchCandidates = async () => {
            try {
                const data = await candidatesService.getAll();
                setCandidates(data);
            } catch (err) {
                setError("Failed to load candidates. Are you logged in?");
            }
        };
        fetchCandidates();
    }, []);

    return (
        <div className="min-h-screen bg-gray-50">
            <Header />

            <div className="p-8 max-w-7xl mx-auto">
                <div className="flex justify-between items-center mb-8">
                    <h1 className="text-3xl font-extrabold text-gray-900">All Candidates</h1>
                </div>

                {error ? (
                    <div className="p-4 bg-red-50 text-red-600 rounded-lg font-medium text-center">
                        {error}
                    </div>
                ) : (
                    <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                        {candidates.map((candidate) => (
                            <div key={candidate.id} className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100 hover:shadow-md transition-shadow">
                                <h2 className="text-xl font-bold text-gray-800">
                                    {candidate.firstName} {candidate.lastName}
                                </h2>
                                <p className="text-gray-500 mt-1">{candidate.email}</p>
                                <div className="mt-4">
                                    <span className="bg-blue-100 text-blue-800 text-xs font-semibold px-3 py-1 rounded-full">
                                        {candidate.status || "New"}
                                    </span>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}

export default Candidates;