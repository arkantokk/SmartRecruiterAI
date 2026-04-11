import { User, Mail, Star, ChevronRight } from "lucide-react";
import { useCandidates } from "../features/candidates/hooks/useCandidates";
import type { Candidate } from "../features/candidates/candidatesService";
import { useState } from "react";

interface Props {
    onSelect: (candidate: Candidate) => void;
}

export const CandidatesList = ({ onSelect }: Props) => {
    const [page, setPage] = useState(1);
    const pageSize = 6;

    const { data, isLoading, isError, error, isFetching } = useCandidates(page, pageSize);

    const items = data?.items || [];
    const totalCount = data?.totalCount || 0;
    const totalPages = Math.ceil(totalCount / pageSize);

    if (isLoading) {
        return (
            <div className="flex flex-col gap-6">
                <div className="grid gap-4">
                    {[...Array(6)].map((_, i) => (
                        <div
                            key={i}
                            className="bg-white p-5 rounded-2xl border border-gray-100 shadow-sm flex items-center justify-between animate-pulse"
                        >
                            <div className="flex items-center gap-4">
                                <div className="h-12 w-12 rounded-xl bg-gray-200" />
                                <div className="flex flex-col gap-2.5">
                                    <div className="h-4 w-32 bg-gray-200 rounded-md" />
                                    <div className="flex items-center gap-3">
                                        <div className="h-3 w-32 bg-gray-200 rounded-md" />
                                        <div className="h-1 w-1 rounded-full bg-gray-200" />
                                        <div className="h-4 w-20 bg-gray-200 rounded-md" />
                                    </div>
                                </div>
                            </div>
                            <div className="flex items-center gap-4">
                                <div className="h-6 w-20 bg-gray-200 rounded-full" />
                                <div className="h-5 w-5 bg-gray-200 rounded-md" />
                            </div>
                        </div>
                    ))}
                </div>
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

    if (!items || items.length === 0) {
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
        <div className="flex flex-col gap-6">
            <div className={`grid gap-4 transition-opacity duration-200 ${isFetching ? 'opacity-50 pointer-events-none' : ''}`}>
                {items.map((candidate) => (
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
                            <span className={`px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wider ${candidate.status === "Rejected" ? 'bg-red-500 text-white' : candidate.status === "Hired" ? 'bg-green-400 text-white' : 'bg-amber-50 text-gray-600'}`}>
                                {candidate.status}
                            </span>
                            <ChevronRight className="text-gray-300 group-hover:text-blue-400 transition-colors" />
                        </div>
                    </div>
                ))}
            </div>

            {totalCount > 0 && (
                <div className="flex items-center justify-between px-2 py-4 border-t border-gray-100 mt-2">
                    <div className="text-sm font-medium text-gray-500">
                        Showing <span className="font-bold text-gray-900">{(page - 1) * pageSize + 1}</span> to <span className="font-bold text-gray-900">{Math.min(page * pageSize, totalCount)}</span> of <span className="font-bold text-gray-900">{totalCount}</span> candidates
                    </div>

                    <div className="flex items-center gap-3">
                        <button
                            disabled={page === 1}
                            onClick={() => setPage(page - 1)}
                            className="px-4 py-2 text-sm font-bold text-gray-700 bg-white border border-gray-200 rounded-xl hover:bg-gray-50 hover:border-gray-300 disabled:opacity-40 disabled:hover:bg-white disabled:cursor-not-allowed transition-all shadow-sm flex items-center gap-1"
                        >
                            Previous
                        </button>

                        <div className="flex items-center gap-1 bg-white border border-gray-200 rounded-xl p-1 shadow-sm">
                            <span className="px-3 py-1 text-sm font-bold text-blue-700 bg-blue-50 rounded-lg">
                                {page}
                            </span>
                            <span className="text-gray-300 font-medium">/</span>
                            <span className="px-3 py-1 text-sm font-bold text-gray-600">
                                {totalPages}
                            </span>
                        </div>

                        <button
                            disabled={page >= totalPages}
                            onClick={() => setPage(page + 1)}
                            className="px-4 py-2 text-sm font-bold text-gray-700 bg-white border border-gray-200 rounded-xl hover:bg-gray-50 hover:border-gray-300 disabled:opacity-40 disabled:hover:bg-white disabled:cursor-not-allowed transition-all shadow-sm flex items-center gap-1"
                        >
                            Next
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
};