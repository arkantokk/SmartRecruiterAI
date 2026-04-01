import { useState, useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import Header from "../components/Header";
import { CandidatesList } from "../components/CandidatesList";
import { CandidateDetails } from "../components/CandidateDetails";
import { useIntegrationStatus } from "../features/integrations/hooks/useIntegrationStatus";
import { integrationService } from "../features/integrations/integrationService";
import { vacanciesService } from "../features/vacancies/vacanciesService";
import {
    MailWarning,
    Plus,
    Briefcase,
    Sparkles,
    Search,
    ListFilter
} from "lucide-react";
import type { Candidate } from "../features/candidates/candidatesService";

export const Candidates = () => {
    const queryClient = useQueryClient();
    const [params, setParams] = useSearchParams();
    const [selectedCandidate, setSelectedCandidate] = useState<Candidate | null>(null);

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

    return (
        <div className="min-h-screen bg-[#F8FAFC]">
            <Header />

            <main className="max-w-[1600px] mx-auto px-6 py-10">
                <div className="flex justify-between items-center mb-10">
                    <div>
                        <h1 className="text-4xl font-black text-gray-900 tracking-tight">Recruitment Dashboard</h1>
                        <p className="text-gray-500 mt-2 flex items-center gap-2 font-medium">
                            <Sparkles size={18} className="text-blue-500" /> AI-powered candidate sourcing and evaluation
                        </p>
                    </div>
                </div>

                {isStatusLoading ? (
                    <div className="flex justify-center py-20">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                    </div>
                ) : !integration?.isConnected ? (
                    <div className="bg-white rounded-3xl p-12 shadow-sm border border-gray-100 text-center max-w-2xl mx-auto mt-10">
                        <div className="w-20 h-20 bg-amber-50 rounded-2xl flex items-center justify-center mx-auto mb-6">
                            <MailWarning className="text-amber-500" size={40} />
                        </div>
                        <h2 className="text-2xl font-bold text-gray-900">Email Integration Required</h2>
                        <p className="text-gray-500 mt-3 mb-8 text-lg font-medium">
                            Connect your Gmail to start receiving and analyzing candidate resumes.
                        </p>
                        <button
                            onClick={handleConnect}
                            className="bg-blue-600 text-white px-8 py-4 rounded-2xl font-bold text-lg hover:bg-blue-700 transition-all shadow-lg shadow-blue-100"
                        >
                            Connect Google Account
                        </button>
                    </div>
                ) : (
                    <div className="grid grid-cols-12 gap-8 items-start">

                        <div className="col-span-12 lg:col-span-4 space-y-6">
                            <section className="bg-white rounded-3xl p-6 shadow-sm border border-gray-100">
                                <h3 className="text-lg font-bold text-gray-900 mb-6 flex items-center gap-2">
                                    <Plus size={20} className="text-blue-600" /> Create Vacancy
                                </h3>
                                <form onSubmit={handleVacancySubmit} className="space-y-4">
                                    <input
                                        type="text"
                                        placeholder="Position Title"
                                        value={vTitle}
                                        onChange={(e) => setVTitle(e.target.value)}
                                        className="w-full bg-gray-50 border-none rounded-xl px-4 py-3 focus:ring-2 focus:ring-blue-500 font-medium"
                                        required
                                    />
                                    <textarea
                                        placeholder="AI Evaluation Prompt..."
                                        value={vPrompt}
                                        onChange={(e) => setVPrompt(e.target.value)}
                                        className="w-full bg-gray-50 border-none rounded-xl px-4 py-3 h-32 focus:ring-2 focus:ring-blue-500 font-medium text-sm"
                                        required
                                    />
                                    <button
                                        type="submit"
                                        disabled={isCreatingVacancy}
                                        className="w-full bg-gray-900 text-white py-3 rounded-xl font-bold hover:bg-gray-800 transition-all disabled:bg-gray-400"
                                    >
                                        {isCreatingVacancy ? "Creating..." : "Add Opening"}
                                    </button>
                                </form>
                            </section>

                            <section className="bg-white rounded-3xl p-6 shadow-sm border border-gray-100">
                                <h3 className="text-lg font-bold text-gray-900 mb-6 flex items-center justify-between">
                                    <span className="flex items-center gap-2"><Briefcase size={20} className="text-blue-600" /> Active Roles</span>
                                    <span className="text-xs bg-gray-100 px-2 py-1 rounded-lg text-gray-500 font-black">{vacancies?.length || 0}</span>
                                </h3>
                                <div className="space-y-3 max-h-[400px] overflow-y-auto pr-2 custom-scrollbar">
                                    {isVacanciesLoading ? (
                                        <div className="animate-pulse space-y-2">
                                            <div className="h-12 bg-gray-100 rounded-xl w-full"></div>
                                            <div className="h-12 bg-gray-100 rounded-xl w-full"></div>
                                        </div>
                                    ) : vacancies?.map((v) => (
                                        <div key={v.id} className="p-4 bg-gray-50 rounded-2xl border border-transparent hover:border-blue-100 hover:bg-white transition-all cursor-pointer group">
                                            <div className="font-bold text-gray-900 group-hover:text-blue-600 transition-colors">{v.title}</div>
                                            <div className="text-[10px] text-gray-400 font-black uppercase mt-1 tracking-widest">{v.id.slice(0,8)}</div>
                                        </div>
                                    ))}
                                </div>
                            </section>
                        </div>

                        <div className="col-span-12 lg:col-span-8 space-y-6">
                            <div className="bg-white p-4 rounded-2xl border border-gray-100 shadow-sm flex items-center gap-4">
                                <div className="flex-1 flex items-center gap-3 px-3">
                                    <Search size={18} className="text-gray-400" />
                                    <input
                                        type="text"
                                        placeholder="Search candidates by name, email or skills..."
                                        className="w-full border-none focus:ring-0 font-medium text-gray-600 placeholder:text-gray-400"
                                    />
                                </div>
                                <div className="h-8 w-px bg-gray-100" />
                                <button className="flex items-center gap-2 text-gray-500 font-bold text-sm px-4 py-2 hover:bg-gray-50 rounded-xl transition-all">
                                    <ListFilter size={18} /> Filters
                                </button>
                            </div>

                            <CandidatesList onSelect={setSelectedCandidate} />
                        </div>
                    </div>
                )}
            </main>

            <CandidateDetails
                candidate={selectedCandidate}
                onClose={() => setSelectedCandidate(null)}
            />
        </div>
    );
};

export default Candidates;