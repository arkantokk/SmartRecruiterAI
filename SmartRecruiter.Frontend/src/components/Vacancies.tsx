import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { vacanciesService } from "../features/vacancies/vacanciesService";

import Header from "./Header";
import {
    Plus,
    Briefcase,
    Wand2,
    TextQuote,
    Layers,
    Trash2,
    ChevronRight,
    Sparkles
} from "lucide-react";

export const Vacancies = () => {
    const queryClient = useQueryClient();
    const [title, setTitle] = useState("");
    const [aiPromptTemplate, setAiPromptTemplate] = useState("");

    const { data, isLoading, isError } = useQuery({
        queryKey: ['vacancies'],
        queryFn: vacanciesService.getVacancies
    });

    const { mutate, isPending } = useMutation({
        mutationFn: vacanciesService.postVacancy,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['vacancies'] });
            setTitle("");
            setAiPromptTemplate("");
        }
    });

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!title.trim() || !aiPromptTemplate.trim()) return;
        mutate({ title, aiPromptTemplate });
    };

    return (
        <div className="min-h-screen bg-[#F8FAFC]">
            <Header />

            <main className="max-w-7xl mx-auto px-6 py-10">
                <div className="mb-10">
                    <h1 className="text-4xl font-extrabold text-gray-900 tracking-tight">Job Vacancies</h1>
                    <p className="text-gray-500 mt-2 flex items-center gap-2 font-medium">
                        <Briefcase size={18} /> Define roles and customize AI evaluation logic
                    </p>
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-3 gap-10 items-start">

                    <div className="lg:col-span-1">
                        <form onSubmit={handleSubmit} className="bg-white rounded-3xl p-8 shadow-sm border border-gray-100 sticky top-10">
                            <div className="flex items-center gap-3 mb-6">
                                <div className="p-2 bg-blue-50 rounded-xl text-blue-600">
                                    <Plus size={20} />
                                </div>
                                <h2 className="text-xl font-bold text-gray-900">New Vacancy</h2>
                            </div>

                            <div className="space-y-6">
                                <div>
                                    <label className="block text-sm font-bold text-gray-700 uppercase tracking-wider mb-2">
                                        Position Title
                                    </label>
                                    <input
                                        type="text"
                                        placeholder="e.g. Senior React Developer"
                                        value={title}
                                        onChange={(e) => setTitle(e.target.value)}
                                        className="w-full bg-gray-50 border-none rounded-2xl px-4 py-3 focus:ring-2 focus:ring-blue-500 transition-all placeholder:text-gray-400 font-medium"
                                        required
                                    />
                                </div>

                                <div>
                                    <label className="block text-sm font-bold text-gray-700 uppercase tracking-wider mb-2 flex justify-between">
                                        AI Prompt Template
                                        <Sparkles size={14} className="text-blue-400" />
                                    </label>
                                    <textarea
                                        placeholder="Describe what AI should look for in candidates..."
                                        value={aiPromptTemplate}
                                        onChange={(e) => setAiPromptTemplate(e.target.value)}
                                        className="w-full bg-gray-50 border-none rounded-2xl px-4 py-3 h-40 focus:ring-2 focus:ring-blue-500 transition-all placeholder:text-gray-400 font-medium text-sm leading-relaxed"
                                        required
                                    />
                                    <p className="mt-2 text-[10px] text-gray-400 font-bold uppercase tracking-widest">
                                        This template guides the AI candidate evaluation
                                    </p>
                                </div>

                                <button
                                    type="submit"
                                    disabled={isPending}
                                    className="w-full bg-blue-600 text-white py-4 rounded-2xl font-bold text-lg hover:bg-blue-700 disabled:bg-blue-300 transition-all shadow-lg shadow-blue-100 flex items-center justify-center gap-2"
                                >
                                    {isPending ? "Creating..." : <><Plus size={20} /> Create Vacancy</>}
                                </button>
                            </div>
                        </form>
                    </div>

                    <div className="lg:col-span-2">
                        <div className="flex items-center justify-between mb-6">
                            <h2 className="text-xl font-bold text-gray-900 flex items-center gap-2">
                                <Layers size={20} className="text-blue-500" />
                                Active Roles <span className="ml-2 px-2 py-0.5 bg-gray-200 text-gray-600 rounded-md text-xs">{data?.length || 0}</span>
                            </h2>
                        </div>

                        {isLoading ? (
                            <div className="space-y-4">
                                {[1, 2].map(i => <div key={i} className="h-32 w-full bg-gray-200 animate-pulse rounded-3xl" />)}
                            </div>
                        ) : isError ? (
                            <div className="p-6 bg-red-50 text-red-600 rounded-3xl border border-red-100 font-bold text-center">
                                Failed to synchronize with server
                            </div>
                        ) : data && data.length > 0 ? (
                            <div className="grid gap-4">
                                {data.map((vacancy) => (
                                    <div key={vacancy.id} className="group bg-white p-6 rounded-3xl border border-gray-100 shadow-sm hover:shadow-md hover:border-blue-200 transition-all">
                                        <div className="flex justify-between items-start mb-4">
                                            <div className="flex items-center gap-4">
                                                <div className="h-12 w-12 rounded-2xl bg-blue-50 flex items-center justify-center text-blue-600">
                                                    <Briefcase size={24} />
                                                </div>
                                                <div>
                                                    <h3 className="text-xl font-black text-gray-900 group-hover:text-blue-600 transition-colors">
                                                        {vacancy.title}
                                                    </h3>
                                                    <span className="text-xs font-bold text-gray-400 uppercase tracking-widest">
                                                        ID: {vacancy.id.slice(0, 8)}
                                                    </span>
                                                </div>
                                            </div>
                                            <button className="p-2 text-gray-300 hover:text-red-500 transition-colors">
                                                <Trash2 size={18} />
                                            </button>
                                        </div>

                                        <div className="relative bg-gray-50 rounded-2xl p-4 border border-gray-100">
                                            <div className="absolute top-4 right-4 text-blue-200">
                                                <Wand2 size={24} />
                                            </div>
                                            <h4 className="text-[10px] font-black text-blue-400 uppercase tracking-[0.2em] mb-2 flex items-center gap-1">
                                                <TextQuote size={12} /> AI Strategy
                                            </h4>
                                            <p className="text-gray-600 text-sm leading-relaxed font-medium line-clamp-3">
                                                {vacancy.aiPromptTemplate}
                                            </p>
                                        </div>

                                        <div className="mt-4 flex justify-end">
                                            <button className="text-blue-600 text-sm font-bold flex items-center gap-1 hover:gap-2 transition-all">
                                                View Applicants <ChevronRight size={16} />
                                            </button>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        ) : (
                            <div className="bg-white rounded-3xl p-16 border-2 border-dashed border-gray-200 text-center">
                                <div className="w-16 h-16 bg-gray-50 rounded-full flex items-center justify-center mx-auto mb-4">
                                    <Briefcase className="text-gray-300" size={32} />
                                </div>
                                <h3 className="text-lg font-bold text-gray-900">No vacancies created</h3>
                                <p className="text-gray-500 mt-1 font-medium">Start by adding your first job role on the left.</p>
                            </div>
                        )}
                    </div>
                </div>
            </main>
        </div>
    );
};