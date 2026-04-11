import React from 'react';
import { ScoreCircle } from './ScoreCircle';
import type { Candidate } from '../features/candidates/candidatesService';
import { CheckCircle2, XCircle, ExternalLink, Zap, FileText, Clock } from 'lucide-react';

interface Props {
    candidate: Candidate;
    onAccept: (id: string) => void;
    onReject: (id: string) => void;
    isActionLoading?: boolean;
}

export const CandidateCard: React.FC<Props> = ({ candidate, onAccept, onReject, isActionLoading }) => {
    const hasAiData = candidate.score > 0 || candidate.summary;
    const status = String(candidate.status);

    const isHired = status === 'Hired' || status === '6';
    const isRejected = status === 'Rejected' || status === '5';
    const isArchived = isHired || isRejected;

    return (
        <div className="bg-white rounded-[2rem] shadow-sm border border-gray-100 p-6 sm:p-8 hover:shadow-xl hover:shadow-blue-500/5 hover:-translate-y-1 transition-all duration-300 flex flex-col gap-6">
            <div className="flex flex-col sm:flex-row justify-between items-start gap-6">
                <div className="flex items-start gap-5">
                    <div className="h-16 w-16 shrink-0 rounded-2xl bg-gradient-to-br from-blue-50 to-indigo-50 flex items-center justify-center text-blue-700 font-black text-2xl shadow-inner border border-blue-100/50">
                        {candidate.firstName[0]}{candidate.lastName[0]}
                    </div>
                    <div className="flex flex-col">
                        <h3 className="text-xl font-black text-gray-900 tracking-tight">
                            {candidate.firstName} {candidate.lastName}
                        </h3>
                        <p className="text-sm font-medium text-gray-500 mt-1">{candidate.email}</p>
                        {candidate.resumeUrl && (
                            <a
                                href={candidate.resumeUrl}
                                target="_blank"
                                rel="noreferrer"
                                className="flex items-center gap-1.5 text-blue-600 text-sm font-bold hover:text-blue-800 hover:bg-blue-50 px-3 py-1.5 rounded-lg -ml-3 mt-2 transition-all w-max"
                            >
                                <ExternalLink size={16} /> Open Resume
                            </a>
                        )}
                    </div>
                </div>

                <div className="flex items-center gap-3 shrink-0 w-full sm:w-auto">
                    {!isArchived ? (
                        <>
                            <button
                                disabled={isActionLoading}
                                onClick={() => onAccept(candidate.id)}
                                className="flex-1 sm:flex-none flex items-center justify-center gap-2 px-5 py-2.5 bg-emerald-50 text-emerald-700 rounded-xl text-sm font-bold hover:bg-emerald-100 disabled:opacity-50 transition-all border border-emerald-100/50"
                            >
                                <CheckCircle2 size={18} /> Accept
                            </button>
                            <button
                                disabled={isActionLoading}
                                onClick={() => onReject(candidate.id)}
                                className="flex-1 sm:flex-none flex items-center justify-center gap-2 px-5 py-2.5 bg-rose-50 text-rose-700 rounded-xl text-sm font-bold hover:bg-rose-100 disabled:opacity-50 transition-all border border-rose-100/50"
                            >
                                <XCircle size={18} /> Reject
                            </button>
                        </>
                    ) : (
                        <div className={`flex items-center justify-center gap-2 px-6 py-2.5 rounded-xl text-sm font-black uppercase tracking-wider w-full sm:w-auto ${
                            isHired
                                ? 'bg-emerald-50 text-emerald-600 border border-emerald-200'
                                : 'bg-rose-50 text-rose-600 border border-rose-200'
                        }`}>
                            {isHired ? <CheckCircle2 size={18} /> : <XCircle size={18} />}
                            {isHired ? 'Hired' : 'Rejected'}
                        </div>
                    )}
                </div>
            </div>

            {hasAiData ? (
                <div className="pt-6 border-t border-gray-100 flex flex-col md:flex-row gap-8 items-start">
                    <div className="flex flex-col items-center shrink-0 bg-gray-50/50 p-4 rounded-2xl border border-gray-100">
                        <ScoreCircle score={candidate.score} />
                        <span className="text-[10px] font-black text-gray-400 uppercase tracking-widest mt-3">AI Score</span>
                    </div>

                    <div className="flex-1 space-y-5">
                        <div>
                            <div className="flex items-center gap-2 mb-2">
                                <Zap size={16} className="text-amber-500" fill="currentColor" />
                                <h4 className="text-xs font-black text-gray-900 uppercase tracking-widest">AI Summary</h4>
                            </div>
                            <p className="text-sm text-gray-600 leading-relaxed font-medium bg-gray-50/50 p-4 rounded-xl border border-gray-100">
                                {candidate.summary || "No summary available."}
                            </p>
                        </div>

                        {candidate.skills && candidate.skills.length > 0 && (
                            <div>
                                <div className="flex items-center gap-2 mb-3">
                                    <FileText size={16} className="text-blue-500" />
                                    <h4 className="text-xs font-black text-gray-900 uppercase tracking-widest">Extracted Skills</h4>
                                </div>
                                <div className="flex flex-wrap gap-2">
                                    {candidate.skills.map((skill, i) => (
                                        <span
                                            key={i}
                                            className="px-3 py-1.5 bg-white text-gray-700 text-xs font-bold rounded-lg border border-gray-200 shadow-sm hover:border-blue-200 hover:text-blue-600 transition-colors cursor-default"
                                        >
                                            {skill}
                                        </span>
                                    ))}
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            ) : (
                <div className="mt-2 p-5 bg-gradient-to-r from-amber-50 to-orange-50 rounded-2xl text-sm text-amber-800 border border-amber-100 flex items-center gap-4">
                    <div className="p-2 bg-amber-100 rounded-full text-amber-600 shrink-0">
                        <Clock size={20} />
                    </div>
                    <span className="font-medium">
                        AI is still analyzing this candidate or a file reading error occurred. Please check back later.
                    </span>
                </div>
            )}
        </div>
    );
};