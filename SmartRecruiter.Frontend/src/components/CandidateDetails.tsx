import { X, Download, Mail, Briefcase, Star, FileText, Zap, CheckCircle2, XCircle, Loader2 } from "lucide-react";
import type { Candidate } from "../features/candidates/candidatesService";

interface Props {
    candidate: Candidate | null;
    onClose: () => void;
    onUpdateStatus: (status: number) => void;
    isUpdating: boolean;
}

export const CandidateDetails = ({ candidate, onClose, onUpdateStatus, isUpdating }: Props) => {
    if (!candidate) return null;

    const getScoreConfig = (score: number) => {
        if (score >= 80) return { color: "text-emerald-600", bg: "bg-emerald-50", border: "border-emerald-200" };
        if (score >= 50) return { color: "text-amber-600", bg: "bg-amber-50", border: "border-amber-200" };
        return { color: "text-rose-600", bg: "bg-rose-50", border: "border-rose-200" };
    };

    const scoreConfig = getScoreConfig(candidate.score);

    return (
        <div className="fixed inset-0 z-50 overflow-hidden flex justify-end">
            <div
                className="absolute inset-0 bg-gray-900/60 backdrop-blur-sm transition-opacity animate-in fade-in duration-300"
                onClick={onClose}
            />

            <div className="relative w-full max-w-xl h-full bg-white shadow-2xl flex flex-col animate-in slide-in-from-right duration-300 sm:rounded-l-[2rem]">
                <div className="px-8 pt-8 pb-6 border-b border-gray-100 bg-white sm:rounded-tl-[2rem] z-10 shrink-0">
                    <div className="flex items-start justify-between mb-8">
                        <div className="flex items-center gap-6">
                            <div className={`flex flex-col items-center justify-center h-24 w-24 shrink-0 rounded-[1.5rem] border ${scoreConfig.border} ${scoreConfig.bg} shadow-inner`}>
                                <span className={`text-3xl font-black ${scoreConfig.color}`}>{candidate.score}</span>
                                <span className={`text-[10px] font-bold uppercase tracking-wider mt-1 ${scoreConfig.color} opacity-80`}>AI Score</span>
                            </div>
                            <div>
                                <h2 className="text-3xl font-black text-gray-900 leading-tight mb-3">
                                    {candidate.firstName} {candidate.lastName}
                                </h2>
                                <div className="flex flex-col gap-2">
                                    <div className="flex items-center gap-2 text-gray-600 text-sm font-medium bg-gray-50 px-3 py-1.5 rounded-lg w-max">
                                        <Mail size={14} className="text-gray-400" /> {candidate.email}
                                    </div>
                                    <div className="flex items-center gap-2 text-gray-600 text-sm font-medium bg-gray-50 px-3 py-1.5 rounded-lg w-max">
                                        <Briefcase size={14} className="text-gray-400" /> Status: <span className="font-bold text-gray-900">{candidate.status}</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <button onClick={onClose} className="p-2.5 bg-gray-50 rounded-full hover:bg-gray-100 text-gray-400 hover:text-gray-900 transition-all">
                            <X size={20} strokeWidth={2.5} />
                        </button>
                    </div>

                    <div className="flex gap-3">
                        <button
                            onClick={() => onUpdateStatus(6)}
                            disabled={isUpdating || candidate.status === "Hired" || candidate.status === "Offer"}
                            className="flex-1 bg-gradient-to-r from-blue-600 to-indigo-600 text-white px-6 py-3.5 rounded-xl font-bold hover:from-blue-700 hover:to-indigo-700 transition-all shadow-lg shadow-blue-500/25 flex items-center justify-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            {isUpdating ? <Loader2 size={18} className="animate-spin" /> : <CheckCircle2 size={18} />}
                            Accept Candidate
                        </button>
                        <button
                            onClick={() => onUpdateStatus(5)}
                            disabled={isUpdating || candidate.status === "Rejected"}
                            className="px-6 py-3.5 bg-white border-2 border-gray-200 text-gray-700 rounded-xl font-bold hover:border-rose-200 hover:bg-rose-50 hover:text-rose-600 transition-all flex items-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            <XCircle size={18} /> Reject
                        </button>
                        {candidate.resumeUrl && (
                            <a
                                href={candidate.resumeUrl}
                                target="_blank"
                                rel="noreferrer"
                                className="flex items-center justify-center w-14 bg-gray-50 border-2 border-gray-100 text-gray-600 rounded-xl hover:bg-gray-100 hover:border-gray-200 hover:text-gray-900 transition-all"
                                title="Download Resume"
                            >
                                <Download size={20} />
                            </a>
                        )}
                    </div>
                </div>

                <div className="flex-1 overflow-y-auto px-8 py-8 space-y-8 bg-gray-50/50 custom-scrollbar">

                    <section className="bg-white p-6 rounded-2xl border border-gray-100 shadow-sm">
                        <div className="flex items-center gap-3 mb-5">
                            <div className="p-2.5 bg-blue-50 rounded-xl text-blue-600">
                                <Zap size={20} fill="currentColor" />
                            </div>
                            <h3 className="text-sm font-black text-gray-900 uppercase tracking-widest">
                                AI Evaluation Summary
                            </h3>
                        </div>
                        <div className="relative bg-gradient-to-br from-blue-50/50 to-indigo-50/50 p-6 rounded-2xl border border-blue-100/50">
                            <p className="text-gray-700 leading-relaxed text-lg font-medium">
                                {candidate.summary}
                            </p>
                        </div>
                    </section>

                    <section className="bg-white p-6 rounded-2xl border border-gray-100 shadow-sm">
                        <div className="flex items-center gap-3 mb-5">
                            <div className="p-2.5 bg-purple-50 rounded-xl text-purple-600">
                                <FileText size={20} />
                            </div>
                            <h3 className="text-sm font-black text-gray-900 uppercase tracking-widest">
                                Identified Skills
                            </h3>
                        </div>
                        <div className="flex flex-wrap gap-2.5">
                            {candidate.skills.map((skill: string, i: number) => (
                                <span
                                    key={i}
                                    className="px-4 py-2 bg-gray-50 text-gray-700 font-bold text-sm rounded-xl border border-gray-100 shadow-sm hover:border-purple-200 hover:bg-purple-50 hover:text-purple-700 transition-all cursor-default"
                                >
                                    {skill}
                                </span>
                            ))}
                        </div>
                    </section>

                </div>

                <div className="p-6 bg-white border-t border-gray-100 flex items-center justify-center gap-2 shrink-0 z-10">
                    <Star size={16} fill="currentColor" className="text-blue-200" />
                    <span className="text-xs font-black uppercase tracking-widest text-gray-400">Generated by SmartRecruiter AI</span>
                </div>

            </div>
        </div>
    );
};