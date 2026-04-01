import { X, Download, Mail, Briefcase, Star, FileText, Zap } from "lucide-react";
import type { Candidate } from "../features/candidates/candidatesService";

interface Props {
    candidate: Candidate | null;
    onClose: () => void;
}

export const CandidateDetails = ({ candidate, onClose }: Props) => {
    if (!candidate) return null;

    const getScoreColor = (score: number) => {
        if (score >= 80) return "text-green-600 bg-green-100 border-green-200";
        if (score >= 50) return "text-yellow-600 bg-yellow-100 border-yellow-200";
        return "text-red-600 bg-red-100 border-red-200";
    };

    return (
        <div className="fixed inset-0 z-50 overflow-hidden">
            <div className="absolute inset-0 bg-gray-900/40 backdrop-blur-sm transition-opacity" onClick={onClose} />

            <div className="fixed inset-y-0 right-0 flex max-w-full pl-10">
                <div className="w-screen max-w-xl transform transition-all duration-500 ease-in-out">
                    <div className="flex h-full flex-col bg-white shadow-2xl rounded-l-3xl overflow-hidden">

                        <div className="bg-white px-8 pt-10 pb-6 border-b border-gray-100">
                            <div className="flex items-start justify-between mb-6">
                                <div className="flex items-center gap-5">
                                    <div className={`flex h-20 w-20 shrink-0 items-center justify-center rounded-2xl border-2 font-black text-2xl shadow-sm ${getScoreColor(candidate.score)}`}>
                                        {candidate.score}
                                    </div>
                                    <div>
                                        <h2 className="text-2xl font-black text-gray-900 leading-tight">
                                            {candidate.firstName} {candidate.lastName}
                                        </h2>
                                        <div className="flex flex-col gap-1 mt-2">
                                            <span className="flex items-center gap-2 text-gray-500 text-sm font-medium">
                                                <Mail size={14} className="text-gray-400" /> {candidate.email}
                                            </span>
                                            <span className="flex items-center gap-2 text-gray-500 text-sm font-medium">
                                                <Briefcase size={14} className="text-gray-400" /> {candidate.status}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <button onClick={onClose} className="p-2 rounded-full hover:bg-gray-100 text-gray-400 hover:text-gray-600 transition-all">
                                    <X size={24} />
                                </button>
                            </div>

                            <div className="flex gap-3">
                                <button className="flex-1 bg-blue-600 text-white px-6 py-3 rounded-xl font-bold hover:bg-blue-700 transition-all shadow-lg shadow-blue-100 flex items-center justify-center gap-2">
                                    Invite to Interview
                                </button>
                                <button className="px-6 py-3 border border-gray-200 text-gray-600 rounded-xl font-bold hover:bg-gray-50 transition-all">
                                    Reject
                                </button>
                                {candidate.resumeUrl && (
                                    <a
                                        href={candidate.resumeUrl}
                                        target="_blank"
                                        rel="noreferrer"
                                        className="flex items-center justify-center w-12 bg-gray-100 text-gray-600 rounded-xl hover:bg-gray-200 transition-all"
                                    >
                                        <Download size={20} />
                                    </a>
                                )}
                            </div>
                        </div>

                        <div className="flex-1 overflow-y-auto px-8 py-8 space-y-10">

                            <section>
                                <div className="flex items-center gap-2 mb-4">
                                    <div className="p-2 bg-blue-50 rounded-lg text-blue-600">
                                        <Zap size={18} fill="currentColor" />
                                    </div>
                                    <h3 className="text-sm font-black text-gray-900 uppercase tracking-widest">
                                        AI Evaluation Summary
                                    </h3>
                                </div>
                                <div className="relative">
                                    <div className="absolute -left-1 top-0 bottom-0 w-1 bg-blue-100 rounded-full" />
                                    <p className="pl-6 text-gray-600 leading-relaxed text-lg font-medium italic">
                                        "{candidate.summary}"
                                    </p>
                                </div>
                            </section>

                            <section>
                                <div className="flex items-center gap-2 mb-5">
                                    <div className="p-2 bg-purple-50 rounded-lg text-purple-600">
                                        <FileText size={18} />
                                    </div>
                                    <h3 className="text-sm font-black text-gray-900 uppercase tracking-widest">
                                        Identified Skills
                                    </h3>
                                </div>
                                <div className="flex flex-wrap gap-2">
                                    {candidate.skills.map((skill: string, i: number) => (
                                        <span
                                            key={i}
                                            className="px-4 py-2 bg-white text-gray-700 text-sm font-bold rounded-xl border border-gray-200 shadow-sm hover:border-purple-200 hover:bg-purple-50/30 transition-all"
                                        >
                                            {skill}
                                        </span>
                                    ))}
                                </div>
                            </section>

                        </div>

                        <div className="p-8 bg-gray-50 border-t border-gray-100">
                            <div className="flex items-center gap-3 text-gray-400">
                                <Star size={16} />
                                <span className="text-xs font-bold uppercase tracking-widest">Generated by SmartRecruiter AI</span>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    );
};