import { useState, useEffect } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import Header from "../components/Header";
import { CandidatesList } from "../components/CandidatesList";
import { CandidateDetails } from "../components/CandidateDetails";
import { useIntegrationStatus } from "../features/integrations/hooks/useIntegrationStatus";
import { integrationService } from "../features/integrations/integrationService";
import { vacanciesService } from "../features/vacancies/vacanciesService";
import { candidatesService } from "../features/candidates/candidatesService";
import {
    MailWarning,
    Plus,
    Briefcase,
    Sparkles,
    Search,
    ListFilter,
    ArrowRight
} from "lucide-react";
import type { Candidate } from "../features/candidates/candidatesService";
import { useDebounce } from "../hooks/useDebounce.ts";

export const Candidates = () => {
    const queryClient = useQueryClient();
    const [params, setParams] = useSearchParams();
    const [selectedCandidate, setSelectedCandidate] = useState<Candidate | null>(null);
    const [searchTerm, setSearchTerm] = useState("");
    const search = useDebounce(searchTerm, 300);
    const [sortBy, setSortBy] = useState("score_desc");
    const [vTitle, setVTitle] = useState("");
    const [vPrompt, setVPrompt] = useState("");

    const { data: integration, isLoading: isStatusLoading } = useIntegrationStatus();

    const { data: vacancies, isLoading: isVacanciesLoading } = useQuery({
        queryKey: ['vacancies'],
        queryFn: vacanciesService.getVacancies,
        enabled: !!integration?.isConnected
    });

    const { mutate: createVacancy, isPending: isCreatingVacancy } = useMutation({
        mutationFn: vacanciesService.postVacancy,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['vacancies'] });
            setVTitle("");
            setVPrompt("");
        }
    });

    const { mutate: updateCandidateStatus, isPending: isUpdatingStatus } = useMutation({
        mutationFn: candidatesService.updateCandidateStatus,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['candidates'] });
            setSelectedCandidate(null);
        }
    });

    useEffect(() => {
        if (params.get("gmail") === "success") {
            params.delete("gmail");
            setParams(params, { replace: true });
        }
    }, [params, setParams]);

    const handleConnect = async () => {
        await integrationService.connectGmail();
    };

    const handleVacancySubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!vTitle.trim() || !vPrompt.trim()) return;
        createVacancy({ title: vTitle, aiPromptTemplate: vPrompt });
    };

    const handleStatusChange = (status: number) => {
        if (selectedCandidate) {
            updateCandidateStatus({ id: selectedCandidate.id, status });
        }
    };

    return (
        <div className="min-h-screen bg-[#F8FAFC]">
            <Header />

            <main className="max-w-[1600px] mx-auto px-6 py-10">
                <div className="flex justify-between items-end mb-10">
                    <div>
                        <h1 className="text-4xl font-black text-gray-900 tracking-tight">Recruitment Dashboard</h1>
                        <p className="text-gray-500 mt-2 flex items-center gap-2 font-medium">
                            <Sparkles size={18} className="text-blue-500" /> AI-powered candidate sourcing and evaluation
                        </p>
                    </div>
                </div>

                {isStatusLoading ? (
                    <div className="flex justify-center py-32">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                    </div>
                ) : !integration?.isConnected ? (
                    <div className="bg-white rounded-[2rem] p-16 shadow-sm border border-gray-100 text-center max-w-2xl mx-auto mt-16">
                        <div className="w-24 h-24 bg-amber-50 rounded-[1.5rem] flex items-center justify-center mx-auto mb-8 shadow-inner border border-amber-100">
                            <MailWarning className="text-amber-500" size={48} />
                        </div>
                        <h2 className="text-3xl font-black text-gray-900 tracking-tight">Email Integration Required</h2>
                        <p className="text-gray-500 mt-4 mb-10 text-lg font-medium leading-relaxed">
                            Connect your Google Workspace to allow our AI to automatically receive, parse, and score incoming candidate applications.
                        </p>
                        <button
                            onClick={handleConnect}
                            className="bg-gray-900 text-white px-10 py-4 rounded-2xl font-bold text-lg hover:bg-blue-600 hover:shadow-xl hover:shadow-blue-500/20 hover:-translate-y-1 transition-all duration-300"
                        >
                            Connect Google Account
                        </button>
                    </div>
                ) : (
                    <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">

                        <div className="lg:col-span-4 flex flex-col gap-8">
                            <section className="bg-white rounded-3xl p-6 shadow-sm border border-gray-100">
                                <div className="flex items-center gap-3 mb-6">
                                    <div className="w-10 h-10 rounded-xl bg-blue-50 flex items-center justify-center text-blue-600">
                                        <Plus size={20} strokeWidth={2.5} />
                                    </div>
                                    <h3 className="text-xl font-bold text-gray-900 tracking-tight">Create Vacancy</h3>
                                </div>
                                <form onSubmit={handleVacancySubmit} className="space-y-4">
                                    <div>
                                        <input
                                            type="text"
                                            placeholder="Position Title (e.g. Senior Frontend Engineer)"
                                            value={vTitle}
                                            onChange={(e) => setVTitle(e.target.value)}
                                            className="w-full bg-gray-50/50 border border-gray-200 rounded-2xl px-5 py-4 focus:bg-white focus:ring-2 focus:ring-blue-500 focus:border-transparent font-medium transition-all outline-none"
                                            required
                                        />
                                    </div>
                                    <div>
                                        <textarea
                                            placeholder="AI Prompt: Define key skills, red flags, and scoring criteria..."
                                            value={vPrompt}
                                            onChange={(e) => setVPrompt(e.target.value)}
                                            className="w-full bg-gray-50/50 border border-gray-200 rounded-2xl px-5 py-4 h-36 focus:bg-white focus:ring-2 focus:ring-blue-500 focus:border-transparent font-medium text-sm transition-all outline-none resize-none"
                                            required
                                        />
                                    </div>
                                    <button
                                        type="submit"
                                        disabled={isCreatingVacancy}
                                        className="w-full bg-blue-600 text-white py-4 rounded-2xl font-bold hover:bg-blue-700 hover:shadow-lg hover:shadow-blue-500/20 transition-all duration-300 disabled:bg-gray-300 disabled:shadow-none disabled:cursor-not-allowed flex items-center justify-center gap-2"
                                    >
                                        {isCreatingVacancy ? "Creating..." : "Add Opening"} <ArrowRight size={18} />
                                    </button>
                                </form>
                            </section>

                            <section className="bg-white rounded-3xl p-6 shadow-sm border border-gray-100 flex flex-col max-h-[600px]">
                                <div className="flex items-center justify-between mb-6 shrink-0">
                                    <div className="flex items-center gap-3">
                                        <div className="w-10 h-10 rounded-xl bg-indigo-50 flex items-center justify-center text-indigo-600">
                                            <Briefcase size={20} strokeWidth={2.5} />
                                        </div>
                                        <h3 className="text-xl font-bold text-gray-900 tracking-tight">Active Roles</h3>
                                    </div>
                                    <span className="text-sm bg-gray-100 px-3 py-1 rounded-full text-gray-600 font-bold">
                                        {vacancies?.length || 0}
                                    </span>
                                </div>
                                <div className="space-y-3 overflow-y-auto pr-2 custom-scrollbar flex-1">
                                    {isVacanciesLoading ? (
                                        <div className="animate-pulse space-y-3">
                                            {[1, 2, 3].map(i => (
                                                <div key={i} className="h-20 bg-gray-50 rounded-2xl w-full border border-gray-100"></div>
                                            ))}
                                        </div>
                                    ) : vacancies?.length === 0 ? (
                                        <div className="text-center py-10 text-gray-400 font-medium">
                                            No active roles found.
                                        </div>
                                    ) : vacancies?.map((v) => (
                                        <Link key={v.id} to={`/vacancies/${v.id}`}>
                                            <div className="p-5 bg-white rounded-2xl border border-gray-100 shadow-sm hover:border-blue-200 hover:shadow-md hover:-translate-y-0.5 transition-all duration-200 cursor-pointer group flex flex-col gap-1">
                                                <div className="font-bold text-gray-900 group-hover:text-blue-600 transition-colors truncate">
                                                    {v.title}
                                                </div>
                                                <div className="text-xs text-gray-400 font-bold uppercase tracking-widest flex items-center gap-2">
                                                    ID: {v.id.slice(0, 8)}
                                                </div>
                                            </div>
                                        </Link>
                                    ))}
                                </div>
                            </section>
                        </div>

                        <div className="lg:col-span-8 flex flex-col gap-6">
                            <div className="bg-white p-2.5 rounded-2xl border border-gray-100 shadow-sm flex flex-col sm:flex-row sm:items-center gap-2 relative z-10">
                                <div className="flex-1 flex items-center gap-3 px-4 py-2 bg-gray-50/50 rounded-xl focus-within:bg-white focus-within:ring-2 focus-within:ring-blue-100 transition-all">
                                    <Search size={18} className="text-gray-400" />
                                    <input
                                        type="text"
                                        placeholder="Search candidates by name, email or skills..."
                                        className="w-full bg-transparent border-none focus:ring-0 font-medium text-gray-700 placeholder:text-gray-400 outline-none"
                                        onChange={(e) => setSearchTerm(e.target.value)}
                                    />
                                </div>
                                <div className="hidden sm:block w-px h-8 bg-gray-100 mx-1" />
                                <div className="flex items-center gap-2 px-3 py-2 bg-gray-50/50 rounded-xl hover:bg-gray-100 transition-colors cursor-pointer shrink-0">
                                    <ListFilter size={18} className="text-gray-500" />
                                    <select
                                        className="bg-transparent border-none focus:ring-0 font-bold text-sm text-gray-600 cursor-pointer outline-none w-full sm:w-auto"
                                        value={sortBy}
                                        onChange={(e) => setSortBy(e.target.value)}
                                    >
                                        <option value="score_desc">Highest AI Score</option>
                                        <option value="score_asc">Lowest AI Score</option>
                                        <option value="name_asc">Name (A-Z)</option>
                                        <option value="name_desc">Name (Z-A)</option>
                                    </select>
                                </div>
                            </div>

                            <CandidatesList onSelect={setSelectedCandidate} searchTerm={search} sortBy={sortBy} />
                        </div>
                    </div>
                )}
            </main>

            {selectedCandidate && (
                <CandidateDetails
                    candidate={selectedCandidate}
                    onClose={() => setSelectedCandidate(null)}
                    onUpdateStatus={handleStatusChange}
                    isUpdating={isUpdatingStatus}
                />
            )}
        </div>
    );
};

export default Candidates;