import React, { useState, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { CandidateCard } from '../components/CandidateCard';
import {type Candidate} from '../features/candidates/candidatesService';
import {
    useVacancy,
    useVacancyCandidates,
    useUpdateCandidateStatus
} from '../features/vacancies/hooks/useVacancyDetails';

type Tab = 'Active' | 'Review' | 'Archived';

export const VacancyDetails: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [activeTab, setActiveTab] = useState<Tab>('Active');

    const { data: vacancy, isLoading: isVacancyLoading } = useVacancy(id);
    const { data: candidates, isLoading: isCandidatesLoading } = useVacancyCandidates(id);
    const { mutate: updateStatus, isPending: isUpdating } = useUpdateCandidateStatus(id);

    const filteredCandidates = useMemo(() => {
        if (!candidates) return [];

        return candidates.filter((c: Candidate) => {
            if (activeTab === 'Active') {
                return c.status !== 'Rejected' && c.status !== 'ManualReview';
            }
            if (activeTab === 'Review') {
                return c.status === 'ManualReview';
            }
            if (activeTab === 'Archived') {
                return c.status === 'Rejected';
            }
            return false;
        });
    }, [candidates, activeTab]);

    const handleAccept = (candidateId: string) => {
        updateStatus({ id: candidateId, status: 3 }); // 3 = Interview
    };

    const handleReject = (candidateId: string) => {
        updateStatus({ id: candidateId, status: 5 }); // 5 = Rejected
    };

    if (isVacancyLoading) {
        return (
            <div className="flex justify-center items-center min-h-[50vh]">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
            </div>
        );
    }

    if (!vacancy) {
        return (
            <div className="max-w-6xl mx-auto px-4 py-16 text-center">
                <h2 className="text-2xl font-bold text-gray-900 mb-2">Vacancy not found</h2>
                <p className="text-gray-500 mb-6">The vacancy you are looking for does not exist or you don't have access to it.</p>
                <button
                    onClick={() => navigate(-1)}
                    className="px-5 py-2.5 bg-blue-600 text-white font-medium rounded-xl hover:bg-blue-700 transition"
                >
                    Go Back
                </button>
            </div>
        );
    }

    return (
        <div className="max-w-6xl mx-auto px-4 py-8">
            <button
                onClick={() => navigate(-1)}
                className="text-gray-500 hover:text-gray-800 mb-6 flex items-center gap-2 text-sm font-medium transition"
            >
                ← Back to vacancies
            </button>

            <div className="bg-white rounded-2xl p-8 shadow-sm border border-gray-100 mb-8">
                <div className="flex justify-between items-start gap-4">
                    <div>
                        <h1 className="text-3xl font-extrabold text-gray-900 mb-3">{vacancy.title}</h1>
                        <p className="text-gray-600 max-w-3xl leading-relaxed whitespace-pre-wrap">
                            {vacancy.aiPromptTemplate || "No description or AI prompt provided."}
                        </p>
                    </div>
                    <button className="px-5 py-2.5 bg-blue-50 text-blue-600 font-semibold rounded-xl hover:bg-blue-100 transition shadow-sm border border-blue-100 shrink-0">
                        ✏️ Edit Vacancy
                    </button>
                </div>
            </div>

            <div className="flex space-x-2 mb-6 bg-gray-100 p-1.5 rounded-xl w-max">
                {(['Active', 'Review', 'Archived'] as Tab[]).map((tab) => (
                    <button
                        key={tab}
                        onClick={() => setActiveTab(tab)}
                        className={`px-6 py-2.5 rounded-lg text-sm font-semibold transition-all duration-200 ${
                            activeTab === tab
                                ? 'bg-white text-gray-900 shadow-sm'
                                : 'text-gray-500 hover:text-gray-700 hover:bg-gray-200/50'
                        }`}
                    >
                        {tab === 'Active' ? '🚀 Active Candidates' : tab === 'Review' ? '👀 Needs Review' : '🗄️ Archived'}
                    </button>
                ))}
            </div>

            <div className="space-y-4">
                {isCandidatesLoading ? (
                    <div className="py-12 text-center text-gray-500 font-medium animate-pulse">
                        Loading candidates...
                    </div>
                ) : filteredCandidates.length === 0 ? (
                    <div className="bg-white rounded-2xl border border-dashed border-gray-300 p-16 text-center">
                        <span className="text-4xl mb-4 block">📭</span>
                        <h3 className="text-lg font-bold text-gray-900">No candidates found</h3>
                        <p className="text-gray-500 mt-1">This tab is currently empty.</p>
                    </div>
                ) : (
                    filteredCandidates.map((candidate: Candidate) => (
                        <CandidateCard
                            key={candidate.id}
                            candidate={candidate}
                            onAccept={handleAccept}
                            onReject={handleReject}
                            isActionLoading={isUpdating}
                        />
                    ))
                )}
            </div>
        </div>
    );
};