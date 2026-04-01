import React from 'react';
import { ScoreCircle } from './ScoreCircle';
import type { Candidate } from '../features/candidates/candidatesService';

interface Props {
    candidate: Candidate;
    onAccept: (id: string) => void;
    onReject: (id: string) => void;
    isActionLoading?: boolean;
}

export const CandidateCard: React.FC<Props> = ({ candidate, onAccept, onReject, isActionLoading }) => {
    // If score is 0 and summary is empty, we assume AI hasn't processed it yet
    const hasAiData = candidate.score > 0 || candidate.summary;

    return (
        <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-6 hover:shadow-md transition-shadow">
            {/* Card Header */}
            <div className="flex justify-between items-start mb-6">
                <div>
                    <h3 className="text-lg font-bold text-gray-900">{candidate.firstName} {candidate.lastName}</h3>
                    <p className="text-sm text-gray-500">{candidate.email}</p>
                    {candidate.resumeUrl && (
                        <a href={candidate.resumeUrl} target="_blank" rel="noreferrer" className="text-blue-600 text-sm hover:text-blue-800 hover:underline mt-1 inline-block font-medium">
                            📄 Open original resume
                        </a>
                    )}
                </div>

                {/* Action Buttons */}
                <div className="flex gap-2">
                    <button
                        disabled={isActionLoading}
                        onClick={() => onAccept(candidate.id)}
                        className="px-4 py-2 bg-emerald-50 text-emerald-700 rounded-xl text-sm font-semibold hover:bg-emerald-100 disabled:opacity-50 transition"
                    >
                        ✅ Accept
                    </button>
                    <button
                        disabled={isActionLoading}
                        onClick={() => onReject(candidate.id)}
                        className="px-4 py-2 bg-rose-50 text-rose-700 rounded-xl text-sm font-semibold hover:bg-rose-100 disabled:opacity-50 transition"
                    >
                        ❌ Reject
                    </button>
                </div>
            </div>

            {/* AI Analysis Section */}
            {hasAiData ? (
                <div className="pt-6 border-t border-gray-100 flex flex-col md:flex-row gap-8 items-start">

                    {/* Left: Score Circle */}
                    <div className="flex flex-col items-center shrink-0">
                        <ScoreCircle score={candidate.score} />
                        <span className="text-xs font-bold text-gray-400 uppercase tracking-wider block mt-3">AI Score</span>
                    </div>

                    {/* Right: Summary and Skills */}
                    <div className="flex-1">
                        <h4 className="text-sm font-bold text-gray-900 mb-2">AI Summary</h4>
                        <p className="text-sm text-gray-600 leading-relaxed mb-4">
                            {candidate.summary || "No summary available."}
                        </p>

                        {candidate.skills && candidate.skills.length > 0 && (
                            <div>
                                <h4 className="text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Extracted Skills</h4>
                                <div className="flex flex-wrap gap-2">
                                    {candidate.skills.map((skill, i) => (
                                        <span key={i} className="px-3 py-1 bg-slate-100 text-slate-700 text-xs font-semibold rounded-lg border border-slate-200">
                      {skill}
                    </span>
                                    ))}
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            ) : (
                <div className="mt-2 p-4 bg-amber-50 rounded-xl text-sm text-amber-800 border border-amber-100 flex items-center gap-3">
                    <span className="text-xl">⏳</span>
                    AI is still analyzing this candidate or a file reading error occurred.
                </div>
            )}
        </div>
    );
};