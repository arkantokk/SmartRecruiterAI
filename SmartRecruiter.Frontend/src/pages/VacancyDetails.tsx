import React, { useState, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { CandidateCard } from '../components/CandidateCard';
import { type Candidate } from '../features/candidates/candidatesService';
import {
    useVacancy,
    useVacancyCandidates,
    useUpdateCandidateStatus,
    useUpdateVacancy
} from '../features/vacancies/hooks/useVacancyDetails';

type Tab = 'Active' | 'Review' | 'Archived';
type ArchiveFilter = 'All' | 'Hired' | 'Rejected';

export const VacancyDetails: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [activeTab, setActiveTab] = useState<Tab>('Active');
    const [archiveFilter, setArchiveFilter] = useState<ArchiveFilter>('All');

    const [isEditing, setIsEditing] = useState(false);
    const [editForm, setEditForm] = useState({ title: '', aiPromptTemplate: '' });

    const { data: vacancy, isLoading: isVacancyLoading } = useVacancy(id);
    const { data: candidates, isLoading: isCandidatesLoading } = useVacancyCandidates(id);
    const { mutate: updateStatus, isPending: isUpdatingStatus } = useUpdateCandidateStatus(id);
    const { mutate: updateVacancy, isPending: isSavingVacancy } = useUpdateVacancy(id);

    const handleStartEdit = () => {
        if (vacancy) {
            setEditForm({
                title: vacancy.title,
                aiPromptTemplate: vacancy.aiPromptTemplate || ''
            });
            setIsEditing(true);
        }
    };

    const handleSave = () => {
        updateVacancy(editForm, {
            onSuccess: () => setIsEditing(false)
        });
    };

    const filteredCandidates = useMemo(() => {
        if (!candidates) return [];

        return candidates.filter((c: Candidate) => {
            const status = String(c.status);

            if (activeTab === 'Active') {
                return status !== 'Rejected' && status !== '5' &&
                    status !== 'ManualReview' && status !== '2' &&
                    status !== 'Hired' && status !== '6';
            }
            if (activeTab === 'Review') {
                return status === 'ManualReview' || status === '2';
            }
            if (activeTab === 'Archived') {
                const isRejected = status === 'Rejected' || status === '5';
                const isHired = status === 'Hired' || status === '6';

                if (!isRejected && !isHired) return false;

                if (archiveFilter === 'Hired') return isHired;
                if (archiveFilter === 'Rejected') return isRejected;
                return true;
            }
            return false;
        });
    }, [candidates, activeTab, archiveFilter]);

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
                {isEditing ? (
                    <div className="space-y-4">
                        <input
                            type="text"
                            value={editForm.title}
                            onChange={(e) => setEditForm({ ...editForm, title: e.target.value })}
                            className="w-full text-3xl font-extrabold border-b-2 border-blue-500 focus:outline-none py-1"
                            placeholder="Vacancy Title"
                        />
                        <textarea
                            value={editForm.aiPromptTemplate}
                            onChange={(e) => setEditForm({ ...editForm, aiPromptTemplate: e.target.value })}
                            rows={5}
                            className="w-full p-4 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none transition"
                            placeholder="AI Prompt Template / Job Description"
                        />
                        <div className="flex gap-3 justify-end">
                            <button
                                onClick={() => setIsEditing(false)}
                                className="px-6 py-2.5 text-gray-500 font-bold hover:text-gray-700 transition"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleSave}
                                disabled={isSavingVacancy}
                                className="px-8 py-2.5 bg-blue-600 text-white font-bold rounded-xl hover:bg-blue-700 disabled:opacity-50 shadow-lg shadow-blue-200 transition"
                            >
                                {isSavingVacancy ? 'Saving...' : 'Save Changes'}
                            </button>
                        </div>
                    </div>
                ) : (
                    <div className="flex justify-between items-start gap-4">
                        <div>
                            <h1 className="text-3xl font-extrabold text-gray-900 mb-3">{vacancy.title}</h1>
                            <p className="text-gray-600 max-w-3xl leading-relaxed whitespace-pre-wrap">
                                {vacancy.aiPromptTemplate || "No description provided."}
                            </p>
                        </div>
                        <button
                            onClick={handleStartEdit}
                            className="px-5 py-2.5 bg-blue-50 text-blue-600 font-semibold rounded-xl hover:bg-blue-100 transition border border-blue-100 shrink-0"
                        >
                            Edit Vacancy
                        </button>
                    </div>
                )}
            </div>

            <div className="flex justify-between items-center mb-6">
                <div className="flex space-x-2 bg-gray-100 p-1.5 rounded-xl w-max">
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
                            {tab}
                        </button>
                    ))}
                </div>

                {activeTab === 'Archived' && (
                    <div className="flex bg-gray-100 p-1 rounded-xl border border-gray-200">
                        {(['All', 'Hired', 'Rejected'] as ArchiveFilter[]).map((filter) => (
                            <button
                                key={filter}
                                onClick={() => setArchiveFilter(filter)}
                                className={`px-4 py-1.5 rounded-lg text-xs font-bold transition-all ${
                                    archiveFilter === filter
                                        ? 'bg-white text-blue-600 shadow-sm'
                                        : 'text-gray-500 hover:text-gray-700'
                                }`}
                            >
                                {filter}
                            </button>
                        ))}
                    </div>
                )}
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
                        <p className="text-gray-500 mt-1">This list is currently empty.</p>
                    </div>
                ) : (
                    filteredCandidates.map((candidate: Candidate) => (
                        <CandidateCard
                            key={candidate.id}
                            candidate={candidate}
                            onAccept={(id) => updateStatus({ id, status: 6 })}
                            onReject={(id) => updateStatus({ id, status: 5 })}
                            isActionLoading={isUpdatingStatus}
                        />
                    ))
                )}
            </div>
        </div>
    );
};