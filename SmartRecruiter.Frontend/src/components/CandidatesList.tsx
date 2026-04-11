import { User, Mail, Sparkles, ChevronRight } from "lucide-react";
import { useCandidates } from "../features/candidates/hooks/useCandidates";
import type { Candidate } from "../features/candidates/candidatesService";
import { useState } from "react";

interface Props {
    onSelect: (candidate: Candidate) => void;
    searchTerm: string;
    sortBy: string;
}

export const CandidatesList = ({ onSelect, searchTerm, sortBy }: Props) => {
    const [page, setPage] = useState(1);
    const pageSize = 5;

    const { data, isLoading, isError, error, isFetching } = useCandidates(page, pageSize, searchTerm, sortBy);

    const items = data?.items || [];
    const totalCount = data?.totalCount || 0;
    const totalPages = Math.ceil(totalCount / pageSize);

    const getStatusStyles = (status: string) => {
        switch (status) {
            case "Rejected":
                return "bg-rose-50 text-rose-600 border-rose-100";
            case "Hired":
                return "bg-emerald-50 text-emerald-600 border-emerald-100";
            case "Interview":
                return "bg-purple-50 text-purple-600 border-purple-100";
            case "Offer":
                return "bg-amber-50 text-amber-600 border-amber-100";
            default:
                return "bg-gray-50 text-gray-600 border-gray-200";
        }
    };

    if (isLoading) {
        return (
            <div className="flex flex-col gap-6">
                <div className="grid gap-4">
                    {[...Array(6)].map((_, i) => (
                        <div
                            key={i}
                            className="bg-white p-6 rounded-3xl border border-gray-100 shadow-sm flex items-start justify-between animate-pulse"
                        >
                            <div className="flex items-start gap-5">
                                <div className="h-14 w-14 rounded-2xl bg-gray-200" />
                                <div className="flex flex-col gap-3 mt-1">
                                    <div className="h-5 w-40 bg-gray-200 rounded-md" />
                                    <div className="h-4 w-32 bg-gray-200 rounded-md" />
                                    <div className="flex gap-2 mt-2">
                                        <div className="h-6 w-16 bg-gray-100 rounded-lg" />
                                        <div className="h-6 w-20 bg-gray-100 rounded-lg" />
                                        <div className="h-6 w-14 bg-gray-100 rounded-lg" />
                                    </div>
                                </div>
                            </div>
                            <div className="flex flex-col items-end gap-3 mt-1">
                                <div className="h-8 w-24 bg-gray-200 rounded-xl" />
                                <div className="h-6 w-20 bg-gray-200 rounded-full" />
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        );
    }

    if (isError) {
        return (
            <div className="p-6 bg-red-50 border border-red-100 rounded-3xl text-red-600 font-medium">
                Error loading candidates: {error instanceof Error ? error.message : "Unknown error"}
            </div>
        );
    }

    if (!items || items.length === 0) {
        return (
            <div className="text-center py-24 bg-white rounded-3xl border border-dashed border-gray-200 shadow-sm">
                <div className="mx-auto w-20 h-20 bg-gray-50 rounded-full flex items-center justify-center mb-5">
                    <User className="text-gray-400" size={36} />
                </div>
                <h3 className="text-xl font-bold text-gray-900">No candidates found</h3>
                <p className="text-gray-500 mt-2 font-medium">Try adjusting your search or wait for new applications.</p>
            </div>
        );
    }

    return (
        <div className="flex flex-col gap-6">
            <div className={`grid gap-4 transition-opacity duration-300 ${isFetching ? 'opacity-50 pointer-events-none' : ''}`}>
                {items.map((candidate) => (
                    <div
                        key={candidate.id}
                        onClick={() => onSelect(candidate)}
                        className="group bg-white p-6 rounded-3xl border border-gray-100 shadow-sm hover:shadow-xl hover:shadow-blue-500/5 hover:-translate-y-1 transition-all duration-300 cursor-pointer flex flex-col sm:flex-row sm:items-center justify-between gap-6"
                    >
                        <div className="flex items-start gap-5 flex-1">
                            <div className="h-14 w-14 shrink-0 rounded-2xl bg-gradient-to-br from-blue-500 to-indigo-600 flex items-center justify-center text-white font-black text-xl shadow-lg shadow-blue-200/50 group-hover:scale-105 transition-transform duration-300">
                                {candidate.firstName[0]}{candidate.lastName[0]}
                            </div>

                            <div className="flex flex-col min-w-0">
                                <h3 className="text-lg font-black text-gray-900 group-hover:text-blue-600 transition-colors truncate">
                                    {candidate.firstName} {candidate.lastName}
                                </h3>
                                <div className="flex items-center gap-1.5 text-sm font-medium text-gray-500 mt-0.5 truncate">
                                    <Mail size={14} className="text-gray-400" />
                                    <span className="truncate">{candidate.email}</span>
                                </div>

                                {candidate.skills && candidate.skills.length > 0 && (
                                    <div className="flex items-center flex-wrap gap-2 mt-3.5">
                                        {candidate.skills.slice(0, 3).map((skill, idx) => (
                                            <span
                                                key={idx}
                                                className="px-2.5 py-1 rounded-lg bg-gray-50 text-gray-600 text-xs font-bold border border-gray-200 shadow-sm whitespace-nowrap"
                                            >
                                                {skill}
                                            </span>
                                        ))}
                                        {candidate.skills.length > 3 && (
                                            <span className="px-2 py-1 rounded-lg bg-blue-50 text-blue-600 text-xs font-black">
                                                +{candidate.skills.length - 3}
                                            </span>
                                        )}
                                    </div>
                                )}
                            </div>
                        </div>

                        <div className="flex sm:flex-col items-center sm:items-end justify-between sm:justify-center gap-4 border-t sm:border-t-0 border-gray-100 pt-4 sm:pt-0">
                            <div className="flex items-center gap-2 bg-gradient-to-r from-blue-50 to-indigo-50/50 px-3.5 py-2 rounded-xl border border-blue-100/50">
                                <Sparkles size={16} className="text-blue-500" />
                                <span className="font-black text-blue-700 text-sm">
                                    {candidate.score} <span className="text-blue-400 font-semibold">/ 100</span>
                                </span>
                            </div>

                            <div className="flex items-center gap-3">
                                <span className={`px-3 py-1 rounded-full text-xs font-black uppercase tracking-wider border ${getStatusStyles(candidate.status)}`}>
                                    {candidate.status}
                                </span>
                                <ChevronRight className="text-gray-300 group-hover:text-blue-500 group-hover:translate-x-1 transition-all" size={20} />
                            </div>
                        </div>
                    </div>
                ))}
            </div>

            {totalCount > 0 && (
                <div className="flex flex-col sm:flex-row items-center justify-between px-2 py-4 mt-2 gap-4">
                    <div className="text-sm font-medium text-gray-500">
                        Showing <span className="font-bold text-gray-900">{(page - 1) * pageSize + 1}</span> to <span className="font-bold text-gray-900">{Math.min(page * pageSize, totalCount)}</span> of <span className="font-bold text-gray-900">{totalCount}</span> candidates
                    </div>

                    <div className="flex items-center gap-2 bg-white p-1.5 rounded-2xl border border-gray-200 shadow-sm">
                        <button
                            disabled={page === 1}
                            onClick={() => setPage(page - 1)}
                            className="px-4 py-2 text-sm font-bold text-gray-700 rounded-xl hover:bg-gray-100 disabled:opacity-40 disabled:hover:bg-transparent disabled:cursor-not-allowed transition-all"
                        >
                            Prev
                        </button>

                        <div className="flex items-center gap-1 px-3 py-1.5 bg-gray-50 rounded-lg border border-gray-100">
                            <span className="text-sm font-black text-gray-900">{page}</span>
                            <span className="text-gray-400 font-bold px-1">/</span>
                            <span className="text-sm font-bold text-gray-500">{totalPages}</span>
                        </div>

                        <button
                            disabled={page >= totalPages}
                            onClick={() => setPage(page + 1)}
                            className="px-4 py-2 text-sm font-bold text-gray-700 rounded-xl hover:bg-gray-100 disabled:opacity-40 disabled:hover:bg-transparent disabled:cursor-not-allowed transition-all"
                        >
                            Next
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
};