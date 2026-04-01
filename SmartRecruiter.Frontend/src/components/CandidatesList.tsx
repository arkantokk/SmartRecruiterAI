import { User, Mail, Star, ChevronRight } from "lucide-react";
import { useCandidates } from "../features/candidates/hooks/useCandidates";
import type { Candidate } from "../features/candidates/candidatesService";

interface Props {
    onSelect: (candidate: Candidate) => void;
}

export const CandidatesList = ({ onSelect }: Props) => {
    const { data: candidates, isLoading, isError, error } = useCandidates();

    if (isLoading) {
        return (
            <div className="grid gap-4">
                {[1, 2, 3].map((i) => (
                    <div key={i} className="h-24 w-full bg-gray-100 animate-pulse rounded-2xl" />
                ))}
            </div>
        );
    }

    if (isError) {
        return (
            <div className="p-6 bg-red-50 border border-red-100 rounded-2xl text-red-600">
                Error loading candidates: {error instanceof Error ? error.message : "Unknown error"}
            </div>
        );
    }

    if (!candidates || candidates.length === 0) {
        return (
            <div className="text-center py-20 bg-white rounded-3xl border-2 border-dashed border-gray-200">
                <div className="mx-auto w-16 h-16 bg-gray-50 rounded-full flex items-center justify-center mb-4">
                    <User className="text-gray-400" size={32} />
                </div>
                <h3 className="text-lg font-semibold text-gray-900">No candidates yet</h3>
                <p className="text-gray-500 mt-1">New candidates will appear here once they apply via email.</p>
            </div>
        );
    }

    return (
        <div className="grid gap-4">
            {candidates.map((candidate) => (
                <div
                    key={candidate.id}
                    onClick={() => onSelect(candidate)}
                    className="group bg-white p-5 rounded-2xl border border-gray-100 shadow-sm hover:shadow-md hover:border-blue-200 transition-all cursor-pointer flex items-center justify-between"
                >
                    <div className="flex items-center gap-4">
                        <div className="h-12 w-12 rounded-xl bg-gradient-to-br from-blue-500 to-indigo-600 flex items-center justify-center text-white font-bold text-lg shadow-lg shadow-blue-100">
                            {candidate.firstName[0]}{candidate.lastName[0]}
                        </div>
                        <div>
                            <h3 className="font-bold text-gray-900 group-hover:text-blue-600 transition-colors">
                                {candidate.firstName} {candidate.lastName}
                            </h3>
                            <div className="flex items-center gap-3 mt-1">
                                <span className="flex items-center gap-1 text-sm text-gray-500">
                                    <Mail size={14} /> {candidate.email}
                                </span>
                                <span className="h-1 w-1 rounded-full bg-gray-300" />
                                <span className="flex items-center gap-1 text-sm font-medium text-blue-600 bg-blue-50 px-2 py-0.5 rounded-md">
                                    <Star size={12} fill="currentColor" /> {candidate.score} AI Score
                                </span>
                            </div>
                        </div>
                    </div>
                    <div className="flex items-center gap-4">
                        <span className="px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wider bg-gray-100 text-gray-600">
                            {candidate.status}
                        </span>
                        <ChevronRight className="text-gray-300 group-hover:text-blue-400 transition-colors" />
                    </div>
                </div>
            ))}
        </div>
    );
};